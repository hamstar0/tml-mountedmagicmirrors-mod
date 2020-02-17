﻿using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;
using HamstarHelpers.Helpers.Players;
using HamstarHelpers.Helpers.TModLoader;
using HamstarHelpers.Helpers.Tiles;
using MountedMagicMirrors.Tiles;


namespace MountedMagicMirrors {
	partial class MMMPlayer : ModPlayer {
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

		public void ClearInvalidMirrorDiscoveries() {
			if( this.CurrentWorldDiscoveredMirrorTiles == null || !LoadHelpers.IsWorldBeingPlayed() ) {
				return;
			}

			IList<(int x, int y)> removals = new List<(int, int)>();

			lock( MMMPlayer.MyCurrentMirrorsLock ) {
				foreach( (int tileX, ISet<int> tileYs) in this.CurrentWorldDiscoveredMirrorTiles.ToArray() ) {
					foreach( int tileY in tileYs.ToArray() ) {
						Tile tile = Framing.GetTileSafely( tileX, tileY );

						if( !MountedMagicMirrorsMod.Instance.MMMTilePattern.Check(tileX, tileY) ) {
							if( Main.netMode != 1 || Helpers.World.TileChunkHelpers.IsTileSyncedForCurrentClient(tileX, tileY) ) {
								removals.Add( (tileX, tileY) );
							}
						}
					}
				}

				if( removals.Count > 0 ) {
					foreach( (int tileX, int tileY) in removals ) {
						this.CurrentWorldDiscoveredMirrorTiles.Remove2D( tileX, tileY );
					}
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
				if( MMMConfig.Instance.DebugModeInfo ) {
					Main.NewText( "Cannot teleport - Invalid mirror tile at " + tileX + "," + tileY );
				}
				return false;
			}

			var pos = new Vector2( (tileX << 4), (tileY << 4) );
			PlayerWarpHelpers.Teleport( this.player, pos, PlayerWarpHelpers.MagicMirrorWarpStyle );

			return true;
		}
	}
}
