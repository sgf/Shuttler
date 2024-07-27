using System;
using System.Collections.Generic;
using System.Text;

namespace Shuttler.Artery
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PerfCounterCategoryAttribute : Attribute
    {
       public string Name
       { get; private set; }

        public PerfCounterCategoryAttribute(string name)
        {
            Name = name;
        }
    }
}
