using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS
{
    public class Item
    {
        public int OrderNumber { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string ItemName { get; set; }
        public int ItemTotal { get; set; }
    }
}
