using System;
using System.Collections.Generic;
using TurnTheDial.Core.Content;
using TurnTheDial.Core.Rng;

namespace TurnTheDial.Core.Combat
{
    /// <summary>One physical copy of a card in a fight. Uids are assigned in deck order.</summary>
    public sealed class CardInstance
    {
        public CardInstance(int uid, CardData card)
        {
            Uid = uid;
            Card = card;
        }

        public int Uid { get; }

        public CardData Card { get; }

        public bool IsInterference => Card.Type == CardType.Interference;
    }

    public enum FightPhase
    {
        NotStarted = 0,
        Player = 1,
        Rival = 2,
        Over = 3,
    }

    public enum FightOutcome
    {
        None = 0,
        WonAudienceZero = 1,
        WonRatings = 2,
        LostRatings = 3,
        LostListenersZero = 4,
    }

    /// <summary>Everything a fight needs. Rng is the run's streams (only rng.shuffle is used here).</summary>
    public sealed class FightSetup
    {
        public BalanceConfig Balance { get; set; }

        public FormatData Format { get; set; }

        public RivalData Rival { get; set; }

        public FightKind Kind { get; set; } = FightKind.Daypart;

        public CardCatalog Catalog { get; set; }

        /// <summary>The run's deck as card data (remastered copies use the upgrade data).</summary>
        public List<CardData> Deck { get; set; } = new List<CardData>();

        /// <summary>Gear in acquisition order.</summary>
        public List<GearData> Gear { get; set; } = new List<GearData>();

        public MarketRules Market { get; set; }

        public RngStreams Rng { get; set; }

        /// <summary>Listeners at fight start. Null = format maximum (Proposed, formats.md: a run starts at full listeners).</summary>
        public int? Listeners { get; set; }

        /// <summary>Null = the format's max listeners.</summary>
        public int? MaxListeners { get; set; }

        /// <summary>Null = balance cash.start.</summary>
        public int? Cash { get; set; }
    }

    /// <summary>A computed Reach number and the math behind it.</summary>
    public struct ReachCalc
    {
        public int Reach;
        public string Math;
        public bool Anthem;
        public int BurnoutStacks;
    }

    /// <summary>Result of applying one hit of Reach (combat.md, Applying Reach).</summary>
    public struct ReachResult
    {
        public string Source;
        public int Reach;
        public int Absorbed;
        public int Through;
        public int ListenersGained;
        public int ListenersBeforeCap;
    }

    /// <summary>The intent as shown: its final number (Buzz included) and the tooltip math.</summary>
    public sealed class IntentPreview
    {
        public IntentKind Kind { get; set; }

        public int BaseAmount { get; set; }

        public int Hits { get; set; }

        /// <summary>Final number per hit (Steal / Big Segment include Buzz); otherwise the amount.</summary>
        public int PerHit { get; set; }

        public int Total => PerHit * Hits;

        public string CardName { get; set; }

        public PileKind Pile { get; set; }

        /// <summary>Short label, e.g. "Steal 9" or "Big Segment 2 x 6".</summary>
        public string Label { get; set; }

        /// <summary>How the number was calculated (tooltip).</summary>
        public string Math { get; set; }

        public int TurnNumber { get; set; }
    }

    public readonly struct CommandResult
    {
        private CommandResult(bool ok, string reason)
        {
            Ok = ok;
            Reason = reason;
        }

        public bool Ok { get; }

        public string Reason { get; }

        public static CommandResult Success => new CommandResult(true, null);

        public static CommandResult Fail(string reason) => new CommandResult(false, reason);
    }

    public sealed class BurnoutEntry
    {
        public BurnoutEntry(string name)
        {
            Name = name;
        }

        public string Name { get; }

        /// <summary>Plays of this name this daypart. Stacks = plays - 1.</summary>
        public int Plays { get; internal set; }

        public int Stacks => Plays > 0 ? Plays - 1 : 0;
    }

    /// <summary>Every rule as it happens, with the math (the prototype's combat log).</summary>
    public sealed class CombatLog
    {
        private readonly List<string> _lines = new List<string>();

        public event Action<string> LineAdded;

        public IReadOnlyList<string> Lines => _lines;

        public void Add(string line)
        {
            _lines.Add(line);
            LineAdded?.Invoke(line);
        }

        public string Join()
        {
            return string.Join("\n", _lines);
        }
    }
}
