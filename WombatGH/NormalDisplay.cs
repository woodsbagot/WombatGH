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
    public class NormalDisplay : GH_Component
    {

        public NormalDisplay() : base("Normal Display", "DirDisplay", "Displays the two sides of a mesh with different colors for normal checking", "Wombat", "Display")
        {
            _meshes = new List<NormalDisplayMesh>();
            _bbox = BoundingBox.Empty;
        }

        private struct NormalDisplayMesh
        {
            public Mesh Mesh;
            public Color FrontFaceColor;
            public Color BackFaceColor;
        }

        private List<NormalDisplayMesh> _meshes;
        private BoundingBox _bbox;

        public override Guid ComponentGuid => new Guid("{0BE21D3C-439B-4C39-9F79-28110C03D05A}");
        protected override Bitmap Icon => Resources.WombatGH_NormalDisplay;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Geometry", "G", "The geometry to preview (will be converted to Mesh)",
                GH_ParamAccess.item);
            pManager.AddColourParameter("Front Face Color", "F", "The front face color", GH_ParamAccess.item,
                Color.Gray);
            pManager.AddColourParameter("Back Face Color", "B", "The back face color", GH_ParamAccess.item,
                Color.Red);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            var frontCol = Color.Gray;
            var backCol = Color.Red;

            if (!DA.GetData("Geometry", ref m)) return;
            if (!DA.GetData("Front Face Color", ref frontCol)) return;
            if (!DA.GetData("Back Face Color", ref backCol)) return;
            if (DA.Iteration == 0)
            {
                _meshes.Clear();
                _bbox = BoundingBox.Empty;
            }

            _meshes.Add(new NormalDisplayMesh
            {
                Mesh = m,
                BackFaceColor = backCol,
                FrontFaceColor = frontCol
            });
            _bbox.Union(m.GetBoundingBox(false));
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {         
            foreach (var m in _meshes)
            {
                try
                {
                    var frontFace = new Rhino.Display.DisplayMaterial(m.FrontFaceColor)
                    {
                        IsTwoSided = true,
                        BackDiffuse = m.BackFaceColor
                    };
                    args.Display.DrawMeshShaded(m.Mesh, frontFace);
                    
                }
                catch
                {
                }
            }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
           DrawViewportMeshes(args);
        }
    }
}
