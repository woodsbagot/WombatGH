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
    public class CurveCurveCP : GH_Component
    {

        public CurveCurveCP() : base("Curve-Curve Closest Point", "CCCP", "Finds the closest point between a pair of curves", "Wombat", "Curve")
        {
        }

        public override Guid ComponentGuid => new Guid("{CA4EF06D-5987-415F-A472-523DDDB67229}");
        protected override Bitmap Icon => Resources.WombatGH_CrvCrvCP;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve A", "A", "The first curve", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve B", "B", "The second curve", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Point on A", "A", "Point on the first curve", GH_ParamAccess.item);
            pManager.AddNumberParameter("Param on A", "tA", "Param on the first curve", GH_ParamAccess.item);

            pManager.AddPointParameter("Point on B", "B", "The second curve", GH_ParamAccess.item);
            pManager.AddNumberParameter("Param on B", "tB", "Param on the second curve", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve A = null;
            Curve B = null;

            if (!DA.GetData("Curve A", ref A)) return;
            if (!DA.GetData("Curve B", ref B)) return;


            A.ClosestPoints(B, out Point3d ptOnA, out Point3d ptOnB);
            DA.SetData("Point on A", ptOnA);
            DA.SetData("Point on B", ptOnB);
            A.ClosestPoint(ptOnA, out double tA);
            B.ClosestPoint(ptOnA, out double tB);

            DA.SetData("Param on A", tA);
            DA.SetData("Param on B", tB);
        }
    }
}
