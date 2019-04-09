using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Linq;
using WombatGH.Properties;

namespace WombatGH
{
    public class TextOutlines : GH_Component
    {
        private Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;

        public TextOutlines()
            : base("Text Outlines", "Text2Crv", "Gets the outline curves from text", "Wombat", "Text")
        {
        }

        public override Guid ComponentGuid => new Guid("4b33e069-d108-4787-b380-b4bc5b46dbb0");
        protected override System.Drawing.Bitmap Icon => Resources.WombatGH_TxtOutlines;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Text Content", "T", "The text content", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "P", "The plane to draw the text in", GH_ParamAccess.item,Plane.WorldXY);
            pManager.HideParameter(1);
            pManager.AddTextParameter("Face", "F", "Face", GH_ParamAccess.item,"Arial");
            pManager.AddNumberParameter("Size", "S", "Size", GH_ParamAccess.item,12);
            pManager.AddBooleanParameter("Bold", "B", "Bold", GH_ParamAccess.item,false);
            pManager.AddBooleanParameter("Italics", "I", "Italics", GH_ParamAccess.item,false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Text Curves", "C", "The exploded text outlines", GH_ParamAccess.list);
            pManager.AddBoxParameter("Text Bounding Box", "B", "The bounding box surrounding the text",
                GH_ParamAccess.item);
            pManager.HideParameter(1);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string face = "";
            bool bold = false;
            bool italics = false;
            double size = 0;
            string content = "";
            Plane pl = Plane.Unset;
            if (!DA.GetData("Face", ref face)) return;
            if (!DA.GetData("Bold", ref bold)) return;
            if (!DA.GetData("Italics", ref italics)) return;
            if (!DA.GetData("Size", ref size)) return;
            if (!DA.GetData("Text Content", ref content)) return;
            if (!DA.GetData("Plane", ref pl)) return;

            if (size == 0)
                size = 1;

            if (!string.IsNullOrEmpty(face) && size > 0 && !string.IsNullOrEmpty(content) &&
                pl.IsValid)
            {
                if(!Rhino.DocObjects.Font.AvailableFontFaceNames().Contains(face))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, ("That face is not available"));
                }
                Rhino.DocObjects.Font.FontWeight weight = Rhino.DocObjects.Font.FontWeight.Normal;
                Rhino.DocObjects.Font.FontStyle style = Rhino.DocObjects.Font.FontStyle.Upright;

                if (bold) weight = Rhino.DocObjects.Font.FontWeight.Bold;     
            
                if (italics) style = Rhino.DocObjects.Font.FontStyle.Italic;
                  
                var te = new TextEntity()
                {
                    Plane = pl,
                    PlainText = content,
                    TextHeight = size,
                    Font = new Rhino.DocObjects.Font(face, weight, style, false, false)
                };
                var crvs = te.Explode();
                DA.SetDataList("Text Curves", crvs);
                var bbox = BoundingBox.Empty;
                foreach (var curve in crvs)
                {
                    bbox.Union(curve.GetBoundingBox(true));
                }
                DA.SetData("Text Bounding Box", bbox);
            }
        }
    }
}
