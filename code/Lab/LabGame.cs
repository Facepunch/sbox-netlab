
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Lab
{
	[Library( "lab" )]
	public partial class Game : Sandbox.Game
	{
		public override void Spawn()
		{
			base.Spawn();

			new HudEntity();
		}

		[ConVar.ClientData( "spectator_only" )]
		public bool SpectatorOnly { get; set; } = false;

		/// <summary>
		/// Client joined, create them a LabPawn and spawn them
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			client.Pawn = new LabPawn( client );

			MoveToSpawnpoint( client.Pawn );
		}
	}

}
