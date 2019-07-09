using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino;
using Rhino.Input;
using Rhino.DocObjects;
using Rhino.Input.Custom;


using System.Drawing;
using WombatGH.Properties;

namespace WombatGH
{
    public class FilletTwoCurves : GH_Component
    {
        public FilletTwoCurves()
          : base("FilletTwoCurves", "FilletTwoCurves",
              "FilletTwoCurves",
              "Wombat", "Curve")
        {
        }

        protected override Bitmap Icon => Resources.WombatGH_FilletTwoCurves;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("First Curve", "C", "Fisrt Curves.", GH_ParamAccess.list);
            pManager.AddCurveParameter("Second Curve", "C", "Second Curves.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Radius", "R", "Fillet Radius.", GH_ParamAccess.item,0);
            pManager.AddBooleanParameter("Join","J","Join.",GH_ParamAccess.item,true);
            pManager.AddBooleanParameter("Arc Extension", "A", "ArcExtension.", GH_ParamAccess.item,true);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve1 Filleted", "C1", "Curve2 Filleted.", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curve2 Filleted", "C2", "Curve2 Filleted.", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curve Joined", "CJ", "Joined curve.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> crv1 = new List<Curve>();
            List<Curve> crv2 = new List<Curve>();
            double radius = 0;
            bool join = true;
            bool trim = true;
            bool arcExtension = true;
            double tol = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            double angelTol = Rhino.RhinoDoc.ActiveDoc.ModelAngleToleranceDegrees;

            if (!DA.GetDataList(0, crv1)) return;
            if (!DA.GetDataList(1, crv2)) return;
            if (!DA.GetData(2, ref radius)) return;
            if (!DA.GetData(3, ref join)) return;
            if (!DA.GetData(4, ref arcExtension)) return;

            List<Curve> filletOutput = new List<Curve>();
            List<Curve> crv1Output = new List<Curve>();
            List<Curve> crv2Output = new List<Curve>();

            for (int i = 0; i < crv1.Count; i++)
            {
                var crvEndPt1 = crv1[i].PointAt(0);
                if (crv2.Count < crv1.Count)
                {
                    return;
                }
                else
                {
                    var crvEndPt2 = crv2[i].PointAt(0);
                    Curve[] filletCurves = Curve.CreateFilletCurves(crv1[i], crvEndPt1, crv2[i], crvEndPt2, radius, join, trim, arcExtension, tol, angelTol);
                    if (join && filletCurves.Length != 0)
                    {
                        filletOutput.Add(filletCurves[0]);
                    }
                    else
                    {
                        if (filletCurves.Length != 0  )
                        {
                            crv1Output.Add(filletCurves[0]);
                            crv2Output.Add(filletCurves[1]);
                        }
                    }
                }
            }

            DA.SetDataList("Curve Joined", filletOutput);
            DA.SetDataList("Curve1 Filleted", crv1Output);
            DA.SetDataList("Curve2 Filleted", crv2Output);

        }


        public override Guid ComponentGuid
        {
            get { return new Guid("4ecd8664-7e01-4b01-9313-a72d48fdd1e9"); }
        }
    }
}