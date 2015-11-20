using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace SHbit
{
    class BidControl
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private int xPoint;

        public int XPoint
        {
            get { return xPoint; }
            set { xPoint = value; }
        }
        private int yPoint;

        public int YPoint
        {
            get { return yPoint; }
            set { yPoint = value; }
        }

        public BidControl(string name)
        {
            this.name = name;
            string controlposition = ConfigurationManager.AppSettings[name];
            xPoint = int.Parse(controlposition.Split(',')[0].Trim());
            yPoint = int.Parse(controlposition.Split(',')[1].Trim());
        }
    }
}
