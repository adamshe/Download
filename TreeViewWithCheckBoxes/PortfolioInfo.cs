using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeViewWithCheckBoxes
{
    public class PortfolioInfo : IPortfolioInfo
    {
        public int PortfolioId { get; set; }

        public string Name { get; set; }

        public double Aum { get; set; }
    }
}
