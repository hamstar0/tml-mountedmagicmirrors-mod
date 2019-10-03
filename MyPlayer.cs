using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using HamstarHelpers.Helpers.DotNET.Extensions;
using Terraria;
using HamstarHelpers.Helpers.Tiles;


namespace MountedMagicMirrors {
	partial class MMMPlayer : ModPlayer {
		internal static readonly object MyLock = new object();



		////////////////

		public IDictionary<int, ISet<int>> DiscoveredMirrorTiles { get; } = new Dictionary<int, ISet<int>>();

		public bool IsMirrorPicking { get; private set; } = false;

		public (int TileX, int TileY)? TargetMirror = null;

		////

		public override bool CloneNewInstances => false;



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

					(int TileX, int TileY) coords;
					bool foundTile = TileFinderHelpers.FindTopLeftOfSquare( mymod.MMMTilePattern, x, y, 3, out coords );
					if( foundTile ) {
						this.DiscoveredMirrorTiles.Set2D( coords.TileX, coords.TileY );
					}
				}
			}
		}


		public override TagCompound Save() {
			lock( MMMPlayer.MyLock ) {
				var tag = new TagCompound {
					{ "discovery_count", this.DiscoveredMirrorTiles.Count2D() }
				};

				int i = 0;
				foreach( (int tileX, ISet<int> tileYs) in this.DiscoveredMirrorTiles ) {
					foreach( int tileY in tileYs ) {
						tag["discovery_x_" + i] = tileX;
						tag["discovery_y_" + i] = tileY;
						i++;
					}
				}

				return tag;
			}
		}


		////////////////

		public override void PreUpdate() {
			if( this.player.whoAmI == Main.myPlayer ) {
				if( this.IsMirrorPicking ) {
					this.UpdateFastTravelPicking();
				}
			}
		}

		////

		private void UpdateFastTravelPicking() {
			if( !Main.mapFullscreen ) {
				this.IsMirrorPicking = false;
				return;
			}

			if( Main.mouseLeft && Main.mouseLeftRelease ) {
				if( this.TargetMirror.HasValue ) {
					var targ = this.TargetMirror.Value;

					if( this.TeleportToMirror( targ.TileX, targ.TileY ) ) {
						this.IsMirrorPicking = false;
						Main.mapFullscreen = false;
					}
				}
			}
		}


		////////////////

		public void BeginFastTravelChoice() {
			this.IsMirrorPicking = true;
		}
	}
}
