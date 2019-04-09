using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino;
using WombatGH.Properties;

namespace WombatGH
{
    public class ConvertCase : GH_Component
    {

        public ConvertCase() : base("Convert Case", "Case", "Converts text to PascalCase, camelCase, and Title Case", "Wombat", "Text")
        {
        }

        public override Guid ComponentGuid => new Guid("{D95384D8-1BFC-4BE8-B091-0DCE75F05D9A}");
        protected override Bitmap Icon => Resources.WombatGH_ConvertCase;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Text", "T", "Text to convert", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Conversion", "C", "Specify conversion type", GH_ParamAccess.item, 0);

            var conVals = pManager[1] as Param_Integer;
            conVals.AddNamedValue("PascalCase", 0);
            conVals.AddNamedValue("camelCase", 1);
            conVals.AddNamedValue("Title Case", 2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Text", "T", "Converted text", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string text = null;
            if (!DA.GetData("Text", ref text)) return;

            int conType = 0;
            if (!DA.GetData("Conversion", ref conType)) return;

            string[] words = text.Split(' ');

            string pascalCase = String.Join("", words.Select(word => Char.ToUpperInvariant(word[0]) + word.Substring(1)));

            switch (conType)
            {
                case 0:
                    //convert Title Case to PascalCase

                    DA.SetData(0, pascalCase);
                    break;

                case 1:
                    //convert Title Case to camelCase
                    string camelCase = Char.ToLowerInvariant(pascalCase[0]) + pascalCase.Substring(1);

                    DA.SetData(0, camelCase);
                    break;

                case 2:
                    //convert PascalCase or camelCase to Title Case
                    var withSpaces = System.Text.RegularExpressions.Regex.Replace(text, "(\\B[A-Z])", " $1");
                    var TI = new System.Globalization.CultureInfo("en-US", false).TextInfo;
                    var titleCase = TI.ToTitleCase(withSpaces);

                    DA.SetData(0, titleCase);
                    break;
            }
        }
    }
}
