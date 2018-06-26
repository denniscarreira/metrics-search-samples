using System;
using System.Collections.Generic;
using System.Text;

namespace Metrics.Domain
{
    public class ProductFamiliy
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
    }
}
