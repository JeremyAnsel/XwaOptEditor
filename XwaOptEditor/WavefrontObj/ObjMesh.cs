using System;
using System.Collections.Generic;
using System.Linq;

namespace WavefrontObj
{
    public class ObjMesh
    {
        public ObjMesh(string name)
        {
            this.Name = name;
            this.FaceGroups = new List<ObjFaceGroup>();
        }

        public string Name { get; set; }

        public IList<ObjFaceGroup> FaceGroups { get; private set; }

        public void CompactFaceGroups()
        {
            var groups = new List<ObjFaceGroup>(this.FaceGroups.Count);

            foreach (var faceGroup in this.FaceGroups)
            {
                ObjFaceGroup index = null;

                foreach (var group in groups)
                {
                    if (group.MaterialName == faceGroup.MaterialName)
                    {
                        index = group;
                        break;
                    }
                }

                if (index == null)
                {
                    groups.Add(faceGroup);
                }
                else
                {
                    foreach (var face in faceGroup.Faces)
                    {
                        index.Faces.Add(face);
                    }
                }
            }

            groups.TrimExcess();
            this.FaceGroups = groups;
        }

        public void SplitFaceGroups()
        {
            this.CompactFaceGroups();

            Func<ObjIndex, int[]> indices = index =>
                index.D < 0 ?
                new[] { index.A, index.B, index.C } :
                new[] { index.A, index.B, index.C, index.D };

            var faceGroups = new List<ObjFaceGroup>();

            foreach (var faceGroup in this.FaceGroups)
            {
                var groups = new List<ObjFaceGroup>();

                foreach (var face in faceGroup.Faces)
                {
                    var faceIndices = indices(face.VerticesIndex);

                    var group = groups
                        .Where(g => g.Faces.SelectMany(t => indices(t.VerticesIndex)).Any(t => faceIndices.Contains(t)))
                        .FirstOrDefault();

                    if (group != null)
                    {
                        group.Faces.Add(face);
                    }
                    else
                    {
                        group = new ObjFaceGroup();
                        group.MaterialName = faceGroup.MaterialName;
                        group.Faces.Add(face);
                        groups.Add(group);
                    }
                }

                faceGroups.AddRange(groups);
            }

            this.FaceGroups = faceGroups;
        }
    }
}
