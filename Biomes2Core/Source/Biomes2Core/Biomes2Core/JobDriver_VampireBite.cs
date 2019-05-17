using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Biomes2Core
{
    public class JobDriver_VampireBite : JobDriver
    {
        public bool firstHit = true;

        public Pawn Prey => (Pawn)job.GetTarget(TargetIndex.A).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            void onHitAction()
            {
                Pawn prey = Prey;
                bool surpriseAttack = firstHit && !prey.IsColonist;
                Job tjob = prey.CurJob;
                if (pawn.needs.food.CurLevelPercentage < 1 && !prey.Dead && pawn.meleeVerbs.TryMeleeAttack(prey, this.job.verbToUse, surpriseAttack))
                {
                    float new_food = pawn.needs.food.CurLevelPercentage + 0.5f;
                    if (new_food > 1)
                        new_food = 1;
                    pawn.needs.food.CurLevelPercentage = new_food;
                    if (prey.Dead)
                    {
                        return;
                    }

                    prey.stances.stunner.StunFor(700, pawn);
                    IEnumerable<Hediff> visibleDiffs = prey.health.hediffSet.hediffs;
                    List<Hediff> toremove = new List<Hediff>();
                    foreach (Hediff h in visibleDiffs)
                    {
                        if ((h.source == pawn.def || (h.def == HediffDefOf.MissingBodyPart && h.ageTicks < 100)) && h.def != HediffDefOf.BloodLoss)
                        {
                            toremove.Add(h);
                        }
                    }
                    foreach (Hediff h in toremove)
                    {
                        prey.health.RemoveHediff(h);
                    }
                    prey.jobs.StartJob(tjob, JobCondition.InterruptForced);
                }
                else
                {
                    if (Math.Abs(pawn.needs.food.CurLevelPercentage - 1) < float.Epsilon)
                    {
                        pawn.jobs.curDriver.ReadyForNextToil();
                    }
                }

                firstHit = false;
            }

            yield return Toils_Combat.FollowAndMeleeAttack(TargetIndex.A, onHitAction);
        }
    }
}
