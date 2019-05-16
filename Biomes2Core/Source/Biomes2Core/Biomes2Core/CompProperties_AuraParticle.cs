using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Biomes2Core
{
    public class CompProperties_AuraParticle : CompProperties
    {
        public enum Parent
        {
            Animal,
            Building,
            Item,
            Plant
        }

        public ThingDef parent = null;

        public Parent releasedBy = Parent.Item;

        public int duration = 3;

        public HediffDef hediff = null;

        public int damage = -1;

        public DamageDef damageDef = null;

        public float severity = 1f;

        public bool forceFlee = false;

        public bool warn = false;

        public CompProperties_AuraParticle()
        {
            compClass = typeof(CompAuraParticle);
        }
    }
}
