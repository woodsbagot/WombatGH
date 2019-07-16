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
            pManager.AddNumberParameter("Tolerance", "T", "Dimensional tolerance for join.", GH_ParamAccess.item,Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
            pManager.AddIntegerParameter("Option", "O", "Joining Option: 0 = Blend, 1 = Extend", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Joined curve.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> crvs = new List<Curve>();
            double tol = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            double angelTol = Rhino.RhinoDoc.ActiveDoc.ModelAngleToleranceDegrees;
            int option = 0;

            if (!DA.GetDataList("Curves", crvs)) return;
            DA.GetData("Tolerance", ref tol);
            if (!DA.GetData("Option", ref option)) return;

            if (option == 0)
            {
                Curve[] crvsJ = Curve.JoinCurves(crvs, tol);
                DA.SetDataList("Curve", crvsJ);
            }else if(option == 1)
            {
                List<Curve> filletOutput = new List<Curve>();
                List<Curve> filletOutputPara = new List<Curve>();
                Curve[] filletSegment = new Curve[0];
                double crv1Parameter = 0.0;
                double crv2Parameter = 1.0;
                for (int i = 0; i < crvs.Count ; i++)
                {
                    if (i != crvs.Count -1 )
                    {
                        if (crvs[i] != null && crvs[i + 1] != null)
                        {
                            if (filletOutput.Count == 0 || filletSegment.Count() == 0)
                            {
                                filletSegment = Curve.CreateFilletCurves(
                                    crvs[i], 
                                    crvs[i]. PointAtNormalizedLength(crv1Parameter), 
                                    crvs[i + 1], 
                                    crvs[i + 1]. PointAtNormalizedLength(crv2Parameter), 
                                    0, false, true, false, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, angelTol);
                            }
                            else
                            {
                                filletSegment = Curve.CreateFilletCurves(
                                    filletOutput[filletOutput.Count - 1], 
                                    filletOutput[filletOutput.Count - 1]. PointAtNormalizedLength(crv1Parameter), 
                                    crvs[i + 1], 
                                    crvs[i + 1]. PointAtNormalizedLength(crv2Parameter),
                                    0, false, true, false, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, angelTol);
                            }

                            if (filletSegment.Count() == 0)
                            {
                                int j = i + 1;
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No geometry produced between curve " + i.ToString() + " and curve " + j.ToString() + ".");
                            }
                            else
                            {
                                if (filletSegment[0].PointAtEnd.DistanceTo(crvs[i + 1].PointAtStart) > filletSegment[0].PointAtEnd.DistanceTo(crvs[i + 1].PointAtEnd))
                                {
                                    if (filletSegment[0].PointAtEnd.DistanceTo(crvs[i].PointAtStart) < filletSegment[0].PointAtEnd.DistanceTo(crvs[i].PointAtEnd))
                                    {
                                        crv1Parameter = 1.0;
                                        crv2Parameter = 0.0;
                                    }
                                    else
                                    {
                                        crv1Parameter = 0.0;
                                        crv2Parameter = 0.0;
                                    }
                                }
                                else
                                {
                                    if (filletSegment[0].PointAtEnd.DistanceTo(crvs[i].PointAtStart) < filletSegment[0].PointAtEnd.DistanceTo(crvs[i].PointAtEnd))
                                    {
                                        crv1Parameter = 1.0;
                                        crv2Parameter = 1.0;
                                    }
                                }

                                if (filletOutput.Count == 0 || filletSegment.Count() == 0)
                                {
                                    filletSegment = Curve.CreateFilletCurves(
                                        crvs[i],
                                        crvs[i]. PointAtNormalizedLength(crv1Parameter),
                                        crvs[i + 1],
                                        crvs[i + 1]. PointAtNormalizedLength(crv2Parameter),
                                        0, false, true, false, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, angelTol);
                                }
                                else
                                {
                                    filletSegment = Curve.CreateFilletCurves(
                                        filletOutput[filletOutput.Count - 1],
                                        filletOutput[filletOutput.Count - 1]. PointAtNormalizedLength(crv1Parameter),
                                        crvs[i + 1],
                                        crvs[i + 1]. PointAtNormalizedLength(crv2Parameter),
                                        0, false, true, false, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, angelTol);
                                }

                                Rhino.RhinoApp.WriteLine(crv1Parameter.ToString(), crv2Parameter.ToString());

                                filletOutput.Add(filletSegment[0]);
                                filletOutput.Add(filletSegment[1]);
                                filletOutputPara.Add(filletSegment[0]);
                                if (i == crvs.Count - 2)
                                {
                                    filletOutputPara.Add(filletSegment[1]);
                                }
                            }
                        }
                    }
                }

                Curve[] crvsJ = Curve.JoinCurves(filletOutputPara, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
                DA.SetDataList("Curve", crvsJ);
            }
        }
    }
}
