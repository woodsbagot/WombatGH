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
    public class ExtendSurface : GH_Component
    {

        public ExtendSurface() : base("Extend Surface", "ExtendSrf", "Extend surface from edge", "Wombat", "Surface")
        {
        }

        public override Guid ComponentGuid => new Guid("{E2BA1CF0-6641-45A9-8087-825459912105}");
        protected override Bitmap Icon => Resources.WombatGH_ExtendSrf;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Surface to extend", GH_ParamAccess.item);
            pManager.AddNumberParameter("West Edge", "W", "Length to extend west edge (0 for no extension)", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("South Edge", "S", "Length to extend south edge (0 for no extension)", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("East Edge", "E", "Length to extend east edge (0 for no extension)", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("North Edge", "N", "Length to extend north edge (0 for no extension)", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("Extension Type", "T", "Set True for smooth extension, False for ruled",
                GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Extended surface", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var ghSrf = default(GH_Surface);
            if (!DA.GetData("Surface", ref ghSrf)) return;

            //get underlying brep from gh srf
            var brep = ghSrf.Value;

            //check for trimmed surface
            if (!brep.IsSurface) AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Extending a trimmed surface will untrim the surface!");

            //get underlying srf from brep
            var srf = brep.Surfaces.First();

            var wEdge = 0.0;
            var sEdge = 0.0;
            var eEdge = 0.0;
            var nEdge = 0.0;

            if (!DA.GetData("West Edge", ref wEdge)) return;
            if (!DA.GetData("South Edge", ref sEdge)) return;
            if (!DA.GetData("East Edge", ref eEdge)) return;
            if (!DA.GetData("North Edge", ref nEdge)) return;

           
            var smooth = true;
            
            if (!DA.GetData("Extension Type", ref smooth)) return;

            var extensionlengths = new[] {wEdge, sEdge, eEdge, nEdge};
            var allIsoStatuses = new[] {IsoStatus.West, IsoStatus.South, IsoStatus.East, IsoStatus.North};


            for (var i = 0; i < extensionlengths.Length; i++)
            {
                var len = extensionlengths[i];
                if(len <= 0) continue;
                srf = srf.Extend(allIsoStatuses[i], len, smooth);
            }

            DA.SetData(0, srf);
        }
    }
}
