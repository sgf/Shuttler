/*Note: T struct only*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Shuttler.Artery
{
   public class CompactHeap<T> : IDisposable where T :new()
   {
      #region Fields

      private IntPtr _ptr, _currPtr;
      private int _rawSize;
      private Type _type;
      private T _t;

      #endregion

      public CompactHeap(int num)
      {
         _t = new T();
         _type = _t.GetType();

         _rawSize = Marshal.SizeOf(_t);
         _ptr = Marshal.AllocHGlobal(_rawSize * num);
         _currPtr = _ptr;
      }

      public void Add(T t)
      {
         Marshal.StructureToPtr(t, _ptr, false);
         _ptr = new IntPtr((_ptr.ToInt32() + _rawSize));
      }

      public T Get(int index)
      {
         IntPtr p = new IntPtr((_currPtr.ToInt32() + _rawSize * index));
         return (T)Marshal.PtrToStructure(p, _type);
      }

      public void Dispose()
      {
         Marshal.FreeHGlobal(_currPtr);
      }
   }
}
