using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class ConstructPathMask : GH_Component
    {

        public ConstructPathMask()
          : base("Construct Path Mask", "ConMask", "Constructs a data tree path mask from a list of index characters.", "Wombat", "Sets")
        {
        }

        public override Guid ComponentGuid => new Guid("{41ef139d-fc07-4e7e-8263-b1ad43d26160}");
        protected override Bitmap Icon => Resources.WombatGH_ConPathMsk;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Index Characters", "I", "Branch path mask index characters as list.", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Mask", "M", "Branch path masks.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> iCharList = new List<string>();
            DA.GetDataList("Index Characters", iCharList);
            string mask = "{" + string.Join(";", iCharList) + "}";

            DA.SetData("Mask", mask);
        }
    }
}