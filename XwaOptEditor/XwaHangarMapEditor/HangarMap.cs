using JeremyAnsel.Xwa.HooksConfig;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    public sealed class HangarMap : Collection<HangarItem>
    {
        public static HangarMap FromText(string text)
        {
            HangarMap map = new();

            if (string.IsNullOrWhiteSpace(text))
            {
                return map;
            }

            IList<string> lines = text.GetLines();

            foreach (string line in lines)
            {
                IList<string> parts = XwaHooksConfig.Tokennize(line);

                if (parts.Count < 6)
                {
                    continue;
                }

                var item = new HangarItem();

                try
                {
                    if (parts.Count == 8)
                    {
                        item.ModelIndex = StringConverter.ToUInt16(parts[0]);
                        item.Markings = StringConverter.ToInt32(parts[1]);
                        item.PositionX = StringConverter.ToInt32(parts[2]);
                        item.PositionY = StringConverter.ToInt32(parts[3]);
                        item.PositionZ = StringConverter.ToInt32(parts[4]);
                        item.HeadingXY = StringConverter.ToInt16OrUInt16(parts[5]);
                        item.HeadingZ = StringConverter.ToInt16OrUInt16(parts[6]);
                        item.ObjectProfile = parts[7].Trim();

                        if (string.IsNullOrEmpty(item.ObjectProfile))
                        {
                            item.ObjectProfile = "Default";
                        }
                    }
                    else if (parts.Count == 7)
                    {
                        item.ModelIndex = StringConverter.ToUInt16(parts[0]);
                        item.Markings = StringConverter.ToInt32(parts[1]);
                        item.PositionX = StringConverter.ToInt32(parts[2]);
                        item.PositionY = StringConverter.ToInt32(parts[3]);
                        item.PositionZ = StringConverter.ToInt32(parts[4]);
                        item.HeadingXY = StringConverter.ToInt16OrUInt16(parts[5]);
                        item.HeadingZ = StringConverter.ToInt16OrUInt16(parts[6]);
                        item.ObjectProfile = "Default";
                    }
                    else if (parts.Count == 6)
                    {
                        item.ModelIndex = StringConverter.ToUInt16(parts[0]);
                        item.Markings = 0;
                        item.PositionX = StringConverter.ToInt32(parts[1]);
                        item.PositionY = StringConverter.ToInt32(parts[2]);
                        item.PositionZ = StringConverter.ToInt32(parts[3]);
                        item.HeadingXY = StringConverter.ToInt16OrUInt16(parts[4]);
                        item.HeadingZ = StringConverter.ToInt16OrUInt16(parts[5]);
                        item.ObjectProfile = "Default";
                    }
                    else
                    {
                        continue;
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
