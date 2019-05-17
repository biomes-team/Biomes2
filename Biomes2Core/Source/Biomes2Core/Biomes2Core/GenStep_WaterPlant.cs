using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Biomes2Core
{
    public class GenStep_WaterPlant : GenStep
    {
        public override int SeedPart => 18506586;

        public override void Generate(Map map, GenStepParams parms)
        {
            List<ThingDef> sources = (from x in DefDatabase<ThingDef>.AllDefsListForReading
                                      where x.category == ThingCategory.Plant && x.GetCompProperties<CompProperties_WaterPlant>() != null
                                      && x.GetCompProperties<CompProperties_WaterPlant>().allowedBiomes.Contains(map.Biome.defName)
                                      select x).ToList();

            if (sources == null || sources.Count == 0)
            {
                return;
            }
            foreach (IntVec3 c in map.AllCells)
            {
                ThingDef source = sources[Rand.RangeInclusive(0, sources.Count - 1)];
                if (c.GetEdifice(map) == null && c.GetCover(map) == null && c.GetFirstBuilding(map) == null)
                {
                    if (source.GetCompProperties<CompProperties_WaterPlant>().allowedTiles.Contains(c.GetTerrain(map)))
                    {
                        if (Rand.Chance(source.GetCompProperties<CompProperties_WaterPlant>().spawnChance))
                        {
                            Plant plant = (Plant)ThingMaker.MakeThing(source, null);
                            plant.Growth = Rand.Range(0.07f, 1f);
                            if (plant.def.plant.LimitedLifespan)
                            {
                                plant.Age = Rand.Range(0, Mathf.Max(plant.def.plant.LifespanTicks - 50, 0));
                            }
                            GenSpawn.Spawn(plant, c, map);
                        }
                    }
                    else
                    {
                        if (source.GetCompProperties<CompProperties_WaterPlant>().growNearWater)
                        {
                            bool flag = false;
                            for (int i = c.x - 1; i < c.x + 2; i++)
                            {
                                for (int j = c.z - 1; j < c.z + 2; j++)
                                {
                                    IntVec3 temp = new IntVec3(i, 0, j);
                                    if (temp.InBounds(map) && source.GetCompProperties<CompProperties_WaterPlant>().allowedTiles.Contains(temp.GetTerrain(map)) && !isWater(temp, map) && !(temp.GetTerrain(map).defName == "Marsh"))
                                    {
                                        if (Rand.Chance(source.GetCompProperties<CompProperties_WaterPlant>().spawnChance))
                                        {
                                            Plant plant = (Plant)ThingMaker.MakeThing(source, null);
                                            plant.Growth = Rand.Range(0.07f, 1f);
                                            if (plant.def.plant.LimitedLifespan)
                                            {
                                                plant.Age = Rand.Range(0, Mathf.Max(plant.def.plant.LifespanTicks - 50, 0));
                                            }
                                            GenSpawn.Spawn(plant, c, map);
                                            flag = true;
                                        }
                                    }
                                    if (flag)
                                    {
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    break;
                                }
                            }
                        }

                    }
                }

            }
        }

        private bool isWater(IntVec3 pos, Map map)
        {
            if (pos.GetTerrain(map).defName.Contains("Water") || pos.GetTerrain(map).defName.Contains("water"))
            {
                return true;
            }
            return false;
        }
    }
}
