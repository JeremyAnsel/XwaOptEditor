using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JeremyAnsel.Xwa.HooksConfig
{
    public sealed class XwaIniFile
    {
        private static readonly Encoding _encoding = Encoding.GetEncoding("iso-8859-1");

        public XwaIniFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!Path.HasExtension(path))
            {
                path = Path.ChangeExtension(path, ".ini");
            }

            this.BasePath = Path.ChangeExtension(path, null);
            this.Extension = Path.GetExtension(path);

            this.CreateSectionIfNotExists(string.Empty);
        }

        public string BasePath { get; private set; }

        public string Extension { get; private set; }

        public IDictionary<string, XwaIniSection> Sections { get; } = new SortedDictionary<string, XwaIniSection>();

        public bool HasLines
        {
            get
            {
                return this.Sections.Any(t => t.Value.Lines.Count != 0);
            }
        }

        public bool HasSettings
        {
            get
            {
                return this.Sections.Any(t => t.Value.Settings.Count != 0);
            }
        }

        public bool HasLinesOrSettings
        {
            get
            {
                return this.HasLines || this.HasSettings;
            }
        }

        public bool HasTxt
        {
            get
            {
                return this.Sections.Any(t => !string.IsNullOrEmpty(t.Value.TxtKey));
            }
        }

        public void ParseIni()
        {
            string path = this.BasePath + this.Extension;

            if (!File.Exists(path))
            {
                return;
            }

            using (var reader = new StreamReader(path, _encoding))
            {
                string fileLine;
                string section = string.Empty;
                bool readSection = true;

                while ((fileLine = reader.ReadLine()) != null)
                {
                    string line = fileLine.Trim();

                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        section = line.Substring(1, line.Length - 2);
                        readSection = this.CreateSectionIfNotExists(section);
                    }
                    else
                    {
                        if (readSection)
                        {
                            this.Sections[section].Lines.Add(fileLine);
                        }
                    }
                }
            }
        }

        public void ParseSettings(string iniKey = null)
        {
            foreach (var section in this.Sections)
            {
                if (iniKey == null || string.Equals(section.Key, iniKey, StringComparison.OrdinalIgnoreCase))
                {
                    var lines = new string[section.Value.Lines.Count];
                    section.Value.Lines.CopyTo(lines, 0);

                    section.Value.Lines.Clear();
                    section.Value.Settings.Clear();

                    foreach (string fileLine in lines)
                    {
                        string line = fileLine.Trim();

                        if (!IsComment(line) && line.IndexOf('=') != -1)
                        {
                            section.Value.Settings.Add(fileLine);
                        }
                        else
                        {
                            section.Value.Lines.Add(fileLine);
                        }
                    }
                }
            }
        }

        public void Read(string iniKey, string txtKey = null, bool parseSettings = false)
        {
            if (string.IsNullOrEmpty(iniKey))
            {
                throw new ArgumentNullException(nameof(iniKey));
            }

            var section = new XwaIniSection();

            if (!string.IsNullOrEmpty(txtKey))
            {
                string txtPath = this.BasePath + txtKey + ".txt";

                if (File.Exists(txtPath))
                {
                    foreach (string fileLine in ReadFileLines(txtPath))
                    {
                        section.Lines.Add(fileLine);
                    }

                    section.TxtKey = txtKey;
                }
            }

            foreach (string fileLine in ReadFileLines(this.BasePath + this.Extension, iniKey))
            {
                section.Lines.Add(fileLine);
            }

            this.Sections[iniKey] = section;

            if (parseSettings)
            {
                this.ParseSettings(iniKey);
            }
        }

        public void Save(string basePath = null)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = this.BasePath;
            }

            this.RemoveDuplicatedEmptyLines();
            this.RemoveEmptyFiles(basePath);

            if (!this.HasLinesOrSettings)
            {
                return;
            }

            using (var writer = new StreamWriter(basePath + this.Extension, false, _encoding))
            {
                foreach (var section in this.Sections)
                {
                    if (!string.IsNullOrEmpty(section.Key))
                    {
                        writer.WriteLine("[" + section.Key + "]");
                    }

                    WriteSection(writer, section.Value);
                }
            }

            foreach (var section in this.Sections)
            {
                string txtKey = section.Value.TxtKey;

                if (string.IsNullOrEmpty(txtKey))
                {
                    continue;
                }

                using (var writer = new StreamWriter(basePath + txtKey + ".txt", false, _encoding))
                {
                    WriteSection(writer, section.Value);
                }
            }

            this.BasePath = basePath;
        }

        public void ClearSectionLines(string iniKey, bool createSection = false)
        {
            if (iniKey == null)
            {
                throw new ArgumentNullException(nameof(iniKey));
            }

            if (this.Sections.TryGetValue(iniKey, out XwaIniSection section))
            {
                section.Lines.Clear();
            }
            else if (createSection)
            {
                this.Sections[iniKey] = new XwaIniSection();
            }
        }

        public ICollection<string> RetrieveLinesList(string iniKey)
        {
            if (iniKey == null)
            {
                throw new ArgumentNullException(nameof(iniKey));
            }

            this.ClearSectionLines(iniKey, true);

            var section = this.Sections[iniKey];
            return section.Lines;
        }

        public bool CreateSectionIfNotExists(string section)
        {
            if (this.Sections.ContainsKey(section))
            {
                return false;
            }
            else
            {
                this.Sections[section] = new XwaIniSection();
                return true;
            }
        }

        private void RemoveDuplicatedEmptyLines()
        {
            foreach (var section in this.Sections)
            {
                var lines = new string[section.Value.Lines.Count];
                section.Value.Lines.CopyTo(lines, 0);

                section.Value.Lines.Clear();

                bool isEmptyLine = false;

                foreach (string fileLine in lines)
                {
                    if (string.IsNullOrWhiteSpace(fileLine))
                    {
                        if (isEmptyLine)
                        {
                            continue;
                        }

                        isEmptyLine = true;
                    }
                    else
                    {
                        isEmptyLine = false;
                    }

                    section.Value.Lines.Add(fileLine);
                }

                if (section.Value.Lines.All(t => string.IsNullOrWhiteSpace(t)))
                {
                    section.Value.Lines.Clear();
                }
            }
        }

        private void RemoveEmptyFiles(string basePath)
        {
            string iniPath = basePath + this.Extension;

            if (!this.HasLinesOrSettings)
            {
                File.Delete(iniPath);

                foreach (var section in this.Sections)
                {
                    string txtKey = section.Value.TxtKey;

                    if (string.IsNullOrEmpty(txtKey))
                    {
                        continue;
                    }

                    string txtPath = basePath + txtKey + ".txt";
                    File.Delete(txtPath);
                }
            }
        }

        private static void WriteSection(TextWriter writer, XwaIniSection section)
        {
            foreach (string line in section.Lines)
            {
                writer.WriteLine(line);
            }

            if (section.Lines.Count == 0 || !string.IsNullOrWhiteSpace(section.Lines.Last()))
            {
                writer.WriteLine();
            }

            if (section.Settings.Count != 0)
            {
                foreach (string line in section.Settings)
                {
                    writer.WriteLine(line);
                }

                writer.WriteLine();
            }
        }

        private static IList<string> ReadFileLines(string path, string section = null)
        {
            section = section ?? string.Empty;

            var values = new List<string>();

            if (!File.Exists(path))
            {
                return values;
            }

            using (var reader = new StreamReader(path, _encoding))
            {
                string fileLine;
                bool readSection = string.IsNullOrEmpty(section);

                while ((fileLine = reader.ReadLine()) != null)
                {
                    string line = fileLine.Trim();

                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        string name = line.Substring(1, line.Length - 2);

                        if (string.Equals(name, section, StringComparison.OrdinalIgnoreCase))
                        {
                            readSection = true;
                        }
                        else
                        {
                            readSection = false;
                        }
                    }
                    else
                    {
                        if (readSection)
                        {
                            values.Add(fileLine);
                        }
                    }
                }
            }

            return values;
        }

        private static bool IsComment(string line)
        {
            return line.StartsWith("#") || line.StartsWith(";") || line.StartsWith("//");
        }
    }
}
