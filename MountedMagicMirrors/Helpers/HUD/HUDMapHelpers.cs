using System;
using Terraria;


namespace MountedMagicMirrors.Helpers.HUD {
	public class HUDMapHelpers {
		public static bool GetFullscreenMapTileOfScreenPosition( int scrPosX, int scrPosY, out (int x, int y) tilePos ) {
			float mapScale = Main.mapFullscreenScale;
			float minX = 10f;
			float minY = 10f;
			float maxX = (float)( Main.maxTilesX - 10 );
			float maxY = (float)( Main.maxTilesY - 10 );

			float mapPosX = Main.mapFullscreenPos.X * mapScale;
			float mapPosY = Main.mapFullscreenPos.Y * mapScale;

			float scrOriginX = (float)( Main.screenWidth / 2 ) - mapPosX;
			scrOriginX += minX * mapScale;
			float scrOriginY = (float)( Main.screenHeight / 2 ) - mapPosY;
			scrOriginY += minX * mapScale;
			
			int tileX = (int)(((float)scrPosX - scrOriginX) / mapScale + minX);
			int tileY = (int)(((float)scrPosY - scrOriginY) / mapScale + minY);

			tilePos = (tileX, tileY);
			return tileX >= minX
				&& tileX < maxX
				&& tileY >= minY
				&& tileY < maxY;
		}
	}
}
