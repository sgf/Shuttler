using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shuttler.Artery
{
   sealed class ArterySettings : ConfigurationSection
   {
     private const string NAME = "ArterySettings";

     [ConfigurationProperty("ArgsCapacity", DefaultValue = 1000)]
     public  int ArgsCapacity
     {
        get { return (int)this["ArgsCapacity"]; }
     }

     [ConfigurationProperty("LogLevel", DefaultValue = "Info")]
     private string LogLevel
     {
        get { return (string)this["LogLevel"]; }
     }

     [ConfigurationProperty("LocalPort", DefaultValue = "8888")]
     public string LocalPort
     {
        get { return (string)this["LocalPort"]; }
     }


     public  Level Level
     {
        get
        {
           switch (LogLevel)
           {
              case "Info": return Level.Info;
              case "Warn": return Level.Warn;
              case "Error": return Level.Error;
              default:
                 return Level.NO;
           }
        }
     }


     public static ArterySettings Instance
     {
        get
        {
           Configurator<ArterySettings> config = new Configurator<ArterySettings>(NAME);
           return config.Load();
        }
     }

   }
}
