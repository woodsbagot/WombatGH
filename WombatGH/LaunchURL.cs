using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using WombatGH.Properties;

namespace WombatGH
{
    public class LaunchURL : GH_Component
    {

        public LaunchURL() : base("Launch URL", "URL", "Launches a web page from a specified URL", "Wombat", "Files / Interop")
        {
        }

        public override Guid ComponentGuid => new Guid("{271DE55D-80FE-4625-A7D0-E4147972A3AF}");
        protected override Bitmap Icon => Resources.WombatGH_LaunchURL;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Launch", "L", "Set True to launch web page from URL", GH_ParamAccess.item, false);
            pManager.AddTextParameter("URL", "U", "URL address for web page", GH_ParamAccess.item, "http://www.grasshopper3d.com/group/wombatgh");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            string url = "http://www.grasshopper3d.com/group/wombatgh";
            if (!DA.GetData("Launch", ref run)) return;
            if (!DA.GetData("URL", ref url)) return;

            if (run)
            {
                Process.Start(url);
            }
        }
    }
}
