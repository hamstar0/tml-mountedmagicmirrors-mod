using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;
using HamstarHelpers.Helpers.Players;
using HamstarHelpers.Helpers.Tiles;
using MountedMagicMirrors.Tiles;
using MountedMagicMirrors.DataStructures;


namespace MountedMagicMirrors {
	partial class MMMPlayer : ModPlayer {
		public static bool? IsMirrorTileInvalid( int tileX, int tileY ) {
			if( Main.netMode == 1 ) {
				var myworld = ModContent.GetInstance<MMMWorld>();
				if( !myworld.IsTileLoaded(tileX, tileY) ) {
					return null;
				}

				/*if( !TileChunkHelpers.IsTileSyncedForCurrentClient( tileX, tileY ) ) {
					Tile tile = Main.tile[tileX, tileY];
					if( tile == null || TileHelpers.IsEqual(tile, new Tile()) ) {
						return null;
					}
				}*/
			}

			return !MountedMagicMirrorsMod.Instance.MMMTilePattern.Check( tileX, tileY );
		}



		////////////////

		public IEnumerable<(int tileX, int tileY)> GetDiscoveredMirrors() {
			if( this.CurrentWorldDiscoveredMirrorTiles == null ) {
				yield break;
			}

			lock( MMMPlayer.MyCurrentMirrorsLock ) {
				foreach( (int tileX, ISet<int> tileYs) in this.CurrentWorldDiscoveredMirrorTiles.ToArray() ) {
					foreach( int tileY in tileYs.ToArray() ) {
						yield return (tileX, tileY);
					}
				}
			}
		}

		////

		internal void SetDiscoveredMirrorsFromNetwork( IDictionary<string, DiscoveredMirrors> mirrors ) {
			this.DiscoveredMirrorTilesPerWorld = mirrors;
		}

		////

		public void ClearInvalidMirrorDiscoveries() {
			if( this.CurrentWorldDiscoveredMirrorTiles == null ) {
				return;
			}

			IList<(int x, int y)> removals = new List<(int, int)>();

			lock( MMMPlayer.MyCurrentMirrorsLock ) {
				foreach( (int tileX, ISet<int> tileYs) in this.CurrentWorldDiscoveredMirrorTiles.ToArray() ) {
					foreach( int tileY in tileYs.ToArray() ) {
						if( MMMPlayer.IsMirrorTileInvalid(tileX, tileY) == true ) {
							removals.Add( (tileX, tileY) );
						}
					}
				}

				foreach( (int tileX, int tileY) in removals ) {
					this.CurrentWorldDiscoveredMirrorTiles.Remove2D( tileX, tileY );
				}
			}
		}


		////

		public bool AddDiscoveredMirror( int mouseTileX, int mouseTileY ) {
			var mymod = (MountedMagicMirrorsMod)this.mod;
			this.GetDiscoveredMirrors();    // Unremember non-existent mirrors

			(int TileX, int TileY) mirrorTile;
			bool foundTile = TileFinderHelpers.FindTopLeftOfSquare( mymod.MMMTilePattern, mouseTileX, mouseTileY, 3, out mirrorTile );
			if( !foundTile ) {
				if( MMMConfig.Instance.DebugModeInfo ) {
					LogHelpers.LogAndPrintOnce( "A - No mirror at " + mouseTileX + "," + mouseTileY );
				}
				return false;
			}

			if( this.CurrentWorldDiscoveredMirrorTiles?.Contains2D(mirrorTile.TileX, mirrorTile.TileY) ?? true ) {
				return false;
			}

			lock( MMMPlayer.MyCurrentMirrorsLock ) {
				this.CurrentWorldDiscoveredMirrorTiles.Set2D( mirrorTile.TileX, mirrorTile.TileY );

				return true;
			}
		}


		////////////////

		public bool TeleportToMirror( int tileX, int tileY ) {
			int mmmTileType = ModContent.TileType<MountedMagicMirrorTile>();
			tileX++;

			Tile tile = Framing.GetTileSafely( tileX, tileY );
			if( tile.type != mmmTileType ) {
				if( MMMPlayer.IsMirrorTileInvalid(tileX, tileY) == true ) {
					if( MMMConfig.Instance.DebugModeInfo ) {
						Main.NewText( "Cannot teleport - Invalid mirror tile at " + tileX + "," + tileY );
					}
					return false;
				}
			}

			var pos = new Vector2( (tileX << 4), (tileY << 4) );
			PlayerWarpHelpers.Teleport( this.player, pos, PlayerWarpHelpers.MagicMirrorWarpStyle );

			return true;
		}
	}
}
