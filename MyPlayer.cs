using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using HamstarHelpers.Helpers.DotNET.Extensions;
using Terraria;
using Microsoft.Xna.Framework;
using HamstarHelpers.Helpers.Tiles;
using MountedMagicMirrors.Tiles;


namespace MountedMagicMirrors {
	class MMMPlayer : ModPlayer {
		internal static readonly object MyLock = new object();



		////////////////

		public IDictionary<int, ISet<int>> DiscoveredMirrorTiles { get; } = new Dictionary<int, ISet<int>>();

		////

		public override bool CloneNewInstances => false;



		////////////////

		public override void Load( TagCompound tag ) {
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

		private IList<(int tileX, int tileY)> _Removals = new List<(int, int)>();

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
	}
}
