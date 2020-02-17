using System;
using Terraria;


namespace MountedMagicMirrors.Helpers.World {
	public class TileChunkHelpers {
		public static bool IsTileSyncedForCurrentClient( int tileX, int tileY ) {
			int sectionX = Netplay.GetSectionX( tileX );
			int sectionY = Netplay.GetSectionY( tileY );
			return Main.sectionManager.SectionLoaded( sectionX, sectionY );
		}
	}
}
