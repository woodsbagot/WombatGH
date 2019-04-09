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
    public class DivideByTargetLength : GH_Component
    {

        public DivideByTargetLength() : base("Divide by Target Length", "Divide Target", 
            "Divides supplied curves evenly into lengths that match the supplied target lengths as closely as possible.", "Wombat", "Curve")
        {
        }

        public override Guid ComponentGuid => new Guid("{7B5F929E-1B52-4776-92B5-D175AF7EBB02}");
        protected override Bitmap Icon => Resources.WombatGH_DivByTarget;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to divide evenly by target length.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "L", "Target curve division length to approximate.",
                GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Division points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Tangents", "T", "Tangent vectors at division points.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Parameters", "t", "Parameter values at division points.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve crv = default(Curve);
            double len = 0.0;

            if (!DA.GetData("Curve", ref crv)) return;
            if (!DA.GetData("Length", ref len)) return;

            int divCnt = (int)Math.Max(1,Math.Round((crv.GetLength())/len));

            double[] tVal = crv.DivideByCount(divCnt, true);

            List<Point3d> ptList = new List<Point3d>();
            List<Vector3d> tanList = new List<Vector3d>();
            for (int i = 0; i < tVal.Length; i++)
            {
                Point3d pt = crv.PointAt(tVal[i]);
                ptList.Add(pt);
                Vector3d tan = crv.TangentAt(tVal[i]);
                tanList.Add(tan);
            }

            DA.SetDataList("Points", ptList);
            DA.SetDataList("Tangents", tanList);
            DA.SetDataList("Parameters", tVal);
        }
    }
}
