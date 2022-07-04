using JeremyAnsel.Xwa.HooksConfig;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    public sealed class HangarSkins : Dictionary<string, Dictionary<int, IList<string>>>
    {
        public HangarSkins()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public static HangarSkins FromText(string text)
        {
            text ??= string.Empty;

            HangarSkins skins = new();
            IList<string> lines = text.GetLines();

            foreach (string line in lines)
            {
                KeyValuePair<string, string> pair = XwaHooksConfigHelpers.GetLineSplitKeyValue(line);

                if (string.IsNullOrEmpty(pair.Key) || string.IsNullOrEmpty(pair.Value))
                {
                    continue;
                }

                int fgcIndex = pair.Key.IndexOf("_fgc_", StringComparison.OrdinalIgnoreCase);
                string optName;
                int markings;

                if (fgcIndex == -1)
                {
                    optName = pair.Key;
                    markings = -1;
                }
                else
                {
                    optName = pair.Key.Substring(0, fgcIndex);

                    if (!int.TryParse(pair.Key.Substring(fgcIndex + 5), out markings)
                        || markings < 0
                        || markings > 255)
                    {
                        continue;
                    }
                }

                Dictionary<int, IList<string>> skin;

                if (skins.ContainsKey(optName))
                {
                    skin = skins[optName];
                }
                else
                {
                    skin = new Dictionary<int, IList<string>>();
                    skins[optName] = skin;
                }

                if (!skin.ContainsKey(markings))
                {
                    skin[markings] = XwaHooksConfig.Tokennize(pair.Value);
                }
            }

            return skins;
        }
    }
}
