using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Biomes2Core
{
    public class CompProperties_WaterAnimal : CompProperties
    {
        public List<TerrainDef> allowedTiles = new List<TerrainDef>();
        public float spawnChance = 0.1f;
        public List<string> allowedBiomes = new List<string>();
        public GraphicData submergedGraphic = null;
        public bool submergeInWater = true;
        public CompProperties_WaterAnimal()
        {
            compClass = typeof(CompWaterAnimal);
        }

    }
}
