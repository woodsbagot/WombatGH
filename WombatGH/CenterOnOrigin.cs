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
    public class CenterOnOrigin : GH_Component
    {

        public CenterOnOrigin() : base("Center Geometry at Origin", "Recenter", "Recenters a geometry object at the origin", "Wombat", "Geometry")
        {
        }

        public override Guid ComponentGuid => new Guid("{07B51697-D2A1-4A4F-A72C-F54331C8D815}");
        protected override Bitmap Icon => Resources.WombatGH_CenterOnOrigin;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "G", "Geometry to center", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("Recentered Geometry", "G", "The repositioned geometry", GH_ParamAccess.item);
            pManager.AddVectorParameter("Translation Vector", "V", "The translation of the geometry", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IGH_GeometricGoo geo = null;
            if (!DA.GetData("Geometry", ref geo)) return;
            var bbox = geo.Boundingbox;
            if (!bbox.IsValid)
            {
                bbox = geo.GetBoundingBox(Transform.Identity);
            }
            var transVec = Point3d.Origin - bbox.Center;
           
            var newGeo = geo.DuplicateGeometry().Transform(Transform.Translation(transVec));

            DA.SetData("Recentered Geometry", newGeo);
            DA.SetData("Translation Vector", transVec);
        }
    }
}
