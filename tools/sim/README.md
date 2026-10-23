# tools/sim

Headless balance simulator. The runner is pure Core (`unity/Assets/TurnTheDial/Core/Sim/`:
`FightSimulator`, `GreedyPolicy`); it uses only `TurnTheDial.Core` and no Unity types.

There is no .NET SDK on the build machine, so the runner is driven from Unity:

- Menu: **Turn the Dial > Sim > Run Proto Combat (5 seeds)** (`TurnTheDial.Data.Editor.SimRunner`).
- Tests: the EditMode test category `Sim` (`ContentAssetTests.Sim_ProtoCombat_FiveSeeds_BuildsReport`,
  `DeterminismTests.Sim_MorningZoo_FiveSeeds_Completes`). Tests build the report in memory and never write it; only the menu writes to `reports/`.

Seeds: the first seed is 1; each next seed is `SeedChain.Next(previous)` (Core RNG, no clock time).
Reports are written to `tools/sim/reports/` as Markdown, with the full combat log of the first seed.
