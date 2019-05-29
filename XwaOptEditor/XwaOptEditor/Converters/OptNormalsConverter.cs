using HelixToolkit.Wpf;
using JeremyAnsel.Xwa.Opt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace XwaOptEditor.Converters
{
    class OptNormalsConverter : BaseConverter, IMultiValueConverter
    {
        public OptNormalsConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: model
            // values[1]: opt or mesh
            // values[2]: show or hide
            // values[3]: distance

            if (values.Take(4).Any(t => t == System.Windows.DependencyProperty.UnsetValue))
            {
                return null;
            }

            if (values[0] == null)
            {
                return null;
            }

            var model = (ModelVisual3D)values[0];
            model.Children.Clear();

            if (values[1] == null || (bool)values[2] == false)
            {
                return null;
            }

            float distance = (float)values[3];

            float normalSize;
            var normals = new List<Tuple<Vector, Vector>>();
            var meshes = new List<Mesh>();

            if (values[1] is OptFile)
            {
                var opt = (OptFile)values[1];
                meshes.AddRange(opt.Meshes);
                var span = opt.SpanSize;
                normalSize = Math.Max(Math.Max(span.X, span.Y), span.Z);
            }
            else if (values[1] is Mesh)
            {
                var mesh = (Mesh)values[1];
                meshes.Add(mesh);
                Vector span = mesh.Descriptor.Span;
                normalSize = Math.Max(Math.Max(span.X, span.Y), span.Z);
            }
            else
            {
                return null;
            }

            normalSize /= 16.0f;

            foreach (var mesh in meshes)
            {
                foreach (var lod in mesh.Lods)
                {
                    if (lod.Distance <= distance)
                    {
                        foreach (var faceGroup in lod.FaceGroups)
                        {
                            foreach (var face in faceGroup.Faces)
                            {
                                Index verticesIndex = face.VerticesIndex;
                                Index vertexNormalsIndex = face.VertexNormalsIndex;

                                normals.Add(Tuple.Create(mesh.Vertices[verticesIndex.A], mesh.VertexNormals[vertexNormalsIndex.A]));
                                normals.Add(Tuple.Create(mesh.Vertices[verticesIndex.B], mesh.VertexNormals[vertexNormalsIndex.B]));
                                normals.Add(Tuple.Create(mesh.Vertices[verticesIndex.C], mesh.VertexNormals[vertexNormalsIndex.C]));

                                if (verticesIndex.IsQuadrangle)
                                {
                                    normals.Add(Tuple.Create(mesh.Vertices[verticesIndex.D], mesh.VertexNormals[vertexNormalsIndex.D]));
                                }

                                Vector facePosition = new Vector();

                                if (verticesIndex.IsTriangle)
                                {
                                    Vector v;
                                    v = mesh.Vertices[verticesIndex.A];
                                    facePosition = facePosition.Move(v.X, v.Y, v.Z);
                                    v = mesh.Vertices[verticesIndex.B];
                                    facePosition = facePosition.Move(v.X, v.Y, v.Z);
                                    v = mesh.Vertices[verticesIndex.C];
                                    facePosition = facePosition.Move(v.X, v.Y, v.Z);
                                    facePosition = facePosition.Scale(1.0f / 3.0f, 1.0f / 3.0f, 1.0f / 3.0f);
                                }
                                else
                                {
                                    Vector v;
                                    v = mesh.Vertices[verticesIndex.A];
                                    facePosition = facePosition.Move(v.X, v.Y, v.Z);
                                    v = mesh.Vertices[verticesIndex.B];
                                    facePosition = facePosition.Move(v.X, v.Y, v.Z);
                                    v = mesh.Vertices[verticesIndex.C];
                                    facePosition = facePosition.Move(v.X, v.Y, v.Z);
                                    v = mesh.Vertices[verticesIndex.D];
                                    facePosition = facePosition.Move(v.X, v.Y, v.Z);
                                    facePosition = facePosition.Scale(1.0f / 4.0f, 1.0f / 4.0f, 1.0f / 4.0f);
                                }

                                Vector faceNormal = face.Normal;

                                normals.Add(Tuple.Create(facePosition, faceNormal));
                            }
                        }

                        break;
                    }
                }
            }

            var points = new Point3DCollection();

            foreach (var normalTuple in normals)
            {
                var position = normalTuple.Item1;
                var normal = normalTuple.Item2;

                points.Add(new Point3D(-position.Y, -position.X, position.Z));

                points.Add(new Point3D(
                    -position.Y + -normal.Y * normalSize,
                    -position.X + -normal.X * normalSize,
                    position.Z + normal.Z * normalSize));
            }

            var linesVisual = new LinesVisual3D()
            {
                Color = Colors.Magenta,
                Points = points
            };

            model.Children.Add(linesVisual);

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
