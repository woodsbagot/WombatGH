using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace WombatGH
{
    public class WombatGHInfo : GH_AssemblyInfo
    {
        public override string Name => "Wombat";

        public override Bitmap Icon => Properties.Resources.WombatGH_WOMBAT_TAB_ICON;
        public override string Description => "WombatGH Plugin. Modified for compatibility with the ShapeDiver Cloud Platform.";

        public override Guid Id => new Guid("ce132bbb-4e91-42da-88c9-14a6f5da87c6");

        public override string AuthorName => "Woods Bagot Design Technology";

        public override string AuthorContact => "Brian.Ringley@woodsbagot.com; Andrew.Heumann@woodsbagot.com";

        public override string Version => "1.4.0.0";
        public override string AssemblyVersion => "1.4.0.0";
        public override Bitmap AssemblyIcon => Properties.Resources.WombatGH_WOMBAT_TAB_ICON;
    }
}
