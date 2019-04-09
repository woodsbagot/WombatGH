using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class UnitWarning : GH_Component
    {

        public UnitWarning()
          : base("Unit Warning", "Unit Warning", "Provides a warning when the Rhino document unit system doesn't match the user-specified unit system.",
              "Wombat", "Document")
        {
        }

        public override Guid ComponentGuid => new Guid("{b100a888-1582-4345-9ce5-02f15347e527}");
        protected override Bitmap Icon => Resources.WombatGH_UnitWarning;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Unit System", "U", "Right-click to select unit system against which to check.", GH_ParamAccess.item);

            var myNamedParam = pManager[0] as Param_Integer;
            //myNamedParam.AddNamedValue("None", 0);
            //myNamedParam.AddNamedValue("Microns", 1);
            myNamedParam.AddNamedValue("Millimeters", 2);
            myNamedParam.AddNamedValue("Centimeters", 3);
            myNamedParam.AddNamedValue("Meters", 4);
            myNamedParam.AddNamedValue("Kilometers", 5);
            //myNamedParam.AddNamedValue("Microinches", 6);
            //myNamedParam.AddNamedValue("Mils", 7);
            myNamedParam.AddNamedValue("Inches", 8);
            myNamedParam.AddNamedValue("Feet", 9);
            myNamedParam.AddNamedValue("Miles", 10);
            //myNamedParam.AddNamedValue("CustomUnitSystem", 11);
            //myNamedParam.AddNamedValue("Angstroms", 12);
            //myNamedParam.AddNamedValue("Nanometers", 13);
            //myNamedParam.AddNamedValue("Decimeters", 14);
            //myNamedParam.AddNamedValue("Dekameters", 15);
            //myNamedParam.AddNamedValue("Hectometers", 16);
            //myNamedParam.AddNamedValue("Megameters", 17);
            //myNamedParam.AddNamedValue("Gigameters", 18);
            myNamedParam.AddNamedValue("Yards", 19);
            //myNamedParam.AddNamedValue("PrinterPoint", 20);
            //myNamedParam.AddNamedValue("PrinterPica", 21);
            //myNamedParam.AddNamedValue("NauticalMile", 22);
            //myNamedParam.AddNamedValue("Astronomical", 23);
            //myNamedParam.AddNamedValue("Lightyears", 24);
            //myNamedParam.AddNamedValue("Parsecs", 25);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            UnitSystem docUnits = RhinoDoc.ActiveDoc.ModelUnitSystem;
            UnitSystem userUnits = RhinoDoc.ActiveDoc.ModelUnitSystem;
            int userUnitsInt = -1;
            if (!DA.GetData("Unit System", ref userUnitsInt)) return;
            userUnits = (UnitSystem)userUnitsInt; 

            if (docUnits != userUnits)
            {
                string msg = $"Warning! Your model is not in {userUnits}. Would you like to change your document to {userUnits}?";
                var result = MessageBox.Show(msg, "Unit Warning", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    RhinoDoc.ActiveDoc.AdjustModelUnitSystem(userUnits, true);
                }
            }
        }
    }
}