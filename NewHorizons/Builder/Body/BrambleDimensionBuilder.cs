using NewHorizons.Builder.Props;
using NewHorizons.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static NewHorizons.External.Modules.BrambleModule;
using Logger = NewHorizons.Utility.Logger;

namespace NewHorizons.Builder.Body
{
    public static class BrambleDimensionBuilder
    {
        // put node here
        //  "position": {"x": 65.2428, "y": -137.305, "z": 198.1078}, "normal": {"x": 0.303696, "y": -0.5609235, "z": 0.7701519}

        // location of all vanilla bramble dimensions
        //-9116.795 -19873.44 2480.327
        //-8460.688 -19873.44 6706.444
        //-5015.165 -19873.44 4142.816
        //-8993.414 -17059.44 4521.747
        //-7044.813 -17135.44 3272.149
        //-6904.48  -17048.44 5574.479
        //-11096.95 -22786.44 4657.534
        //-8716.807 -22786.44 4496.394

        private static int DIMENSION_COUNTER = 0;

        
        // keys are all node names that have been referenced as an exit by at least one dimension but do not (yet) exist
        // values are all dimensions' warp controllers that link to a given dimension
        // unpairedNodes[name of node that doesn't exist yet] => List{warp controller for dimension that exits to that node, ...}
        private static Dictionary<string, List<OuterFogWarpVolume>> unpairedDimensions = new();

        public static GameObject Make(NewHorizonsBody body)
        {
            var config = body.Config.Bramble.dimension;

            // spawn the dimension body
            var dimensionPrefab = SearchUtilities.Find("DB_HubDimension_Body");
            var dimension = GameObject.Instantiate(dimensionPrefab);
            var ao = dimension.GetComponent<AstroObject>();

            // fix name
            var name = body.Config.name ?? "Custom Bramble Dimension";
            ao._customName = name;
            ao._name = AstroObject.Name.CustomString;
            dimension.name = name.Replace(" ", "").Replace("'", "") + "_Body";

            // set position
            ao.transform.position = new Vector3(6904.48f, 17048.44f, 5574.479f) + new Vector3(0, 3000, 0)*DIMENSION_COUNTER;
            DIMENSION_COUNTER++;

            // TODO: radius (need to determine what the base radius is first)

            // fix children's names and remove base game props (mostly just bramble nodes that are children to Interactibles) and set up the OuterWarp child
            var dimensionSector = SearchUtilities.FindChild(dimension, "Sector_HubDimension");
            dimensionSector.name = "Sector";
            var atmo = SearchUtilities.FindChild(dimensionSector, "Atmosphere_HubDimension");
            var geom = SearchUtilities.FindChild(dimensionSector, "Geometry_HubDimension");
            var vols = SearchUtilities.FindChild(dimensionSector, "Volumes_HubDimension");
            var efxs = SearchUtilities.FindChild(dimensionSector, "Effects_HubDimension");
            var intr = SearchUtilities.FindChild(dimensionSector, "Interactables_HubDimension");
            var exitWarps = SearchUtilities.FindChild(intr, "OuterWarp_Hub");

            exitWarps.name = "OuterWarp";
            exitWarps.transform.parent = dimensionSector.transform;
            atmo.name = "Atmosphere";
            geom.name = "Geometry"; // disable this?
            vols.name = "Volumes";
            efxs.name = "Effects";
            intr.name = "Interactibles";
            GameObject.Destroy(intr);

            // set up warps
            var outerFogWarpVolume = exitWarps.GetComponent<OuterFogWarpVolume>();
            outerFogWarpVolume._senderWarps.Clear();
            outerFogWarpVolume._linkedInnerWarpVolume = null;
            outerFogWarpVolume._name = OuterFogWarpVolume.Name.None;

            PairExit(config.linksTo, outerFogWarpVolume);

            return dimension;
        }

        public static void PairExit(string exitName, OuterFogWarpVolume warpController)
        {
            Logger.Log($"attempting to pair exit {exitName}");
            if (!BrambleNodeBuilder.namedNodes.ContainsKey(exitName))
            {
                if (!unpairedDimensions.ContainsKey(exitName)) unpairedDimensions[exitName] = new();
                unpairedDimensions[exitName].Add(warpController);
                return;
            }
            Logger.Log($"pairing exit {exitName}");
            warpController._linkedInnerWarpVolume = BrambleNodeBuilder.namedNodes[exitName];
        }

        public static void FinishPairingDimensionsForExitNode(string nodeName)
        {
            if (!unpairedDimensions.ContainsKey(nodeName)) return;

            var warpControllers = unpairedDimensions[nodeName].ToList();
            foreach (var dimensionWarpController in warpControllers)
            {
                PairExit(nodeName, dimensionWarpController);    
            }

            //unpairedDimensions.Remove(nodeName);
        }

    }
}
