using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Biomes2Core
{
    class CompReactiveDefense : ThingComp
    {
        private IDictionary<StatDef, float> stats = new Dictionary<StatDef, float>();

        bool initalized = false;

        bool hidden = false;

        bool stoppedAttacker = false;

        DamageInfo lastattack = new DamageInfo();

        bool newattack = false;

        public CompProperties_ReactiveDefense Props => (CompProperties_ReactiveDefense)props;

        public void Attacked()
        {
            if (newattack)
            {
                newattack = false;
            }
        }

        public void Aura(IntVec3 pos, Map map, CompProperties_ReactiveDefense props)
        {
            if (props.aura != null && props.aura.GetCompProperties<CompProperties_AuraParticle>() != null)
            {
                for (int i = pos.x - props.auraSize; i <= pos.x + props.auraSize; i++)
                {
                    for (int j = pos.z - props.auraSize; j <= pos.z + props.auraSize; j++)
                    {
                        IntVec3 vec = new IntVec3(i, 0, j);
                        if (vec != pos && vec.InBounds(map))
                        {
                            List<ThingDef> thingDefs = vec.GetThingList(map).Select(x => x.def).ToList();
                            if (vec.GetFirstBuilding(map) == null && !thingDefs.Contains(props.aura))
                            {
                                props.aura.GetCompProperties<CompProperties_AuraParticle>().duration = props.duration;
                                GenSpawn.Spawn(props.aura, vec, map);
                            }
                        }
                    }
                }
            }
        }

        public void Hide(CompProperties_ReactiveDefense props)
        {
            if (props.statDefs == null || props.statDefs.Count == 0 || props.statValues == null || props.statValues.Count == 0 || props.statDefs.Count != props.statValues.Count)
            {
                return;
            }

            Pawn pawn = (Pawn)parent;
            if (pawn.Dead)
            {
                if (pawn.apparel.WornApparel.Count > 0)
                {
                    List<Apparel> tap = pawn.apparel.WornApparel.ToList();
                    foreach (Apparel apparel in tap)
                    {
                        pawn.apparel.Remove(apparel);
                    }
                }
                return;
            }

            if (pawn.health.hediffSet.GetPartHealth(pawn.RaceProps.body.corePart) < pawn.RaceProps.body.corePart.def.hitPoints * props.hpThreshold * pawn.def.race.baseHealthScale)
            {
                if (!hidden)
                {
                    if (Props.apparel != null)
                    {
                        Apparel apparel = (Apparel)ThingMaker.MakeThing(Props.apparel);
                        for (int i = 0; i < props.statDefs.Count; i++)
                        {
                            apparel.def.SetStatBaseValue(props.statDefs[i], props.statValues[i]);
                        }
                        if (ApparelUtility.HasPartsToWear(pawn, apparel.def))
                        {
                            if (pawn.apparel == null)
                            {
                                pawn.apparel = new Pawn_ApparelTracker(pawn);
                            }
                            pawn.apparel.Wear(apparel, false);
                        }
                    }
                    if (Props.stopAttacker && !stoppedAttacker)
                    {
                        ((Pawn)lastattack.Instigator).jobs.StartJob(new Job(JobDefOf.Wait_Wander), JobCondition.InterruptForced);
                        stoppedAttacker = true;
                    }
                    ResolveHideGraphic();
                    hidden = true;
                    pawn.jobs.StartJob(new Job(JobDefOf.FleeAndCower, pawn.Position), JobCondition.InterruptForced);
                }
                else
                {
                    //prevent from moving
                    pawn.pather.StopDead();
                }
            }
            else
            {
                if (hidden)
                {
                    if (Props.apparel != null && pawn.apparel != null)
                    {
                        pawn.apparel.DestroyAll();
                    }
                    ResolveBaseGraphic();
                    hidden = false;
                    stoppedAttacker = false;

                    pawn.pather.StartPath(pawn.Position, PathEndMode.OnCell);
                }
            }
        }

        public void React(IntVec3 pos, Map map, CompProperties_ReactiveDefense props)
        {
            switch (props.defenseType)
            {
                // TODO: Implement other types
                case CompProperties_ReactiveDefense.DefenseType.Aura:
                    Aura(pos, map, props);
                    break;
                case CompProperties_ReactiveDefense.DefenseType.Hide:
                    Hide(props);
                    break;
            }
        }

        public void Proximity(IntVec3 pos, Map map, CompProperties_ReactiveDefense props)
        {
            for (int i = pos.x - props.proximity; i <= pos.x + props.proximity; i++)
            {
                for (int j = pos.z - props.proximity; j <= pos.z + props.proximity; j++)
                {
                    IntVec3 temp = new IntVec3(i, 0, j);
                    if (temp.InBounds(map) && temp.GetFirstPawn(map) != null && temp != pos && !temp.GetFirstPawn(map).Dead && !temp.GetFirstPawn(map).Downed)
                    {
                        React(pos, map, props);
                    }
                }
            }
        }

        public void Defend(CompProperties_ReactiveDefense props)
        {
            IntVec3 pos = parent.Position;
            Map map = parent.Map;
            switch (props.defenseTrigger)
            {
                case CompProperties_ReactiveDefense.DefenseTrigger.Health:
                    Attacked();
                    break;
                case CompProperties_ReactiveDefense.DefenseTrigger.Proximity:
                    Proximity(pos, map, props);
                    break;
                case CompProperties_ReactiveDefense.DefenseTrigger.Attacked:
                    React(pos, map, props);
                    break;
            }
        }

        public override void CompTick()
        {
            if (Props == null)
            {
                return;
            }
            if (!initalized)
            {
                foreach (StatModifier stat in parent.def.statBases)
                {
                    stats.Add(stat.stat, stat.value);
                }
                initalized = true;
            }
            Defend(Props);
            base.CompTick();
        }

        public void ResolveHideGraphic()
        {
            if (Props.hideGraphic != null &&
                ((Pawn)parent).Drawer?.renderer?.graphics != null)
            {
                PawnGraphicSet pawnGraphicSet = ((Pawn)parent).Drawer?.renderer?.graphics;
                pawnGraphicSet.ClearCache();
                pawnGraphicSet.nakedGraphic = Props.hideGraphic.Graphic;
            }
        }

        public void ResolveBaseGraphic()
        {
            if (Props.hideGraphic != null && ((Pawn)parent).Drawer?.renderer?.graphics != null)
            {
                PawnGraphicSet pawnGraphicSet = ((Pawn)parent).Drawer?.renderer?.graphics;
                pawnGraphicSet.ClearCache();

                var curKindLifeStage = ((Pawn)parent).ageTracker.CurKindLifeStage;
                if (((Pawn)parent).gender != Gender.Female || curKindLifeStage.femaleGraphicData == null)
                {
                    pawnGraphicSet.nakedGraphic = curKindLifeStage.bodyGraphicData.Graphic;
                }
                else
                {
                    pawnGraphicSet.nakedGraphic = curKindLifeStage.femaleGraphicData.Graphic;
                }
                pawnGraphicSet.rottingGraphic = pawnGraphicSet.nakedGraphic.GetColoredVersion(ShaderDatabase.CutoutSkin, PawnGraphicSet.RottingColor, PawnGraphicSet.RottingColor);
                if (((Pawn)parent).RaceProps.packAnimal)
                {
                    pawnGraphicSet.packGraphic = GraphicDatabase.Get<Graphic_Multi>(pawnGraphicSet.nakedGraphic.path + "Pack", ShaderDatabase.Cutout, pawnGraphicSet.nakedGraphic.drawSize, Color.white);
                }
                if (curKindLifeStage.dessicatedBodyGraphicData != null)
                {
                    pawnGraphicSet.dessicatedGraphic = curKindLifeStage.dessicatedBodyGraphicData.GraphicColoredFor(((Pawn)parent));
                }
            }
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            lastattack = dinfo;
            newattack = true;
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
            if (parent as Pawn != null)
            {
                Pawn pawn = (Pawn)parent;
                if (pawn.Dead)
                {
                    if (Props.apparel != null && pawn.apparel != null)
                    {
                        pawn.apparel.DestroyAll();
                    }
                    ResolveBaseGraphic();
                    hidden = false;
                }
            }

        }
    }
}
