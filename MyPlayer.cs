using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;
using HamstarHelpers.Helpers.World;


namespace MountedMagicMirrors {
	class DiscoveredMirrors : Dictionary<int, ISet<int>> { }




	partial class MMMPlayer : ModPlayer {
		internal static readonly object MyLock = new object();



		////////////////

		private IDictionary<string, DiscoveredMirrors> DiscoveredMirrorTiles = new Dictionary<string, DiscoveredMirrors>();
		
		////

		 private DiscoveredMirrors _CurrentWorldDiscoveredMirrorTiles = null;

		public DiscoveredMirrors CurrentWorldDiscoveredMirrorTiles {
			get {
				if( this._CurrentWorldDiscoveredMirrorTiles == null ) {
					string worldUid = WorldHelpers.GetUniqueIdForCurrentWorld( true );
					if( !this.DiscoveredMirrorTiles.TryGetValue(worldUid, out this._CurrentWorldDiscoveredMirrorTiles) ) {
						this._CurrentWorldDiscoveredMirrorTiles = null;
					}
				}
				return this._CurrentWorldDiscoveredMirrorTiles;
			}
		}

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
				foreach( (string worldUid, DiscoveredMirrors mirrors) in this.DiscoveredMirrorTiles ) {
					myclone.DiscoveredMirrorTiles[worldUid] = new DiscoveredMirrors();

					foreach( (int tileX, ISet<int> tileYs) in mirrors ) {
						myclone.DiscoveredMirrorTiles[worldUid][tileX] = new HashSet<int>( tileYs );
					}
				}
			}
		}



		////////////////

		public override void Load( TagCompound tag ) {
			lock( MMMPlayer.MyLock ) {
				this.DiscoveredMirrorTiles.Clear();

				int count = 0;

				if( !tag.ContainsKey("world_count") && tag.ContainsKey( "discovery_count" ) ) {
					count = this.LoadOld( tag );
				} else {
					count = this.LoadNew( tag );
				}

				LogHelpers.Log( "Loaded "+count+" discovered mirrors for "+this.player.name+" ("+this.player.whoAmI+")" );
			}
		}

		private int LoadOld( TagCompound tag ) {
			string worldUid = WorldHelpers.GetUniqueIdForCurrentWorld( true );
			int count = tag.GetInt( "discovery_count" );

			for( int i = 0; i < count; i++ ) {
				int x = tag.GetInt( "discovery_x_" + i );
				int y = tag.GetInt( "discovery_y_" + i );

				this.DiscoveredMirrorTiles[worldUid].Set2D( x, y );
			}

			return count;
		}

		private int LoadNew( TagCompound tag ) {
			string currWorldUid = WorldHelpers.GetUniqueIdForCurrentWorld( true );
			int worldCount = tag.GetInt( "world_count" );
			int currMirrorCount = 0;

			for( int i=0; i<worldCount; i++ ) {
				string worldUid = tag.GetString( "world_uid_"+i );
				int mirrorCount = tag.GetInt( "discovery_count_for_"+i );

				for( int j=0; j<mirrorCount; j++ ) {
					int tileX = tag.GetInt( "discovery_x_"+i+"_"+j );
					int tileY = tag.GetInt( "discovery_y_"+i+"_"+j );

					this.DiscoveredMirrorTiles[worldUid].Set2D( tileX, tileY );
				}

				if( worldUid.Equals(currWorldUid) ) {
					currMirrorCount = mirrorCount;
				}
			}

			return currMirrorCount;
		}


		////

		public override TagCompound Save() {
			lock( MMMPlayer.MyLock ) {
				var tag = new TagCompound {
					{ "world_count", this.DiscoveredMirrorTiles.Count }
				};

				int i = 0;
				foreach( string worldUid in this.DiscoveredMirrorTiles.Keys ) {
					IDictionary<int, ISet<int>> mirrors = this.DiscoveredMirrorTiles[worldUid];
					int count = mirrors.Count2D();

					tag[ "world_uid_"+i ] = worldUid;
					tag[ "discovery_count_for_"+i ] = count;

					int j = 0;
					foreach( (int tileX, ISet<int> tileYs) in mirrors ) {
						foreach( int tileY in tileYs ) {
							tag["discovery_x_" + i + "_" + j] = (int)tileX;
							tag["discovery_y_" + i + "_" + j] = (int)tileY;
							j++;
						}
					}

					LogHelpers.Log( "Saved "
						+j+" of "+count
						+" discovered mirrors of world "
						+worldUid+" ("+i+") for "
						+this.player.name+" ("+this.player.whoAmI+")" );
				}

				return tag;
			}
		}


		////////////////

		public override void PostSavePlayer() {
			this._CurrentWorldDiscoveredMirrorTiles = null;
		}
	}
}
