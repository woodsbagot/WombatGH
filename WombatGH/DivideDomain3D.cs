using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class DivideDomain3D : GH_Component
    {

        public DivideDomain3D()
          : base("Divide Domain³", "Divide 3D", "Divides a three-dimensional or 'box' domain into equal segments.",
              "Wombat", "Sets")
        {
        }

        public override Guid ComponentGuid => new Guid("{6d2f6167-d423-411c-8054-2aa7c8ef6381}");
        protected override Bitmap Icon => Resources.WombatGH_DivDom3D;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntervalParameter("U Domain", "Iu", "Base {u} domain", GH_ParamAccess.item, new Interval(0, 1));
            pManager.AddIntervalParameter("V Domain", "Iv", "Base {v} domain", GH_ParamAccess.item, new Interval(0, 1));
            pManager.AddIntervalParameter("W Domain", "Iw", "Base {w} domain", GH_ParamAccess.item, new Interval(0, 1));
            pManager.AddIntegerParameter("U Count", "U", "Number of segments in {u} direction.", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("V Count", "V", "Number of segments in {v} direction.", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("W Count", "W", "Number of segments in {w} direction.", GH_ParamAccess.item, 10);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntervalParameter("U Segments as list", "Su", "Individual {u} segments", GH_ParamAccess.list);
            pManager.AddIntervalParameter("V Segments as list", "Sv", "Individual {v} segments", GH_ParamAccess.list);
            pManager.AddIntervalParameter("W Segments as list", "Sw", "Individual {w} segments", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var intU = new Interval(0, 1);
            var intV = new Interval(0, 1);
            var intW = new Interval(0, 1);
            int cntU = 10;
            int cntV = 10;
            int cntW = 10;
            DA.GetData("U Domain", ref intU);
            DA.GetData("V Domain", ref intV);
            DA.GetData("W Domain", ref intW);
            DA.GetData("U Count", ref cntU);
            DA.GetData("V Count", ref cntV);
            DA.GetData("W Count", ref cntW);

            List<Interval> domListU = new List<Interval>();
            List<Interval> domListV = new List<Interval>();
            List<Interval> domListW = new List<Interval>();


            for (int i = 0; i < cntU; i++)
            {
                Interval subIntU = GetSubInterval(intU, cntU, i);
                domListU.Add(new Interval(subIntU));
            }

            for (int i = 0; i < cntV; i++)
            {
                Interval subIntV = GetSubInterval(intV, cntV, i);
                domListV.Add(new Interval(subIntV));
            }

            for (int i = 0; i < cntW; i++)
            {
                Interval subIntW = GetSubInterval(intW, cntW, i);
                domListW.Add(new Interval(subIntW));
            }



            DA.SetDataList("U Segments as list", domListU);
            DA.SetDataList("V Segments as list", domListV);
            DA.SetDataList("W Segments as list", domListW);
        }

        private static Interval GetSubInterval(Interval domain, int count, int current)
        {
            double tStart = (double)current / (count);
            double tEnd = (double)(current+1) / (count);
            double domStart = domain.ParameterAt(tStart);
            double domEnd = domain.ParameterAt(tEnd);
            Interval subDom = new Interval(domStart, domEnd);
            return subDom;
        }
    }
}