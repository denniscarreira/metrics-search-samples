using System;
using System.Collections.Generic;
using System.Text;

namespace Metrics.Domain
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int RetailerId { get; set; }
        public Guid SellerId { get; set; }
        public string ProductSku { get; set; }
        public string ProductName { get; set; }
        public string[] ProductFamilies { get; set; }
        public string SerialNumber { get; set; }
        public bool IsReturned { get; set; }
        public int LastOperationType { get; set; }
        public string Distribution { get; set; }
    }
}
