using JeremyAnsel.Xwa.Opt;

namespace JeremyAnsel.Xwa.WpfOpt
{
    public sealed class MeshLodFace
    {
        internal MeshLodFace(Mesh mesh, MeshLod lod, FaceGroup face)
        {
            this.Mesh = mesh;
            this.Lod = lod;
            this.Face = face;
        }

        public Mesh Mesh { get; private set; }

        public MeshLod Lod { get; private set; }

        public FaceGroup Face { get; private set; }
    }
}
