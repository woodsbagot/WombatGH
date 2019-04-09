using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;
using WombatGH.Properties;

namespace WombatGH
{
    public class BrepClosestPointAdvanced : GH_Component
    {
       
        public BrepClosestPointAdvanced() : base("Brep Closest Point (Advanced)", "Brep CP+",
              "An extended version of the Brep Closest Point component with additional information.",
              "Wombat", "Brep")
        {
        }

        public override Guid ComponentGuid => new Guid("afff991d-6556-499a-85b5-37c9fabc8503");
        protected override System.Drawing.Bitmap Icon => Resources.WombatGH_BRepCPAdv;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Base Brep", GH_ParamAccess.item);
            pManager.AddPointParameter("Point", "P", "Sample Point", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Distance", "MD", "The maximum search distance. Will ignore if value is 0 - but can substantially speed up search if provided.", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Closest Point", "P", "The closest point on the Brep", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "D", "The distance to the closest point", GH_ParamAccess.item);
            pManager.AddVectorParameter("Normal", "N", "The normal of the Brep at this point.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Component Index", "CI", "The index of the Brep Component that's closest", GH_ParamAccess.item);
            pManager.AddTextParameter("Component Type", "CT", "The type of Brep Component that is closest to the sample pt", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep B = null;
            Point3d P = Point3d.Unset;
            double maxDist = 0;
            if (!DA.GetData("Brep", ref B)) return;
            if (!DA.GetData("Point", ref P)) return;
            DA.GetData("Max Distance", ref maxDist);
            if (B == null || P == null) return;
            Point3d closestPoint;
            ComponentIndex componentIndex;
            double s;
            double t;
            Vector3d normal;
            B.ClosestPoint(P, out closestPoint, out componentIndex, out s, out t, maxDist, out normal);
            

            if (componentIndex.ComponentIndexType == ComponentIndexType.BrepEdge)
            {
                int[] faceIndices = B.Edges[componentIndex.Index].AdjacentFaces();
                Vector3d tempNormal = new Vector3d(0, 0, 0);
                foreach (int i in faceIndices)
                {
                    double u, v;
                    B.Faces[i].ClosestPoint(P, out u, out v);
                    tempNormal += B.Faces[i].NormalAt(u, v);
                }
                tempNormal.Unitize();
                normal = tempNormal;
            }

            var ci = componentIndex.Index;
            var dist = closestPoint.DistanceTo(P);
            var ct = componentIndex.ComponentIndexType.ToString();

            DA.SetData("Closest Point", closestPoint);
            DA.SetData("Distance", dist);
            DA.SetData("Normal", normal);
            DA.SetData("Component Index", ci);
            DA.SetData("Component Type", ct);
        }
    }
}
