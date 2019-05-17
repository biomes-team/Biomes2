using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Biomes2Core
{
    public class CompProperties_WaterPlant : CompProperties
    {
        public List<TerrainDef> allowedTiles = new List<TerrainDef>();
        public float spawnChance = 0.1f;
        public List<String> allowedBiomes = new List<String>();
        public bool growNearWater = false;
        public CompProperties_WaterPlant()
        {
            compClass = typeof(CompWaterPlant);
        }
    }
}
