using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class FootInchToDecimalFoot : GH_Component
    {

        public FootInchToDecimalFoot()
          : base("Foot Inch To Decimal Foot", "Ft-In to Decimal Ft", "Converts fractional foot-inch notation to decimal foot notation.",
              "Wombat", "Document")
        {
        }

        public override Guid ComponentGuid => new Guid("{c9553763-a66c-4fd7-ab7a-aac3b0b40ef1}");
        protected override Bitmap Icon => Resources.WombatGH_FtInToDecimalFt;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Formatted Value", "V", "Values formatted in fractional notation.",
                GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Value in Feet", "F", "Decimal value in feet.", GH_ParamAccess.item);
            pManager.AddTextParameter("Value in Inches", "I", "Decimal value in inches.", GH_ParamAccess.item);
            pManager.AddTextParameter("Value in Document Units", "U", "Decimal value in active document unit system.", GH_ParamAccess.item);
        }

        string[] removeEmpties(string[] toproc)
        {
            List<string> resOut = new List<string>();
            foreach (string str in toproc)
            {
                if (str != String.Empty)
                {
                    resOut.Add(str);
                }
            }
            return resOut.ToArray();
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string val = "";

            if (!DA.GetData("Formatted Value", ref val)) return;

            string splitchars = "'";
            string[] ftParts = val.Split(splitchars.ToCharArray());

            string ftPart = ftParts[0];
            string remains = String.Join("", ftParts, 1, ftParts.Length - 1);
            double ft;
            Double.TryParse(ftPart, out ft);
            bool isNegative = false;
            if (ft < 0)  isNegative = true;
            splitchars = "- \"";

            string[] inchParts = remains.Split(splitchars.ToCharArray());

            double inchVal = 0.0;
            foreach (string str in removeEmpties(inchParts))
            {
                //Print(String.Format("parsing string chunk {0}", str));
                double tempVal = 0;
                if (str.Contains("/"))
                {
                    try
                    {
                        string[] fracParts = removeEmpties(str.Split("/".ToCharArray()));
                        double tempVal2 = 1.0;
                        Double.TryParse(fracParts[0], out tempVal);
                        Double.TryParse(fracParts[1], out tempVal2);
                        if (tempVal2 != 0)
                        {
                            //Print(String.Format("Adding Fraction {0}/{1} = {2}", tempVal, tempVal2, (tempVal / tempVal2)));
                            inchVal += (tempVal / tempVal2);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, ex.Message);
                        //Print(ex.ToString());
                    }
                }
                else
                {
                    Double.TryParse(str, out tempVal);
                    inchVal += tempVal;
                }
                //Print(String.Format("inchVal = {0}", inchVal));
            }
            //Print(String.Format("inchVal/12 = {0}", inchVal / 12.0));
            
            double feet = isNegative ? ft - inchVal / 12.0 : ft + inchVal / 12.0;
            double inches = isNegative ? ft * 12 - inchVal : ft * 12 + inchVal;

            DA.SetData("Value in Feet", feet);
            DA.SetData("Value in Inches", inches);

            //Convert to document units
            UnitSystem docUnits = RhinoDoc.ActiveDoc.ModelUnitSystem;
            double unitConvert = RhinoMath.UnitScale(Rhino.UnitSystem.Feet, docUnits);
            double docUnitVal = feet*unitConvert;

            DA.SetData("Value in Document Units", docUnitVal);
        }

        public override void AddedToDocument(GH_Document document)
        {
            if (Locked)
            {
                RemoveHandlers();
            }
            else
            {
                AddHandlers();
            }
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            RemoveHandlers();
        }

        private void AddHandlers()
        {
            RemoveHandlers();
            RhinoDoc.DocumentPropertiesChanged += OnUnitsChanged;
        }

        private void RemoveHandlers()
        {
            RhinoDoc.DocumentPropertiesChanged -= OnUnitsChanged;
        }

        private UnitSystem cachedUnitSystem = RhinoDoc.ActiveDoc.ModelUnitSystem;

        private void OnUnitsChanged(object sender, DocumentEventArgs e)
        {

            if (e.Document.ModelUnitSystem != cachedUnitSystem)
            {
                cachedUnitSystem = e.Document.ModelUnitSystem;
                ExpireSolution(true);
            }
        }
    }
}