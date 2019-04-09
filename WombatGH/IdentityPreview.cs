using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Display;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class IdentityPreview : GH_Component
    {

        private struct PreviewItem
        {
            public IGH_PreviewData Geo;
            public Color DrawColor;
        }

        public IdentityPreview() : base("Identity Preview", "RainbowID", "Displays geometry with random colors assigned so as to differentiate separate items", "Wombat", "Display")
        {
        }

        public override Guid ComponentGuid => new Guid("{D55CBCB1-DEA0-44E3-A58C-160081B33CA4}");
        protected override Bitmap Icon => Resources.WombatGH_IdentityPreview;

        private readonly List<PreviewItem> _allPreviewItems = new List<PreviewItem>();
        private int _seed = 2;
        private BoundingBox _clipBox;
        private Random _rand;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "G", "The geometry to preview", GH_ParamAccess.list);
            pManager.HideParameter(0);
            pManager.AddIntegerParameter("Seed", "S", "The random seed for color generation", GH_ParamAccess.item, 2);
        }

        public override void ClearData()
        {
            _allPreviewItems.Clear();
            base.ClearData();
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var allGeo = new List<IGH_GeometricGoo>();
            if (!DA.GetDataList("Geometry", allGeo)) return;
            if (!DA.GetData("Seed", ref _seed)) return;

            if (DA.Iteration == 0)
            {
                _allPreviewItems.Clear();
                _clipBox = BoundingBox.Empty;
                _rand = new Random(_seed);
                
            }
           
            _allPreviewItems.AddRange(allGeo.OfType<IGH_PreviewData>().Select(pd => new PreviewItem{ Geo = pd, DrawColor = NextRandomColor()}));
            allGeo.ForEach(a => _clipBox.Union(a.Boundingbox));
        }

        private Color NextRandomColor()
        {
            if(_rand == null) _rand = new Random(_seed);
            return Color.FromArgb(255, _rand.Next(255), _rand.Next(255), _rand.Next(255));
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (_allPreviewItems == null || _allPreviewItems.Count == 0) return;

            foreach (var previewItem in _allPreviewItems)
            {
                try
                {
                    var drawArgs = new GH_PreviewWireArgs(args.Viewport,args.Display,previewItem.DrawColor,1);
                    previewItem.Geo.DrawViewportWires(drawArgs);
                }
                catch (Exception e)
                {
                   AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                }
            }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            if (_allPreviewItems == null || _allPreviewItems.Count == 0) return;

            foreach (var previewItem in _allPreviewItems)
            {
                try
                {
                    var drawArgs = new GH_PreviewMeshArgs(args.Viewport, args.Display, new DisplayMaterial(previewItem.DrawColor),MeshingParameters.FastRenderMesh);
                    previewItem.Geo.DrawViewportMeshes(drawArgs);
                }
                catch (Exception e)
                {
                   AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                }
            }
        }
    }
}
