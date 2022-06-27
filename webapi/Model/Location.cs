using System;
using System.Collections.Generic;

namespace webapi.Model
{
    public class Location
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Dimension { get; set; }

        public IEnumerable<Uri> Residents { get; set; }

        public Uri Url { get; set; }

        public DateTime? Created { get; set; }
    }
}
