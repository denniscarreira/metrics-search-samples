using System;
using System.Collections.Generic;
using System.Text;

namespace Metrics.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public int ProductFamilyId { get; set; }

        public string[] ProductFamilies { get; set; }
    }
}
