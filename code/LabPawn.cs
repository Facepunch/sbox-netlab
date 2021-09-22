using Sandbox;
using Sandbox.UI;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab
{

	public struct DataStruct
	{
		public int a;
		public int b;
		public int c;

		public DataStruct( int seed )
		{
			a = seed;
			b = seed + 1;
			c = seed + 2;
		}

		public override string ToString()
		{
			return $"{a}-{b}-{c}";
		}
	}

	public partial class DataClass : NetworkComponent
	{
		[Net] public string DataString { get; set; }
		[Net] public BasePlayerController Controller { get; set; }
	}

	public partial class LabPawn : BasePlayerPawn
	{

		[Net] public string DataString { get; set; }
		[Net, Predicted] public int DataInt { get; set; }
		[Net] public float? DataFloat { get; set; }
		[Net] public Vector3 Vector3 { get; set; }
		[Net] public DataStruct DataStruct { get; set; }
		[Net] public List<int> IntList { get; set; }
		[Net] public List<string> StringList { get; set; }
		[Net] public Entity DataEntity { get; set; }
		[Net] public DataClass DataClass { get; set; }

		[Net] public BasePlayerController PlayerController { get; set; }


		public LabPawn() 
		{

		}

		public LabPawn( Client cl ) : base ( cl )
		{
			
		}

		RealTimeSince timeSinceUpdate;

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			int line = Host.IsServer ? 2 : 30;
			DebugOverlay.ScreenText( line, $"" +
				$"DataString:    {DataString}\n" +
				$"DataInt:       {DataInt}\n" +
				$"DataFloat:     {DataFloat} ({DataFloat.HasValue})\n" +
				$"Vector3:       {Vector3}\n" +
				$"DataStruct:    {DataStruct}\n" +
				$"DataEntity:    {DataEntity}\n" +
				$"IntList:       {string.Join( $", ", IntList?.Select( x => x.ToString()))}\n" +
				$"StringList:    {string.Join( $", ", StringList?.Select( x => x.ToString()))}\n" +
				$"PlController:  {PlayerController}\n" +
				$"DataClass:     {DataClass}\n" +
				$"      DataString:      {DataClass?.DataString}\n" +
				$"      Controller:      {DataClass?.Controller}\n" +
				$"", 0.05f );

			DataInt = Time.Tick;

			if ( Host.IsServer )
			{
				DataString = DateTime.Now.ToLongTimeString();
				DataFloat = DateTime.Now.Second / 60.0f;

				if ( DateTime.Now.Second > 30 )
					DataFloat = null;

				Vector3 = Transform.Position;

				if ( timeSinceUpdate > 0.5f )
				{
					timeSinceUpdate = 0;

					DataEntity = Entity.All.Skip( Rand.Int( 0, Entity.All.Count() - 1 ) ).FirstOrDefault();
					DataStruct = new DataStruct( DateTime.Now.Second );

					IntList.Add( Rand.Int( 0, 100 ) );
					if ( IntList.Count > 10 )
						IntList.RemoveAt( 0 );


					StringList.Add( Guid.NewGuid().ToString().Substring( 0, 6 ) );
					if ( StringList.Count > 10 )
						StringList.RemoveAt( 0 );

					if ( PlayerController is NoclipController )
						PlayerController = new FlyingController();
					else
						PlayerController = new NoclipController();

					if( DataClass  == null )
					{
						DataClass = new DataClass();
						DataClass.DataString = Guid.NewGuid().ToString();
						DataClass.Controller = new NoclipController();
					}
					else
					{
						DataClass = null;
					}
				}

			}

		}
	}

}
