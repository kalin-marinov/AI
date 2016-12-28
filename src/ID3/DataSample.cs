using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ID3
{
    public class DataSample
    {
        public bool IsPositive { get; set; }

        public string[] Attributes { get; set; }

        public DataSample(string line)
        {
            Attributes = line.Split(',');
            IsPositive = Attributes.Last() == "'recurrence-events'";
        }

    }
}
