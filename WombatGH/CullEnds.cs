using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class CullEnds : GH_Component
    {

        public CullEnds()
          : base("Cull Ends", "Cull Ends", "Separates the first and last item from a list.", "Wombat", "Sets")
        {
        }

        public override Guid ComponentGuid => new Guid("{1765aaa5-5e01-4ee1-a9cf-ab460c82db6d}");
        protected override System.Drawing.Bitmap Icon => Resources.WombatGH_CullEnds;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("List", "L", "List to cull", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("List", "L", "Culled list.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Culled", "C", "Culled items.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<object> listToCull = new List<object>();
            DA.GetDataList("List", listToCull);

            List<object> culledList = new List<object>();
            for (int i = 1; i < (listToCull.Count - 1); i++)
            {
                var newListItem = listToCull[i];
                culledList.Add(newListItem);
            }
            DA.SetDataList("List", culledList);

            List<object> culledItems = new List<object>();
            culledItems.Add(listToCull[0]);
            culledItems.Add(listToCull[listToCull.Count-1]);

            DA.SetDataList("Culled", culledItems);
        }      
    }
}