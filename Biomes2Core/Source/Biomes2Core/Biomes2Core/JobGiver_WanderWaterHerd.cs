using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Biomes2Core
{
    public class JobGiver_WanderWaterHerd : JobGiver_WanderWater
    {
        public JobGiver_WanderWaterHerd()
        {
            wanderRadius = 5f;
            ticksBetweenWandersRange = new IntRange(125, 200);
        }

        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            bool validator(Thing t)
            {
                if (((Pawn)t).RaceProps != pawn.RaceProps || t == pawn)
                {
                    return false;
                }
                if (t.Faction != pawn.Faction)
                {
                    return false;
                }
                if (t.Position.IsForbidden(pawn))
                {
                    return false;
                }
                if (!pawn.CanReach(t, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
                {
                    return false;
                }
                if (Rand.Value < 0.5f)
                {
                    return false;
                }
                List<Pawn> allPawnsSpawned = pawn.Map.mapPawns.AllPawnsSpawned;
                for (int i = 0; i < allPawnsSpawned.Count; i++)
                {
                    Pawn pawn2 = allPawnsSpawned[i];
                    if (pawn2.RaceProps.Humanlike && (pawn2.Position - t.Position).LengthHorizontalSquared < 225)
                    {
                        return false;
                    }
                }
                return true;
            }
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 35f, validator, null, 13, -1, false, RegionType.Set_Passable, false);
            if (thing != null)
            {
                return thing.Position;
            }
            return pawn.Position;
        }
    }
}
