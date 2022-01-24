using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LWManager
{
    public class MWViewContract
    {
        public int OrderId { get; set; }
        public int RowNumber { get; set; }
        public string FISH { get; set; }
        public string CreationDateTime { get; set; }
        public string UsedDays { get; set; }
        public string LLease { get; set; }
        public string BLease { get; set; }
        public string Wheel { get; set; }
        public string Phone { get; set; }
        public int DeliveryPrice { get; set; }
        public int PaidAmount { get; set; }
        public int Sum { get; set; }
        public string IsDebtor { get; set; }
        public string DeliveryAddress { get; set; }
        /// <summary>
        /// Order Status 0 = new order, 1 = returned order, 2 = closed order
        /// </summary>
        public int OrderStatus { get; set; }
    }
}
