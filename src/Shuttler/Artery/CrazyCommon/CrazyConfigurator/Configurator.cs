using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace Shuttler.Artery
{
   public class Configurator<T> where T : ConfigurationSection
   {
      #region Fields

      private const string Suffix = ".config";
      private T _instance;

      #endregion

      #region Methods

      public Configurator(string name)
      {
         Name = name;
      }

      public T Load()
      {
         if (_instance == null)
         {
            string exeConfig = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            if (exeConfig.EndsWith(Suffix, StringComparison.InvariantCultureIgnoreCase))
               exeConfig = exeConfig.Remove(exeConfig.Length - Suffix.Length);

            Configuration config = ConfigurationManager.OpenExeConfiguration(exeConfig);
            if (config.Sections[Name] == null)
               throw new ConfigurationErrorsException("section error!");
            else
               _instance = (T)config.Sections[Name];
         }
         return _instance;
      }

      #endregion

      private string Name
      { get; set; }
   }
}
