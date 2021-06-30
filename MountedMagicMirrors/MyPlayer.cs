using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Libraries.DotNET.Extensions;
using ModLibsCore.Libraries.TModLoader;
using ModLibsCore.Libraries.World;
using MountedMagicMirrors.DataStructures;


namespace MountedMagicMirrors {
	partial class MMMPlayer : ModPlayer {
		private ConcurrentDictionary<string, DiscoveredMirrors> DiscoveredMirrorTilesPerWorld
			= new ConcurrentDictionary<string, DiscoveredMirrors>();
		
		////

		 private DiscoveredMirrors _CurrentWorldDiscoveredMirrorTiles = null;

		public DiscoveredMirrors CurrentWorldDiscoveredMirrorTiles {
			get {
				if( this._CurrentWorldDiscoveredMirrorTiles == null ) {
					string worldUid = WorldIdentityLibraries.GetUniqueIdForCurrentWorld( true );

					if( !this.DiscoveredMirrorTilesPerWorld.TryGetValue(worldUid, out this._CurrentWorldDiscoveredMirrorTiles) ) {
						if( !this.DiscoveredMirrorTilesPerWorld.TryGetValue("_", out this._CurrentWorldDiscoveredMirrorTiles) ) {
							this._CurrentWorldDiscoveredMirrorTiles = null;
						}
					}
				}
				return this._CurrentWorldDiscoveredMirrorTiles;
			}
		}

		////

		public bool IsMapMirrorPicking { get; private set; } = false;

		public bool ClickSafetyLock { get; private set; } = false;

		internal (int TileX, int TileY)? TargetMirror = null;

		////

		public override bool CloneNewInstances => false;



		////////////////

		public override void Load( TagCompound tag ) {
			this.DiscoveredMirrorTilesPerWorld.Clear();

			if( !tag.ContainsKey("world_count") ) {
				return;
			}

			int worldCount = tag.GetInt( "world_count" );
			int totalMirrorCount = 0;

			for( int i=0; i<worldCount; i++ ) {
				string worldUid = tag.GetString( "world_uid_"+i );
				int mirrorCount = tag.GetInt( "discovery_count_for_"+i );

				this.DiscoveredMirrorTilesPerWorld[worldUid] = new DiscoveredMirrors();

				for( int j=0; j<mirrorCount; j++ ) {
					int tileX = tag.GetInt( "discovery_x_"+i+"_"+j );
					int tileY = tag.GetInt( "discovery_y_"+i+"_"+j );

					this.DiscoveredMirrorTilesPerWorld[worldUid].Set2D( tileX, tileY );

					if( MMMConfig.Instance.DebugModeInfo ) {
						LogLibraries.Log( "Loaded mirror at " + tileX + ", " + tileY );
					}
				}

				totalMirrorCount += mirrorCount;
			}

			LogLibraries.Log( "Loaded "+totalMirrorCount+" discovered mirrors for "+this.player.name+" ("+this.player.whoAmI+")" );
		}


		////

		public override TagCompound Save() {
			var tag = new TagCompound {
				{ "world_count", this.DiscoveredMirrorTilesPerWorld.Count }
			};

			int i = 0;
			foreach( string worldUid in this.DiscoveredMirrorTilesPerWorld.Keys ) {
				IDictionary<int, ISet<int>> mirrors = this.DiscoveredMirrorTilesPerWorld[ worldUid ];
				int count = mirrors.Count2D();

				string myWorldUid = worldUid;
				if( worldUid == "_" ) {
					myWorldUid = WorldIdentityLibraries.GetUniqueIdForCurrentWorld( true );

					if( MMMConfig.Instance.DebugModeInfo ) {
						LogLibraries.Log( "Saving for world UID " + myWorldUid );
					}
				}

				tag[ "world_uid_"+i ] = myWorldUid;
				tag[ "discovery_count_for_"+i ] = count;

				int j = 0;
				foreach( (int tileX, ISet<int> tileYs) in mirrors ) {
					foreach( int tileY in tileYs ) {
						tag["discovery_x_"+i+"_"+j] = (int)tileX;
						tag["discovery_y_"+i+"_"+j] = (int)tileY;
						j++;
					}
				}

				LogLibraries.Log( "Saved "
					+j+" of "+count
					+" discovered mirrors of world "
					+myWorldUid+" ("+i+") for "
					+this.player.name+" ("+this.player.whoAmI+")" );

				i++;
			}

			return tag;
		}


		////////////////

		public override void PostSavePlayer() {
			this._CurrentWorldDiscoveredMirrorTiles = null;
		}


		////////////////

		public override void PreUpdate() {
			if( this.player.whoAmI == Main.myPlayer ) {
				this.PreUpdateLocal();
			}
		}

		private void PreUpdateLocal() {
			if( this.CurrentWorldDiscoveredMirrorTiles == null ) {
				if( LoadLibraries.IsWorldBeingPlayed() ) {
					string currWorldUid = WorldIdentityLibraries.GetUniqueIdForCurrentWorld( true );
					this.DiscoveredMirrorTilesPerWorld[currWorldUid] = new DiscoveredMirrors();
				}
			}

			this.ClearInvalidMirrorDiscoveries();

			if( MMMConfig.Instance.DebugModePosition ) {
				DebugLibraries.Print( "WhereAmI", ( this.player.Center / 16 ).ToShortString() + " (" + this.player.Center.ToString() + ")" );
			}

			if( this.IsMapMirrorPicking ) {
				this.UpdateMapMirrorPicking();
			}
		}
	}
}
