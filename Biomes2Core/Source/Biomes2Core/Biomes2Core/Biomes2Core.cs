using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Biomes2Core
{
    [StaticConstructorOnStartup]
    public static class Biomes2Core
    {
        // github.com/kfish610
        public const string Id = "rimworld.biomes-2-core";
        public const string Name = "Biomes! 2: Core";
        public const string Version = "0.1";
        static Biomes2Core()
        {
            HarmonyInstance.Create(Id).PatchAll();
            LogMessage("Initialized");
        }

        public static void LogMessage(string message) => Log.Message(prefixMessage(message));
        public static void LogError(string message) => Log.Error(prefixMessage(message));
        private static string prefixMessage(string message) => $"[{Name} v{Version}] {message}";
    }
}
