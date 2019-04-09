using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class UVDisplay : GH_Component
    {

        public UVDisplay() : base("Surface UV Display", "UVDisplay", "Displays the U and V axes of surface or polysurface faces", "Wombat", "Display")
        {
            _faces = new List<UVFace>();
            _bbox = BoundingBox.Empty;
        }

        private struct UVFace
        {
            public BrepFace Face;
            public List<Plane> OrientationPlanes;
            public List<Vector3d> Normals;
            public int Size;
        }

        private List<UVFace> _faces;
        private BoundingBox _bbox;

        public override Guid ComponentGuid => new Guid("2AE34938-5AA4-4BC2-80D7-EC41EC2644EB");
        protected override Bitmap Icon => Resources.WombatGH_UVDisplay;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Surfaces", "S", "The surfaces or polysurfaces to preview",
                GH_ParamAccess.item);
            pManager.AddIntegerParameter("Size", "S", "The size of the directional arrows to draw", GH_ParamAccess.item, 1);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep b = null;
            var size = 20;
            if (!DA.GetData("Surfaces", ref b)) return;
            if (!DA.GetData("Size", ref size)) return;
            int numDivisions = 5;

            if (DA.Iteration == 0)
            {
                _faces.Clear();
                _bbox = BoundingBox.Empty;
            }

            foreach (var bf in b.Faces)
            {
                List<double[]> uvs = GetUVs(bf, numDivisions);
                _faces.Add(new UVFace
                {
                    Face = bf,
                    Normals = GetNormals(bf, uvs),
                    OrientationPlanes = GetPlanes(bf, uvs),
                    Size = size
                });
            }
            _bbox.Union(b.GetBoundingBox(false));
        }

        private List<Plane> GetPlanes(BrepFace bf, List<double[]> uvs)
        {
            var frames = new List<Plane>();
            foreach (var uv in uvs)
            {
                Plane frame = Plane.Unset;
                bf.FrameAt(uv[0], uv[1], out frame);
                frames.Add(frame);
            }
            return frames;
        }

        private List<Vector3d> GetNormals(BrepFace bf, List<double[]> uvs)
        {
            return uvs.Select(uv => bf.NormalAt(uv[0], uv[1])).ToList();
        }

        private List<double[]> GetUVs(BrepFace bf, int numDivisions)
        {
            var uDom = bf.Domain(0);
            var vDom = bf.Domain(1);
            var uvs = new List<double[]>();
            for (int u = 0; u <= numDivisions; u++)
            {
                for (int v = 0; v <= numDivisions; v++)
                {
                    double pU = uDom.ParameterAt((double)u / numDivisions);
                    double pV = vDom.ParameterAt((double)v / numDivisions);
                    if (bf.IsPointOnFace(pU, pV) != PointFaceRelation.Exterior) uvs.Add(new[] { pU, pV });
                }
            }
            return uvs;
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            foreach (var m in _faces)
            {
                try
                {
                    var face = m.Face;
                    var size = m.Size;
                    var planes = m.OrientationPlanes;
                    var normals = m.Normals;

                    for (int i = 0; i < planes.Count; i++)
                    {
                        var plane = planes[i];
                        var normal = normals[i];
                        normal.Unitize();
                        args.Display.DrawDirectionArrow(plane.Origin,normal*size, face.OrientationIsReversed ? Color.Blue : Color.White);
                        args.Display.DrawDirectionArrow(plane.Origin, plane.XAxis * size, face.OrientationIsReversed ? Color.Green : Color.Red);
                        args.Display.DrawDirectionArrow(plane.Origin, plane.YAxis * size, face.OrientationIsReversed ? Color.Red : Color.Green);
                    }
                }
                catch
                {
                }
            }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            DrawViewportMeshes(args);
        }
    }
}
