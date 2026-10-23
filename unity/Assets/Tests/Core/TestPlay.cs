using NUnit.Framework;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;

namespace TurnTheDial.Core.Tests
{
    /// <summary>Small helpers for placing and playing exact cards.</summary>
    public static class TestPlay
    {
        /// <summary>Puts a new copy in hand, makes sure Airtime covers its cost, plays it, asserts success.</summary>
        public static CardInstance Play(Fight f, CardData card)
        {
            CardInstance inst = f.TestPutInHand(card);
            if (f.Airtime < card.Cost) f.TestSetAirtime(card.Cost);
            CommandResult r = f.PlayCard(inst.Uid);
            Assert.IsTrue(r.Ok, r.Reason);
            return inst;
        }

        /// <summary>Audience drop from playing the card = the Reach that got past Lock.</summary>
        public static int ReachOf(Fight f, CardData card)
        {
            int before = f.RivalAudience;
            Play(f, card);
            return before - f.RivalAudience;
        }

        /// <summary>A free On Air card with no numbers, to avoid Dead Air in loop tests.</summary>
        public static CardData FreeOnAir()
        {
            return new CardData
            {
                Id = "test.free_on_air", Name = "Free On Air", Type = CardType.DjBreak, Cost = 0, Rarity = Rarity.Common, Pool = CardPool.Shared,
                Keywords = new CardKeywords { OnAir = true },
            };
        }

        public static CardData NonSongReach(int reach, int drop = 0)
        {
            return new CardData
            {
                Id = "test.reach_" + reach + "_" + drop, Name = "Test Reach " + reach, Type = CardType.DjBreak, Cost = 0, Reach = reach, Rarity = Rarity.Common,
                Pool = CardPool.Shared, Keywords = new CardKeywords { OnAir = true, Drop = drop },
            };
        }

        public static FormatData BigFormat(int max = 1000)
        {
            return new FormatData { Id = "test.format", Name = "Test Format", MaxListeners = max, Genre = Genre.Rock, Signature = Signature.None };
        }
    }
}
