using System;
using System.Drawing;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.GUI;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using Rhino.Display;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using WombatGH.Properties;

namespace WombatGH
{

    public class ScreenOrientedText : GH_Component
    {
        internal struct GH_PathDisplayObj
        {
            internal string m_text;
            internal Point3d m_loc;
            internal Color m_col;
            internal double m_size;
            internal string m_font;
            internal int m_horizAlign;
            internal int m_vertAlign;
        }

        private List<GH_PathDisplayObj> m_items;
        private BoundingBox m_clipbox;
        private string _ViewportFilter;
        private double m_depth;
        private bool m_absolute = false;

        public string ViewportFilter
        {
            get => _ViewportFilter;
            set
            {
                _ViewportFilter = value;
                UpdateMenu();
            }
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            var item = Menu_AppendItem(menu, "Viewport Filter");
            var vpItem = Menu_AppendTextItem(menu, ViewportFilter, VPFilterKeyDown, null, false);
            Menu_AppendItem(menu, "Absolute Sizing", AbsoluteSizingClicked, null, true, m_absolute);
        }

        private void UpdateMenu()
        {
            if (!string.IsNullOrEmpty(ViewportFilter))
            {
                Message = ViewportFilter;
            }
        }

        public override bool Write(GH_IWriter writer)
        {

            writer.SetString("viewportFilter", ViewportFilter);
            writer.SetBoolean("Absolute", m_absolute);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {

            reader.TryGetString("viewportFilter", ref _ViewportFilter);
            reader.TryGetBoolean("Absolute", ref m_absolute);
            UpdateMenu();
            return base.Read(reader);
        }

        private void AbsoluteSizingClicked(object sender, EventArgs e)
        {
            m_absolute = !m_absolute;
            ExpireSolution(true);
        }

        private void VPFilterKeyDown(GH_MenuTextBox sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode != System.Windows.Forms.Keys.Return) return;
            if (Operators.CompareString(ViewportFilter, sender.Text, false) == 0)
            {
                return;
            }
            RecordUndoEvent("Filter: " + sender.Text);
            ViewportFilter = sender.Text;
            Message = sender.Text;
            Attributes.ExpireLayout();
            Instances.RedrawAll();
        }

        public override BoundingBox ClippingBox => m_clipbox;

        public ScreenOrientedText()
            : base("Path Label", "PathLbl", "Displays the data tree paths of geometry", "Wombat", "Display")
        {
        }

        public override Guid ComponentGuid => new Guid("{331A3502-F6CB-442E-8FC8-B9350A2567A0}");
        protected override Bitmap Icon => Resources.WombatGH_LabelPaths;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "G", "The geometry to label", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Size Factor", "S", "A multiplier on the scale. Use this to adjust the size of the text in the display.", GH_ParamAccess.item, 1.0);

            pManager.AddColourParameter("Color", "C", "The color to display the text.", GH_ParamAccess.item, Color.White);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            m_items = null;
            m_clipbox = BoundingBox.Empty;
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (DA.Iteration == 0)
            {
                m_items = new List<GH_PathDisplayObj>();
                m_clipbox = BoundingBox.Empty;
            }

            GH_Structure<IGH_GeometricGoo> geo;

            var scFac = 1.0;
            var col = Color.Black;

            if (!DA.GetDataTree("Geometry", out geo)) return;

            DA.GetData("Size Factor", ref scFac);
            DA.GetData("Color", ref col);

            m_depth = 0.0;

            for (var i = 0; i < geo.Branches.Count; i++)
            {
                var path = geo.Paths[i];
                var branch = geo.Branches[i];
                foreach (var geoItem in branch)
                {
                    var item = new GH_PathDisplayObj
                    {
                        m_text = path.ToString(true),
                        m_loc = geoItem.Boundingbox.Center,
                        m_size = scFac,
                        m_col = col,
                        m_font = "Arial",
                        m_horizAlign = 1,
                        m_vertAlign = 1
                    };
                    m_items.Add(item);
                }
            }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (m_items == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(ViewportFilter) && Operators.CompareString(ViewportFilter, "*", false) != 0 && !LikeOperator.LikeString(args.Viewport.Name, ViewportFilter, CompareMethod.Binary))
            {
                return;
            }

            foreach (var obj in m_items)
            {
                //try
                //{
                Plane plane;
                Plane nearPlane;
                args.Viewport.GetFrustumFarPlane(out plane);
                args.Viewport.GetFrustumNearPlane(out nearPlane);
                plane.Origin = obj.m_loc;
                var bounds = args.Viewport.Bounds;

                var depthDir = Vector3d.Zero;
                if (args.Display.Viewport.IsParallelProjection)
                {
                    depthDir = plane.Normal;
                }
                else
                {
                    depthDir = new Vector3d(args.Viewport.CameraLocation - plane.Origin);
                }
                depthDir.Unitize();
                var depthAdjustedOrigin = plane.Origin + (depthDir * m_depth);
                plane.Origin = depthAdjustedOrigin;
                //plane.Origin = center;

                //Figure out the size. This means measuring the visible size in the viewport AT the current location.
                double pixPerUnit;
                var viewport = args.Viewport;
                viewport.GetWorldToScreenScale(obj.m_loc, out pixPerUnit);

                var size = obj.m_size;           

                if (!m_absolute)
                {
                    size = size / pixPerUnit;

                }

                var font = "Arial";
                if (!string.IsNullOrEmpty(obj.m_font))
                {
                    font = obj.m_font;
                }

                var proxyText = new Text3d(obj.m_text, Plane.WorldXY, size);
                proxyText.FontFace = font;
                var bb = proxyText.BoundingBox;
                double xOffset = 0;
                double yOffset = 0;
                switch (obj.m_horizAlign)
                {
                    case 0: //left
                        xOffset = 0;
                        break;
                    case 1: //center
                        xOffset = bb.Max.X * -0.5;
                        break;
                    case 2: //right
                        xOffset = bb.Max.X * -1;
                        break;
                }

                switch (obj.m_vertAlign)
                {
                    case 0: //bottom
                        yOffset = 0;
                        break;
                    case 1: //center
                        yOffset = size * -0.5;
                        break;
                    case 2: //top
                        yOffset = size * -1;
                        break;

                }

                plane.Origin = plane.PointAt(xOffset, yOffset);

                args.Display.Draw3dText(obj.m_text, obj.m_col, plane, size, font);
      
                //}
                //catch { }
            }
        }
    }
}