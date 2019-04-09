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
    public class SurfaceCenter : GH_Component
    {

        public SurfaceCenter() : base("Surface Center", "SrfCenter", 
            "Returns point, normal, and frame at reparameterized surface center.", "Wombat", "Surface")
        {
        }

        public override Guid ComponentGuid => new Guid("{0BEB55E3-C524-4955-AC7F-0C2337308CAE}");
        protected override Bitmap Icon => Resources.WombatGH_SrfCtr;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Surface", "S", "Surface to evaluate at center", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Center", "P", "Point at center", GH_ParamAccess.item);
            pManager.AddVectorParameter("Normal", "N", "Normal at center", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Frame", "F", "Frame at center", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var brep = default(Brep);
            if (!DA.GetData("Surface", ref brep)) return;

            var srf = brep.Faces[0];
            var reparam = new Interval(0,1);
            srf.SetDomain(0, reparam);
            srf.SetDomain(1, reparam);

            var center = srf.PointAt(0.5, 0.5);
            var normal = srf.NormalAt(0.5, 0.5);
            srf.FrameAt(0.5, 0.5, out Plane frame);

            if (srf.OrientationIsReversed)
            {
                frame.Flip();
            }

            DA.SetData("Center", center);
            DA.SetData("Normal", normal);
            DA.SetData("Frame", frame);
        }
    }
}
