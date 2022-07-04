using JeremyAnsel.Xwa.HooksConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    public sealed class HangarObjects
    {
        public bool LoadShuttle { get; set; }

        public ushort ShuttleModelIndex { get; set; }

        public int ShuttleMarkings { get; set; }

        public string ShuttleObjectProfile { get; set; }

        public int ShuttlePositionX { get; set; }

        public int ShuttlePositionY { get; set; }

        public int ShuttlePositionZ { get; set; }

        public ushort ShuttleOrientation { get; set; }

        public bool IsShuttleFloorInverted { get; set; }

        public string ShuttleAnimation { get; set; }

        public int ShuttleAnimationStraightLine { get; set; }

        public int ShuttleAnimationElevation { get; set; }

        public bool LoadDroids { get; set; }

        public int DroidsPositionZ { get; set; }

        public bool IsDroidsFloorInverted { get; set; }

        public bool LoadDroid1 { get; set; }

        public int Droid1PositionZ { get; set; }

        public bool IsDroid1FloorInverted { get; set; }

        public bool Droid1Update { get; set; }

        public ushort Droid1ModelIndex { get; set; }

        public int Droid1Markings { get; set; }

        public string Droid1ObjectProfile { get; set; }

        public bool LoadDroid2 { get; set; }

        public int Droid2PositionZ { get; set; }

        public bool IsDroid2FloorInverted { get; set; }

        public bool Droid2Update { get; set; }

        public ushort Droid2ModelIndex { get; set; }

        public int Droid2Markings { get; set; }

        public string Droid2ObjectProfile { get; set; }

        public int HangarRoofCranePositionX { get; set; }

        public int HangarRoofCranePositionY { get; set; }

        public int HangarRoofCranePositionZ { get; set; }

        public int HangarRoofCraneAxis { get; set; }

        public int HangarRoofCraneLowOffset { get; set; }

        public int HangarRoofCraneHighOffset { get; set; }

        public bool IsHangarFloorInverted { get; set; }

        public byte HangarIff { get; set; }

        public int PlayerAnimationElevation { get; set; }

        public int PlayerAnimationStraightLine { get; set; }

        public int PlayerOffsetX { get; set; }

        public int PlayerOffsetY { get; set; }

        public int PlayerOffsetZ { get; set; }

        public bool IsPlayerFloorInverted { get; set; }

        public uint LightColorIntensity { get; set; }

        public uint LightColorRgb { get; set; }

        public Dictionary<string, string> ObjectsReplace { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static HangarObjects FromText(string text)
        {
            text ??= String.Empty;

            HangarObjects hangar = new();
            IList<string> lines = text.GetLines();

            hangar.LoadShuttle = XwaHooksConfig.GetFileKeyValueInt(lines, "LoadShuttle", 1) == 1;
            hangar.ShuttleModelIndex = (ushort)XwaHooksConfig.GetFileKeyValueInt(lines, "ShuttleModelIndex", 50);
            hangar.ShuttleMarkings = XwaHooksConfig.GetFileKeyValueInt(lines, "ShuttleMarkings");
            hangar.ShuttleObjectProfile = XwaHooksConfig.GetFileKeyValue(lines, "ShuttleObjectProfile");

            if (string.IsNullOrEmpty(hangar.ShuttleObjectProfile))
            {
                hangar.ShuttleObjectProfile = "Default";
            }

            hangar.ShuttlePositionX = XwaHooksConfig.GetFileKeyValueInt(lines, "ShuttlePositionX", 0x467);
            hangar.ShuttlePositionY = XwaHooksConfig.GetFileKeyValueInt(lines, "ShuttlePositionY", 0x3BF);
            hangar.ShuttlePositionZ = XwaHooksConfig.GetFileKeyValueInt(lines, "ShuttlePositionZ", 0);
            hangar.ShuttleOrientation = (ushort)XwaHooksConfig.GetFileKeyValueInt(lines, "ShuttleOrientation", 0xA880);
            hangar.IsShuttleFloorInverted = XwaHooksConfig.GetFileKeyValueInt(lines, "IsShuttleFloorInverted", 0) != 0;
            hangar.ShuttleAnimation = XwaHooksConfig.GetFileKeyValue(lines, "ShuttleAnimation");
            hangar.ShuttleAnimationStraightLine = XwaHooksConfig.GetFileKeyValueInt(lines, "ShuttleAnimationStraightLine", 0);
            hangar.ShuttleAnimationElevation = XwaHooksConfig.GetFileKeyValueInt(lines, "ShuttleAnimationElevation", 0);
            hangar.LoadDroids = XwaHooksConfig.GetFileKeyValueInt(lines, "LoadDroids", 1) == 1;
            hangar.DroidsPositionZ = XwaHooksConfig.GetFileKeyValueInt(lines, "DroidsPositionZ", 0);
            hangar.IsDroidsFloorInverted = XwaHooksConfig.GetFileKeyValueInt(lines, "IsDroidsFloorInverted", 0) != 0;
            hangar.LoadDroid1 = XwaHooksConfig.GetFileKeyValueInt(lines, "LoadDroid1", 1) == 1;
            hangar.Droid1PositionZ = XwaHooksConfig.GetFileKeyValueInt(lines, "Droid1PositionZ", hangar.DroidsPositionZ);
            hangar.IsDroid1FloorInverted = XwaHooksConfig.GetFileKeyValueInt(lines, "IsDroid1FloorInverted", 0) != 0;
            hangar.Droid1Update = XwaHooksConfig.GetFileKeyValueInt(lines, "Droid1Update", 1) != 0;
            hangar.Droid1ModelIndex = (ushort)XwaHooksConfig.GetFileKeyValueInt(lines, "Droid1ModelIndex", 311); // ModelIndex_311_1_33_HangarDroid
            hangar.Droid1Markings = XwaHooksConfig.GetFileKeyValueInt(lines, "Droid1Markings", 0);
            hangar.Droid1ObjectProfile = XwaHooksConfig.GetFileKeyValue(lines, "Droid1ObjectProfile");

            if (string.IsNullOrEmpty(hangar.Droid1ObjectProfile))
            {
                hangar.Droid1ObjectProfile = "Default";
            }

            hangar.LoadDroid2 = XwaHooksConfig.GetFileKeyValueInt(lines, "LoadDroid2", 1) == 1;
            hangar.Droid2PositionZ = XwaHooksConfig.GetFileKeyValueInt(lines, "Droid2PositionZ", hangar.DroidsPositionZ);
            hangar.IsDroid2FloorInverted = XwaHooksConfig.GetFileKeyValueInt(lines, "IsDroid2FloorInverted", 0) != 0;
            hangar.Droid2Update = XwaHooksConfig.GetFileKeyValueInt(lines, "Droid2Update", 1) != 0;
            hangar.Droid2ModelIndex = (ushort)XwaHooksConfig.GetFileKeyValueInt(lines, "Droid2ModelIndex", 312); // ModelIndex_312_1_34_HangarDroid2
            hangar.Droid2Markings = XwaHooksConfig.GetFileKeyValueInt(lines, "Droid2Markings", 0);
            hangar.Droid2ObjectProfile = XwaHooksConfig.GetFileKeyValue(lines, "Droid2ObjectProfile");

            if (string.IsNullOrEmpty(hangar.Droid2ObjectProfile))
            {
                hangar.Droid2ObjectProfile = "Default";
            }

            hangar.HangarRoofCranePositionX = XwaHooksConfig.GetFileKeyValueInt(lines, "HangarRoofCranePositionX", -1400);
            hangar.HangarRoofCranePositionY = XwaHooksConfig.GetFileKeyValueInt(lines, "HangarRoofCranePositionY", 786);
            hangar.HangarRoofCranePositionZ = XwaHooksConfig.GetFileKeyValueInt(lines, "HangarRoofCranePositionZ", -282);
            hangar.HangarRoofCraneAxis = XwaHooksConfig.GetFileKeyValueInt(lines, "HangarRoofCraneAxis", 0);
            hangar.HangarRoofCraneLowOffset = XwaHooksConfig.GetFileKeyValueInt(lines, "HangarRoofCraneLowOffset", -1400 + 1400);
            hangar.HangarRoofCraneHighOffset = XwaHooksConfig.GetFileKeyValueInt(lines, "HangarRoofCraneHighOffset", 1400 + 1400);
            hangar.IsHangarFloorInverted = XwaHooksConfig.GetFileKeyValueInt(lines, "IsHangarFloorInverted", 0) != 0;
            hangar.HangarIff = (byte)XwaHooksConfig.GetFileKeyValueInt(lines, "HangarIff", -1);
            hangar.PlayerAnimationElevation = XwaHooksConfig.GetFileKeyValueInt(lines, "PlayerAnimationElevation", 0);
            hangar.PlayerAnimationStraightLine = XwaHooksConfig.GetFileKeyValueInt(lines, "PlayerAnimationStraightLine", 0);
            hangar.PlayerOffsetX = XwaHooksConfig.GetFileKeyValueInt(lines, "PlayerOffsetX", 0);
            hangar.PlayerOffsetY = XwaHooksConfig.GetFileKeyValueInt(lines, "PlayerOffsetY", 0);
            hangar.PlayerOffsetZ = XwaHooksConfig.GetFileKeyValueInt(lines, "PlayerOffsetZ", 0);
            hangar.IsPlayerFloorInverted = XwaHooksConfig.GetFileKeyValueInt(lines, "IsPlayerFloorInverted", 0) != 0;
            hangar.LightColorIntensity = XwaHooksConfigHelpers.GetFileKeyValueUnsignedInt(lines, "LightColorIntensity", 0xC0);
            hangar.LightColorRgb = XwaHooksConfigHelpers.GetFileKeyValueUnsignedInt(lines, "LightColorRgb", 0xFFFFFF);

            foreach (string line in lines)
            {
                KeyValuePair<string, string> pair = XwaHooksConfigHelpers.GetLineSplitKeyValue(line);

                if (string.IsNullOrEmpty(pair.Key)
                    || string.IsNullOrEmpty(pair.Value))
                {
                    continue;
                }

                if (!pair.Key.StartsWith("FlightModels\\", StringComparison.OrdinalIgnoreCase)
                    || !pair.Key.EndsWith(".opt", StringComparison.OrdinalIgnoreCase)
                    || !pair.Value.StartsWith("FlightModels\\", StringComparison.OrdinalIgnoreCase)
                    || !pair.Value.EndsWith(".opt", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string pairKey = XwaHooksConfig.GetStringWithoutExtension(pair.Key.Substring(pair.Key.IndexOf('\\') + 1));
                string pairValue = XwaHooksConfig.GetStringWithoutExtension(pair.Value.Substring(pair.Value.IndexOf('\\') + 1));

                if (hangar.ObjectsReplace.ContainsKey(pairKey))
                {
                    continue;
                }

                hangar.ObjectsReplace[pairKey] = pairValue;
            }

            return hangar;
        }
    }
}
