using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JeremyAnsel.Xwa.HooksConfig
{
    public sealed class XwaIniSection
    {
        public string TxtKey { get; set; }

        public ICollection<string> Lines { get; } = new List<string>();

        public ICollection<string> Settings { get; } = new SortedSet<string>();

        public string GetKeyValueInLines(string key)
        {
            return GetKeyValue(this.Lines, key);
        }

        public void SetKeyValueInLines(string key, string value)
        {
            bool set = SetKeyValue(this.Lines, key, value);

            if (!set)
            {
                this.Lines.Add(key + " = " + value);
            }
        }

        public string GetKeyValueInSettings(string key)
        {
            return GetKeyValue(this.Settings, key);
        }

        public void SetKeyValueInSettings(string key, string value)
        {
            bool set = SetKeyValue(this.Settings, key, value);

            if (!set)
            {
                this.Settings.Add(key + " = " + value);
            }
        }

        private static string GetKeyValue(ICollection<string> lines, string key)
        {
            foreach (string line in lines)
            {
                int pos = line.IndexOf('=');

                if (pos == -1)
                {
                    continue;
                }

                string name = line.Substring(0, pos);
                name = name.Trim();

                if (name.Length == 0)
                {
                    continue;
                }

                if (string.Equals(name, key, StringComparison.OrdinalIgnoreCase))
                {
                    string value = line.Substring(pos + 1);
                    value = value.Trim();
                    return value;
                }
            }

            return string.Empty;
        }

        private static bool SetKeyValue(ICollection<string> lines, string key, string value)
        {
            var newLines = new List<string>(lines.Count + 1);
            bool set = false;

            foreach (string line in lines)
            {
                if (!set)
                {
                    int pos = line.IndexOf('=');

                    if (pos != -1)
                    {
                        string name = line.Substring(0, pos);
                        name = name.Trim();

                        if (name.Length != 0)
                        {
                            if (string.Equals(name, key, StringComparison.OrdinalIgnoreCase))
                            {
                                string newLine = key + " = " + value;
                                newLines.Add(newLine);
                                set = true;
                                continue;
                            }
                        }
                    }
                }

                newLines.Add(line);
            }

            lines.Clear();
            foreach (string newLine in newLines)
            {
                lines.Add(newLine);
            }

            return set;
        }
    }
}
