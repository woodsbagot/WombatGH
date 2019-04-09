using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class UnrollBrep : GH_Component
    {

        public UnrollBrep()
          : base("Unroll Brep", "Unroll Brep", "Unrolls brep and optional associated curves and points", "Wombat", "Brep")
        {
        }

        public override Guid ComponentGuid => new Guid("68762b7d-7e11-469a-a50c-e1bc1964ba42");
        protected override System.Drawing.Bitmap Icon => Resources.WombatGH_UnrollBrep;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Brep", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curves", "C", "Curves", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddPointParameter("Points", "P", "Points", GH_ParamAccess.list);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Brep to Unroll", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curves", "C", "Curves to unroll with Brep", GH_ParamAccess.list);
            pManager.AddPointParameter("Points", "P", "Points to unroll with Brep", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep Brep = null;
            var Curves = new List<Curve>(); ;
            var Points = new List<Point3d>(); ;
            if (!DA.GetData("Brep", ref Brep)) return;
            bool hasCurves = DA.GetDataList("Curves", Curves);
            bool hasPoints = DA.GetDataList("Points", Points);
            
            if (Brep != null)
            {
                Unroller unroller = new Unroller(Brep);
                if(hasCurves) unroller.AddFollowingGeometry(Curves);
                if(hasPoints) unroller.AddFollowingGeometry(Points);
                TextDot[] dots;
                Point3d[] pts;
                Curve[] unrolledCrvs;
                unroller.ExplodeOutput = false;
                var B = unroller.PerformUnroll(out unrolledCrvs, out pts, out dots);
                var C = unrolledCrvs;
                var P = pts;

                DA.SetDataList("Brep", B);
                DA.SetDataList("Curves", C);
                DA.SetDataList("Points", P);
            }
        }
    }
}
