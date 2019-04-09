using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class CanvasColor : GH_Component
    {

        public CanvasColor()
          : base("Canvas Color", "Canvas Color", "Alters canvas appearance properties.", "Wombat", "Document")
        {
        }

        public override Guid ComponentGuid => new Guid("{ad1ee62a-1ba4-4af3-8f43-0c8c3a69618d}");
        protected override Bitmap Icon => Resources.WombatGH_CanvasColor;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Toggle", "T", "Toggles between user-defined and default canvas settings.", GH_ParamAccess.item);
            pManager.AddColourParameter("Background Color", "B", "Set canvas background color.", GH_ParamAccess.item);
            pManager.AddColourParameter("Grid Color", "G", "Set canvas grid color.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Grid Cell Width", "W", "Set grid cell width.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Grid Cell Height", "H", "Set grid cell height.", GH_ParamAccess.item);
            for (int i = 1; i < 5; i++)
            {
                pManager[i].Optional = true;
            }
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool toggle = false;
            Color bgColor = Color.FromArgb(30, 0, 0, 0);
            Color gColor = Color.FromArgb(255, 212, 208, 200);
            int width = 100;
            int height = 100;

            if (!DA.GetData("Toggle", ref toggle)) return;

            bool hasBgColor = DA.GetData("Background Color", ref bgColor);
            bool hasGColor = DA.GetData("Grid Color", ref gColor);
            bool hasWidth = DA.GetData("Grid Cell Width", ref width);
            bool hasHeight = DA.GetData("Grid Cell Height", ref height);

            limitToRange(ref width);
            limitToRange(ref height);

            if (toggle)
            {
                if (hasBgColor) GH_Skin.canvas_back = bgColor;
                if (hasGColor) GH_Skin.canvas_grid = gColor;
                GH_Skin.canvas_edge = Color.FromArgb(255, 0, 0, 0);
                GH_Skin.canvas_shade = Color.FromArgb(80, 0, 0, 0);
                if (hasWidth) GH_Skin.canvas_grid_col = width;
                if (hasHeight) GH_Skin.canvas_grid_row = height;
            }
            else
            {
                GH_Skin.canvas_grid = Color.FromArgb(30, 0, 0, 0);
                GH_Skin.canvas_back = Color.FromArgb(255, 212, 208, 200);
                GH_Skin.canvas_edge = Color.FromArgb(255, 0, 0, 0);
                GH_Skin.canvas_shade = Color.FromArgb(80, 0, 0, 0);
                GH_Skin.canvas_grid_col = 150;
                GH_Skin.canvas_grid_row = 50;
            }
        }

        private void limitToRange(ref int val)
        {
            if (val == -1) TetrisSong();

            if (val > 10000 || val < 10)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, 
                    "Value is outside of acceptable range and has been automatically limited!");
            }

            val = Math.Min(val, 10000);
            val = Math.Max(val, 10);
        }

        private void TetrisSong()
        {
            //play tetris song using console beep
            //source https://gist.github.com/XeeX/6220067
            Console.Beep(658, 125);
            Console.Beep(1320, 500);
            Console.Beep(990, 250);
            Console.Beep(1056, 250);
            Console.Beep(1188, 250);
            Console.Beep(1320, 125);
            Console.Beep(1188, 125);
            Console.Beep(1056, 250);
            Console.Beep(990, 250);
            Console.Beep(880, 500);
            Console.Beep(880, 250);
            Console.Beep(1056, 250);
            Console.Beep(1320, 500);
            Console.Beep(1188, 250);
            Console.Beep(1056, 250);
            Console.Beep(990, 750);
            Console.Beep(1056, 250);
            Console.Beep(1188, 500);
            Console.Beep(1320, 500);
            Console.Beep(1056, 500);
            Console.Beep(880, 500);
            Console.Beep(880, 500);
            System.Threading.Thread.Sleep(250);
            Console.Beep(1188, 500);
            Console.Beep(1408, 250);
            Console.Beep(1760, 500);
            Console.Beep(1584, 250);
            Console.Beep(1408, 250);
            Console.Beep(1320, 750);
            Console.Beep(1056, 250);
            Console.Beep(1320, 500);
            Console.Beep(1188, 250);
            Console.Beep(1056, 250);
            Console.Beep(990, 500);
            Console.Beep(990, 250);
            Console.Beep(1056, 250);
            Console.Beep(1188, 500);
            Console.Beep(1320, 500);
            Console.Beep(1056, 500);
            Console.Beep(880, 500);
            Console.Beep(880, 500);
            System.Threading.Thread.Sleep(500);
            Console.Beep(1320, 500);
            Console.Beep(990, 250);
            Console.Beep(1056, 250);
            Console.Beep(1188, 250);
            Console.Beep(1320, 125);
            Console.Beep(1188, 125);
            Console.Beep(1056, 250);
            Console.Beep(990, 250);
            Console.Beep(880, 500);
            Console.Beep(880, 250);
            Console.Beep(1056, 250);
            Console.Beep(1320, 500);
            Console.Beep(1188, 250);
            Console.Beep(1056, 250);
            Console.Beep(990, 750);
            Console.Beep(1056, 250);
            Console.Beep(1188, 500);
            Console.Beep(1320, 500);
            Console.Beep(1056, 500);
            Console.Beep(880, 500);
            Console.Beep(880, 500);
            System.Threading.Thread.Sleep(250);
            Console.Beep(1188, 500);
            Console.Beep(1408, 250);
            Console.Beep(1760, 500);
            Console.Beep(1584, 250);
            Console.Beep(1408, 250);
            Console.Beep(1320, 750);
            Console.Beep(1056, 250);
            Console.Beep(1320, 500);
            Console.Beep(1188, 250);
            Console.Beep(1056, 250);
            Console.Beep(990, 500);
            Console.Beep(990, 250);
            Console.Beep(1056, 250);
            Console.Beep(1188, 500);
            Console.Beep(1320, 500);
            Console.Beep(1056, 500);
            Console.Beep(880, 500);
            Console.Beep(880, 500);
            System.Threading.Thread.Sleep(500);
            Console.Beep(660, 1000);
            Console.Beep(528, 1000);
            Console.Beep(594, 1000);
            Console.Beep(495, 1000);
            Console.Beep(528, 1000);
            Console.Beep(440, 1000);
            Console.Beep(419, 1000);
            Console.Beep(495, 1000);
            Console.Beep(660, 1000);
            Console.Beep(528, 1000);
            Console.Beep(594, 1000);
            Console.Beep(495, 1000);
            Console.Beep(528, 500);
            Console.Beep(660, 500);
            Console.Beep(880, 1000);
            Console.Beep(838, 2000);
            Console.Beep(660, 1000);
            Console.Beep(528, 1000);
            Console.Beep(594, 1000);
            Console.Beep(495, 1000);
            Console.Beep(528, 1000);
            Console.Beep(440, 1000);
            Console.Beep(419, 1000);
            Console.Beep(495, 1000);
            Console.Beep(660, 1000);
            Console.Beep(528, 1000);
            Console.Beep(594, 1000);
            Console.Beep(495, 1000);
            Console.Beep(528, 500);
            Console.Beep(660, 500);
            Console.Beep(880, 1000);
            Console.Beep(838, 2000);
        }
    }
}