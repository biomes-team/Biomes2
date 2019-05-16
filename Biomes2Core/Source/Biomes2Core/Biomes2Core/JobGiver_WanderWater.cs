using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Biomes2Core
{
    public abstract class JobGiver_WanderWater : ThinkNode_JobGiver
    {
        protected float wanderRadius;

        protected Func<Pawn, IntVec3, IntVec3, bool> wanderDestValidator;

        protected IntRange ticksBetweenWandersRange = new IntRange(20, 100);

        protected LocomotionUrgency locomotionUrgency = LocomotionUrgency.Walk;

        protected Danger maxDanger = Danger.None;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_WanderWater jobGiver_WanderWater = (JobGiver_WanderWater)base.DeepCopy(resolve);
            jobGiver_WanderWater.wanderRadius = wanderRadius;
            jobGiver_WanderWater.wanderDestValidator = wanderDestValidator;
            jobGiver_WanderWater.ticksBetweenWandersRange = ticksBetweenWandersRange;
            jobGiver_WanderWater.locomotionUrgency = locomotionUrgency;
            jobGiver_WanderWater.maxDanger = maxDanger;
            return jobGiver_WanderWater;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            bool nextOrderIsWait = pawn.mindState.nextMoveOrderIsWait;
            pawn.mindState.nextMoveOrderIsWait = !pawn.mindState.nextMoveOrderIsWait;
            if (nextOrderIsWait)
            {
                return new Job(JobDefOf.Wait_Wander)
                {
                    expiryInterval = ticksBetweenWandersRange.RandomInRange
                };
            }
            IntVec3 dest = GetExactWanderDest(pawn);
            if (!dest.IsValid)
            {
                pawn.mindState.nextMoveOrderIsWait = false;
                return null;
            }
            Job job = new Job(JobDefOf.GotoWander, dest);
            pawn.Map.pawnDestinationReservationManager.Reserve(pawn, job, dest);
            job.locomotionUrgency = locomotionUrgency;
            return job;
        }

        protected virtual IntVec3 GetExactWanderDest(Pawn pawn)
        {
            IntVec3 wanderRoot = GetWanderRoot(pawn);
            if (!wanderRoot.GetTerrain(pawn.Map).defName.Contains("Water"))
            {
                float radius = 12;
                if (TryFindRandomCellNear(wanderRoot, pawn.Map, Mathf.FloorToInt(radius), (IntVec3 c) => c.InBounds(pawn.Map) && pawn.CanReach(c, PathEndMode.OnCell, Danger.None, false, TraverseMode.ByPawn) && !c.IsForbidden(pawn), out IntVec3 position))
                {
                    if (position != wanderRoot)
                    {
                        return position;
                    }
                }
            }
            return RCellFinder.RandomWanderDestFor(pawn, wanderRoot, wanderRadius, wanderDestValidator, PawnUtility.ResolveMaxDanger(pawn, this.maxDanger));
        }

        protected abstract IntVec3 GetWanderRoot(Pawn pawn);

        private static float getDistance(IntVec3 a, IntVec3 b)
        {
            return Mathf.Sqrt(Mathf.Pow((a.x - b.x), 2) + Mathf.Pow((a.z - b.z), 2));
        }

        public static bool TryFindRandomCellNear(IntVec3 root, Map map, int squareRadius, Predicate<IntVec3> validator, out IntVec3 result)
        {
            int num = root.x - squareRadius;
            int num2 = root.x + squareRadius;
            int num3 = root.z - squareRadius;
            int num4 = root.z + squareRadius;
            if (num < 0)
            {
                num = 0;
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num2 > map.Size.x)
            {
                num2 = map.Size.x;
            }
            if (num4 > map.Size.z)
            {
                num4 = map.Size.z;
            }
            IntVec3 intVec = new IntVec3(1000000000, 100000000, 100000000);
            float shortestDist = float.MaxValue;
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    IntVec3 tintVec = new IntVec3(i, 0, j);
                    if (tintVec.GetTerrain(map).defName.Contains("Water") && (validator == null || validator(tintVec) && getDistance(root, tintVec) < shortestDist))
                    {
                        intVec = tintVec;
                        shortestDist = getDistance(root, tintVec);
                    }
                }
            }
            if (intVec != new IntVec3(1000000000, 100000000, 100000000))
            {
                if (DebugViewSettings.drawDestSearch)
                {
                    map.debugDrawer.FlashCell(intVec, 0f, "inv", 50);
                }
                if (DebugViewSettings.drawDestSearch)
                {
                    map.debugDrawer.FlashCell(intVec, 0.5f, "found", 50);
                }
                result = intVec;
                return true;
            }
            result = root;
            return false;
        }
    }
}
