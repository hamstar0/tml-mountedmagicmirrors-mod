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
			this.GetDiscoveredMirrors();	// Removes old mirrors

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

			Tile tile = Main.tile[tileX, tileY];
			if( tile == null || TileHelpers.IsAir( tile ) || tile.type != mmmTileType ) {
				return false;
			}

			int i, j = 0;

			// Find the exact tile
			for( i = 0; i < 3; i++ ) {
				for( j = 0; j < 3; j++ ) {
					tile = Framing.GetTileSafely( tileX - i, tileY - j );
					if( tile.type != mmmTileType ) {
						break;
					}
				}
				j--;

				tile = Framing.GetTileSafely( tileX - i, tileY - j );
				if( tile.type != mmmTileType ) {
					break;
				}
			}
			i--;

			tile = Framing.GetTileSafely( tileX - i, tileY - j );
			if( tile.type == mmmTileType ) {
				this.TargetMirror = (tileX - i, tileY - j);
				return true;
			}

			return false;
		}

		
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
