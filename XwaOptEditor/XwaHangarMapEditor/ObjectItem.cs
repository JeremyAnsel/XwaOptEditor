using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaHangarMapEditor
{
    public sealed class ObjectItem
    {
        public int ModelIndex { get; set; }

        public string ModelName { get; set; }

        public List<string> ObjectProfiles { get; set; }

        public List<string> Skins { get; set; }
    }
}
