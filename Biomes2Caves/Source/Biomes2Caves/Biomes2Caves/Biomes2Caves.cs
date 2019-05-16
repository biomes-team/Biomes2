using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Biomes2Caves
{
    [StaticConstructorOnStartup]
    public static class Biomes2Caves
    {
        // github.com/kfish610
        public const string Id = "rimworld.biomes-2-caves";
        public const string Name = "Biomes! 2: Caves";
        public const string Version = "0.1";
        static Biomes2Caves()
        {
            LogMessage("Initialized");
        }

        public static void LogMessage(string message) => Log.Message(prefixMessage(message));
        public static void LogError(string message) => Log.Error(prefixMessage(message));
        private static string prefixMessage(string message) => $"[{Name} v{Version}] {message}";
    }
}
