using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.DotNET.Extensions;
using Terraria;
using HamstarHelpers.Helpers.Tiles;
using MountedMagicMirrors.Tiles;
using HamstarHelpers.Helpers.Players;
using Microsoft.Xna.Framework;


namespace MountedMagicMirrors {
	partial class MMMPlayer : ModPlayer {
		private IList<(int tileX, int tileY)> _Removals = new List<(int, int)>();



		////////////////

		public IEnumerable<(int tileX, int tileY)> GetDiscoveredMirrors() {
			lock( MMMPlayer.MyLock ) {
				int mmmType = ModContent.TileType<MountedMagicMirrorTile>();

				foreach( (int tileX, ISet<int> tileYs) in this.DiscoveredMirrorTiles ) {
					foreach( int tileY in tileYs ) {
						Tile tile = Framing.GetTileSafely( tileX, tileY );

						if( TileHelpers.IsAir(tile) || tile.type != mmmType ) {
							this._Removals.Add( (tileX, tileY) );
						} else {
							yield return (tileX, tileY);
						}
					}
				}

				if( this._Removals.Count > 0 ) {
					foreach( (int tileX, int tileY) in this._Removals ) {
						this.DiscoveredMirrorTiles.Remove2D( tileX, tileY );
					}
					this._Removals.Clear();
				}
			}
		}


		public bool AddDiscoveredMirror( int tileX, int tileY ) {
			var mymod = (MountedMagicMirrorsMod)this.mod;
			this.GetDiscoveredMirrors();    // Removes old mirrors

			(int TileX, int TileY) tileAt;
			bool foundTile = TileFinderHelpers.FindTopLeftOfSquare( mymod.MMMTilePattern,
				tileX, tileY, 3, out tileAt );
			if( !foundTile ) {
				if( mymod.Config.DebugModeInfo ) {
					Main.NewText( "A - No mirror at " + tileX + "," + tileY );
				}
				return false;
			}

			tileX = tileAt.TileX;
			tileY = tileAt.TileY;

			lock( MMMPlayer.MyLock ) {
				for( int i=-3; i<3; i++ ) {
					for( int j=-3; j<3; j++ ) {
						if( !this.DiscoveredMirrorTiles.ContainsKey(tileX+i) ) {
							continue;
						}
						if( !this.DiscoveredMirrorTiles[tileX+i].Contains(tileY+j) ) {
							continue;
						}
						return false;
					}
				}

				this.DiscoveredMirrorTiles.Set2D( tileX, tileY );
				return true;
			}
		}


		////////////////

		public bool SetTargetMirror( int tileX, int tileY ) {
			var mymod = (MountedMagicMirrorsMod)this.mod;

			(int, int) tileAt;
			bool foundTile = TileFinderHelpers.FindTopLeftOfSquare(
				mymod.MMMTilePattern,
				tileX, tileY, 3, out tileAt );

			if( foundTile ) {
				this.TargetMirror = tileAt;
			} else {
				this.TargetMirror = null;
			}

			if( mymod.Config.DebugModeInfo ) {
				if( !this.TargetMirror.HasValue ) {
					Main.NewText( "B - No mirror at " + this.TargetMirror );
				}
			}
			return this.TargetMirror != null;
		}


		////////////////

		public bool TeleportToMirror( int tileX, int tileY ) {
			var mymod = (MountedMagicMirrorsMod)this.mod;
			int mmmTileType = ModContent.TileType<MountedMagicMirrorTile>();
			tileX++;

			Tile tile = Framing.GetTileSafely( tileX, tileY );
			if( tile.type != mmmTileType ) {
				if( mymod.Config.DebugModeInfo ) {
					Main.NewText( "C - No mirror at " + tileX + "," + tileY );
				}
				return false;
			}

			var pos = new Vector2( (tileX << 4), (tileY << 4) );
			PlayerWarpHelpers.Teleport( this.player, pos, PlayerWarpHelpers.MagicMirrorWarpStyle );

			return true;
		}
	}
}
