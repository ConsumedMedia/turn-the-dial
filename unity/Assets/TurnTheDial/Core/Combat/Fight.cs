using System;
using System.Collections.Generic;
using System.Text;
using TurnTheDial.Core.Content;
using TurnTheDial.Core.Rng;

namespace TurnTheDial.Core.Combat
{
    /// <summary>
    /// One daypart fight (brief §2, combat.md). State is read through the getters; it changes only
    /// through the commands (Start, PlayCard, Cue, EndTurn, Debug*). Every rule writes a line to
    /// <see cref="Log"/> with its math, and raises events for the Game layer to animate.
    /// Every rule the design docs mark Proposed is built as its Proposed default and marked
    /// "Proposed (doc.md)" below (user, 2026-10-23).
    /// </summary>
    public sealed partial class Fight
    {
        private readonly BalanceConfig _b;
        private readonly FormatData _format;
        private readonly RivalData _rival;
        private readonly CardCatalog _catalog;
        private readonly IRng _shuffle;
        private readonly int[] _deadAirSteps;
        private readonly MarketRules _market;

        private readonly List<CardInstance> _all = new List<CardInstance>();
        private readonly List<CardInstance> _draw = new List<CardInstance>();
        private readonly List<CardInstance> _hand = new List<CardInstance>();
        private readonly List<CardInstance> _discard = new List<CardInstance>();
        private readonly List<CardInstance> _removed = new List<CardInstance>();
        private readonly List<CardInstance> _cue = new List<CardInstance>();
        private readonly List<BurnoutEntry> _burnout = new List<BurnoutEntry>();
        private readonly List<GearData> _gear = new List<GearData>();
        private readonly List<bool> _gearFiredThisTurn = new List<bool>();
        private readonly List<CardInstance> _monitoredDraw = new List<CardInstance>();

        private int _nextUid = 1;
        private int _lastSongRampTempo;

        public Fight(FightSetup setup)
        {
            if (setup == null) throw new ArgumentNullException(nameof(setup));
            _b = setup.Balance ?? throw new ArgumentException("FightSetup.Balance is required.");
            _format = setup.Format ?? throw new ArgumentException("FightSetup.Format is required.");
            _rival = setup.Rival ?? throw new ArgumentException("FightSetup.Rival is required.");
            _catalog = setup.Catalog ?? new CardCatalog();
            RngStreams streams = setup.Rng ?? throw new ArgumentException("FightSetup.Rng is required.");
            _shuffle = streams.Get(RngStreams.Shuffle);
            Seed = streams.RunSeed;
            _market = setup.Market;
            _deadAirSteps = _market != null && _market.DeadAirSteps != null && _market.DeadAirSteps.Length > 0
                ? _market.DeadAirSteps
                : _b.DeadAirSteps;
            Kind = setup.Kind;
            TurnsTotal = _b.TurnsFor(setup.Kind);
            MaxListeners = setup.MaxListeners ?? _format.MaxListeners;
            // Proposed (formats.md): a run starts at full listeners.
            Listeners = Math.Min(MaxListeners, setup.Listeners ?? MaxListeners);
            Cash = setup.Cash ?? _b.CashStart;
            RivalStartingAudience = _rival.StartingAudience;
            RivalAudience = _rival.StartingAudience;
            FlowCap = _format.Signature == Signature.Loop ? _b.FlowCapLoop : _b.FlowCap;

            for (int i = 0; i < setup.Deck.Count; i++)
            {
                _all.Add(new CardInstance(_nextUid++, setup.Deck[i]));
            }

            for (int i = 0; i < setup.Gear.Count; i++)
            {
                _gear.Add(setup.Gear[i]);
                _gearFiredThisTurn.Add(false);
            }

            Phase = FightPhase.NotStarted;
        }

        // ---------------------------------------------------------------- events

        public event Action<CardInstance> CardPlayed;
        public event Action<ReachResult> ReachDealt;
        /// <summary>old, new, reason</summary>
        public event Action<int, int, string> FlowChanged;
        public event Action<FightOutcome> FightEnded;
        /// <summary>Raised after every command finishes.</summary>
        public event Action StateChanged;

        // ---------------------------------------------------------------- read-only view

        public CombatLog Log { get; } = new CombatLog();
        public BalanceConfig Balance => _b;
        public FormatData Format => _format;
        public RivalData Rival => _rival;
        public CardCatalog Catalog => _catalog;
        public FightKind Kind { get; }
        public ulong Seed { get; }

        public FightPhase Phase { get; private set; }
        public FightOutcome Outcome { get; private set; }
        public bool RunEnded { get; private set; }
        public int RatingsLossPenalty { get; private set; }

        public int Turn { get; private set; }
        public int TurnsTotal { get; }

        public int Listeners { get; private set; }
        public int MaxListeners { get; }
        public int Cash { get; private set; }
        public int Airtime { get; private set; }
        public int Loyalty { get; private set; }
        public int Flow { get; private set; }
        public int FlowCap { get; private set; }
        public int Hype { get; private set; }
        public int DeadAirCount { get; private set; }
        public int PendingTempoBonus { get; private set; }
        public int MonitorIntentCount { get; private set; }
        public int MonitorDrawCount { get; private set; }

        public int RivalAudience { get; private set; }
        public int RivalStartingAudience { get; }
        public int Lock { get; private set; }
        public int Buzz { get; private set; }

        public IntentData CurrentIntent { get; private set; }
        public int CurrentIntentIndex { get; private set; }

        public CardData LastSong { get; private set; }

        /// <summary>Proposed (flow.md): consecutive on-genre Songs ending at the last song; 0 after an Ad.</summary>
        public int GenreStreakCount { get; private set; }

        public bool OnAirPlayedThisTurn { get; private set; }
        public bool AnthemUsedThisTurn { get; private set; }
        public bool DjBreakPlayedThisTurn { get; private set; }
        public bool CuedThisTurn { get; private set; }

        public IReadOnlyList<CardInstance> DrawPile => _draw;
        public IReadOnlyList<CardInstance> Hand => _hand;
        public IReadOnlyList<CardInstance> DiscardPile => _discard;
        public IReadOnlyList<CardInstance> RemovedPile => _removed;
        public IReadOnlyList<CardInstance> CueSlot => _cue;
        public IReadOnlyList<BurnoutEntry> Burnout => _burnout;
        public IReadOnlyList<GearData> Gear => _gear;

        public int[] DeadAirSteps => _deadAirSteps;

        /// <summary>Exposed applies while the shown intent is Commercial Break (statuses.md).</summary>
        public bool IsRivalExposed => Phase == FightPhase.Player && CurrentIntent != null && CurrentIntent.Kind == IntentKind.CommercialBreak;

        /// <summary>Interference cards in your piles (brief §3 Static row).</summary>
        public int StaticCount
        {
            get
            {
                return CountInterference(_draw) + CountInterference(_hand) + CountInterference(_discard) + CountInterference(_cue);
            }
        }

        public bool IsOver => Phase == FightPhase.Over;

        /// <summary>On-air light warning (ui.md): ending the turn now would cause Dead Air.</summary>
        public bool WouldTakeDeadAir => Phase == FightPhase.Player && !OnAirPlayedThisTurn;

        /// <summary>The Dead Air loss the next Dead Air would cause.</summary>
        public int NextDeadAirLoss => DeadAirLossFor(DeadAirCount + 1);

        /// <summary>On-air light warning (ui.md): the shown steal, after current Loyalty, would end the run.</summary>
        public bool NextStealWouldEndRun
        {
            get
            {
                if (CurrentIntent == null || IsOver) return false;
                if (CurrentIntent.Kind != IntentKind.Steal && CurrentIntent.Kind != IntentKind.BigSegment) return false;
                IntentPreview p = PreviewIntent();
                int loyalty = Loyalty;
                int listeners = Listeners;
                for (int h = 0; h < p.Hits; h++)
                {
                    int absorbed = Math.Min(loyalty, p.PerHit);
                    loyalty -= absorbed;
                    listeners -= p.PerHit - absorbed;
                }

                return listeners <= 0;
            }
        }

        public int BurnoutStacksFor(string name)
        {
            BurnoutEntry e = FindBurnout(name);
            return e == null ? 0 : e.Stacks;
        }

        /// <summary>The deck after the daypart: every card back, Interference removed except Sticky (brief §3).</summary>
        public List<CardData> DeckAfterDaypart()
        {
            var deck = new List<CardData>();
            for (int i = 0; i < _all.Count; i++)
            {
                CardInstance inst = _all[i];
                if (inst.IsInterference && !inst.Card.Keywords.Sticky)
                {
                    continue;
                }

                deck.Add(inst.Card);
            }

            return deck;
        }

        // ---------------------------------------------------------------- daypart start

        /// <summary>Daypart start (brief §2), then turn 1's player phase.</summary>
        public void Start()
        {
            if (Phase != FightPhase.NotStarted)
            {
                return;
            }

            Add("Daypart start: " + _format.Name + " (" + NumberFormat.Int(Listeners) + "/" + NumberFormat.Int(MaxListeners)
                + " listeners) vs " + _rival.Name + " (" + NumberFormat.Int(RivalAudience) + " audience), "
                + NumberFormat.Int(TurnsTotal) + " turns. Seed " + Seed.ToString(System.Globalization.CultureInfo.InvariantCulture) + ".");

            // 1. Shuffle the deck into the draw pile; reset daypart state.
            _draw.Clear();
            _draw.AddRange(_all);
            Shuffle(_draw);
            Add("Shuffled " + NumberFormat.Int(_draw.Count) + " cards into the draw pile (rng.shuffle).");
            Flow = 0;
            Hype = 0;
            Loyalty = 0;
            DeadAirCount = 0;
            _burnout.Clear();
            _cue.Clear();
            LastSong = null;
            GenreStreakCount = 0;
            Add("Flow, Hype, Burnout, Loyalty and the Dead Air count start at 0. The cue slot is empty.");

            // 2. Market modifiers.
            if (_market == null)
            {
                Add("Market: no modifiers.");
            }
            else
            {
                Add("Market: " + _market.Name + ".");
                if (_market.RivalBuzzAtStart > 0)
                {
                    Buzz += _market.RivalBuzzAtStart;
                    Add(_market.Name + ": " + _rival.Name + " gains Buzz " + NumberFormat.Int(_market.RivalBuzzAtStart) + " (Buzz " + NumberFormat.Int(Buzz) + ").");
                }
            }

            // 3. Daypart-start gear, in acquisition order. Proposed (combat.md): gear before staff.
            FireGear(GearTrigger.DaypartStart, null);
            Phase = FightPhase.Player;
            if (CheckEnd())
            {
                Changed();
                return;
            }

            // 4. Reveal turn 1's intent, then turn 1's player phase.
            Turn = 1;
            RevealIntent();
            BeginPlayerTurn();
            Changed();
        }

        // ---------------------------------------------------------------- player phase

        private void BeginPlayerTurn()
        {
            Phase = FightPhase.Player;
            OnAirPlayedThisTurn = false;
            AnthemUsedThisTurn = false;
            DjBreakPlayedThisTurn = false;
            CuedThisTurn = false;
            for (int i = 0; i < _gearFiredThisTurn.Count; i++)
            {
                _gearFiredThisTurn[i] = false;
            }

            Add("-- Turn " + NumberFormat.Int(Turn) + " of " + NumberFormat.Int(TurnsTotal) + " --");

            // 1. Turn start.
            if (Loyalty > 0)
            {
                Add("Turn start: Loyalty " + NumberFormat.Int(Loyalty) + " clears to 0.");
            }

            Loyalty = 0;
            Airtime = _b.AirtimePerTurn;
            Add("Airtime set to " + NumberFormat.Int(Airtime) + ".");
            FireGear(GearTrigger.TurnStart, null);
            if (CheckEnd()) return;

            // 2. Draw.
            DrawCards(_b.DrawPerTurn, "Draw " + NumberFormat.Int(_b.DrawPerTurn));
            CheckEnd();
        }

        public bool CanPlay(int uid, out string reason)
        {
            reason = null;
            if (Phase == FightPhase.Over)
            {
                reason = "The fight is over.";
                return false;
            }

            if (Phase != FightPhase.Player)
            {
                reason = "It's not your turn.";
                return false;
            }

            CardInstance inst = FindPlayable(uid, out _);
            if (inst == null)
            {
                reason = "That card isn't in your hand or cue slot.";
                return false;
            }

            if (CuedThisTurn)
            {
                // Proposed (combat.md, player phase order; pending user decision): cueing ends your plays this turn.
                // Brief §2 player phase: Program (step 3) comes before Cue (step 4).
                reason = "You cued this turn, so programming is over. End the turn.";
                return false;
            }

            if (inst.Card.Keywords.Unplayable)
            {
                reason = inst.Card.Name + " is Unplayable.";
                return false;
            }

            if (inst.Card.Cost > Airtime)
            {
                reason = inst.Card.Name + " costs " + NumberFormat.Int(inst.Card.Cost) + " Airtime; you have " + NumberFormat.Int(Airtime) + ".";
                return false;
            }

            return true;
        }

        /// <summary>Plays a card from hand or the cue slot (cards.md, Effect order on play).</summary>
        public CommandResult PlayCard(int uid)
        {
            if (!CanPlay(uid, out string reason))
            {
                return CommandResult.Fail(reason);
            }

            CardInstance inst = FindPlayable(uid, out bool fromCue);
            CardData c = inst.Card;

            // 1. Pay Airtime.
            Airtime -= c.Cost;
            // 2. Move the card to "resolving". Playing from the cue slot empties it (cards.md).
            if (fromCue) _cue.Remove(inst);
            else _hand.Remove(inst);
            Add("Play " + c.DisplayName + (fromCue ? " from the cue slot" : "") + ": paid " + NumberFormat.Int(c.Cost)
                + " Airtime (" + NumberFormat.Int(Airtime) + " left).");

            if (c.Keywords.OnAir) OnAirPlayedThisTurn = true;
            if (c.Type == CardType.DjBreak) DjBreakPlayedThisTurn = true;

            int burnoutStacks = 0;
            if (c.IsSong)
            {
                burnoutStacks = RegisterBurnout(c.Name);
            }

            bool anthem = AnthemApplies(c);
            if (anthem)
            {
                AnthemUsedThisTurn = true;
            }

            // 3-4. Reach as one number, applied as one hit.
            if (DealsReach(c))
            {
                ReachCalc calc = ComputeReach(c, burnoutStacks, anthem);
                ApplyReach(c.DisplayName, calc);
            }

            // 5. Other base numbers: Loyalty, Hype, Cash, listenerCost. Proposed (cards.md): after the Reach.
            if (c.Loyalty > 0) GainLoyalty(c.Loyalty, c.DisplayName);
            if (anthem)
            {
                int anthemLoyalty = c.Tempo * _b.AnthemLoyaltyPerTempo;
                GainLoyalty(anthemLoyalty, "Anthem (tempo " + NumberFormat.Int(c.Tempo) + " x " + NumberFormat.Int(_b.AnthemLoyaltyPerTempo) + ")");
            }

            if (c.Hype > 0) GainHype(c.Hype, c.DisplayName);
            if (c.Cash > 0) GainCash(c.Cash, c.DisplayName);
            if (c.ListenerCost > 0) LoseListeners(c.ListenerCost, c.DisplayName + " listener cost");

            // 6. Effects list in order. Proposed (cards.md): after the card's Reach.
            for (int i = 0; i < c.Effects.Count; i++)
            {
                ApplyEffect(c.Effects[i], c.DisplayName);
            }

            // 7. Songs update Flow; an Ad breaks Flow and clears last song.
            if (c.IsSong)
            {
                UpdateFlowFromSong(c);
            }
            else if (c.Type == CardType.Ad)
            {
                SetFlow(0, "Ad breaks Flow");
                LastSong = null;
                GenreStreakCount = 0;
                Add("The Ad clears the last song.");
            }

            // 8. Drop resolves last: Flow is always 0 after a Drop card.
            if (c.Keywords.Drop > 0)
            {
                SetFlow(0, "Drop");
            }

            // 9. To discard, or removed for the daypart.
            if (c.Keywords.Pulled)
            {
                _removed.Add(inst);
                Add(c.DisplayName + " is Pulled: removed for the rest of the daypart.");
            }
            else
            {
                _discard.Add(inst);
            }

            // Card-played gear. Pending user decision (cards.md: effect order has no gear step): fires
            // after the card reaches its pile (step 9), before the win/loss check (step 10).
            // Proposed (gear.md): a card played from the cue slot counts for "first X each turn".
            FireGear(GearTrigger.CardPlayed, c);
            FireGear(GearTrigger.FirstCardOfTypeEachTurn, c);

            CardPlayed?.Invoke(inst);

            // 10. Win/loss check (rival at 0 mid-card: the card finishes, then you win).
            CheckEnd();
            Changed();
            return CommandResult.Success;
        }

        public bool CanCue(int uid, out string reason)
        {
            reason = null;
            if (Phase != FightPhase.Player)
            {
                reason = Phase == FightPhase.Over ? "The fight is over." : "It's not your turn.";
                return false;
            }

            if (_cue.Count >= _b.CueSlots)
            {
                reason = "The cue slot is full.";
                return false;
            }

            if (FindIn(_hand, uid) == null)
            {
                reason = "That card isn't in your hand.";
                return false;
            }

            return true;
        }

        /// <summary>Cue (brief §2 player phase step 4): move 1 card from hand into the empty cue slot.</summary>
        public CommandResult Cue(int uid)
        {
            if (!CanCue(uid, out string reason))
            {
                return CommandResult.Fail(reason);
            }

            CardInstance inst = FindIn(_hand, uid);
            _hand.Remove(inst);
            _cue.Add(inst);
            CuedThisTurn = true;
            Add("Cued " + inst.Card.DisplayName + ". It stays in the cue slot and can be played from there.");
            Changed();
            return CommandResult.Success;
        }

        /// <summary>End turn (brief §2 step 5), then the rival phase.</summary>
        public CommandResult EndTurn()
        {
            if (Phase != FightPhase.Player)
            {
                return CommandResult.Fail(Phase == FightPhase.Over ? "The fight is over." : "It's not your turn.");
            }

            Add("End of turn " + NumberFormat.Int(Turn) + ".");

            // 1. Dead Air check (cueing isn't playing).
            if (!OnAirPlayedThisTurn)
            {
                DeadAirCount++;
                int loss = DeadAirLossFor(DeadAirCount);
                Add("Dead Air #" + NumberFormat.Int(DeadAirCount) + ": no On Air card played this turn.");
                LoseListeners(loss, "Dead Air");
                SetFlow(0, "Dead Air");
                if (CheckEnd()) { Changed(); return CommandResult.Success; }
            }

            // 2. End-of-turn card effects (cards in hand; a cued card isn't in hand).
            // Proposed (statuses.md): a cued Complaint Call doesn't trigger.
            var handNow = new List<CardInstance>(_hand);
            for (int i = 0; i < handNow.Count; i++)
            {
                CardData c = handNow[i].Card;
                for (int e = 0; e < c.EndOfTurnInHand.Count; e++)
                {
                    ApplyEffect(c.EndOfTurnInHand[e], c.DisplayName + " in hand at end of turn");
                    if (CheckEnd()) { Changed(); return CommandResult.Success; }
                }
            }

            // 3. Discard the hand except Hold cards and the cued card. Unspent Airtime is lost.
            var kept = new List<string>();
            int discarded = 0;
            for (int i = _hand.Count - 1; i >= 0; i--)
            {
                CardInstance inst = _hand[i];
                if (inst.Card.Keywords.Hold)
                {
                    kept.Insert(0, inst.Card.DisplayName);
                    continue;
                }

                _hand.RemoveAt(i);
                _discard.Add(inst);
                discarded++;
            }

            string discardLine = "Discarded " + NumberFormat.Int(discarded) + " card" + (discarded == 1 ? "" : "s") + " from hand";
            if (kept.Count > 0) discardLine += " (Hold kept: " + string.Join(", ", kept) + ")";
            if (_cue.Count > 0) discardLine += "; " + _cue[0].Card.DisplayName + " stays cued";
            Add(discardLine + ".");
            if (Airtime > 0)
            {
                Add("Unspent Airtime lost: " + NumberFormat.Int(Airtime) + ".");
            }

            Airtime = 0;
            MonitorDrawCount = 0;
            _monitoredDraw.Clear();
            if (CheckEnd()) { Changed(); return CommandResult.Success; }

            // 4. Hype decay (Mailbag exception).
            if (Hype > 0)
            {
                // Proposed (flow.md): a DJ Break played from the cue slot counts for Mailbag.
                if (_format.Signature == Signature.Mailbag && DjBreakPlayedThisTurn)
                {
                    Add("Mailbag: you played a DJ Break this turn, so Hype stays " + NumberFormat.Int(Hype) + ".");
                }
                else
                {
                    int old = Hype;
                    Hype = Math.Max(0, Hype - _b.HypeDecayPerTurn);
                    Add("Hype decays: " + NumberFormat.Int(old) + " -> " + NumberFormat.Int(Hype) + ".");
                }
            }

            if (CheckEnd()) { Changed(); return CommandResult.Success; }

            RivalPhase();
            Changed();
            return CommandResult.Success;
        }

        // ---------------------------------------------------------------- rival phase

        private void RivalPhase()
        {
            Phase = FightPhase.Rival;
            Add("-- " + _rival.Name + "'s phase --");

            // 1. Lock clears.
            if (Lock > 0)
            {
                Add(_rival.Name + "'s Lock " + NumberFormat.Int(Lock) + " clears to 0.");
            }

            Lock = 0;

            // 2. The intent you saw.
            PerformIntent();
            if (CheckEnd()) return;

            // 3. Buzz -1.
            if (Buzz > 0)
            {
                Buzz--;
                Add(_rival.Name + "'s Buzz drops to " + NumberFormat.Int(Buzz) + ".");
            }

            // 4. Ratings check after the last turn, otherwise reveal the next intent.
            if (Turn >= TurnsTotal)
            {
                RatingsCheck();
                return;
            }

            Turn++;
            RevealIntent();
            BeginPlayerTurn();
        }

        private void RevealIntent()
        {
            // Proposed (rivals.md): a pattern shorter than the fight loops from turn 1.
            CurrentIntentIndex = (Turn - 1) % _rival.Pattern.Count;
            CurrentIntent = _rival.Pattern[CurrentIntentIndex];
            IntentPreview p = PreviewIntent();
            Add(_rival.Name + " reveals its turn " + NumberFormat.Int(Turn) + " intent: " + p.Label + ". (" + p.Math + ")");
            if (CurrentIntent.Kind == IntentKind.CommercialBreak)
            {
                Add(_rival.Name + " is Exposed this turn: your Reach x" + NumberFormat.Factor(_b.ExposedPercent) + ".");
            }
        }

        /// <summary>The shown intent with its final number. The resolve uses this same preview.</summary>
        public IntentPreview PreviewIntent()
        {
            if (CurrentIntent == null) return null;
            return BuildPreview(CurrentIntent, Buzz, Turn);
        }

        /// <summary>
        /// Monitor peeks: the next N intents after the current one, never past the last turn
        /// (cards.md edge case). Future numbers are shown before Buzz.
        /// </summary>
        public List<IntentPreview> PreviewUpcomingIntents(int count)
        {
            var list = new List<IntentPreview>();
            for (int k = 1; k <= count; k++)
            {
                int turn = Turn + k;
                if (turn > TurnsTotal) break;
                IntentData intent = _rival.Pattern[(turn - 1) % _rival.Pattern.Count];
                list.Add(BuildPreview(intent, 0, turn));
            }

            return list;
        }

        /// <summary>
        /// The cards a Monitor draw-pile peek showed, as they were when the effect resolved. Pending
        /// user decision (cards.md: how long a peek stays visible): kept until end of turn.
        /// </summary>
        public IReadOnlyList<CardInstance> MonitoredDraw => _monitoredDraw;

        /// <summary>Monitor on the draw pile: the top N cards, or all of them if fewer; no reshuffle.</summary>
        public List<CardInstance> PeekDraw(int count)
        {
            var list = new List<CardInstance>();
            for (int i = 0; i < count && i < _draw.Count; i++)
            {
                list.Add(_draw[i]);
            }

            return list;
        }

        private IntentPreview BuildPreview(IntentData intent, int buzz, int turn)
        {
            var p = new IntentPreview
            {
                Kind = intent.Kind,
                BaseAmount = intent.Amount,
                Hits = Math.Max(1, intent.Hits),
                PerHit = intent.Amount,
                Pile = intent.Pile,
                TurnNumber = turn,
            };
            string amount = NumberFormat.Int(intent.Amount);
            switch (intent.Kind)
            {
                case IntentKind.Steal:
                case IntentKind.BigSegment:
                {
                    int factor = CardRules.PercentBase + _b.BuzzStealBonusPercent;
                    p.PerHit = buzz > 0 ? intent.Amount * factor / CardRules.PercentBase : intent.Amount;
                    string math = buzz > 0
                        ? amount + " base, Buzz " + NumberFormat.Int(buzz) + ": floor(" + amount + " x " + NumberFormat.Factor(factor) + ") = " + NumberFormat.Int(p.PerHit)
                        : amount + " base, no Buzz = " + NumberFormat.Int(p.PerHit);
                    if (p.Hits > 1)
                    {
                        math += " per hit, " + NumberFormat.Int(p.Hits) + " hits (" + NumberFormat.Int(p.Total) + " total)";
                    }

                    p.Math = math + ". Loyalty absorbs each hit first.";
                    if (intent.Kind == IntentKind.Steal)
                    {
                        p.Label = "Steal " + NumberFormat.Int(p.PerHit);
                    }
                    else
                    {
                        p.Label = p.Hits > 1
                            ? "Big Segment " + NumberFormat.Int(p.Hits) + " x " + NumberFormat.Int(p.PerHit)
                            : "Big Segment " + NumberFormat.Int(p.PerHit);
                    }

                    break;
                }

                case IntentKind.CommercialBreak:
                    p.PerHit = 0;
                    p.Label = "Commercial Break";
                    p.Math = "Skips its move. The rival is Exposed during your turn: your Reach x" + NumberFormat.Factor(_b.ExposedPercent) + ".";
                    break;
                case IntentKind.Contest:
                    p.Label = "Contest " + amount;
                    p.Math = "Rival regains " + amount + " audience, not above its starting " + NumberFormat.Int(RivalStartingAudience) + ".";
                    break;
                case IntentKind.HeavyRotation:
                    p.Label = "Heavy Rotation " + amount;
                    p.Math = "Rival gains " + amount + " Lock. Lock absorbs your Reach first.";
                    break;
                case IntentKind.Interference:
                {
                    string cardName = intent.CardId;
                    if (_catalog.TryGet(intent.CardId, out CardData card)) cardName = card.Name;
                    p.CardName = cardName;
                    string pile = intent.Pile == PileKind.Discard ? "discard pile" : "draw pile";
                    p.Label = "Interference " + amount + " " + cardName + " -> " + pile;
                    p.Math = "Adds " + amount + " " + cardName + " to your " + pile
                        + (intent.Pile == PileKind.Draw ? ", each shuffled in at a random position." : ".");
                    break;
                }

                case IntentKind.Trending:
                    p.Label = "Trending " + amount;
                    p.Math = "Rival gains Buzz " + amount + ": its steals take " + NumberFormat.Int(_b.BuzzStealBonusPercent) + "% more while Buzz is above 0.";
                    break;
            }

            return p;
        }

        private void PerformIntent()
        {
            IntentPreview p = PreviewIntent();
            IntentData intent = CurrentIntent;
            switch (intent.Kind)
            {
                case IntentKind.Steal:
                case IntentKind.BigSegment:
                    Add(_rival.Name + ": " + p.Label + " (" + p.Math + ")");
                    for (int h = 0; h < p.Hits; h++)
                    {
                        StealHit(p.PerHit, p.Hits > 1 ? "hit " + NumberFormat.Int(h + 1) : "steal");
                        if (Listeners <= 0) return;
                    }

                    break;
                case IntentKind.CommercialBreak:
                    Add(_rival.Name + ": Commercial Break. It skips its move.");
                    break;
                case IntentKind.Contest:
                {
                    int old = RivalAudience;
                    RivalAudience = Math.Min(RivalStartingAudience, RivalAudience + intent.Amount);
                    Add(_rival.Name + ": Contest " + NumberFormat.Int(intent.Amount) + ", regains " + NumberFormat.Int(RivalAudience - old)
                        + " audience (" + NumberFormat.Int(old) + " -> " + NumberFormat.Int(RivalAudience) + ", never above " + NumberFormat.Int(RivalStartingAudience) + ").");
                    break;
                }

                case IntentKind.HeavyRotation:
                    Lock += intent.Amount;
                    Add(_rival.Name + ": Heavy Rotation, gains " + NumberFormat.Int(intent.Amount) + " Lock (Lock " + NumberFormat.Int(Lock) + ").");
                    break;
                case IntentKind.Interference:
                    Add(_rival.Name + ": " + p.Label + ".");
                    AddCards(intent.CardId, intent.Pile, intent.Amount, _rival.Name);
                    break;
                case IntentKind.Trending:
                    Buzz += intent.Amount;
                    Add(_rival.Name + ": Trending, gains Buzz " + NumberFormat.Int(intent.Amount) + " (Buzz " + NumberFormat.Int(Buzz) + ").");
                    break;
            }
        }

        private void StealHit(int amount, string label)
        {
            int absorbed = Math.Min(Loyalty, amount);
            Loyalty -= absorbed;
            int lost = amount - absorbed;
            int old = Listeners;
            Listeners = Math.Max(0, Listeners - lost);
            Add("  " + label + " " + NumberFormat.Int(amount) + ": Loyalty absorbs " + NumberFormat.Int(absorbed) + " (Loyalty " + NumberFormat.Int(Loyalty)
                + " left), you lose " + NumberFormat.Int(lost) + " listeners (" + NumberFormat.Int(old) + " -> " + NumberFormat.Int(Listeners) + ").");
        }

        private void RatingsCheck()
        {
            // Shares compared by cross-multiplying: exact, no floating point.
            long you = (long)Listeners * RivalStartingAudience;
            long them = (long)RivalAudience * MaxListeners;
            string shares = "you " + NumberFormat.Int(Listeners) + "/" + NumberFormat.Int(MaxListeners) + " = " + NumberFormat.Share(Listeners, MaxListeners)
                + ", " + _rival.Name + " " + NumberFormat.Int(RivalAudience) + "/" + NumberFormat.Int(RivalStartingAudience) + " = " + NumberFormat.Share(RivalAudience, RivalStartingAudience);
            if (you >= them)
            {
                Add("Ratings check: " + shares + ". " + (you == them ? "A tie goes to you. " : "") + "You win the daypart.");
                End(FightOutcome.WonRatings, false);
                return;
            }

            Add("Ratings check: " + shares + ". " + _rival.Name + " wins the daypart.");
            if (Kind == FightKind.Boss)
            {
                Add("Losing a boss on ratings ends the run.");
                End(FightOutcome.LostRatings, true);
                return;
            }

            // Ratings loss: lose 10% of current listeners, rounded up; maximum unchanged.
            int penalty = (Listeners * _b.RatingsLossPenaltyPercent + CardRules.PercentBase - 1) / CardRules.PercentBase;
            RatingsLossPenalty = penalty;
            int old = Listeners;
            Listeners = Math.Max(0, Listeners - penalty);
            Add("Ratings loss: no rewards, lose " + NumberFormat.Int(_b.RatingsLossPenaltyPercent) + "% of " + NumberFormat.Int(old) + " rounded up = "
                + NumberFormat.Int(penalty) + " listeners (" + NumberFormat.Int(old) + " -> " + NumberFormat.Int(Listeners) + "). Max stays " + NumberFormat.Int(MaxListeners) + ".");
            End(FightOutcome.LostRatings, Listeners <= 0);
            if (Listeners <= 0)
            {
                Add("Your listeners hit 0: the run ends.");
            }
        }

        // ---------------------------------------------------------------- debug commands (prototype)

        public CommandResult DebugDraw()
        {
            if (Phase != FightPhase.Player) return CommandResult.Fail("Not in your turn.");
            DrawCards(1, "[debug] Draw 1");
            Changed();
            return CommandResult.Success;
        }

        public CommandResult DebugWin()
        {
            if (IsOver) return CommandResult.Fail("The fight is over.");
            Add("[debug] Rival audience set to 0.");
            RivalAudience = 0;
            CheckEnd();
            Changed();
            return CommandResult.Success;
        }

        public CommandResult DebugLose()
        {
            if (IsOver) return CommandResult.Fail("The fight is over.");
            Add("[debug] Listeners set to 0.");
            Listeners = 0;
            CheckEnd();
            Changed();
            return CommandResult.Success;
        }

        /// <summary>Adds a note to the log without changing state (e.g. the debug seed key).</summary>
        public void AddNote(string note)
        {
            Add(note);
            Changed();
        }

        // ---------------------------------------------------------------- Reach

        /// <summary>The Reach this card would deal if played now (for the bot and card previews).</summary>
        public int PreviewReach(int uid, out string math)
        {
            math = null;
            CardInstance inst = FindPlayable(uid, out _);
            if (inst == null || !DealsReach(inst.Card)) return 0;
            int stacks = inst.Card.IsSong ? BurnoutStacksFor(inst.Card.Name) + (FindBurnout(inst.Card.Name) != null ? 1 : 0) : 0;
            ReachCalc calc = ComputeReach(inst.Card, stacks, AnthemApplies(inst.Card));
            math = calc.Math;
            return calc.Reach;
        }

        private static bool DealsReach(CardData c)
        {
            return c.IsSong || c.Reach > 0 || c.Keywords.Drop > 0;
        }

        private bool AnthemApplies(CardData c)
        {
            return c.IsSong && c.Keywords.Anthem && _format.Signature == Signature.Anthem && !AnthemUsedThisTurn;
        }

        /// <summary>
        /// Reach as one number (combat.md, Reach resolution; flow.md, Drop). Exact integer math:
        /// every factor is a percent, one floor at the end.
        /// </summary>
        private ReachCalc ComputeReach(CardData c, int burnoutStacks, bool anthem)
        {
            const int P = CardRules.PercentBase;
            long num;
            long den = 1;
            var expr = new StringBuilder();
            int drop = c.Keywords.Drop;
            int hype = c.IsSong ? Hype : 0;

            if (drop > 0)
            {
                // Drop: (base + Hype if Song + N x Flow), no Flow multiplier.
                num = c.Reach + hype + drop * Flow;
                expr.Append("(").Append(NumberFormat.Int(c.Reach));
                if (hype > 0) expr.Append(" + ").Append(NumberFormat.Int(hype)).Append(" Hype");
                expr.Append(" + ").Append(NumberFormat.Int(drop)).Append(" x ").Append(NumberFormat.Int(Flow)).Append(" Flow)");
            }
            else if (c.IsSong)
            {
                num = c.Reach + hype;
                if (hype > 0) expr.Append("(").Append(NumberFormat.Int(c.Reach)).Append(" + ").Append(NumberFormat.Int(hype)).Append(" Hype)");
                else expr.Append(NumberFormat.Int(c.Reach));
                int flowPct = P + _b.FlowReachPerStackPercent * Flow;
                num *= flowPct;
                den *= P;
                if (Flow > 0) expr.Append(" x ").Append(NumberFormat.Factor(flowPct)).Append(" Flow");
            }
            else
            {
                // Non-song: skips Hype and the Flow multiplier.
                num = c.Reach;
                expr.Append(NumberFormat.Int(c.Reach));
            }

            if (anthem)
            {
                num *= _b.AnthemReachPercent;
                den *= P;
                expr.Append(" x ").Append(NumberFormat.Factor(_b.AnthemReachPercent)).Append(" Anthem");
            }

            if (c.IsSong && burnoutStacks > 0)
            {
                int pct = Math.Max(_b.BurnoutFloorPercent, P - _b.BurnoutPerStackPercent * burnoutStacks);
                num *= pct;
                den *= P;
                expr.Append(" x ").Append(NumberFormat.Factor(pct)).Append(" Burnout ").Append(NumberFormat.Int(burnoutStacks));
            }

            if (IsRivalExposed)
            {
                num *= _b.ExposedPercent;
                den *= P;
                expr.Append(" x ").Append(NumberFormat.Factor(_b.ExposedPercent)).Append(" Exposed");
            }

            int reach = (int)(num / den);
            return new ReachCalc
            {
                Reach = reach,
                Math = expr + " = " + NumberFormat.Int(reach) + " Reach",
                Anthem = anthem,
                BurnoutStacks = burnoutStacks,
            };
        }

        /// <summary>Applying Reach (combat.md): Lock first, then audience, then listener conversion. One hit.</summary>
        private void ApplyReach(string source, ReachCalc calc)
        {
            int reach = calc.Reach;
            int absorbed = Math.Min(Lock, reach);
            Lock -= absorbed;
            int through = reach - absorbed;
            int oldAudience = RivalAudience;
            RivalAudience = Math.Max(0, RivalAudience - through);
            int pct = _format.Signature == Signature.Mailbag ? _b.MailbagConversionPercent : _b.ListenerConversionPercent;
            int converted = through * pct / CardRules.PercentBase;
            int gained = Math.Min(converted, MaxListeners - Listeners);
            if (gained < 0) gained = 0;
            Listeners += gained;

            var line = new StringBuilder();
            line.Append(source).Append(": ").Append(calc.Math);
            if (absorbed > 0) line.Append(", Lock absorbs ").Append(NumberFormat.Int(absorbed));
            line.Append(", ").Append(NumberFormat.Int(through)).Append(" to the rival, +").Append(NumberFormat.Int(gained)).Append(" listeners");
            line.Append(" (").Append(NumberFormat.Int(pct)).Append("% of ").Append(NumberFormat.Int(through)).Append(" = ").Append(NumberFormat.Int(converted));
            if (gained < converted) line.Append(", capped at max ").Append(NumberFormat.Int(MaxListeners));
            line.Append("). Rival audience ").Append(NumberFormat.Int(oldAudience)).Append(" -> ").Append(NumberFormat.Int(RivalAudience)).Append(".");
            Add(line.ToString());

            ReachDealt?.Invoke(new ReachResult
            {
                Source = source,
                Reach = reach,
                Absorbed = absorbed,
                Through = through,
                ListenersGained = gained,
                ListenersBeforeCap = converted,
            });
        }

        // ---------------------------------------------------------------- Flow

        private void UpdateFlowFromSong(CardData c)
        {
            int tempo = c.Tempo + PendingTempoBonus;
            if (PendingTempoBonus > 0)
            {
                Add(c.DisplayName + " counts as tempo " + NumberFormat.Int(tempo) + " for the ramp (+" + NumberFormat.Int(PendingTempoBonus) + " tempo bonus used).");
                PendingTempoBonus = 0;
            }

            bool onGenre = _format.Genre != Genre.None && c.Genre == _format.Genre;
            if (LastSong == null)
            {
                Add(c.DisplayName + " has no last song to follow: no Flow gain (Flow " + NumberFormat.Int(Flow) + ").");
            }
            else
            {
                bool ramp = tempo > _lastSongRampTempo;
                // Loop (brief §5): T1 right after T5 ramps. Proposed (formats.md): an Air-Horned T5
                // counts as 6 and still wraps. Pending user decision (formats.md: Loop + Air Horn):
                // the wrap checks the new song's own tempo 1 (an Air-Horned T1 still wraps) against the
                // last song's ramp tempo (an Air-Horned T4 counted as 5 lets a T1 wrap).
                bool loopWrap = _format.Signature == Signature.Loop && _lastSongRampTempo >= CardRules.TempoMax && c.Tempo == CardRules.TempoMin;
                bool streak = onGenre && LastSong.Genre == _format.Genre;
                int gain = 0;
                var reasons = new List<string>();
                if (ramp || loopWrap)
                {
                    gain += _b.FlowRampGain;
                    reasons.Add("tempo ramp T" + NumberFormat.Int(_lastSongRampTempo) + " to T" + NumberFormat.Int(tempo) + (loopWrap && !ramp ? " (Loop)" : ""));
                }

                if (streak)
                {
                    gain += _b.FlowStreakGain;
                    reasons.Add("genre streak (" + CardTextGenerator.GenreName(_format.Genre) + ")");
                }

                if (gain > 0)
                {
                    SetFlow(Flow + gain, c.DisplayName + ": " + string.Join(" + ", reasons));
                }
                else
                {
                    Add(c.DisplayName + ": no ramp (T" + NumberFormat.Int(tempo) + " is not above T" + NumberFormat.Int(_lastSongRampTempo)
                        + ")" + (onGenre ? "" : ", no genre streak") + ". Flow stays " + NumberFormat.Int(Flow) + ".");
                }
            }

            GenreStreakCount = onGenre ? GenreStreakCount + 1 : 0;
            LastSong = c;
            _lastSongRampTempo = tempo;
        }

        private void SetFlow(int value, string reason)
        {
            int old = Flow;
            int clamped = Math.Max(0, Math.Min(FlowCap, value));
            Flow = clamped;
            string capNote = value > FlowCap ? " (cap " + NumberFormat.Int(FlowCap) + ")" : "";
            if (old == clamped)
            {
                Add("Flow stays " + NumberFormat.Int(Flow) + capNote + " (" + reason + ").");
            }
            else
            {
                Add("Flow " + NumberFormat.Int(old) + " -> " + NumberFormat.Int(Flow) + capNote + " (" + reason + ").");
                FlowChanged?.Invoke(old, Flow, reason);
            }
        }

        // ---------------------------------------------------------------- effects and resources

        private void ApplyEffect(EffectData e, string source)
        {
            switch (e.Kind)
            {
                case EffectKind.DrawCards:
                    DrawCards(e.Amount, source + ": draw " + NumberFormat.Int(e.Amount));
                    break;
                case EffectKind.GainFlow:
                    SetFlow(Flow + e.Amount, source + ": +" + NumberFormat.Int(e.Amount) + " Flow");
                    break;
                case EffectKind.GainHype:
                    GainHype(e.Amount, source);
                    break;
                case EffectKind.GainLoyalty:
                    GainLoyalty(e.Amount, source);
                    break;
                case EffectKind.GainCash:
                    GainCash(e.Amount, source);
                    break;
                case EffectKind.LoseListeners:
                    LoseListeners(e.Amount, source);
                    break;
                case EffectKind.MonitorIntents:
                    MonitorIntentCount = Math.Max(MonitorIntentCount, e.Amount);
                    Add(source + ": Monitor " + NumberFormat.Int(e.Amount) + ", you see the rival's next " + NumberFormat.Int(e.Amount) + " intents this daypart.");
                    break;
                case EffectKind.MonitorDraw:
                {
                    MonitorDrawCount = Math.Max(MonitorDrawCount, e.Amount);
                    List<CardInstance> top = PeekDraw(e.Amount);
                    _monitoredDraw.Clear();
                    _monitoredDraw.AddRange(top);
                    var names = new List<string>();
                    for (int i = 0; i < top.Count; i++) names.Add(top[i].Card.DisplayName);
                    Add(source + ": Monitor " + NumberFormat.Int(e.Amount) + ", top of your draw pile: " + (names.Count > 0 ? string.Join(", ", names) : "(empty)") + ".");
                    break;
                }

                case EffectKind.NextSongTempoBonus:
                    // Proposed (flow.md): bonuses stack (two Air Horns = +2) and survive an Ad;
                    // the next Song consumes them.
                    PendingTempoBonus += e.Amount;
                    Add(source + ": your next Song counts as +" + NumberFormat.Int(PendingTempoBonus) + " tempo for the ramp.");
                    break;
                case EffectKind.RaiseFlowCap:
                    FlowCap += e.Amount;
                    Add(source + ": Flow cap raised to " + NumberFormat.Int(FlowCap) + " this daypart.");
                    break;
                case EffectKind.AddCardToPile:
                    AddCards(e.CardId, e.Pile, e.Amount, source);
                    break;
            }
        }

        private void GainLoyalty(int amount, string source)
        {
            Loyalty += amount;
            Add(source + ": +" + NumberFormat.Int(amount) + " Loyalty (Loyalty " + NumberFormat.Int(Loyalty) + ").");
        }

        /// <summary>Proposed (flow.md): Hype has no cap.</summary>
        private void GainHype(int amount, string source)
        {
            Hype += amount;
            Add(source + ": +" + NumberFormat.Int(amount) + " Hype (Hype " + NumberFormat.Int(Hype) + ").");
        }

        private void GainCash(int amount, string source)
        {
            Cash += amount;
            Add(source + ": +" + NumberFormat.Int(amount) + " Cash (Cash " + NumberFormat.Int(Cash) + ").");
        }

        /// <summary>Every listener loss except rival steals ignores Loyalty (combat.md, Decided).</summary>
        private void LoseListeners(int amount, string source)
        {
            int old = Listeners;
            Listeners = Math.Max(0, Listeners - amount);
            Add(source + ": lose " + NumberFormat.Int(amount) + " listeners, ignoring Loyalty (" + NumberFormat.Int(old) + " -> " + NumberFormat.Int(Listeners) + ").");
        }

        private int DeadAirLossFor(int occurrence)
        {
            int index = Math.Min(occurrence, _deadAirSteps.Length) - 1;
            return _deadAirSteps[Math.Max(0, index)];
        }

        private int RegisterBurnout(string name)
        {
            BurnoutEntry e = FindBurnout(name);
            if (e == null)
            {
                e = new BurnoutEntry(name);
                _burnout.Add(e);
            }

            e.Plays++;
            if (e.Stacks > 0)
            {
                Add(name + " repeat play: Burnout " + NumberFormat.Int(e.Stacks) + ".");
            }

            return e.Stacks;
        }

        private BurnoutEntry FindBurnout(string name)
        {
            for (int i = 0; i < _burnout.Count; i++)
            {
                if (_burnout[i].Name == name) return _burnout[i];
            }

            return null;
        }

        private void FireGear(GearTrigger trigger, CardData played)
        {
            for (int i = 0; i < _gear.Count; i++)
            {
                GearData g = _gear[i];
                if (g.Trigger != trigger) continue;
                string label;
                if (trigger == GearTrigger.CardPlayed)
                {
                    if (played == null || played.Type != g.TriggerCardType) continue;
                    label = g.Name;
                }
                else if (trigger == GearTrigger.FirstCardOfTypeEachTurn)
                {
                    if (played == null || played.Type != g.TriggerCardType || _gearFiredThisTurn[i]) continue;
                    _gearFiredThisTurn[i] = true;
                    label = g.Name + " (first " + CardTextGenerator.TypeName(g.TriggerCardType) + " this turn)";
                }
                else
                {
                    label = g.Name;
                }

                for (int e = 0; e < g.Effects.Count; e++)
                {
                    ApplyEffect(g.Effects[e], label);
                }
            }
        }

        // ---------------------------------------------------------------- piles

        private void DrawCards(int count, string source)
        {
            var names = new List<string>();
            for (int i = 0; i < count; i++)
            {
                if (_draw.Count == 0)
                {
                    if (_discard.Count == 0)
                    {
                        FlushDraws(source, names);
                        Add(source + ": the draw pile and discard pile are both empty, so drawing stops.");
                        return;
                    }

                    FlushDraws(source, names);
                    int n = _discard.Count;
                    _draw.AddRange(_discard);
                    _discard.Clear();
                    Shuffle(_draw);
                    Add("Draw pile empty: shuffled " + NumberFormat.Int(n) + " discarded cards into the draw pile (rng.shuffle).");
                }

                CardInstance inst = _draw[0];
                _draw.RemoveAt(0);
                // Proposed (combat.md): the cued card doesn't count toward the hand limit.
                if (_hand.Count >= _b.HandLimit)
                {
                    FlushDraws(source, names);
                    _discard.Add(inst);
                    Add("Hand is full (" + NumberFormat.Int(_b.HandLimit) + "): " + inst.Card.DisplayName + " goes to the discard pile.");
                    continue;
                }

                _hand.Add(inst);
                names.Add(inst.Card.DisplayName);
            }

            FlushDraws(source, names);
        }

        private void FlushDraws(string source, List<string> names)
        {
            if (names.Count == 0) return;
            Add(source + ": drew " + string.Join(", ", names) + ".");
            names.Clear();
        }

        private void AddCards(string cardId, PileKind pile, int count, string source)
        {
            CardData card = _catalog.Get(cardId);
            for (int i = 0; i < count; i++)
            {
                var inst = new CardInstance(_nextUid++, card);
                _all.Add(inst);
                if (pile == PileKind.Draw)
                {
                    // Proposed (statuses.md): shuffled in at a random position on rng.shuffle.
                    // An empty draw pile simply becomes this card (rivals.md edge case).
                    int pos = _shuffle.NextInt(_draw.Count + 1);
                    _draw.Insert(pos, inst);
                    Add(source + ": " + card.Name + " shuffled into your draw pile (position " + NumberFormat.Int(pos + 1) + " of "
                        + NumberFormat.Int(_draw.Count) + " from the top, rng.shuffle).");
                }
                else if (pile == PileKind.Hand && _hand.Count < _b.HandLimit)
                {
                    _hand.Add(inst);
                    Add(source + ": " + card.Name + " added to your hand.");
                }
                else
                {
                    _discard.Add(inst);
                    Add(source + ": " + card.Name + " added to your discard pile.");
                }
            }

            Add("Static count: " + NumberFormat.Int(StaticCount) + ".");
        }

        private void Shuffle(List<CardInstance> pile)
        {
            for (int i = pile.Count - 1; i > 0; i--)
            {
                int j = _shuffle.NextInt(i + 1);
                CardInstance tmp = pile[i];
                pile[i] = pile[j];
                pile[j] = tmp;
            }
        }

        private CardInstance FindPlayable(int uid, out bool fromCue)
        {
            CardInstance inst = FindIn(_hand, uid);
            if (inst != null)
            {
                fromCue = false;
                return inst;
            }

            inst = FindIn(_cue, uid);
            fromCue = inst != null;
            return inst;
        }

        private static CardInstance FindIn(List<CardInstance> pile, int uid)
        {
            for (int i = 0; i < pile.Count; i++)
            {
                if (pile[i].Uid == uid) return pile[i];
            }

            return null;
        }

        private static int CountInterference(List<CardInstance> pile)
        {
            int n = 0;
            for (int i = 0; i < pile.Count; i++)
            {
                if (pile[i].IsInterference) n++;
            }

            return n;
        }

        // ---------------------------------------------------------------- win / loss

        /// <summary>Win/loss check (brief §2). Listeners at 0 ends the run immediately; rival at 0 wins.</summary>
        private bool CheckEnd()
        {
            if (Phase == FightPhase.Over) return true;
            // Pending user decision (combat.md: simultaneous 0): if one card drops both your listeners
            // and the rival's audience to 0, the loss is checked first.
            if (Listeners <= 0)
            {
                Add("Your listeners hit 0: the run ends.");
                End(FightOutcome.LostListenersZero, true);
                return true;
            }

            if (RivalAudience <= 0)
            {
                Add(_rival.Name + "'s audience hit 0: you win the daypart.");
                End(FightOutcome.WonAudienceZero, false);
                return true;
            }

            return false;
        }

        private void End(FightOutcome outcome, bool runEnded)
        {
            Phase = FightPhase.Over;
            Outcome = outcome;
            RunEnded = runEnded;
            Airtime = 0;
            FightEnded?.Invoke(outcome);
        }

        private void Add(string line)
        {
            Log.Add(line);
        }

        private void Changed()
        {
            StateChanged?.Invoke();
        }
    }
}
