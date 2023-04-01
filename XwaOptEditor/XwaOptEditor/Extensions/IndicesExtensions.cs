using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XwaOptEditor.Extensions
{
    static class IndicesExtensions
    {
        public static int At(this Indices indices, int index)
        {
            return index switch
            {
                0 => indices.A,
                1 => indices.B,
                2 => indices.C,
                3 => indices.D,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
        }
    }
}
