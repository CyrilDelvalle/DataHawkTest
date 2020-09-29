using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataHawkTest.Model
{
    public class CustomerReview
    {
        public string AmazonASIN { get; set; }
        public string Id { get;  set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public int Rate { get; set; }
        public string Date { get; set; }
        public string Body { get; set; }
    }
}
