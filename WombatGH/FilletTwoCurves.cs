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
            pManager.AddCurveParameter("First Curve", "C", "Fisrt Curves.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Second Curve", "C", "Second Curves.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Radius", "R", "Fillet Radius.", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("Join", "J", "Join.", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Arc Extension", "A", "ArcExtension.", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve1 Trimmed", "C1", "Curve1 Trimmed.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve2 Trimmed", "C2", "Curve2 Trimmed.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Filleted Curve", "CF", "Filleted Curve.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve Joined", "CJ", "Joined curve.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve crv1 = null;
            Curve crv2 = null;

            double radius = 0;
            bool join = true;
            bool trim = true;
            bool arcExtension = true;
            double tol = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            double angelTol = Rhino.RhinoDoc.ActiveDoc.ModelAngleToleranceDegrees;

            if (!DA.GetData(0, ref crv1)) return;
            if (!DA.GetData(1, ref crv2)) return;
            if (!DA.GetData<double>(2, ref radius)) return;
            if (!DA.GetData<bool>(3, ref join)) return;
            if (!DA.GetData<bool>(4, ref arcExtension)) return;


            var crvEndPt1 = crv1.PointAt(0);
            var crvEndPt2 = crv2.PointAt(0);
            Curve[] filletCurves = Curve.CreateFilletCurves(crv1, crvEndPt1, crv2, crvEndPt2, radius, join, trim, arcExtension, tol, angelTol);

            if (filletCurves.Length == 0)
            {
                if (arcExtension == false)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to extend curve2 to curve1. No filleted geometry produced.");
                }
                else if (arcExtension == true)
                {
                    if (Curve.CreateFilletCurves(crv1, crvEndPt1, crv2, crvEndPt2, radius, join, trim, false, tol, angelTol).Length == 0)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to extend curve2 to curve1. No filleted geometry produced.");
                    }
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to create arc extension. No filleted geometry produced");
                    }
                }
            }
            else
            {
                if (join)
                {
                    DA.SetData("Curve Joined", filletCurves[0]);
                }
                else
                {
                    if (radius == 0)
                    {
                        DA.SetData("Curve1 Trimmed", filletCurves[0]);
                        DA.SetData("Curve2 Trimmed", filletCurves[1]);
                    }
                    else
                    {
                        DA.SetData("Curve1 Trimmed", filletCurves[0]);
                        DA.SetData("Curve2 Trimmed", filletCurves[1]);
                        DA.SetData("Filleted Curve", filletCurves[2]);
                    }
                }
            }
        }



        public override Guid ComponentGuid
        {
            get { return new Guid("4ecd8664-7e01-4b01-9313-a72d48fdd1e9"); }
        }
    }
}