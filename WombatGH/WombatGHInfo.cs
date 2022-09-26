using System;
using System.Drawing;
using System.Reflection;
using Grasshopper.Kernel;

namespace WombatGH
{
    public class WombatGHInfo : GH_AssemblyInfo
    {
        public override string Name => "Wombat";

        public override Bitmap Icon => Properties.Resources.WombatGH_WOMBAT_TAB_ICON;
        public override string Description => "";

        public override Guid Id => new Guid("ce132bbb-4e91-42da-88c9-14a6f5da87c6");

        public override string AuthorName => "Woods Bagot Design Technology";

        public override string AuthorContact => "Brian.Ringley@woodsbagot.com; Andrew.Heumann@woodsbagot.com";

        public override string Version
        {
            get
            {
                return AssemblyVersion;
            }
        }

        public override string AssemblyVersion
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var assemblyName = new AssemblyName(assembly.FullName);
                return assemblyName.Version.ToString();
            }
        }

        public override Bitmap AssemblyIcon => Properties.Resources.WombatGH_WOMBAT_TAB_ICON;
    }
}
