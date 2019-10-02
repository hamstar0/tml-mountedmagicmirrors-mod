using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.DotNET.Extensions;
using Terraria;
using HamstarHelpers.Helpers.Tiles;
using MountedMagicMirrors.Tiles;
using HamstarHelpers.Helpers.Players;
using Microsoft.Xna.Framework;
using HamstarHelpers.Classes.Tiles.TilePattern;


namespace MountedMagicMirrors {
	partial class MMMPlayer : ModPlayer {
		private IList<(int tileX, int tileY)> _Removals = new List<(int, int)>();



		////////////////

		public IEnumerable<(int tileX, int tileY)> GetDiscoveredMirrors() {
			lock( MMMPlayer.MyLock ) {
				int mmmType = this.mod.TileType<MountedMagicMirrorTile>();

				foreach( (int tileX, ISet<int> tileYs) in this.DiscoveredMirrorTiles ) {
					foreach( int tileY in tileYs ) {
						Tile tile = Main.tile[tileX, tileY];

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
			this.GetDiscoveredMirrors();    // Removes old mirrors

			var pattern = new TilePattern( new TilePatternBuilder {
				IsAnyOfType = new HashSet<int> { this.mod.TileType<MountedMagicMirrorTile>() }
			} );

			(int TileX, int TileY)? tileAt = TileFinderHelpers.FindTopLeft( pattern, tileX, tileY, 3, 3 );
			if( tileAt == null ) {
				return false;
			}

			tileX = tileAt.Value.TileX;
			tileY = tileAt.Value.TileY;

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
			int mmmTileType = this.mod.TileType<MountedMagicMirrorTile>();

			var pattern = new TilePattern( new TilePatternBuilder {
				IsAnyOfType = new HashSet<int> { mmmTileType }
			} );
			(int, int)? tileAt = TileFinderHelpers.FindTopLeft( pattern, tileX, tileY, 3, 3 );

			this.TargetMirror = tileAt ?? (0, 0);
			return tileAt != null;
		}


		////////////////

		public bool TeleportToMirror( int tileX, int tileY ) {
			int mmmTileType = this.mod.TileType<MountedMagicMirrorTile>();

			Tile tile = Framing.GetTileSafely( tileX + 1, tileY + 1 );
			if( tile.type != mmmTileType ) {
				return false;
			}

			var pos = new Vector2( (tileX << 4) + 16, (tileY << 4) + 16 );
			PlayerWarpHelpers.Teleport( this.player, pos, PlayerWarpHelpers.MagicMirrorWarpStyle );

			return true;
		}
	}
}
