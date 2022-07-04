using JeremyAnsel.Xwa.HooksConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    public sealed class HangarCamera
    {
        public int Key1_X { get; set; }

        public int Key1_Y { get; set; }

        public int Key1_Z { get; set; }

        public int Key2_X { get; set; }

        public int Key2_Y { get; set; }

        public int Key2_Z { get; set; }

        public int Key3_X { get; set; }

        public int Key3_Y { get; set; }

        public int Key3_Z { get; set; }

        public int Key6_X { get; set; }

        public int Key6_Y { get; set; }

        public int Key6_Z { get; set; }

        public int Key9_X { get; set; }

        public int Key9_Y { get; set; }

        public int Key9_Z { get; set; }

        public static HangarCamera FromText(string text)
        {
            text ??= String.Empty;

            HangarCamera camera = new();
            IList<string> lines = text.GetLines();

            camera.Key1_X = XwaHooksConfig.GetFileKeyValueInt(lines, "Key1_X", 1130);
            camera.Key1_Y = XwaHooksConfig.GetFileKeyValueInt(lines, "Key1_Y", -2320);
            camera.Key1_Z = XwaHooksConfig.GetFileKeyValueInt(lines, "Key1_Z", -300);

            camera.Key2_X = XwaHooksConfig.GetFileKeyValueInt(lines, "Key2_X", 1240);
            camera.Key2_Y = XwaHooksConfig.GetFileKeyValueInt(lines, "Key2_Y", -330);
            camera.Key2_Z = XwaHooksConfig.GetFileKeyValueInt(lines, "Key2_Z", -700);

            camera.Key3_X = XwaHooksConfig.GetFileKeyValueInt(lines, "Key3_X", -1120);
            camera.Key3_Y = XwaHooksConfig.GetFileKeyValueInt(lines, "Key3_Y", 1360);
            camera.Key3_Z = XwaHooksConfig.GetFileKeyValueInt(lines, "Key3_Z", -790);

            camera.Key6_X = XwaHooksConfig.GetFileKeyValueInt(lines, "Key6_X", -1200);
            camera.Key6_Y = XwaHooksConfig.GetFileKeyValueInt(lines, "Key6_Y", -1530);
            camera.Key6_Z = XwaHooksConfig.GetFileKeyValueInt(lines, "Key6_Z", -850);

            camera.Key9_X = XwaHooksConfig.GetFileKeyValueInt(lines, "Key9_X", 1070);
            camera.Key9_Y = XwaHooksConfig.GetFileKeyValueInt(lines, "Key9_Y", 4640);
            camera.Key9_Z = XwaHooksConfig.GetFileKeyValueInt(lines, "Key9_Z", -130);

            return camera;
        }
    }
}
