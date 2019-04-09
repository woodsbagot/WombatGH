using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using WombatGH.Properties;

namespace WombatGH
{
    public class BackupData : GH_Component
    {

        public BackupData() : base("Backup Data", "PlanB", "Passes through a data list if that list contains elements; otherwise, passes a \"backup\"", "Wombat", "Sets")
        {
        }

        public override Guid ComponentGuid => new Guid("{61BA5EEC-BC8D-43A3-B093-C77674DA0ABB}");
        protected override Bitmap Icon => Resources.WombatGH_BackupData;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Primary Data", "D1", "The data to prefer and use as long as it is not null",
                GH_ParamAccess.tree);
            pManager[0].Optional = true;
            pManager.AddGenericParameter("Backup Data", "D2",
                "The secondary data to rely on if the primary data is empty or null", GH_ParamAccess.tree);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "The resulting data", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<IGH_Goo> primary;
            GH_Structure<IGH_Goo> secondary;

            var hasPrimary = DA.GetDataTree("Primary Data", out primary);
            var hasSecondary = DA.GetDataTree("Backup Data", out secondary);

            var primaryisNullOrEmpty = !primary.AllData(true).Any();

            var passPrimary = hasPrimary && !primaryisNullOrEmpty;

            DA.SetDataTree(0, passPrimary ? primary : secondary);
        }
    }
}
