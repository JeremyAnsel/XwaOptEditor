
namespace WavefrontObj
{
    public class ObjMaterial
    {
        public ObjMaterial()
        {
            this.DiffuseColor = new ObjVector3(1.0f, 1.0f, 1.0f);
            this.DissolveFactor = 1.0f;
        }

        public string Name { get; set; }

        public ObjVector3 DiffuseColor { get; set; }

        public float DissolveFactor { get; set; }

        public string DiffuseMapFileName { get; set; }

        public string AlphaMapFileName { get; set; }
    }
}
