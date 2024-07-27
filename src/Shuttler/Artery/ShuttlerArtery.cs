using System;
using System.Collections.Generic;

namespace Shuttler.Artery
{
  public  class ShuttlerArtery
  {
	#region Fields

	private ShuttlerAgentManager _shuttlerManager;
     private IListener _listener;

	#endregion

	#region Methods

	public ShuttlerArtery(string remotEpid)
	{
	   if (_listener != null)
		 throw new InvalidOperationException("Artery constructor faild!");

        if (remotEpid.StartsWith(SocketProtocol.TCP.ToLower()))
        {
           ArgsPoolTuples.InitializeAll();
           _listener = new TcpListener(remotEpid);
        }
        else
           throw new ArgumentException("Support tcp portocol only!");

	   _shuttlerManager = new ShuttlerAgentManager(_listener);
        _shuttlerManager.OnShuttlerCreated += (sender,e)=> 
        {
           ShuttlerCreated.OnEvent<ShuttlerEventArgs>(this,e);
        };
	}

	public void Start()
	{
	   _listener.Listen();
	}

	public void Stop()
	{
        ArgsPoolTuples.DisposeAll();
	   _listener.Stop();
	}

	#endregion

	public ShuttlerAgentManager ShuttlerAgentManager
	{
	   get { return _shuttlerManager; }
	}

	public event EventHandler<ShuttlerEventArgs> ShuttlerCreated;
   }

}
