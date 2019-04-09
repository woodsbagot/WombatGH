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
    public class ExtrudeMesh : GH_Component
    {

        public ExtrudeMesh() : base("Extrude as Mesh", "ExtrudeM", "Extrude", "Wombat", "Mesh")
        {
        }

        public override Guid ComponentGuid => new Guid("{B53277E9-A509-45F9-BD4E-6D5DD5B8E6C4}");
        protected override Bitmap Icon => Resources.WombatGH_ExtrudeAsMesh;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polyline", "P", "Polyline to extrude", GH_ParamAccess.item);
            pManager.AddVectorParameter("Direction", "D", "Extrusion direction", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Cap", "C", "Set True to cap extrusion",
                GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Resulting mesh", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var crv = default(Curve);
            if (!DA.GetData("Polyline", ref crv)) return;
            if (!crv.TryGetPolyline(out Polyline pline))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot convert input curve to polyline");
                return;
            }

            var dir = default(Vector3d);
            if (!DA.GetData("Direction", ref dir)) return;

            if (pline.IsClosed)
            {
                UnifyPolylineWinding(pline, dir);
            }

            var joinedMesh = ExtrudePolylineToMesh(pline, dir);

            var cap = false;
            DA.GetData("Cap", ref cap);
            if (cap && pline.IsClosed)
            {
                var bottomMesh = Mesh.CreateFromClosedPolyline(pline);
                var topMesh = bottomMesh.DuplicateMesh();
                bottomMesh.Flip(true, true, true);
                topMesh.Transform(Transform.Translation(dir));
                joinedMesh.Append(bottomMesh);
                joinedMesh.Append(topMesh);
            }

            if (cap && !pline.IsClosed) AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Cannot cap an open polyline extrusion! Try closing your polyline or set cap to False");

            DA.SetData("Mesh", joinedMesh);
        }

        private static Mesh ExtrudePolylineToMesh(Polyline pline, Vector3d dir)
        {
            var joinedMesh = new Mesh();
            for (var i = 0; i < (pline.IsClosed?pline.Count:pline.Count-1); i++)
            {
                var ptA = pline[i];
                var ptB = pline[(i + 1) % pline.Count];
                var ptC = ptB + dir;
                var ptD = ptA + dir;
                var mesh = new Mesh();
                mesh.Vertices.AddVertices(new[] { ptA, ptB, ptC, ptD });
                mesh.Faces.AddFace(new MeshFace(0, 1, 2, 3));
                joinedMesh.Append(mesh);
            }
            return joinedMesh;
        }

        private static void UnifyPolylineWinding(Polyline pline, Vector3d z)
        {
            var sum = Vector3d.Zero;
            for (var i = 0; i < pline.Count; i++)
            {
                var ptA = pline[i];
                var ptB = pline[(i + 1) % pline.Count];
                var ptC = pline[(i + 2) % pline.Count];
                var AB = ptB - ptA;
                var BC = ptC - ptB;
                sum += Vector3d.CrossProduct(AB, BC);
            }
            if (sum * z < 0)
            {
                pline.Reverse();
            }
        }
    }
}
