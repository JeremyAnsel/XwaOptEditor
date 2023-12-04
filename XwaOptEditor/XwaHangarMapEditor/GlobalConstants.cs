using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    internal static class GlobalConstants
    {
        public static readonly string DefaultHangarSkinsText =
@"XWing = Default
XWing_fgc_0 = Default
";

        public static readonly string DefaultHangarObjectsText =
@"LoadShuttle = 1
ShuttleModelIndex = 50
ShuttleMarkings = 0
ShuttleObjectProfile = Default
ShuttlePositionX = 1127
ShuttlePositionY = 959
ShuttlePositionZ = 0
ShuttleOrientation = 43136
IsShuttleFloorInverted = 0
ShuttleAnimation = Right
ShuttleAnimationStraightLine = 0
ShuttleAnimationElevation = 0
LoadDroids = 1
DroidsPositionZ = 0
IsDroidsFloorInverted = 0
LoadDroid1 = 1
Droid1PositionZ = 0
IsDroid1FloorInverted = 0
Droid1Update = 1
Droid1ModelIndex = 311
Droid1Markings = 0
Droid1ObjectProfile = Default
LoadDroid2 = 1
Droid2PositionZ = 0
IsDroid2FloorInverted = 0
Droid2Update = 1
Droid2ModelIndex = 312
Droid2Markings = 0
Droid2ObjectProfile = Default
HangarRoofCranePositionX = -1400
HangarRoofCranePositionY = 786
HangarRoofCranePositionZ = -282
HangarRoofCraneAxis = 0
HangarRoofCraneLowOffset = 0
HangarRoofCraneHighOffset = 2800
IsHangarFloorInverted = 0
HangarFloorInvertedHeight = 0
HangarIff = -1
DrawShadows = 1
PlayerAnimationElevation = 0
PlayerAnimationInvertedElevation = 0
PlayerAnimationStraightLine = 0
PlayerOffsetX = 0
PlayerOffsetY = 0
PlayerOffsetZ = 0
PlayerInvertedOffsetX = 0
PlayerInvertedOffsetY = 0
PlayerInvertedOffsetZ = 0
PlayerModelIndices =
PlayerOffsetsX =
PlayerOffsetsY =
PlayerOffsetsZ =
IsPlayerFloorInverted = 0
PlayerFloorInvertedModelIndices =
LightColorIntensity = 192
LightColorRgb = FFFFFF
FlightModels\XWing.opt = FlightModels\XWing.opt
";

        public static readonly string DefaultHangarCameraText =
@"Key1_X = 1130
Key1_Y = -2320
Key1_Z = -300

Key2_X = 1240
Key2_Y = -330
Key2_Z = -700

Key3_X = -1120
Key3_Y = 1360
Key3_Z = -790

Key6_X = -1200
Key6_Y = -1530
Key6_Z = -850

Key9_X = 1070
Key9_Y = 4640
Key9_Z = -130
";

        public static readonly string DefaultHangarMapText =
@"; Must contain at least 4 object line.
; Format is : model index, position X, position Y, position Z, heading XY, heading Z
; or : model index, markings, position X, position Y, position Z, heading XY, heading Z
; or : model index, markings, position X, position Y, position Z, heading XY, heading Z, object profile
; or : model index, markings, position X, position Y, position Z, heading XY, heading Z, object profile, isFloorInverted
; Numbers can be in decimal or hexadecimal (0x) notation.
; When position Z is set to 0x7FFFFFFF, this means that the object stands at the ground.

; XWing
1, 0, 0, 0, 0x7FFFFFFF, 0, 0, Default, 0
";
    }
}
