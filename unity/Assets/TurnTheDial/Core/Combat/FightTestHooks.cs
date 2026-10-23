using TurnTheDial.Core.Content;

namespace TurnTheDial.Core.Combat
{
    /// <summary>
    /// Internal setters so Core tests can build an exact board state (e.g. "Flow 4, Hype 2, Lock 6")
    /// without long play sequences. Not visible to Game or Data: they only use the commands.
    /// </summary>
    public sealed partial class Fight
    {
        internal void TestSetFlow(int value) { Flow = value; }
        internal void TestSetHype(int value) { Hype = value; }
        internal void TestSetLoyalty(int value) { Loyalty = value; }
        internal void TestSetLock(int value) { Lock = value; }
        internal void TestSetBuzz(int value) { Buzz = value; }
        internal void TestSetAirtime(int value) { Airtime = value; }
        internal void TestSetListeners(int value) { Listeners = value; }
        internal void TestSetRivalAudience(int value) { RivalAudience = value; }

        internal void TestSetLastSong(CardData song)
        {
            LastSong = song;
            _lastSongRampTempo = song == null ? 0 : song.Tempo;
        }

        /// <summary>Creates a new copy of the card straight in hand (no hand-limit check).</summary>
        internal CardInstance TestPutInHand(CardData card)
        {
            var inst = new CardInstance(_nextUid++, card);
            _all.Add(inst);
            _hand.Add(inst);
            return inst;
        }

        /// <summary>Creates a new copy of the card on top of the draw pile.</summary>
        internal CardInstance TestPutOnDraw(CardData card)
        {
            var inst = new CardInstance(_nextUid++, card);
            _all.Add(inst);
            _draw.Insert(0, inst);
            return inst;
        }

        internal CardInstance TestPutInDiscard(CardData card)
        {
            var inst = new CardInstance(_nextUid++, card);
            _all.Add(inst);
            _discard.Add(inst);
            return inst;
        }
    }
}
