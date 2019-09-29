using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    class HangarMap : Collection<HangarItem>
    {
        public static HangarMap FromText(string text)
        {
            HangarMap map = new HangarMap();

            if (string.IsNullOrWhiteSpace(text))
            {
                return map;
            }

            string[] lines = text.SplitLines();

            foreach (string line in lines)
            {
                if (line.StartsWith("#") || line.StartsWith(";") || line.StartsWith("//"))
                {
                    continue;
                }

                string[] parts = line.Split(',');

                if (parts.Length < 6)
                {
                    continue;
                }

                var item = new HangarItem();

                try
                {
                    if (parts.Length == 7)
                    {
                        item.ModelIndex = StringConverter.ToUInt16(parts[0]);
                        item.Markings = StringConverter.ToInt32(parts[1]);
                        item.PositionX = StringConverter.ToInt32(parts[2]);
                        item.PositionY = StringConverter.ToInt32(parts[3]);
                        item.PositionZ = StringConverter.ToInt32(parts[4]);
                        item.HeadingXY = StringConverter.ToInt16OrUInt16(parts[5]);
                        item.HeadingZ = StringConverter.ToInt16OrUInt16(parts[6]);
                    }
                    else
                    {
                        item.ModelIndex = StringConverter.ToUInt16(parts[0]);
                        item.Markings = 0;
                        item.PositionX = StringConverter.ToInt32(parts[1]);
                        item.PositionY = StringConverter.ToInt32(parts[2]);
                        item.PositionZ = StringConverter.ToInt32(parts[3]);
                        item.HeadingXY = StringConverter.ToInt16OrUInt16(parts[4]);
                        item.HeadingZ = StringConverter.ToInt16OrUInt16(parts[5]);
                    }
                }
                catch
                {
                    continue;
                }

                map.Add(item);
            }

            return map;
        }
    }
}
