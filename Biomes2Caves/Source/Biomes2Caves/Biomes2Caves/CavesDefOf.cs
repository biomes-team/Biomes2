using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Biomes2Caves
{
    [DefOf]
    public static class CavesDefOf
    {
        public static ThingDef Biomes_Skulltop;
        public static SiteCoreDef Biomes_SkulltopSprout;

        static CavesDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(CavesDefOf));
        }
    }
}
