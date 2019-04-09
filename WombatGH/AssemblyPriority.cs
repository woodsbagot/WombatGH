using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using WombatGH.Properties;


namespace WombatGH
{
    public class AssemblyPriority : GH_AssemblyPriority
    {
       

        public override GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("Wombat", Resources.WombatGH_WOMBAT_TAB_ICON);
            return GH_LoadingInstruction.Proceed;
        }


    }

}
