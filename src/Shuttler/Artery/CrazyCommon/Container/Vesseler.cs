using System;
using System.Collections.Generic;
using System.Text;

namespace Shuttler.Artery
{
   internal class Vesseler<K, V>
   {
	 public K Key;
	 public V Value;

	 public Vesseler(K key, V value)
	 {
	    Key = key;
	    Value = value;
	 }

   }
}
