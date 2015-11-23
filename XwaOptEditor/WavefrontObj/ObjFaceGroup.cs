using System.Collections.Generic;

namespace WavefrontObj
{
    public class ObjFaceGroup
    {
        public ObjFaceGroup()
        {
            this.Faces = new List<ObjFace>();
        }

        public IList<ObjFace> Faces { get; private set; }

        public string MaterialName { get; set; }
    }
}
