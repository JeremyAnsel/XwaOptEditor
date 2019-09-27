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
    }
}
