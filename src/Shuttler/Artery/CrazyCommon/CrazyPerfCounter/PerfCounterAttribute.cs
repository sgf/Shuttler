using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Shuttler.Artery
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PerfCounterAttribute : Attribute
    {
       #region Attributes

       public string Help
        { get; private set; }

        public string Name
        { get; private set; }

        public PerformanceCounterType Type
        { get; private set; }

       #endregion

       public PerfCounterAttribute(string name, string help, PerformanceCounterType type)
        {
            Name = name;
            Help = help;
            Type = type;
        }
    }
}
