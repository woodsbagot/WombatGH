using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Rhino.Geometry;
using SurfaceComponents.SurfaceComponents;

namespace WombatGH
{
    public class LoftEx : GH_Component
    {

        public LoftEx() : base("Loft Ex", "LoftEx", "An advanced version of the loft component including more options", "Wombat", "Surface")
        {
        }

        public override Guid ComponentGuid => new Guid("{F6DFFD09-68EC-4C06-9129-985DFB57686B}");
        protected override Bitmap Icon => Properties.Resources.WombatGH_LoftEx;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "The Loft Sections", GH_ParamAccess.list);
            var start = pManager.AddPointParameter("Start", "S", "The point at the loft start", GH_ParamAccess.item);
            var end = pManager.AddPointParameter("End", "E", "The point at the loft end", GH_ParamAccess.item);
            var loftParam = new Param_LoftOptions();
            loftParam.SetPersistentData(new LoftOptions());
            var options = pManager.AddParameter(loftParam, "Options", "O", "Loft options", GH_ParamAccess.item);
            new[] { start, end, options }.ToList().ForEach(p => pManager[p].Optional = true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Loft", "L", "Loft Results", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var crvs = new List<Curve>();

            LoftOptions lo = null;

            var start = Point3d.Unset;
            var end = Point3d.Unset;
            if (!DA.GetDataList("Curves", crvs)) return;
            var hasStart = DA.GetData("Start", ref start);
            var hasEnd = DA.GetData("End", ref end);
            if (!DA.GetData("Options", ref lo)) return;


            Brep[] results = null;
            switch (lo.LoftFit)
            {
                case LoftSimplify.None:
                    results = Brep.CreateFromLoft(crvs, start, end, lo.LoftType, lo.ClosedLoft);
                    break;
                case LoftSimplify.Rebuild:
                    results = Brep.CreateFromLoftRebuild(crvs, start, end, lo.LoftType, lo.ClosedLoft, lo.RebuildCount);
                    break;
                case LoftSimplify.Refit:
                    results = Brep.CreateFromLoftRefit(crvs, start, end, lo.LoftType, lo.ClosedLoft, lo.RefitTolerance);
                    break;
            }

            Brep brep = null;

            if (results.Length == 1)
            {
                brep = results[0];
            }
            else
            {
                brep = Brep.JoinBreps(results, 1E-05)[0];
            }
            if (brep != null)
            {
                brep.Faces.SplitKinkyFaces();
                DA.SetData("Loft", brep);
            }
        }
    }
}
