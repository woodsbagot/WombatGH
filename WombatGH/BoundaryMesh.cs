using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class BoundaryMesh : GH_Component
    {

        public BoundaryMesh() : base("Boundary Mesh", "BoundaryM", "Create mesh from polyline.", "Wombat", "Mesh")
        {
        }

        public override Guid ComponentGuid => new Guid("{904B3013-434D-4867-B7BA-F9D5E6E1883F}");
        protected override Bitmap Icon => Resources.WombatGH_BoundaryMesh;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polyline", "P", "Polyline boundary to mesh.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Resulting mesh", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve crv = default(Curve);
            if (!DA.GetData("Polyline", ref crv)) return;
            if (!crv.TryGetPolyline(out Polyline pline))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot convert input curve to polyline");
                return;
            }
            if (!crv.IsClosed)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Operation will not work on open curves");
                return;
            }

            Mesh mesh = Mesh.CreateFromClosedPolyline(pline);

            DA.SetData("Mesh", mesh);
        }
    }
}
