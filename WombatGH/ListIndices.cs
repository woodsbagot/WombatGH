using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class ListIndices : GH_Component
    {

        public ListIndices()
          : base("List Indices", "Indices", "Provides a list of indices for items in a list.", "Wombat", "Sets")
        {
        }

        public override Guid ComponentGuid => new Guid("{3aa444f4-5a91-418a-ba28-683558ccc02d}");
        protected override Bitmap Icon => Resources.WombatGH_ListIndices;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "Data from which indices will be extracted.", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Indices", "i", "Indices of provided data.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var data = new List<object>();
            if (!DA.GetDataList("Data", data)) return;

            var indices = Enumerable.Range(0, data.Count);

            DA.SetDataList("Indices", indices);
        }      
    }
}