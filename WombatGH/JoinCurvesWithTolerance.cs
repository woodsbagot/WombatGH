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
   
    public class JoinCurvesWithTolerance : GH_Component
    {

        public JoinCurvesWithTolerance() : base("Join Curves With Tolerance", "Join T", "Joins curves with supplied dimensional tolerance.", 
            "Wombat", "Curve")
        {
        }

        public override Guid ComponentGuid => new Guid("{1D4DB55F-F344-441A-ABCA-4215B797A081}");
        protected override Bitmap Icon => Resources.WombatGH_JoinCrvWTol;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Curves to join.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "T", "Dimensional tolerance for join.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Joined curve.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> crvs = new List<Curve>();
            double tol = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;

            if (!DA.GetDataList("Curves", crvs)) return;
            if (!DA.GetData("Tolerance", ref tol)) return;

            Curve[] crvsJ = Curve.JoinCurves(crvs, tol);

            DA.SetDataList("Curve", crvsJ);
        }
    }
}
