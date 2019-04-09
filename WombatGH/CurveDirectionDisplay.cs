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
    public class CurveDirectionDisplay : GH_Component
    {
        private readonly List<DirDisplayCrv> _curves;
        private BoundingBox _clipBox = BoundingBox.Empty;

        public CurveDirectionDisplay() : base("Curve Direction Display", "CrvDir", "Custom Preview to display curve direction", "Wombat", "Display")
        {
            _curves = new List<DirDisplayCrv>();
        }

        public override Guid ComponentGuid => new Guid("{65DFC460-AF75-4886-9BB9-77C59BA009E8}");
        protected override Bitmap Icon => Resources.WombatGH_CrvDirDisplay;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "The curves to display", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "Col", "The color to use for direction display", GH_ParamAccess.item,
                Color.Red);
            pManager.AddIntegerParameter("Size", "S", "The size to display direction arrows", GH_ParamAccess.item, 20);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (DA.Iteration == 0)
            {
                _curves.Clear();
                _clipBox = BoundingBox.Empty;
            }

            Curve c = null;
            var col = Color.Red;
            var size = 20;
            if (!DA.GetData("Curves", ref c)) return;
            if (!DA.GetData("Color", ref col)) return;
            if (!DA.GetData("Size", ref size)) return;

            _curves.Add(new DirDisplayCrv
            {
                curve = c,
                color = col,
                size = size
            });
            _clipBox.Union(c.GetBoundingBox(false));
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            foreach (var dispCrv in _curves)
            {
                try
                {
                    var c = dispCrv.curve;
                    var CParams = c.DivideByCount(c.ToNurbsCurve().Points.Count, true);
                    foreach (var t in CParams)
                    {
                        args.Display.DrawArrowHead(c.PointAt(t), c.TangentAt(t), dispCrv.color, dispCrv.size, 0);
                    }  
                }
                catch
                { 
                }
            }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            DrawViewportWires(args);
        }

        private struct DirDisplayCrv
        {
            public Curve curve;
            public Color color;
            public int size;
        }
    }
}
