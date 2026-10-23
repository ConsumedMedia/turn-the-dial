using System.Collections.Generic;
using System.IO;
using TurnTheDial.Core.Content;
using TurnTheDial.Core.Sim;
using UnityEditor;
using UnityEngine;

namespace TurnTheDial.Data.Editor
{
    /// <summary>
    /// Editor entry point for tools/sim (there is no .NET SDK on this machine, so the headless
    /// Core runner is driven from here). Exports the content assets to Core data, runs
    /// FightSimulator, and writes the report under tools/sim/reports/.
    /// </summary>
    public static class SimRunner
    {
        public const string ContentPath = "Assets/TurnTheDial/Content/ProtoCombatContent.asset";
        public const ulong DefaultFirstSeed = 1;
        public const int DefaultSeedCount = 5;

        [MenuItem("Turn the Dial/Sim/Run Proto Combat (5 seeds)")]
        public static void RunMenu()
        {
            string path = Run(DefaultFirstSeed, DefaultSeedCount, out _);
            Debug.Log("Sim report written: " + path);
        }

        [MenuItem("Turn the Dial/Content/Validate Proto Combat Content")]
        public static void ValidateMenu()
        {
            FightContent content = AssetDatabase.LoadAssetAtPath<FightContent>(ContentPath);
            List<string> errors = ContentExporter.Validate(content);
            if (errors.Count == 0) Debug.Log("Content valid: " + ContentPath);
            for (int i = 0; i < errors.Count; i++) Debug.LogError(errors[i]);
        }

        public static string ReportsFolder()
        {
            // Application.dataPath = <repo>/unity/Assets
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "tools", "sim", "reports"));
        }

        /// <summary>Runs the sim and returns the Markdown report without writing anything.</summary>
        public static string BuildReport(ulong firstSeed, int count, out List<SimFightResult> results, out string fileName)
        {
            FightContent content = AssetDatabase.LoadAssetAtPath<FightContent>(ContentPath);
            if (content == null) throw new FileNotFoundException("Missing content asset", ContentPath);
            var policy = new GreedyPolicy();
            results = FightSimulator.RunMany(seed => ContentExporter.BuildSetup(content, seed), firstSeed, count, policy);
            string title = "Proto combat sim: " + content.format.displayName + " vs " + content.rival.displayName
                + ", " + NumberFormat.Int(count) + " seeds from " + firstSeed;
            fileName = "proto_combat_" + content.rival.id.Replace('.', '_') + "_seeds_" + firstSeed + "_x" + count + ".md";
            return FightSimulator.Report(title, policy.Name, results, true);
        }

        /// <summary>Runs the sim and writes the report to tools/sim/reports/ (menu use only, never from tests).</summary>
        public static string Run(ulong firstSeed, int count, out List<SimFightResult> results)
        {
            string report = BuildReport(firstSeed, count, out results, out string fileName);
            string folder = ReportsFolder();
            Directory.CreateDirectory(folder);
            string file = Path.Combine(folder, fileName);
            File.WriteAllText(file, report);
            return file;
        }
    }
}
