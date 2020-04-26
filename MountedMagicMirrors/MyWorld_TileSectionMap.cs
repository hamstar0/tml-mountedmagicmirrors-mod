using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;


namespace MountedMagicMirrors {
	partial class MMMWorld : ModWorld {
		private IDictionary<int, ISet<int>> TileSectionMapByTile = new ConcurrentDictionary<int, ISet<int>>();



		////////////////

		public void RegisterTileSectionTile( int tileX, int tileY ) {
			int sectionX = Netplay.GetSectionX( tileX );
			int sectionY = Netplay.GetSectionY( tileY );

			this.TileSectionMapByTile.Set2D( sectionX, sectionY );
		}

		////

		public bool IsTileLoaded( int tileX, int tileY ) {
			int sectionX = Netplay.GetSectionX( tileX );
			int sectionY = Netplay.GetSectionY( tileY );

			return this.TileSectionMapByTile.Contains2D( sectionX, sectionY );
		}


		////////////////

		public void UnregisterTileSections() {
			this.TileSectionMapByTile.Clear();
		}
	}
}
