using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Biomes2Core
{
    public class JobGiver_WanderWaterAnywhere : JobGiver_WanderWater
    {
        public JobGiver_WanderWaterAnywhere()
        {
            wanderRadius = 7f;
            locomotionUrgency = LocomotionUrgency.Walk;
            ticksBetweenWandersRange = new IntRange(125, 200);
        }
        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            return pawn.Position;
        }
    }
}
