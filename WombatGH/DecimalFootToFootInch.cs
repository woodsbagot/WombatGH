using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class DecimalFootToFootInch : GH_Component
    {

        public DecimalFootToFootInch()
          : base("Decimal Foot To Foot-Inch", "Decimal Ft to Ft-In", "Converts decimal foot notation to fractional foot-inch notation.",
              "Wombat", "Document")
        {
        }

        public override Guid ComponentGuid => new Guid("{74d9a407-e181-4e13-a394-e172c19cfef9}");
        protected override Bitmap Icon => Resources.WombatGH_DecimalFtToFtIn;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Values", "V",
                "Decimal foot-inch values to convert to fractional foot-inch notation.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Accuracy", "A", "Right-click to select fractional level of accuracy.",
                GH_ParamAccess.item, 2);
            pManager.AddBooleanParameter("Display Feet", "F", "Set true if input values are in feet, false if they are in inches.",
                GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Drop Zeros", "D", "Set true to drop inch notation where inches equal zero.",
                GH_ParamAccess.item, false);

            var myNamedParam = pManager[1] as Param_Integer;
            myNamedParam.AddNamedValue("10 ft", 0);
            myNamedParam.AddNamedValue("ft", 1);
            myNamedParam.AddNamedValue("in", 2);
            myNamedParam.AddNamedValue("1/2", 3);
            myNamedParam.AddNamedValue("1/4", 4);
            myNamedParam.AddNamedValue("1/8", 5);
            myNamedParam.AddNamedValue("1/16", 6);
            myNamedParam.AddNamedValue("1/32", 7);

            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Formatted Value", "V", "Values formatted in fractional notation.",
                GH_ParamAccess.item);
        }

        internal static double[] unitValues => new double[]
        {
            120,
            12,
            1,
            0.5,
            0.25,
            0.125,
            0.0625,
            1/32.0
        };

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double val = 1;
            int accu = 2;
            bool displayFt = true;
            bool dropZeros = false;

            if (!DA.GetData("Values", ref val)) return;
            if (!DA.GetData("Accuracy", ref accu)) return;

            double decAccu = unitValues[accu];

            bool hasDisplayFt = DA.GetData("Display Feet", ref displayFt);
            bool hasDropZeros = DA.GetData("Drop Zeros", ref dropZeros);

            double unitMultiplier = displayFt ? 12 : 1;

            // Deal with negative numbers
            bool negative = false;
            if (val < 0)
            {
                // If the input value is negative > take the absolute value
                val = Math.Abs(val);
                // And store info that it was a negative number
                negative = true;
            }

            try
            {
                double inchVal = val * unitMultiplier;
                int fracDenom = 1;
                if (decAccu != 0)
                {
                    fracDenom = (int)(1.0 / decAccu);
                    inchVal = inchVal / decAccu;
                    inchVal = Math.Round(inchVal) * decAccu;
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Divide by Zero");
                    return;
                }

                int ft = (int)inchVal / 12;
                int inches = (int)inchVal - ft * 12;

                int fracNum = (int)((inchVal - inches - ft * 12) * fracDenom);

                string formatResult = "";

                if (fracNum == 0)
                {
                    if (inches == 0)
                    {
                        if (dropZeros)
                        {
                            formatResult = $"{ft}'";
                        }
                        else
                        {
                            formatResult = $"{ft}' - {inches}\"";
                        }
                    }
                    else
                    {
                        formatResult = $"{ft}' - {inches}\"";
                    }
                }
                else
                {
                    int GCD = 1;
                    if (decAccu < 1.0)
                    {
                        GCD = GetGCD(fracNum, fracDenom);
                    }
                    if (GCD < 1)
                    {
                        GCD = 1;
                    }
                    formatResult = $"{ft}' - {inches} {fracNum / GCD}/{fracDenom / GCD}\"";
                }

                // Make number negative if it was negative to begin with
                if (negative)
                {
                    formatResult = "-" + formatResult;
                }
                DA.SetData("Formatted Value", formatResult);
            }
            catch (Exception ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, ex.Message);
            }
        }

        static int GetGCD(int num1, int num2)
        {
            while (num1 != num2)
            {
                if (num1 > num2)
                    num1 = num1 - num2;

                if (num2 > num1)
                    num2 = num2 - num1;
            }
            return num1;
        }

        static int GetLCM(int num1, int num2)
        {
            return (num1 * num2) / GetGCD(num1, num2);
        }
    }
}