using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Biomes2Core
{
    class CompProperties_ReactiveDefense : CompProperties
    {
        public enum DefenseTrigger
        {
            Health,
            Proximity,
            Attacked
        }

        public enum DefenseType
        {
            Projectile,
            Aura,
            Hide,
            Buff,
            Trail,
            Reflect
        }

        public float hpThreshold = 0.2f;

        public int proximity = 2;

        public int duration = 3;

        public int auraSize = 1;

        public ThingDef aura = null;

        public DefenseTrigger defenseTrigger;

        public DefenseType defenseType;

        public float reflectPercent = 0.2f;

        public GraphicData hideGraphic = null;

        public ThingDef apparel = null;

        public float moveSpeedPenalty = 0.5f;

        public bool stopAttacker = true;

        public List<StatDef> statDefs = new List<StatDef>();

        public List<float> statValues = new List<float>();

        public CompProperties_ReactiveDefense()
        {
            compClass = typeof(CompReactiveDefense);
        }
    }
}
