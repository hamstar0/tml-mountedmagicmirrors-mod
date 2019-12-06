using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using HamstarHelpers.Helpers.DotNET.Extensions;
using Terraria;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Services.Timers;
using MountedMagicMirrors.Tiles;


namespace MountedMagicMirrors {
	partial class MMMPlayer : ModPlayer {
		internal static readonly object MyLock = new object();



		////////////////

		public IDictionary<int, ISet<int>> DiscoveredMirrorTiles { get; private set; }
			= new Dictionary<int, ISet<int>>();

		////

		public bool IsMapMirrorPicking { get; private set; } = false;

		public bool ClickSafetyLock { get; private set; } = false;

		public (int TileX, int TileY)? TargetMirror = null;

		////

		public override bool CloneNewInstances => false;



		////////////////

		public override void clientClone( ModPlayer clientClone ) {
			var myclone = (MMMPlayer)clientClone;

			lock( MMMPlayer.MyLock ) {
				foreach( (int tileX, ISet<int> tileYs) in this.DiscoveredMirrorTiles ) {
					myclone.DiscoveredMirrorTiles[tileX] = new HashSet<int>( tileYs );
				}
			}
		}



		////////////////

		public override void Load( TagCompound tag ) {
			var mymod = (MountedMagicMirrorsMod)this.mod;

			lock( MMMPlayer.MyLock ) {
				this.DiscoveredMirrorTiles.Clear();

				if( !tag.ContainsKey( "discovery_count" ) ) {
					return;
				}

				int count = tag.GetInt( "discovery_count" );

				for( int i = 0; i < count; i++ ) {
					int x = tag.GetInt( "discovery_x_" + i );
					int y = tag.GetInt( "discovery_y_" + i );

					this.DiscoveredMirrorTiles.Set2D( x, y );
				}

				LogHelpers.Log( "Loaded "+count+" discovered mirrors for "+this.player.name+" ("+this.player.whoAmI+")" );
			}
		}


		public override TagCompound Save() {
			lock( MMMPlayer.MyLock ) {
				int count = this.DiscoveredMirrorTiles.Count2D();
				var tag = new TagCompound {
					{ "discovery_count", count }
				};

				int i = 0;
				foreach( (int tileX, ISet<int> tileYs) in this.DiscoveredMirrorTiles ) {
					foreach( int tileY in tileYs ) {
						tag["discovery_x_" + i] = (int)tileX;
						tag["discovery_y_" + i] = (int)tileY;
						i++;
					}
				}

				LogHelpers.Log( "Saved "+i+" of "+count+" discovered mirrors for "+this.player.name+" ("+this.player.whoAmI+")" );

				return tag;
			}
		}


		////////////////

		public override void PreUpdate() {
			if( this.player.whoAmI == Main.myPlayer ) {
				if( MMMConfig.Instance.DebugModePosition ) {
					DebugHelpers.Print( "WhereAmI", (this.player.Center/16).ToShortString()+" ("+this.player.Center.ToString()+")" );
				}

				if( this.IsMapMirrorPicking ) {
					this.UpdateMapMirrorPicking();
				}
			}
		}

		////

		private void UpdateMapMirrorPicking() {
			if( !Main.mapFullscreen ) {
				this.EndMapMirrorPicking();
				return;
			}

			bool isClick =	(Main.mouseRight && Main.mouseRightRelease) ||
							(Main.mouseLeft && Main.mouseLeftRelease);

			if( isClick ) {
				if( !this.ClickSafetyLock ) {
					bool isPickingNow = Timers.GetTimerTickDuration( "MMMIsMapMirrorPickingNow" ) > 0;

					if( isPickingNow && this.TargetMirror.HasValue ) {
						(int TileX, int TileY) target = this.TargetMirror.Value;

						if( this.TeleportToMirror( target.TileX, target.TileY ) ) {
							this.EndMapMirrorPicking();
						}
					}
				}
			} else {
				//if( (!Main.mouseLeft && !Main.mouseLeftRelease) && (!Main.mouseRight && !Main.mouseRightRelease) ) {
				this.ClickSafetyLock = false;
				//}
			}
		}


		////////////////

		public void BeginMapMirrorPicking() {
			this.IsMapMirrorPicking = true;
			this.ClickSafetyLock = true;
		}

		public void EndMapMirrorPicking() {
			this.IsMapMirrorPicking = false;
			this.ClickSafetyLock = false;
			Main.mapFullscreen = false;
		}
	}
}
