using Sandbox;
using Sandbox.UI;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab
{
	public partial class LabPawn : BasePlayerPawn
	{
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			//
			// Local client should get only one RPC - will supress the server version
			// Other clients will also get one RPC straight from the server.
			//
			if ( Input.Pressed( InputButton.Slot1 ) )
				ParameterlessRpc();

			//
			// Everyone should get one RPC. The local client should see that
			// they never ran this rpc during their tick, so will not supress it.
			//
			if ( Input.Pressed( InputButton.Slot2 ) && IsServer )
				ParameterlessRpc();

			//
			// The local client should call the RPC during prediction then when
			// they get the RPC from the server they'll see that the parameters are
			// different and run the RPC again, assuming that the difference was on 
			// purpose.
			// Other clients will just get the server version
			//
			if ( Input.Pressed( InputButton.Slot3 ) )
				ParameterRpc( Host.Name );

			//
			// Should be the same as slot1
			//
			if ( Input.Pressed( InputButton.Slot4 ) )
				ParameterRpc( "Nice" );
		}

		[ClientRpc]
		public void ParameterlessRpc()
		{
			DebugOverlay.Text( EyePos + Vector3.Random * 20.0f, "RPC CALLED!", 5.0f );
		}

		[ClientRpc]
		public void ParameterRpc( string text )
		{
			DebugOverlay.Text( EyePos + Vector3.Random * 20.0f, $"RPC: {text}", 5.0f );
		}
	}

}
