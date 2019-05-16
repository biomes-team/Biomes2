using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Biomes2Caves
{
    class SiteCoreWorker_SkulltopSprout : SiteCoreWorker
    {
        private static readonly IntRange countRange = new IntRange(5, 15);

        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);
            if (!tryFindRootCell(map, out IntVec3 root))
            {
                return;
            }
            for (int i = 0; i < countRange.RandomInRange; i++)
            {
                if (!CellFinder.TryRandomClosewalkCellNear(root, map, 6, out IntVec3 cell, (IntVec3 x) => canSpawnAt(x, map)))
                {
                    break;
                }
                Plant plant = cell.GetPlant(map);
                if (plant != null)
                {
                    plant.Destroy(DestroyMode.Vanish);
                }
                GenSpawn.Spawn(CavesDefOf.Biomes_Skulltop, cell, map);
            }
        }

        private bool tryFindRootCell(Map map, out IntVec3 cell) => CellFinderLoose.TryFindRandomNotEdgeCellWith(10, (IntVec3 x) => canSpawnAt(x, map) && x.GetRoom(map, RegionType.Set_Passable).CellCount >= 64, map, out cell);

        private bool canSpawnAt(IntVec3 c, Map map)
        {
            if (!c.Standable(map) || c.Fogged(map) || map.fertilityGrid.FertilityAt(c) < CavesDefOf.Biomes_Skulltop.plant.fertilityMin || !c.GetRoom(map, RegionType.Set_Passable).PsychologicallyOutdoors || c.GetEdifice(map) != null || !PlantUtility.GrowthSeasonNow(c, map, false))
            {
                return false;
            }
            Plant plant = c.GetPlant(map);
            if (plant != null && plant.def.plant.growDays > 10f)
            {
                return false;
            }
            List<Thing> thingList = c.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i].def == CavesDefOf.Biomes_Skulltop)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
