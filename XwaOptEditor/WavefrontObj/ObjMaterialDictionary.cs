using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;

namespace WavefrontObj
{
    [Serializable]
    public class ObjMaterialDictionary : Dictionary<string, ObjMaterial>
    {
        public ObjMaterialDictionary()
        {
        }

        protected ObjMaterialDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public static ObjMaterialDictionary FromFile(string fileName)
        {
            ObjMaterialDictionary materials = new ObjMaterialDictionary();

            using (StreamReader file = new StreamReader(fileName))
            {
                string line;
                ObjMaterial material = null;

                while ((line = file.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    line = line.Trim();

                    if (line.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string[] values = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    switch (values[0].ToUpperInvariant())
                    {
                        case "NEWMTL":
                            if (values.Length != 2)
                            {
                                throw new InvalidDataException("missing material name");
                            }

                            material = new ObjMaterial();
                            material.Name = values[1];
                            materials.Add(material.Name, material);
                            break;

                        case "KD":
                            if (material == null || values.Length != 4)
                            {
                                break;
                            }

                            material.DiffuseColor = new ObjVector3(
                                float.Parse(values[1], CultureInfo.InvariantCulture),
                                float.Parse(values[2], CultureInfo.InvariantCulture),
                                float.Parse(values[3], CultureInfo.InvariantCulture));
                            break;

                        case "D":
                            if (material == null || values.Length != 2)
                            {
                                break;
                            }

                            material.DissolveFactor = float.Parse(values[1], CultureInfo.InvariantCulture);
                            break;

                        case "MAP_KD":
                            if (material == null || values.Length != 2)
                            {
                                break;
                            }

                            material.DiffuseMapFileName = values[1];
                            break;

                        case "MAP_D":
                            if (material == null || values.Length != 2)
                            {
                                break;
                            }

                            material.AlphaMapFileName = values[1];
                            break;
                    }
                }
            }

            return materials;
        }

        public void Save(string fileName)
        {
            using (StreamWriter file = new StreamWriter(fileName))
            {
                foreach (var material in this.Values)
                {
                    file.WriteLine("newmtl {0}", material.Name);
                    file.WriteLine("map_Kd {0}", material.DiffuseMapFileName);

                    if (material.AlphaMapFileName != null)
                    {
                        file.WriteLine("map_D {0}", material.AlphaMapFileName);
                    }
                }
            }
        }
    }
}
