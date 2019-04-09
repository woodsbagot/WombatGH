using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class ZoomBoundingBox : GH_Component
    {

        public ZoomBoundingBox()
          : base("Zoom Bounding Box", "ZBB", "Zoom viewport to fit specified box", "Wombat", "Display")
        {
        }

        public override Guid ComponentGuid => new Guid("8f35acb3-83d5-4817-a740-d8e01e5313b6");
        protected override System.Drawing.Bitmap Icon => Resources.WombatGH_ZoomBbox;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBoxParameter("Box", "B", "Box to zoom to", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run", "R", "Set to true to zoom", GH_ParamAccess.item, false);
            pManager.AddTextParameter("Viewport Names", "VN", "Viewport Names to zoom to. Will use active viewport if left blank", GH_ParamAccess.list);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Box box = Box.Unset;
            bool run = false;
            var viewportNames = new List<string>(); ; if (!DA.GetData("Box", ref box)) return;
            if (!DA.GetData("Run", ref run)) return;
            DA.GetDataList("Viewport Names", viewportNames);
            var RhinoDocument = Rhino.RhinoDoc.ActiveDoc;
            if (run)
            {
                var vps = new List<Rhino.Display.RhinoViewport>();
                if (viewportNames == null || viewportNames.Count() == 0)
                {
                    vps.Add(RhinoDocument.Views.ActiveView.ActiveViewport);
                }
                foreach (string vpName in viewportNames)
                {
                    var View = RhinoDocument.Views.Find(vpName, true);
                    vps.Add(View.ActiveViewport);
                }

                foreach (Rhino.Display.RhinoViewport vp in vps)
                {
                    vp.ZoomBoundingBox(box.BoundingBox);
                }
            }
        }
    }
}
