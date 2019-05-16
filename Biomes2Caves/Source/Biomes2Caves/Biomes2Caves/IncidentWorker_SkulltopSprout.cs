using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Biomes2Caves
{
    class IncidentWorker_SkulltopSprout : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms) => tryFindTile(out _);

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!tryFindTile(out int tile))
            {
                return false;
            }
            if (!SiteMakerHelper.TryFindSiteParams_SingleSitePart(CavesDefOf.Biomes_SkulltopSprout, (string)null, out SitePartDef sitePart, out Faction faction))
            {
                return false;
            }
            int timeout = SiteTuning.QuestSiteTimeoutDaysRange.RandomInRange;
            Site site = SiteMaker.MakeSite(CavesDefOf.Biomes_SkulltopSprout, sitePart, tile, faction);
            site.sitePartsKnown = true;
            site.GetComponent<TimeoutComp>().StartTimeout(SiteTuning.QuestSiteTimeoutDaysRange.RandomInRange * 60000);
            Find.WorldObjects.Add(site);
            SendStandardLetter(site);
            return true;
        }

        private bool tryFindTile(out int tile)
        {
            int findTile(int root)
            {
                bool validator(int x) => !Find.WorldObjects.AnyWorldObjectAt(x) && TileFinder.IsValidTileForNewSettlement(x, null) && (Find.WorldGrid[x].biome.defName == "CaveOasis" || Find.WorldGrid[x].biome.defName == "TunnelworldCave" || Find.WorldGrid[x].biome.defName == "CaveEntrance" || Find.WorldGrid[x].biome.defName == "InfestedMountains" || Find.WorldGrid[x].biome.defName == "DesertCanyon");
                if (TileFinder.TryFindPassableTileWithTraversalDistance(root, SiteTuning.ItemStashQuestSiteDistanceRange.min, SiteTuning.ItemStashQuestSiteDistanceRange.max, out int result, validator))
                {
                    return result;
                }
                return -1;
            }
            if (!TileFinder.TryFindRandomPlayerTile(out int arg, false, (int x) => findTile(x) != -1))
            {
                tile = -1;
                return false;
            }
            tile = findTile(arg);
            return tile != -1;
        }
    }
}
