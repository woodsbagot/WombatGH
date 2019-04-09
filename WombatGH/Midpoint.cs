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
    public class Midpoint : GH_Component
    {

        public Midpoint() : base("Midpoint", "MP", "Evaluates the midpoint of a curve.", "Wombat", "Curve")
        {
        }

        public override Guid ComponentGuid => new Guid("{B371E20A-0FD4-4228-B578-DB1F1D1D9824}");
        protected override Bitmap Icon => Resources.WombatGH_Midpoint;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curves to evaluate.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Midpoint", "M", "Midpoint of curve.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve crv = default(Curve);
            if (!DA.GetData("Curve", ref crv)) return;

            crv.Domain = new Interval(0, 1);
            Point3d mp = crv.PointAt(0.5);

            DA.SetData("Midpoint", mp);
        }
    }
}
