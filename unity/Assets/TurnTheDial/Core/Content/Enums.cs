namespace TurnTheDial.Core.Content
{
    // Enum values are serialized as integers by the Data assets. Never reorder or reuse a value;
    // only append.

    /// <summary>Card type (brief §4).</summary>
    public enum CardType
    {
        Song = 0,
        DjBreak = 1,
        Ad = 2,
        Promo = 3,
        Liner = 4,
        Interference = 5,
    }

    /// <summary>Song genre (brief §4). None for non-songs.</summary>
    public enum Genre
    {
        None = 0,
        Rock = 1,
        HipHop = 2,
        Country = 3,
        Pop = 4,
    }

    public enum Rarity
    {
        Starter = 0,
        Common = 1,
        Uncommon = 2,
        Rare = 3,
        Special = 4,
    }

    public enum CardPool
    {
        ClassicRock = 0,
        HipHopHeat = 1,
        CountryRoads = 2,
        Shared = 3,
        Rival = 4,
    }

    /// <summary>Where an added card goes.</summary>
    public enum PileKind
    {
        Draw = 0,
        Discard = 1,
        Hand = 2,
    }

    /// <summary>The v1 effect library (cards.md). New effects are added here, never per card.</summary>
    public enum EffectKind
    {
        DrawCards = 0,
        GainFlow = 1,
        GainHype = 2,
        GainLoyalty = 3,
        GainCash = 4,
        LoseListeners = 5,
        MonitorIntents = 6,
        MonitorDraw = 7,
        /// <summary>Air Horn (brief §5): the next Song counts as a rising ramp. Amount unused.</summary>
        NextSongCountsAsRamp = 8,
        RaiseFlowCap = 9,
        AddCardToPile = 10,
    }

    /// <summary>When a card's trigger fires instead of on play (brief §4 `triggers`).</summary>
    public enum CardTriggerKind
    {
        /// <summary>End of turn, while the card is in hand (end-turn step 2). A cued card isn't in hand.</summary>
        EndOfTurnInHand = 0,
    }

    /// <summary>Rival intents (brief §7, rivals.md).</summary>
    public enum IntentKind
    {
        Steal = 0,
        BigSegment = 1,
        CommercialBreak = 2,
        Contest = 3,
        HeavyRotation = 4,
        Interference = 5,
        Trending = 6,
    }

    /// <summary>Format signatures (brief §5).</summary>
    public enum Signature
    {
        None = 0,
        Anthem = 1,
        Loop = 2,
        Mailbag = 3,
    }

    /// <summary>Gear triggers built so far (gear.md lists more; they are added when needed).</summary>
    public enum GearTrigger
    {
        DaypartStart = 0,
        TurnStart = 1,
        CardPlayed = 2,
        FirstCardOfTypeEachTurn = 3,
    }

    public enum FightKind
    {
        Daypart = 0,
        PrimeTime = 1,
        Boss = 2,
    }

    /// <summary>Keywords that have a tooltip (brief §4, pillar 5).</summary>
    public enum Keyword
    {
        OnAir = 0,
        Drop = 1,
        Anthem = 2,
        Hold = 3,
        Monitor = 4,
        Pulled = 5,
        Sticky = 6,
        Unplayable = 7,
    }
}
