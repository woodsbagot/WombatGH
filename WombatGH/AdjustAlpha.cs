using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using WombatGH.Properties;

namespace WombatGH
{
    public class AdjustColorAlpha : GH_Component
    {

        public AdjustColorAlpha() : base("Adjust Alpha", "Alpha", "Sets the alpha channel of a color", "Wombat", "Display")
        {
        }

        public override Guid ComponentGuid => new Guid("{2199FAA7-6E71-49EF-82B9-E2AC483D875C}");
        protected override Bitmap Icon => Resources.WombatGH_AdjustAlpha;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddColourParameter("Color", "C", "The color to adjust", GH_ParamAccess.item);
            pManager.AddNumberParameter("Alpha", "A", "The alpha value (0-1) to assign", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddColourParameter("Color", "C", "The adjusted color", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var c = Color.Transparent;
            var alpha = Double.NaN;
            if (!DA.GetData("Color", ref c)) return;
            if (!DA.GetData("Alpha", ref alpha)) return;

            DA.SetData("Color", Color.FromArgb((int) Math.Min(255, alpha * 256), c.R, c.G, c.B));
        }
    }
}
