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
            pManager.AddGenericParameter("Geometry", "G", "Geometry to arrange", GH_ParamAccess.list);
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
            List<GeometryBase> castGeo = new List<GeometryBase>();
            List<GH_ObjectWrapper> geoToTransform = new List<GH_ObjectWrapper>();
            int rowCount = -1;
            double spacing = 2.0;
            
            if (!DA.GetDataList("Geometry", geoToTransform)) return;
            if (!DA.GetData("Items in Row", ref rowCount)) return;
            if (!DA.GetData("Spacing", ref spacing)) return;

            List<GeometryBase> transformedGeo = new List<GeometryBase>();
            List<Transform> xforms = new List<Transform>();

            geoToTransform.ForEach(item =>
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, item.Value.GetType().ToString());
                if (GH_Convert.ToGeometryBase(item.Value) == null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, item.Value.GetType().ToString() + "not supported. Contact Woods Bagot DT to add support");
                }
                else
                {
                    castGeo.Add(GH_Convert.ToGeometryBase(item.Value));
                }
            });

            double x = 0;
            double y = 0;
            int i = 0;
            double maxY = 0;

            castGeo.ForEach(geo =>
            {
                if (i == rowCount)
                {
                    x = 0;
                    i = 0;
                    y += maxY + spacing;
                }
                i++;

                var bbox = geo.GetBoundingBox(false);
                var width = bbox.Diagonal.X;
                var height = bbox.Diagonal.Y;
                var lowerLeft = bbox.Min;

                var currPoint = new Point3d(x, y, 0);

                var transform = Transform.Translation(currPoint - lowerLeft);

                geo.Transform(transform);
                transformedGeo.Insert(0, geo);
                xforms.Add(transform);
                x += width + spacing;
                if (maxY < height) maxY = height;
               

            });

            DA.SetDataList("Transformed Geometry", transformedGeo);
            DA.SetDataList("Transforms", xforms);
        }
    }
}
