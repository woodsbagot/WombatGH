using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class ArrangeInGrid : GH_Component
    {

        public ArrangeInGrid() : base("Arrange in Grid", "Grid", "Arrange a list of geometry in rows", "Wombat", "Geometry")
        {
        }

        public override Guid ComponentGuid => new Guid("{8DE7B354-400C-46C4-83BF-8E03E4E7E256}");
        protected override Bitmap Icon => Resources.WombatGH_ArrangeInGrid;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "G", "Geometry to arrange", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Items in Row", "I", "How many items to include in each row",
                GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Spacing", "S", "The minimum spacing between items", GH_ParamAccess.item, 2.0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("Transformed Geometry", "G", "The transformed items arranged in a grid",
                GH_ParamAccess.list);
            pManager.AddTransformParameter("Transforms", "X", "The transforms used to locate the items",
                GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<IGH_GeometricGoo> geoRaw = new List<IGH_GeometricGoo>();
            int rowCount = -1;
            double spacing = 2.0;
            if (!DA.GetDataList("Geometry", geoRaw)) return;
            if (!DA.GetData("Items in Row", ref rowCount)) return;
            if (!DA.GetData("Spacing", ref spacing)) return;

            List<IGH_GeometricGoo> transformedGeo = new List<IGH_GeometricGoo>();
            List<Transform> xforms = new List<Transform>();

            double x = 0;
            double y = 0;

            var geoToTransform = geoRaw.Where(g => g != null).ToList();
            geoToTransform.Reverse();

            while (geoToTransform.Count > 0)
            {
                double maxY = 0;
                x = 0;
                for (int i = 0; i < rowCount; i++) // for each item in the row 
                {
                    //if (geoToTransform.Count < i + 1) continue;
                    //pop
                    if (geoToTransform.Count == 0) break;
                    var thisGeo = geoToTransform[0].DuplicateGeometry();
                    geoToTransform.RemoveAt(0);

                    var bbox = thisGeo.Boundingbox;
                    var width = bbox.Diagonal.X;
                    var height = bbox.Diagonal.Y;
                    var lowerLeft = bbox.Min;

                    var currPoint = new Point3d(x, y, 0);

                    var transform = Transform.Translation(currPoint - lowerLeft);

                    thisGeo.Transform(transform);
                    transformedGeo.Insert(0, thisGeo);
                    xforms.Add(transform);
                    x += width + spacing;
                    if (maxY < height) maxY = height; //for a given row, keep bumping the max row height
                }
                y += maxY + spacing;
            }
            transformedGeo.Reverse();
            xforms.Reverse();

            DA.SetDataList("Transformed Geometry", transformedGeo);
            DA.SetDataList("Transforms", xforms);
        }
    }
}
