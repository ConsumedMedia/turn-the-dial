using System.Collections.Generic;
using NUnit.Framework;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using static TurnTheDial.Core.Tests.TestContent;
using static TurnTheDial.Core.Tests.TestPlay;

namespace TurnTheDial.Core.Tests
{
    /// <summary>formats.md Anthem edge cases, Ribbon Mic (gear.md), remaster carry-overs.</summary>
    public class FormatGearTests
    {
        [Test]
        public void Anthem_AlbumCutThenPowerBallad_StillGetsBonus()
        {
            Fight f = Started();
            Play(f, AlbumCut());
            Assert.AreEqual(13, ReachOf(f, PowerBallad())); // 9 x 1.0 Flow (lower tempo, no ramp before) x 1.5
            Assert.AreEqual(4, f.Loyalty);
        }

        [Test]
        public void Anthem_TwoAnthemSongs_OnlyFirstGetsBonus()
        {
            Fight f = Started();
            CardData other = Song("test.anthem2", "Second Anthem", Genre.Rock, 4, 1, 8, anthem: true);
            Assert.AreEqual(13, ReachOf(f, PowerBallad()));
            Assert.AreEqual(8, ReachOf(f, other)); // Flow 0 at Reach time, no Anthem
            Assert.AreEqual(4, f.Loyalty);
        }

        [Test]
        public void Anthem_ResetsEachTurn()
        {
            Fight f = Started();
            Assert.AreEqual(13, ReachOf(f, PowerBallad()));
            f.EndTurn();
            f.TestSetFlow(0);
            CardData other = Song("test.anthem2", "Second Anthem", Genre.Rock, 4, 1, 8, anthem: true);
            Assert.AreEqual(12, ReachOf(f, other)); // 8 x 1.5
        }

        [Test]
        public void Anthem_OnlyForAnthemFormat()
        {
            Fight f = Started(format: BigFormat());
            Assert.AreEqual(9, ReachOf(f, PowerBallad()));
            Assert.AreEqual(0, f.Loyalty);
        }

        [Test]
        public void RibbonMic_FirstDjBreakEachTurn_Plus3Loyalty()
        {
            Fight f = Started(gear: new List<GearData> { RibbonMic() });
            Play(f, RequestLine());
            Assert.AreEqual(8, f.Loyalty);
            Play(f, RequestLine());
            Assert.AreEqual(13, f.Loyalty);
            f.EndTurn();
            Play(f, RequestLine());
            Assert.AreEqual(8, f.Loyalty);
        }

        [Test]
        public void RibbonMic_IgnoresNonDjBreaks()
        {
            Fight f = Started(gear: new List<GearData> { RibbonMic() });
            Play(f, AlbumCut());
            Play(f, StationId());
            Assert.AreEqual(0, f.Loyalty);
        }

        [Test]
        public void RibbonMic_DjBreakFromCueSlot_Counts()
        {
            Fight f = Started(gear: new List<GearData> { RibbonMic() });
            CardInstance rl = f.TestPutInHand(RequestLine());
            f.Cue(rl.Uid);
            f.EndTurn();
            Assert.IsTrue(f.PlayCard(rl.Uid).Ok);
            Assert.AreEqual(8, f.Loyalty);
        }

        [Test]
        public void Remastered_CarDealerSpot_KeepsListenerCost()
        {
            Fight f = Started();
            Play(f, CarDealerSpot().Upgrade);
            Assert.AreEqual(85, f.Cash);
            Assert.AreEqual(81, f.Listeners);
        }

        [Test]
        public void StartingDeck_ClassicRock_10Cards()
        {
            List<CardData> deck = ClassicRockDeck();
            Assert.AreEqual(10, deck.Count);
            Assert.AreEqual(4, deck.FindAll(c => c.Id == "card.cr.album_cut").Count);
            Assert.AreEqual(3, deck.FindAll(c => c.Id == "card.cr.request_line").Count);
            Assert.AreEqual(1, deck.FindAll(c => c.Id == "card.sh.station_id").Count);
            Assert.AreEqual(1, deck.FindAll(c => c.Id == "card.cr.car_dealer_spot").Count);
            Assert.AreEqual(1, deck.FindAll(c => c.Id == "card.cr.power_ballad").Count);
        }
    }
}
