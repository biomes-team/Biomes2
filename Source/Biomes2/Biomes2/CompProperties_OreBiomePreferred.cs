using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using RimWorld;

namespace Biomes2
{
    public class CompProperties_OreBiomePreferred : CompProperties
    {
        public List<BiomeDef> allowedBiomes = new List<BiomeDef>();
        public CompProperties_OreBiomePreferred()
        {
            compClass = typeof(CompOreBiomePreferred);
        }
    }
}
