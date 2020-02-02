﻿using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;
using HamstarHelpers.Helpers.Tiles;
using HamstarHelpers.Helpers.Players;
using HamstarHelpers.Services.Timers;
using MountedMagicMirrors.Tiles;


namespace MountedMagicMirrors {
	partial class MMMPlayer : ModPlayer {
		private readonly IList<(int TileX, int TileY)> _Removals = new List<(int, int)>();



		////////////////

		public IEnumerable<(int tileX, int tileY)> GetDiscoveredMirrors() {
			lock( MMMPlayer.MyLock ) {
				foreach( (int tileX, ISet<int> tileYs) in this.CurrentWorldDiscoveredMirrorTiles ) {
					foreach( int tileY in tileYs ) {
						Tile tile = Framing.GetTileSafely( tileX, tileY );

						if( !MountedMagicMirrorsMod.Instance.MMMTilePattern.Check(tileX, tileY) ) {
							this._Removals.Add( (tileX, tileY) );
						} else {
							yield return (tileX, tileY);
						}
					}
				}

				if( this._Removals.Count > 0 ) {
					foreach( (int tileX, int tileY) in this._Removals ) {
						this.CurrentWorldDiscoveredMirrorTiles.Remove2D( tileX, tileY );
					}
					this._Removals.Clear();
				}
			}
		}


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

			if( this.CurrentWorldDiscoveredMirrorTiles.Contains2D(mirrorTile.TileX, mirrorTile.TileY) ) {
				return false;
			}

			lock( MMMPlayer.MyLock ) {
				this.CurrentWorldDiscoveredMirrorTiles.Set2D( mirrorTile.TileX, mirrorTile.TileY );

				return true;
			}
		}


		////////////////

		public bool SetTargetMirror( int tileX, int tileY ) {
			var mymod = (MountedMagicMirrorsMod)this.mod;

			(int TileX, int TileY) tileAt;
			bool foundTile = TileFinderHelpers.FindTopLeftOfSquare(
				mymod.MMMTilePattern,
				tileX, tileY, 3, out tileAt );

			if( foundTile ) {
				Timers.SetTimer( "MMMIsMapMirrorPickingNow", 2, true, () => false );
				this.TargetMirror = tileAt;
			} else {
				this.TargetMirror = null;
			}

			if( MMMConfig.Instance.DebugModeInfo ) {
				if( !this.TargetMirror.HasValue ) {
					Main.NewText( "Cannot target; undiscovered mirror at " + this.TargetMirror.Value );
				}
			}

			return this.TargetMirror != null;
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
