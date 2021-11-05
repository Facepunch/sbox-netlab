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

	public partial class ComponentRed : EntityComponent
	{
		[Net] public int DataInt { get; set; }
		[Net] public string DataString { get; set; }
		[Net] public DataClass DataClass { get; set; }
	}	
	
	public partial class ComponentGreen : EntityComponent
	{

	}	

	public partial class ComponentBlue : EntityComponent
	{

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
		[Net] public IList<string> StringList { get; set; }
		[Net] public IDictionary<int, int> IntIntDict { get; set; }
		[Net] public IDictionary<int, string> IntStringDict { get; set; }
		[Net] public IDictionary<string, int> StringIntDict { get; set; }
		[Net] public IDictionary<string, Entity> StringEntityDict { get; set; }
		[Net] public Entity DataEntity { get; set; }
		[Net] public LabPawn Pawn { get; set; }
		[Net] public DataClass DataClass { get; set; }

		[Net] public BasePlayerController PlayerController { get; set; }


		public LabPawn() 
		{

		}

		public LabPawn( Client cl ) : base ( cl )
		{
			Components.GetOrCreate<ComponentRed>();
			Components.GetOrCreate<ComponentGreen>();
		}

		RealTimeSince timeSinceUpdate;

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			var componentText = "";

			foreach( var component in Components.GetAll<EntityComponent>() )
			{
				if ( componentText != "" ) componentText += ", ";
				
				componentText += $"{component}";
			}

			var position = Host.IsServer ? new Vector2( 100, 100 ) : new Vector2( 100, 400 );
			DebugOverlay.ScreenText( position, 0, Color.White, $"" +
				$"DataString:    {DataString}\n" +
				$"DataInt:       {DataInt}\n" +
				$"DataFloat:     {DataFloat} ({DataFloat.HasValue})\n" +
				$"Vector3:       {Vector3}\n" +
				$"DataStruct:    {DataStruct}\n" +
				$"DataEntity:    {DataEntity}\n" +
				$"IntList:       {string.Join( $", ", IntList?.Select( x => x.ToString()))}\n" +
				$"StringList:    {string.Join( $", ", StringList?.Select( x => x.ToString()))}\n" +
				$"IntIntDict:    {string.Join( $", ", IntIntDict?.Select( x => x.ToString()))}\n" +
				$"IntStringDict: {string.Join( $", ", IntStringDict?.Select( x => x.ToString()))}\n" +
				$"StringIntDict: {string.Join( $", ", StringIntDict?.Select( x => x.ToString()))}\n" +
				$"StringEntityDict: {string.Join( $", ", StringEntityDict?.Select( x => x.ToString()))}\n" +
				$"PlController:  {PlayerController}\n" +
				$"DataClass:     {DataClass}\n" +
				$"      DataString:      {DataClass?.DataString}\n" +
				$"      Controller:      {DataClass?.Controller}\n" +
				$"Components:      {componentText}\n" +
				$"ComponentRed:      {Components.Get<ComponentRed>()}\n" +
				$"      DataInt:      {Components.Get<ComponentRed>().DataInt}\n" +
			//	$"      DataString:     {Components.Get<ComponentRed>().DataString}\n" +
				$"      DataClass:		 {Components.Get<ComponentRed>().DataClass}\n" +
				$"", 0.05f );

			DataInt = Time.Tick;

			if ( Host.IsServer )
			{
				DataString = DateTime.Now.ToLongTimeString();
				DataFloat = DateTime.Now.Second / 60.0f;

				if ( DateTime.Now.Second % 2 == 1 )
					DataFloat = null;

				Vector3 = Transform.Position;
				DataEntity = Entity.All.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

				if ( timeSinceUpdate > 0.5f )
				{
					timeSinceUpdate = 0;

					Components.Get<ComponentRed>().DataInt = Rand.Int( 10000, 99999 );

					DataStruct = new DataStruct( DateTime.Now.Second );

					IntList.Add( Rand.Int( 0, 100 ) );
					if ( IntList.Count > 10 )
						IntList.RemoveAt( 0 );


					StringList.Add( Guid.NewGuid().ToString().Substring( 0, 6 ) );
					if ( StringList.Count > 10 )
						StringList.RemoveAt( 0 );

					IntIntDict.Clear();
					for ( int i = 0; i < 5; i++ )
						IntIntDict[Rand.Int(0, 100)] = Rand.Int( 0, 100 );

					IntStringDict.Clear();
					for ( int i = 0; i < 5; i++ )
						IntStringDict[Rand.Int( 0, 100 )] = Guid.NewGuid().ToString().Substring( 0, 6 );

					StringIntDict.Clear();
					for ( int i = 0; i < 5; i++ )
						StringIntDict[Guid.NewGuid().ToString().Substring( 0, 6 )] = Rand.Int( 0, 100 );

					StringEntityDict.Clear();
					for ( int i = 0; i < 5; i++ )
						StringEntityDict[Guid.NewGuid().ToString().Substring( 0, 6 )] = Entity.All.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

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

					var c = Components.Get<ComponentBlue>();
					if ( c == null )
					{
						Components.Create<ComponentBlue>();
					}
					else
					{
						Components.Remove( c );
					}
				}

			}

		}
	}

}
