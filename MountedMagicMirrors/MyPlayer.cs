﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;
using HamstarHelpers.Helpers.World;
using HamstarHelpers.Helpers.TModLoader;
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
					string worldUid = WorldHelpers.GetUniqueIdForCurrentWorld( true );

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
			int count = 0;

			if( !tag.ContainsKey("world_count") ) {
				if( tag.ContainsKey( "discovery_count" ) ) {
					this.DiscoveredMirrorTilesPerWorld.Clear();
					count = this.LoadOld( tag );
				}
			} else {
				this.DiscoveredMirrorTilesPerWorld.Clear();
				count = this.LoadNew( tag );
			}

			LogHelpers.Log( "Loaded "+count+" discovered mirrors for "+this.player.name+" ("+this.player.whoAmI+")" );
		}

		private int LoadOld( TagCompound tag ) {
			int count = tag.GetInt( "discovery_count" );

			this.DiscoveredMirrorTilesPerWorld["_"] = new DiscoveredMirrors();

			for( int i = 0; i < count; i++ ) {
				int tileX = tag.GetInt( "discovery_x_" + i );
				int tileY = tag.GetInt( "discovery_y_" + i );

				this.DiscoveredMirrorTilesPerWorld["_"].Set2D( tileX, tileY );
				
				if( MMMConfig.Instance.DebugModeInfo ) {
					LogHelpers.Log( "(Old) Loaded mirror at " + tileX + ", " + tileY );
				}
			}

			return count;
		}

		private int LoadNew( TagCompound tag ) {
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
						LogHelpers.Log( "Loaded mirror at " + tileX + ", " + tileY );
					}
				}

				totalMirrorCount += mirrorCount;
			}

			return totalMirrorCount;
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
					myWorldUid = WorldHelpers.GetUniqueIdForCurrentWorld( true );
					if( MMMConfig.Instance.DebugModeInfo ) {
						LogHelpers.Log( "Saving for world UID " + myWorldUid );
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

				LogHelpers.Log( "Saved "
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
				if( LoadHelpers.IsWorldBeingPlayed() ) {
					string currWorldUid = WorldHelpers.GetUniqueIdForCurrentWorld( true );
					this.DiscoveredMirrorTilesPerWorld[currWorldUid] = new DiscoveredMirrors();
				}
			}

			this.ClearInvalidMirrorDiscoveries();

			if( MMMConfig.Instance.DebugModePosition ) {
				DebugHelpers.Print( "WhereAmI", ( this.player.Center / 16 ).ToShortString() + " (" + this.player.Center.ToString() + ")" );
			}

			if( this.IsMapMirrorPicking ) {
				this.UpdateMapMirrorPicking();
			}
		}
	}
}
