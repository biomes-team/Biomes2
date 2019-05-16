using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace Biomes2Core
{
    public class CompAuraParticle : ThingComp
    {
        private int startTicks = GenTicks.TicksGame;

        private bool hasWarned = false;

        private int lastWarned = GenTicks.TicksGame;

        public CompProperties_AuraParticle Props => (CompProperties_AuraParticle)props;

        private bool workingOnItem(Pawn _) => false;

        private bool workingOnPlant(Pawn p) => (p.CurJob?.def == JobDefOf.Harvest || p.CurJob?.def == JobDefOf.CutPlant || p.CurJob?.def == JobDefOf.Ingest) && Props.parent != null && p.CurJob?.targetA.Thing.def == Props.parent;

        private bool workingOnAnimal(Pawn p) => (p.CurJob?.def == JobDefOf.AttackMelee) && Props.parent != null && p.CurJob?.targetA.Thing.def == Props.parent;

        private bool workingOnBuilding(Pawn p) => (p.CurJob?.def == JobDefOf.AttackMelee || p.CurJob?.def == JobDefOf.Deconstruct) && Props.parent != null && p.CurJob?.targetA.Thing.def == Props.parent;

        public override void CompTick()
        {
            if (hasWarned)
            {
                if (lastWarned > GenTicks.TicksGame + GenTicks.SecondsToTicks(3))
                {
                    hasWarned = false;
                }
            }
            base.CompTick();

            if (Props.hediff != null)
            {
                foreach (Pawn p in parent.Map.mapPawns.AllPawns)
                {
                    if (p.Position == parent.Position)
                    {
                        if (!p.Downed)
                        {
                            HealthUtility.AdjustSeverity(p, Props.hediff, Props.severity);
                        }
                        else
                        {
                            HealthUtility.AdjustSeverity(p, Props.hediff, Props.severity / 4);
                        }

                        if (Props.warn && Math.Abs(p.health.hediffSet.GetFirstHediffOfDef(Props.hediff).Severity - Props.severity) < double.Epsilon && p.Faction != null && p.Faction.IsPlayer)
                        {
                            Messages.Message(p.Name + " has walked into a " + parent.Label, p, MessageTypeDefOf.ThreatSmall);
                        }

                        if ((Props.forceFlee && !p.Downed && p.Faction != null && p.Faction.IsPlayer && p.Drafted && p.health.hediffSet.GetFirstHediffOfDef(Props.hediff).Severity > 0.7) ||
                            (Props.forceFlee && !p.Downed && (p.health.hediffSet.GetFirstHediffOfDef(Props.hediff).Severity > 0.7)) || 
                            (p.health.hediffSet.GetFirstHediffOfDef(Props.hediff).Severity > 0.5 && !(p.Faction != null && p.Faction.IsPlayer && p.Drafted) && p.CurJob?.def != JobDefOf.Flee && !((Props.releasedBy == CompProperties_AuraParticle.Parent.Plant && workingOnPlant(p)) || 
                            (Props.releasedBy == CompProperties_AuraParticle.Parent.Animal && workingOnAnimal(p)) || 
                            (Props.releasedBy == CompProperties_AuraParticle.Parent.Building && workingOnBuilding(p)) || 
                            (Props.releasedBy == CompProperties_AuraParticle.Parent.Item && workingOnItem(p)))))
                        {
                            if (GenTicks.TicksGame - startTicks > GenTicks.SecondsToTicks(0.5f))
                            {
                                if (Props.warn && p.Faction != null && p.Faction.IsPlayer && !hasWarned)
                                {
                                    Messages.Message(p.Name + " is fleeing from a " + parent.Label, p, MessageTypeDefOf.ThreatSmall);
                                    hasWarned = true;
                                    lastWarned = GenTicks.TicksGame;
                                }
                                IntVec3 dest = CellFinderLoose.GetFleeDest(p, new List<Thing> { parent }, 8f);
                                if (dest == parent.Position)
                                {
                                    dest = dest.RandomAdjacentCell8Way();
                                }
                                p.jobs?.TryTakeOrderedJob(new Job(JobDefOf.Flee, dest), JobTag.Escaping);
                            }
                        }
                    }
                }
            }

            if (GenTicks.TicksGame > startTicks + GenTicks.SecondsToTicks(Props.duration))
            {
                parent.Destroy();
            }
        }
    }
}
