using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Display;

namespace WombatGH
{
    public class DisplaySettings : GH_Component
    {

        public DisplaySettings() : base("Display Settings", "DispSettings", "Set various options for display preview", "Wombat", "Display")
        {
        }

        public override Guid ComponentGuid => new Guid("{D96EF04B-CBAA-4FC9-A32E-8A8295CCAC1E}");
        protected override Bitmap Icon => Properties.Resources.WombatGH_DispSettings;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Bump Preview", "BE", "Set to true to bump edges slightly in front of meshes", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Preview Mesh Edges", "ME", "Control preview of mesh edges", GH_ParamAccess.item);
            pManager.AddNumberParameter("Plane Radius", "PR", "Display size of planes in preview", GH_ParamAccess.item);
            var PSindex = pManager.AddIntegerParameter("Point Style", "PS", "Set the point display style", GH_ParamAccess.item);
            var PSParam = pManager[PSindex] as Param_Integer;
            PSParam.AddNamedValue("Simple", 0);
            PSParam.AddNamedValue("ControlPoint", 1);
            PSParam.AddNamedValue("ActivePoint", 2);
            PSParam.AddNamedValue("X", 3);
            pManager.AddColourParameter("Preview Color", "PC", "Geometry Preview Color", GH_ParamAccess.item);
            pManager.AddColourParameter("Selected Color", "PS", "The color for selected Geometry", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Reset Preview Colors", "RPC", "Set to true to reset your preview colors to Grasshopper Defaults.", GH_ParamAccess.item);
            Enumerable.Range(0, 7).ToList().ForEach(i => pManager[i].Optional = true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool bumpZ = false;
            var hasbumpZ = DA.GetData("Bump Preview", ref bumpZ);
            bool meshEdges = false;
            var hasmeshEdges = DA.GetData("Preview Mesh Edges", ref meshEdges);
            double planeRadius = 0;
            var hasplaneRadius = DA.GetData("Plane Radius", ref planeRadius);
            int pointStyle = -1;
            var haspointStyle = DA.GetData("Point Style", ref pointStyle);
            Color prevCol = Color.Transparent;
            var hasprevCol = DA.GetData("Preview Color", ref prevCol);
            Color selectedCol = Color.Transparent;
            var hasselectedCol = DA.GetData("Selected Color", ref selectedCol);
            bool resetPrev = false;
            var hasresetPrev = DA.GetData("Reset Preview Colors", ref resetPrev);

            if (hasbumpZ) Grasshopper.CentralSettings.PreviewBumpZBuffer = bumpZ;
            if (hasmeshEdges) Grasshopper.CentralSettings.PreviewMeshEdges = meshEdges;
            if (hasplaneRadius) Grasshopper.CentralSettings.PreviewPlaneRadius = planeRadius;
            if (haspointStyle) Grasshopper.CentralSettings.PreviewPointStyle = (PointStyle)pointStyle;
            var doc = Grasshopper.Instances.ActiveCanvas.Document;
            if (hasprevCol) doc.PreviewColour = prevCol;
            if (hasselectedCol) doc.PreviewColourSelected = selectedCol;
            if (hasresetPrev)
            {
                doc.PreviewColour = GH_Document.DefaultPreviewColour;
                doc.PreviewColourSelected = GH_Document.DefaultSelectedPreviewColour;
            }
        }
    }
}
