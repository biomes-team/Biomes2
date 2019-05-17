using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Biomes2Core
{
    public class CompProperties_Regenerate : CompProperties
    {
        public float amount = 0.1f;
        public float startThreshold = 0.2f;
        public float endThreshold = 1f;
        public bool restoreBodyParts = false;
        public int delay = 15;
        public CompProperties_Regenerate()
        {
            compClass = typeof(CompRegenerate);
        }
    }
}
