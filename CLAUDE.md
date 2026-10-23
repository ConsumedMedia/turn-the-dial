# Turn the Dial: Project Rules

Roguelike deckbuilder about running a local radio station. Consumed Media. Unity 6 (URP), Steam.
These rules apply to every session. If a rule here blocks a request, say so and ask.

## Read order at session start
1. `CLAUDE.md` (this file)
2. `docs/GAME_BRIEF.md`
3. `docs/LESSONS.md`
4. `docs/NEXT.md`
5. The design doc in `docs/design/` for the current task

## Source of truth
- `docs/GAME_BRIEF.md` wins over everything: design docs, `docs/CONCEPT.md`, code, and memory.
- Design docs add detail the brief leaves open. They never contradict it.
- If a request conflicts with the brief or a design doc, **stop and ask**. Never guess.
- If the brief is silent, the design pillars (brief section 13) settle it. If they don't, ask.
- Anything not in the brief is out of scope until it is added there.

## Architecture
- **TurnTheDial.Core**: a pure C# assembly holding every game rule: combat, cards, Flow,
  statuses, rivals, map, economy, meta, save data. Its asmdef has `noEngineReferences: true`.
  No `UnityEngine`, no `MonoBehaviour`, no `Time`, no `Debug.Log`.
- **TurnTheDial.Game**: Unity code. It only reads Core state and sends player actions
  (play card, cue, end turn, pick node, buy). It never decides a rule.
- **TurnTheDial.Data**: ScriptableObject definitions and their exporters to Core's plain data.
- The rules must run headless: `tools/sim` and the Core tests run a full run with no Unity scene.
- Core exposes state as read-only views and changes it only through commands. Game listens to
  Core events (card played, Reach dealt, Flow changed) to drive animation and sound.

## Determinism
- Every random roll goes through one seeded RNG service in Core (`IRng`). No `System.Random`,
  `UnityEngine.Random`, `Guid`, or clock time anywhere else.
- Same seed plus same choices gives the same run, bit for bit. Saves store the seed and RNG state.
- Separate named streams (map, rewards, shop, shuffle, rivals, events) so one system's rolls don't shift
  another's. Each stream is derived from the run seed.
- Iterate collections in a defined order. Never rely on `Dictionary` or `HashSet` order.

## Data
- Every card, rival, gear piece, staff member, event, format, and market is a ScriptableObject
  that exports to a plain C# data form Core can read (no Unity types in it).
- Card behavior comes from data (keywords plus an ordered effect list). No per-card code.
- No magic numbers in code. Every tunable value (brief values marked *(tune)*) lives in data or a
  balance config asset, and its starting number matches the design doc.
- Ids are stable strings (`card.cr.album_cut`). Never rename a shipped id; saves reference them.
- Every new piece of content gets a row in `docs/CONTENT.md`.

## Unity rules
- Unity 6 (6000.6), URP, new Input System only (no legacy `Input.*`).
- Namespaces start with `TurnTheDial.` and match the folder path.
- One MonoBehaviour per file; the file name matches the class name.
- Every screen works with mouse, keyboard, and gamepad, with no exceptions. Test all three.
- No text below 12 px at 1280×800 (Steam Deck).
- Scenes hold layout only. Logic lives in code and data.

## Blender rules
- 1 unit = 1 meter.
- Apply all transforms (location, rotation, scale) before export.
- Put pivots where parts rotate or slide: platters, faders, needles, tuner, knobs, doors.
- Export FBX with **-Z forward** and **Y up**.
- Name meshes `SM_` and materials `M_` (e.g. `SM_Turntable`, `M_Palette`).
- Every prop maps its faces to the one shared palette texture. No other textures without asking.
- Source `.blend` files live in `blender/`. Exported FBX goes to `assets/clean/` and then Unity.

## Art rules
- References live in `docs/refs`: booth keys in `booth/`, town keys at the top level, style
  references in `style/`. Take the principles, not the elements; every model is our own.
- **The color rule in GAME_BRIEF.md section 11 is a hard requirement.** You are a steady round
  amber dot on the left; the rival is a blinking teal diamond on the right. Every you-versus-rival
  signal uses color, shape, side, and motion. Red means danger only.
- Every daypart wash must pass a grayscale check before it is done.
- Where a booth key differs from the color rule, the color rule wins.

## Testing
- Every rule in Core gets a unit test: NUnit via the Unity Test Framework, EditMode, in
  `TurnTheDial.Core.Tests`. Test the exact numbers from the brief and design doc.
- Edge cases listed in a design doc each get their own test.
- A system isn't done until its tests pass and its design doc's done checklist is ticked.
- Never delete or weaken a failing test to make it pass. Fix the code or ask.

## Workflow
- One system per session. Name it at the start; update `docs/NEXT.md` at the end.
- Play after every change: run the scene (or the sim for Core-only work) and confirm it works.
- Commit after every working step. Always give me the git commit command to run.
- Keep commits small and focused; the message says what changed and why.
- Don't start the next system until the current one is done.

## Review
- The builder never approves its own work.
- The reviewer agent (`.claude/agents/reviewer.md`) signs off each phase against the brief, the
  design doc, this file, and the tests. Its findings are fixed before the phase is called done.
- After every phase, call the reviewer agent. Do not move on until it returns APPROVED. Log
  anything it caught in docs/LESSONS.md.

## Lessons
- Every bug fix, and every correction from me, gets one line in `docs/LESSONS.md`:
  `- <what went wrong> → <the rule that prevents it>`
- Read LESSONS.md at the start of every session and follow it like this file.

## Folder map
```
turn-the-dial/
├── CLAUDE.md                  This file
├── .claude/agents/            Agent definitions (game-designer, unity-builder, blender-artist,
│                              balance-analyst, reviewer)
├── docs/
│   ├── GAME_BRIEF.md          Single source of truth
│   ├── CONCEPT.md             Original approved concept (history only)
│   ├── LESSONS.md             One line per mistake and the rule that prevents it
│   ├── NEXT.md                What the next session picks up
│   ├── CONTENT.md             Every card, rival, gear, staff, event, and its status
│   ├── design/                One design doc per system
│   └── refs/                  Town keys (top level), booth/ keys, style/ references
├── tools/sim/                 Headless balance simulator, runs on TurnTheDial.Core
├── blender/                   Source .blend files
├── assets/
│   ├── raw/                   Untouched Tripo and generator output
│   └── clean/                 Cleaned FBX and palette texture ready for Unity
├── audio/
│   ├── raw/                   Source loops and foley
│   └── clean/                 Trimmed, normalized files ready for Unity
└── unity/                     Unity 6 project
    └── Assets/
        ├── TurnTheDial/
        │   ├── Core/          TurnTheDial.Core: pure C# rules, no UnityEngine
        │   ├── Data/          TurnTheDial.Data: ScriptableObjects and exporters
        │   ├── Game/          TurnTheDial.Game: MonoBehaviours, UI, input, presentation
        │   ├── Content/       ScriptableObject assets (cards, rivals, gear, staff, events...)
        │   ├── Art/           Models, materials, palette texture, VFX
        │   ├── Audio/         Imported loops and foley
        │   └── Scenes/        Boot, Title, Map, Booth, Diorama scenes
        └── Tests/
            └── Core/          TurnTheDial.Core.Tests (EditMode, NUnit)
```
