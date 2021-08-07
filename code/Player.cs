using Sandbox;
using System;
using System.Linq;

namespace Stronghold
{
	partial class Player : Sandbox.Player
	{
		private ModelEntity ent;
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			//
			// Use WalkController for movement (you can make your own PlayerController for 100% control)
			//
			Controller = new WalkController();

			//
			// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
			//
			Animator = new StandardPlayerAnimator();

			//
			// Use ThirdPersonCamera (you can make your own Camera for 100% control)
			//
			Camera = new ThirdPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			base.Respawn();
		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			//
			// If you have active children (like a weapon etc) you should call this to 
			// simulate those too.
			//
			SimulateActiveChild( cl, ActiveChild );

			//
			// If we're running serverside and Attack1 was just pressed, spawn a ragdoll
			//
			if ( IsServer && Input.Pressed( InputButton.Attack1 ) )
			{
				var ragdoll = new ModelEntity();
				ragdoll.SetModel( "models/citizen/citizen.vmdl" );  
				ragdoll.Position = EyePos + EyeRot.Forward * 40;
				ragdoll.Rotation = Rotation.LookAt( Vector3.Random.Normal );
				ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
				ragdoll.PhysicsGroup.Velocity = EyeRot.Forward * 1000;
			}

			if(Input.Pressed( InputButton.Attack2) )
			{
				ent = new ModelEntity( "addons/citizen/models/citizen_props/crate01.vmdl" );
				ent.RenderAlpha = 0.5f;
			}
			
			TraceResult tr = Trace.Ray( EyePos, EyePos + EyeRot.Forward * 200 ).Ignore( this ).Run();
			if(IsClient && Input.Down( InputButton.Attack2) && ent != null && ent.IsValid())
			{
				ent.Position = tr.EndPos;
			}
			
			if(Input.Released( InputButton.Attack2) )
			{
				Log.Info( "spawn" );
				if(ent != null && ent.IsValid())
				{
					ent.Delete();
				}
				if(IsServer)
				{
					Prop p = new Prop();
					p.SetModel( "addons/citizen/models/citizen_props/crate01.vmdl" );
					p.Position = tr.EndPos;
					p.Spawn();
					p.SetupPhysicsFromModel( PhysicsMotionType.Static, false );
				}
			}

			
		}

		public override void OnKilled()
		{
			base.OnKilled();

			EnableDrawing = false;
		}
	}
}
