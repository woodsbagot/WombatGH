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
    public class CloseCurve : GH_Component
    {

        public CloseCurve() : base("Close Curve", "Close", "Close a list of curves if they're open", "Wombat", "Curve")
        {
        }

        public override Guid ComponentGuid => new Guid("{31841fce-36e5-11e9-b210-d663bd873d93}");
        protected override Bitmap Icon => Resources.WombatGH_CloseCurve;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Curve", "C", "Curve to close", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "T", "Tolerance for curve close operation", GH_ParamAccess.item, 0.0);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Closed Curve", "C", "The closed curves",
                GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double tolerance = 0;
            List<Curve> curvesToClose = new List<Curve>();
            if (!DA.GetDataList("Curve", curvesToClose)) return;
            DA.GetData("Tolerance", ref tolerance);

            List<Curve[]> closedCurves = new List<Curve[]>();

            foreach (Curve curve in curvesToClose)
            {
                //this is new in Rhino 6.12, and I don't think it's working correctly yet
                //Boolean canClose = curve.MakeClosed(tolerance);
                //closedCurves.Add(curve);

                //So we'll do a more crude close until they get that sorted

                double tempTolerance = tolerance;
                if (tempTolerance == 0) {
                    tempTolerance = curve.PointAtStart.DistanceTo(curve.PointAtEnd)+1;
                }
                if (curve.PointAtStart.DistanceTo(curve.PointAtEnd) < tempTolerance)
                {
                    Line newLine = new Line(curve.PointAtEnd, curve.PointAtStart);
                    List<Curve> curves = new List<Curve>();
                    curves.Add(curve);
                    curves.Add(newLine.ToNurbsCurve());
                    Curve[] newCurve  = Curve.JoinCurves(curves);
                    DA.SetDataList("Closed Curve", newCurve);
                }
                tempTolerance = tolerance;
            }

        }
    }
}
