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
    public class InflateBox : GH_Component
    {

        public InflateBox() : base("Inflate Box", "Inflate", "Expand a box by a constant dimension on all sides", "Wombat", "Brep")
        {
        }

        public override Guid ComponentGuid => new Guid("{0AF61A22-FC78-4DD5-BCDE-C6546C926AEF}");
        protected override Bitmap Icon => Resources.WombatGH_InflateBox;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBoxParameter("Box", "B", "Box to inflate", GH_ParamAccess.item);
            pManager.AddNumberParameter("Amount", "A", "Amount to inflate", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBoxParameter("Box", "B", "Inflated Box", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var b = Box.Unset;
            var a = Double.NaN;
            if (!DA.GetData("Box", ref b)) return;
            if (!DA.GetData("Amount", ref a)) return;

            b.Inflate(a);

            DA.SetData("Box", b);
        }
    }
}
