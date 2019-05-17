using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Biomes2Core
{
    public class GenStep_WaterAnimal : GenStep
    {
        public override int SeedPart => 925489023;

        public override void Generate(Map map, GenStepParams parms)
        {
            List<PawnKindDef> sources = (from x in DefDatabase<PawnKindDef>.AllDefsListForReading
                                        where x.race.GetCompProperties<CompProperties_WaterAnimal>() != null
                                        && x.race.GetCompProperties<CompProperties_WaterAnimal>().allowedBiomes.Contains(map.Biome.defName)
                                        select x).ToList();

            if (sources == null || sources.Count == 0)
            {
                return;
            }
            foreach (IntVec3 c in map.AllCells)
            {
                PawnKindDef source = sources[Rand.RangeInclusive(0, sources.Count - 1)];
                if (c.GetEdifice(map) == null && c.GetCover(map) == null && c.GetFirstBuilding(map) == null)
                {
                    if (source.race.GetCompProperties<CompProperties_WaterAnimal>().allowedTiles.Contains(c.GetTerrain(map)))
                    {
                        if (Rand.Chance(source.race.GetCompProperties<CompProperties_WaterAnimal>().spawnChance))
                        {
                            int randomInRange = source.wildGroupSize.RandomInRange;
                            int radius = Mathf.CeilToInt(Mathf.Sqrt((float)source.wildGroupSize.max));
                            for (int i = 0; i < randomInRange; i++)
                            {
                                IntVec3 loc2 = CellFinder.RandomClosewalkCellNear(c, map, radius, null);
                                Pawn newThing = PawnGenerator.GeneratePawn(source, null);
                                GenSpawn.Spawn(newThing, loc2, map);
                            }
                        }
                    }
                }
            }
        }
    }
}
