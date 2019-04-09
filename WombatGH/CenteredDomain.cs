using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class CenteredDomain : GH_Component
    {

        public CenteredDomain()
          : base("Centered Domain", "DomCen", "Center a domain about zero from an input size.", "Wombat", "Sets")
        {
        }

        public override Guid ComponentGuid => new Guid("{a73eebb6-017e-4ef4-aa46-678849b44cc0}");
        protected override Bitmap Icon => Resources.WombatGH_CenteredDomain;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Domain Size", "S", "Size of domain as number.", GH_ParamAccess.item, 1.0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntervalParameter("Centered Domain", "I", "Domain centered about zero.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double size = 1.0;
            DA.GetData("Domain Size", ref size);

            double domStart = -size/2;
            double domEnd = size/2;
            Interval domCen = new Interval(domStart,domEnd);

            DA.SetData("Centered Domain", domCen);
        }
    }
}