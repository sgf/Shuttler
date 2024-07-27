using System;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   [Flags]
  public enum Command : ushort
   {
      ///// <summary>
      /////  logout
      ///// </summary>
      //Logout = 0x0001,
      ///// <summary>
      ///// keeplive
      ///// </summary>
      //Keep_Alive = 0x0002,
      ///// <summary>
      ///// modify self
      ///// </summary>
      //Modify_Info = 0x0004,
      ///// <summary>
      ///// search
      ///// </summary>
      //Search_User = 0x0005,
      ///// <summary>
      ///// get buddyinfo 
      ///// </summary>
      //Get_UserInfo = 0x0006,

      ///// <summary>
      ///// delete 
      ///// </summary>
      //Delete_Friend = 0x000A,
	
      ///// <summary>
      ///// status
      ///// </summary>
      //Change_Status = 0x000D,
      ///// <summary>
      ///// IM
      ///// </summary>
      //IM = 0x0016,

	 /// <summary>
	 /// login
	 /// </summary>
	 Login = 0x0001,
	 /// <summary>
	 /// get buddies
	 /// </summary>
	 Get_Buddy_List = 0x0002,
	 
	 /// <summary>
	 /// Unknown 
	 /// </summary>
	 Unknown = 0xFFFF,
   }
}
