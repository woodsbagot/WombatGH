using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using SquishyXMorphs;
using WombatGH.Properties;

namespace WombatGH
{

    public class SubBox : GH_Component
    {

        public SubBox() : base("Sub-Box", "SubBox", "Retrieves an Isoparametric subset of a box", "Wombat", "Brep")
        {
        }

        public override Guid ComponentGuid => new Guid("{95DF5148-80D2-45D6-AA07-DB54CD0BF44C}");
        protected override Bitmap Icon => Resources.WombatGH_SubBox;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Parameter_TwistedBox(), "Box", "B", "The box to evaluate", GH_ParamAccess.item);
            pManager.AddIntervalParameter("U Domain", "U", "The U Domain", GH_ParamAccess.item, new Interval(0, 1));
            pManager.AddIntervalParameter("V Domain", "V", "The V Domain", GH_ParamAccess.item, new Interval(0, 1));
            pManager.AddIntervalParameter("W Domain", "W", "The W Domain", GH_ParamAccess.item, new Interval(0, 1));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Sub-Box", "SB", "The retrieved sub-box", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_TwistedBox tb = null;
            Interval uDom = Interval.Unset;
            Interval vDom = Interval.Unset;
            Interval wDom = Interval.Unset;
            try
            {
                if (!DA.GetData("Box", ref tb)) return;
            }
            catch // this is maybe not necessary? IDK LOL
            {
                object tboxObj = null;
                DA.GetData("Box", ref tboxObj);
            }
            if (!DA.GetData("U Domain", ref uDom)) return;
            if (!DA.GetData("V Domain", ref vDom)) return;
            if (!DA.GetData("W Domain", ref wDom)) return;

            if (tb == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "one or more boxes was null");
                return;
            }

            var paramSpaceCorners = GetParamSpaceCorners(uDom, vDom, wDom);

            var corners = paramSpaceCorners.Select(p => tb.PointAt(p.X, p.Y, p.Z));

            var resultBox = new GH_TwistedBox(corners);

            Box asBox;
            if (IsBox(resultBox, out asBox,Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance))
            {
                DA.SetData("Sub-Box", asBox);
            }
            else
            {
                DA.SetData("Sub-Box", resultBox);

            }
        }

        private static List<Point3d> GetParamSpaceCorners(Interval uDom, Interval vDom, Interval wDom)
        {
            return new List<Point3d>()
            {
                new Point3d(uDom.Min, vDom.Min, wDom.Min),
                new Point3d(uDom.Max, vDom.Min, wDom.Min),
                new Point3d(uDom.Max, vDom.Max, wDom.Min),
                new Point3d(uDom.Min, vDom.Max, wDom.Min),
                new Point3d(uDom.Min, vDom.Min, wDom.Max),
                new Point3d(uDom.Max, vDom.Min, wDom.Max),
                new Point3d(uDom.Max, vDom.Max, wDom.Max),
                new Point3d(uDom.Min, vDom.Max, wDom.Max)
            };
        }

        internal static Box GetBox(GH_TwistedBox tb)
        {
            var plane = new Plane(tb.Corner(0), tb.Corner(1), tb.Corner(3));
            var bbox = tb.GetBoundingBox(Transform.ChangeBasis(Plane.WorldXY, plane));

            return new Box(plane, bbox);
        }

        internal static bool IsBox(GH_TwistedBox tb, out Box asBox, double tolerance)
        {
            asBox = GetBox(tb);
            var boxCorners = asBox.GetCorners();
            for (int i = 0; i < 8; i++)
            {
                if(tb.Corner(i).DistanceTo(boxCorners[i]) > tolerance)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
