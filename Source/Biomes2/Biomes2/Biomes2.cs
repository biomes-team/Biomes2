using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Biomes2
{
    [StaticConstructorOnStartup]
    public static class Biomes2
    {
        // github.com/kfish610
        public const string Id = "rimworld.biomes-2";
        public const string Name = "Biomes! 2";
        public const string Version = "0.1";
        static Biomes2()
        {
            HarmonyInstance.Create(Id).PatchAll();
            LogMessage("Initialized");
        }

        public static void LogMessage(string message) => Verse.Log.Message(prefixMessage(message));
        public static void LogError(string message) => Verse.Log.Error(prefixMessage(message));
        private static string prefixMessage(string message) => $"[{Name} v{Version}] {message}";
    }
}
