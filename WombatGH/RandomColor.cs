using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Utility;
using Rhino.Geometry;
using VectorComponents.PointComponents;
using WombatGH.Properties;

namespace WombatGH
{
    public class RandomColor : GH_Component
    {

        public RandomColor() : base("Random Color", "RandCol", "Generates a random color for every item in a list", "Wombat", "Display")
        {
            UpdateMessage();
            FastMode = false;
        }

        private bool FastMode { get; set; }

        public override Guid ComponentGuid => new Guid("{9B332F01-D23E-4444-B343-A885986CA3A8}");
        protected override Bitmap Icon => Resources.WombatGH_RandomColor;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D",
                "The data list to assign colors for. One color per item will be generated", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Seed", "S", "The random seed", GH_ParamAccess.item, 2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddColourParameter("Colors", "C", "A unique color for each item supplied", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var refList = new List<object>();
            var seed = 2;
            if (!DA.GetDataList("Data", refList)) return;
            if (!DA.GetData("Seed", ref seed)) return;

            DA.SetDataList("Colors", GenerateRandomColors(FastMode, refList.Count, seed));
        }

        private static List<Color> GenerateRandomColors(bool fastMode, int count, int seed)
        {
            return fastMode ? GenerateRandomColorsFast(count, seed) : GenerateRandomColorsBetter(count, seed);
        }

        public static List<Color> GenerateRandomColorsFast(int count, int seed)
        {
            List<Color> colors = new List<Color>();
            var random = new Random(seed);
            for (var i = 0; i < count; i++)
            {
                colors.Add(Color.FromArgb(255,random.Next(255),random.Next(255), random.Next(255)));
            }

            return colors;
        }

        public static List<Color> GenerateRandomColorsBetter(int count, int seed)
        {
            List<Color> colors = new List<Color>();
            var cbs = new ColorBoxSolver(seed);

            var pointList = cbs.Populate(count, null);

            return pointList.Select(p => Color.FromArgb(255,(int)p.X, (int)p.Y, (int)p.Z)).ToList();
        }

        //message updater
        private void UpdateMessage()
        {
            Message = FastMode ? "Fast" : "Better";
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("FastMode", FastMode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            FastMode = false;
            var fastmode = false;
            reader.TryGetBoolean("SomeProperty", ref fastmode);
            FastMode = fastmode;
            UpdateMessage();
            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Fast Mode", fastModeClicked, null, true, FastMode);
        
        }

        public void fastModeClicked(object sender, EventArgs e)
        {
            FastMode = !FastMode;
            UpdateMessage();
            ExpireSolution(true);
        }
    }

    public class ColorBoxSolver : PopulationSolver
    {
        private Box _box;
        public ColorBoxSolver(int seed) : base(seed)
        {
            var dim = new Interval(0,256);
            _box = new Box(Plane.WorldXY,dim,dim,dim);
        }

        protected override Point3d NextPoint()
        {
            return new Point3d(RandomInteger(256), RandomInteger(256), RandomInteger(256));
        }

        protected override BoundingBox BoundingBox()
        {
            return _box.BoundingBox;
        }
    }
}
