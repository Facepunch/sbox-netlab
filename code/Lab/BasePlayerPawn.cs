using Sandbox;
using Sandbox.UI;
using System.Linq;

namespace Lab
{
	public partial class BasePlayerPawn : Sandbox.AnimEntity
	{
		/// <summary>
		/// The clothing container is what dresses the citizen
		/// </summary>
		public Clothing.Container Clothing = new();

		public BasePlayerPawn( Client cl )
		{
			Clothing.LoadFromClient( cl );
		}

		public BasePlayerPawn()
		{
		}

		public override void Spawn()
		{
			base.Spawn();

			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
			Tags.Add( "player", "pawn" );
			SetModel( "models/citizen/citizen.vmdl" );

			Camera = new FirstPersonCamera();

			Clothing.DressEntity( this );

			EnableHitboxes = true;
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( cl.DevCamera != null )
				return;

			EyeRot = Input.Rotation;
			EyePosLocal = Vector3.Up * 64.0f;

			DoMovement( cl );
		}

		public virtual void DoMovement( Client cl )
		{
			if ( Input.Pressed( InputButton.View) )
			{
				Camera = (Camera is FirstPersonCamera) ? new ThirdPersonCamera() : new FirstPersonCamera();
			}

			var maxSpeed = 150;
			if ( Input.Down( InputButton.Run ) ) maxSpeed = 200;
			if ( Input.Down( InputButton.Duck ) ) maxSpeed = 70;

			var moveDir = (Input.Rotation * new Vector3( Input.Forward, Input.Left, Input.Up )).WithZ( 0 ).Normal;
			var wishDir = moveDir;

			foreach( var ent in Entity.All.OfType<BasePlayerPawn>() )
			{
				if ( ent == this ) continue;

				var delta = Position - ent.Position;
				if ( delta.Length > 50.0f ) continue;

				moveDir += delta.Normal.WithZ( 0 ) * delta.Length.LerpInverse( 50.0f, 10.0f ) * 3.0f;
			}

			if ( moveDir.Length > 0.01f )
			{
				Velocity = Velocity.AddClamped( moveDir * maxSpeed * 5 * Time.Delta, maxSpeed );
			}

			if ( wishDir.Length > 0.1f )
			{
				var targetRotation = Rotation.LookAt( wishDir.WithZ( 0 ) );
				Rotation = Rotation.Slerp( Rotation, targetRotation, Time.Delta * 10.0f );
			}


			if ( GroundEntity == null )
			{
				Velocity += Vector3.Down * 900.0f * Time.Delta;
			}
			else
			{
				Velocity = Velocity.Approach( 0, Time.Delta * maxSpeed * 3 );

				if ( Input.Pressed( InputButton.Jump ) )
				{
					Velocity = Velocity.WithZ( 300.0f );
					SetAnimBool( "b_jump", true );
				}
			}

			//
			// Move helper traces and slides along surfaces for us
			//
			MoveHelper helper = new MoveHelper( Position, Velocity );
			helper.Trace = helper.Trace.Size( BBox.FromHeightAndRadius( 72.0f, 10.0f ) );

			helper.TryUnstuck();
			helper.TryMoveWithStep( Time.Delta, 30.0f );
			Position = helper.Position;
			Velocity = helper.Velocity;

			if ( Velocity.z <= 50.0f )
			{
				var tr = helper.TraceDirection( Vector3.Down * 6.0f );
				GroundEntity = tr.Entity;
				if ( GroundEntity != null )
				{
					Position += tr.Distance * Vector3.Down;

					if ( Velocity.z < 0.0f )
						Velocity = Velocity.WithZ( 0 );
				}
			}
			else
			{
				GroundEntity = null;
			}

			SetAnimBool( "b_grounded", GroundEntity != null );

			//
			// Update animation
			//
			CitizenAnimationHelper animHelper = new CitizenAnimationHelper( this );
			animHelper.WithVelocity( Velocity );
			animHelper.WithWishVelocity( wishDir * maxSpeed );
			animHelper.WithLookAt( EyePos + Input.Rotation.Forward * 100.0f );
		}

		public override void FrameSimulate( Client cl )
		{
			base.FrameSimulate( cl );

			if ( cl.DevCamera != null )
				return;

			EyeRot = Input.Rotation;
			Position += Velocity * Time.Delta;
		}
	}

}
