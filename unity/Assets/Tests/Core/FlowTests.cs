using NUnit.Framework;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using static TurnTheDial.Core.Tests.TestContent;
using static TurnTheDial.Core.Tests.TestPlay;

namespace TurnTheDial.Core.Tests
{
    /// <summary>flow.md: ramp, streak, first song, caps, Ad, Dead Air, Loop, Air Horn, Hype.</summary>
    public class FlowTests
    {
        [Test]
        public void FirstSong_GainsNothing()
        {
            Fight f = Started();
            Play(f, AlbumCut());
            Assert.AreEqual(0, f.Flow);
        }

        [Test]
        public void Ramp_And_Streak_EachPlusOne()
        {
            Fight f = Started();
            Play(f, PowerBallad()); // T2
            Play(f, AlbumCut()); // T3 Rock after T2 Rock
            Assert.AreEqual(2, f.Flow);
        }

        [Test]
        public void SongNeverBenefitsFromItsOwnFlow()
        {
            Fight f = Started();
            f.TestSetLastSong(PowerBallad());
            Assert.AreEqual(6, ReachOf(f, AlbumCut())); // Flow 0 at Reach time
            Assert.AreEqual(2, f.Flow);
        }

        [Test]
        public void LowerTempo_OnlyMissesRamp()
        {
            Fight f = Started();
            f.TestSetFlow(3);
            f.TestSetLastSong(AlbumCut()); // T3
            Play(f, PowerBallad()); // T2
            Assert.AreEqual(4, f.Flow); // streak only
        }

        [Test]
        public void EqualTempo_NoRamp()
        {
            Fight f = Started();
            f.TestSetLastSong(AlbumCut());
            Play(f, AlbumCut());
            Assert.AreEqual(1, f.Flow);
        }

        [Test]
        public void OffGenreSong_NoStreak_DoesNotBreakFlow()
        {
            Fight f = Started();
            f.TestSetFlow(3);
            f.TestSetLastSong(AlbumCut()); // T3 Rock
            Play(f, CornerCypher()); // T1 Hip-Hop in a Classic Rock deck
            Assert.AreEqual(3, f.Flow);
        }

        [Test]
        public void FlowAtCap_GainingTwo_StaysAtCap()
        {
            Fight f = Started();
            f.TestSetFlow(10);
            f.TestSetLastSong(PowerBallad());
            Play(f, AlbumCut());
            Assert.AreEqual(10, f.Flow);
        }

        [Test]
        public void Ad_BreaksFlow_AndClearsLastSong()
        {
            Fight f = Started();
            f.TestSetFlow(5);
            f.TestSetLastSong(PowerBallad());
            Play(f, CarDealerSpot());
            Assert.AreEqual(0, f.Flow);
            Assert.IsNull(f.LastSong);
            Play(f, AlbumCut()); // no last song: no ramp, no streak
            Assert.AreEqual(0, f.Flow);
        }

        [Test]
        public void NonSongNonAd_DoesNotChangeLastSong()
        {
            Fight f = Started();
            f.TestSetLastSong(PowerBallad());
            Play(f, RequestLine());
            Play(f, StationId());
            Assert.AreEqual("Power Ballad", f.LastSong.Name);
        }

        [Test]
        public void DeadAir_BreaksFlow()
        {
            Fight f = Started();
            f.TestSetFlow(6);
            f.EndTurn();
            Assert.AreEqual(0, f.Flow);
        }

        [Test]
        public void Loop_Cap15()
        {
            Fight f = Started(format: HipHopHeat());
            Assert.AreEqual(15, f.FlowCap);
            f.TestSetFlow(14);
            f.TestSetLastSong(BlockParty());
            Play(f, BasementSession());
            Assert.AreEqual(15, f.FlowCap);
            Assert.AreEqual(15, f.Flow);
        }

        [Test]
        public void Loop_Tempo5To1_IsRamp_5To2_IsNot()
        {
            Fight f = Started(format: HipHopHeat());
            f.TestSetLastSong(Banger()); // T5
            Play(f, CornerCypher()); // T1
            Assert.AreEqual(2, f.Flow);

            Fight g = Started(format: HipHopHeat());
            g.TestSetLastSong(Banger());
            Play(g, BlockParty()); // T2
            Assert.AreEqual(1, g.Flow); // streak only
        }

        [Test]
        public void AirHorn_NextSongPlusOneTempoForRamp()
        {
            Fight f = Started(format: HipHopHeat());
            f.TestSetLastSong(BlockParty()); // T2
            Play(f, AirHorn());
            Play(f, BlockParty()); // counts as T3
            Assert.AreEqual(2, f.Flow);
            Assert.AreEqual(0, f.PendingTempoBonus);
        }

        [Test]
        public void TwoAirHorns_PlusTwo()
        {
            Fight f = Started(format: HipHopHeat());
            f.TestSetLastSong(BlockParty()); // T2
            Play(f, AirHorn());
            Play(f, AirHorn());
            Play(f, CornerCypher()); // T1 + 2 = 3 > 2
            Assert.AreEqual(2, f.Flow);
        }

        [Test]
        public void AirHorn_SurvivesAnAd_ConsumedByNextSong()
        {
            Fight f = Started(format: HipHopHeat());
            f.TestSetLastSong(BlockParty());
            Play(f, AirHorn());
            Play(f, CarDealerSpot());
            Assert.AreEqual(1, f.PendingTempoBonus);
            Play(f, RooftopJam()); // no last song after the Ad: no ramp
            Assert.AreEqual(0, f.Flow);
            Assert.AreEqual(0, f.PendingTempoBonus);
        }

        [Test]
        public void Loop_PlusAirHorn_T5CountsAs6_T1AfterStillRamps()
        {
            Fight f = Started(format: HipHopHeat());
            Play(f, AirHorn());
            Play(f, Banger()); // counted as T6; Drop leaves Flow 0
            Assert.AreEqual(0, f.Flow);
            Play(f, CornerCypher());
            Assert.AreEqual(2, f.Flow);
        }

        [Test]
        public void Hype_Decays_ByOne_AtEndOfTurn_AndStaysAtZero()
        {
            Fight f = Started();
            f.TestSetHype(2);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(1, f.Hype);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(0, f.Hype);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(0, f.Hype);
        }

        [Test]
        public void Mailbag_NoHypeDecay_OnTurnWithDjBreak()
        {
            Fight f = Started(format: CountryRoads());
            f.TestSetHype(2);
            Play(f, ListenerMail()); // +1 Hype
            f.EndTurn();
            Assert.AreEqual(3, f.Hype);
            Play(f, BackPorchTune());
            f.EndTurn();
            Assert.AreEqual(2, f.Hype);
        }

        [Test]
        public void Mailbag_DjBreakFromCueSlot_Counts()
        {
            Fight f = Started(format: CountryRoads());
            CardInstance mail = f.TestPutInHand(ListenerMail());
            Assert.IsTrue(f.Cue(mail.Uid).Ok);
            f.EndTurn(); // Dead Air, fine
            f.TestSetHype(2);
            Assert.IsTrue(f.PlayCard(mail.Uid).Ok);
            f.EndTurn();
            Assert.AreEqual(3, f.Hype);
        }

        [Test]
        public void CardsOwnHype_NeverBoostsItsOwnReach()
        {
            Fight f = Started();
            CardData hypeSong = Song("test.hype_song", "Hype Song", Genre.Rock, 3, 1, 6);
            hypeSong.Hype = 2;
            Assert.AreEqual(6, ReachOf(f, hypeSong));
            Assert.AreEqual(2, f.Hype);
        }

        [Test]
        public void GenreStreakCount_OneSongIsOne_AdResets_OffGenreResets()
        {
            Fight f = Started();
            Play(f, AlbumCut());
            Assert.AreEqual(1, f.GenreStreakCount);
            Play(f, PowerBallad());
            Assert.AreEqual(2, f.GenreStreakCount);
            Play(f, CarDealerSpot());
            Assert.AreEqual(0, f.GenreStreakCount);
            Play(f, AlbumCut());
            Play(f, CornerCypher());
            Assert.AreEqual(0, f.GenreStreakCount);
        }

        [Test]
        public void SameNameSongs_ShareBurnout_RegardlessOfTempo()
        {
            Fight f = Started();
            Play(f, Song("test.a", "Same Name", Genre.Rock, 2, 1, 6));
            Play(f, Song("test.b", "Same Name", Genre.Rock, 4, 1, 6));
            Assert.AreEqual(1, f.BurnoutStacksFor("Same Name"));
        }

        [Test]
        public void HipHopStarterSongs_DistinctNames_NoBurnout()
        {
            Fight f = Started(format: HipHopHeat());
            Play(f, CornerCypher());
            Play(f, BlockParty());
            Play(f, BasementSession());
            Play(f, RooftopJam());
            Assert.AreEqual(0, f.BurnoutStacksFor("Corner Cypher"));
            Assert.AreEqual(0, f.BurnoutStacksFor("Rooftop Jam"));
            Assert.AreEqual(6, f.Flow); // three ramps + three streaks
        }
    }
}
