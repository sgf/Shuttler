using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace Shuttler.Artery
{
   public static class PerfCounterFactory
   {
      #region Fields

      private static Dictionary<Type, object> _dict = new Dictionary<Type, object>(256);
      private static object _syncRoot = new object();

      #endregion

      #region Methods

      private static void Register<T>() where T : class, new()
      {
         try
         {
            object[] attribs = typeof(T).GetCustomAttributes(typeof(PerfCounterCategoryAttribute), false);
            if (attribs.Length == 0)
               throw new InvalidOperationException();

            PerfCounterCategoryAttribute attr = (PerfCounterCategoryAttribute)attribs[0];
            string category = attr.Name;

            bool countersExist = PerformanceCounterCategory.Exists(category);
            if (countersExist)
               PerformanceCounterCategory.Delete(category);

            CounterCreationDataCollection list = new CounterCreationDataCollection();
            foreach (FieldInfo prop in typeof(T).GetFields())
            {
               attribs = prop.GetCustomAttributes(typeof(PerfCounterAttribute), false);
               foreach (PerfCounterAttribute fieldAttrib in attribs)
               {
                  CounterCreationData data = new CounterCreationData();
                  data.CounterName = fieldAttrib.Name;
                  data.CounterHelp = fieldAttrib.Help;
                  data.CounterType = fieldAttrib.Type;

                  list.Add(data);
               }
            }

            PerformanceCounterCategory.Create(category, null, PerformanceCounterCategoryType.SingleInstance, list);

            T instance = new T();
            foreach (FieldInfo prop in typeof(T).GetFields())
            {
               attribs = prop.GetCustomAttributes(typeof(PerfCounterAttribute), false);
               foreach (PerfCounterAttribute fieldAttrib in attribs)
               {
                  try
                  {
                     PerformanceCounter pc = new PerformanceCounter(category, fieldAttrib.Name, false);
                     pc.RawValue = 0;
                     prop.SetValue(instance, pc);
                  }
                  catch (Exception ex)
                  { }
               }
            }

            lock (_syncRoot)
               _dict.Add(typeof(T), instance);
         }
         catch (Exception ex)
         { 
            
         }
            
      }

      public static T Create<T>() where T : class, new()
      {
         Register<T>();
         return Resolve<T>();
      }

      private static T Resolve<T>() where T : class
      {
         T t = _dict[typeof(T)] as T;
         if (t != null)
            return t;
         return default(T);
      }

      #endregion
   }

}
