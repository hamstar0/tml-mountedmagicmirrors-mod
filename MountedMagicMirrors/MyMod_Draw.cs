using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.HUD;
using HamstarHelpers.Helpers.TModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;


namespace MountedMagicMirrors {
	partial class MountedMagicMirrorsMod : Mod {
		public const int MaxTileClickDistance = 16;




		public override void PostDrawFullscreenMap( ref string mouseText ) {
			var myplayer = TmlHelpers.SafelyGetModPlayer<MMMPlayer>( Main.LocalPlayer );
			if( !myplayer.IsMapMirrorPicking ) {
				return;
			}

			int maxDistSqr = MountedMagicMirrorsMod.MaxTileClickDistance * MountedMagicMirrorsMod.MaxTileClickDistance;
			(int x, int y) mouseTile;
			Helpers.HUD.HUDMapHelpers.GetFullscreenMapTileOfScreenPosition( Main.mouseX, Main.mouseY, out mouseTile );

			int closestTileDistSqr = maxDistSqr;
			(int x, int y) closestTilePos = (0, 0);
			IEnumerable<(int, int)> discoveredMirrors = myplayer.GetDiscoveredMirrors().ToArray();

			foreach( (int tileX, int tileY) in discoveredMirrors ) {
				int distX = mouseTile.x - tileX;
				int distY = mouseTile.y - tileY;
				int distSqr = ( distX * distX ) + ( distY * distY );

				if( distSqr < closestTileDistSqr ) {
					closestTileDistSqr = distSqr;
					closestTilePos = (tileX, tileY);
				}
			}

			foreach( (int tileX, int tileY) in discoveredMirrors ) {
				bool isTarget = tileX == closestTilePos.x
						&& tileY == closestTilePos.y
						&& closestTileDistSqr < maxDistSqr;
				
				if( isTarget ) {
					myplayer.TargetMirror = (tileX, tileY);
				}

				this.DrawMirrorOnFullscreenMap( tileX, tileY, isTarget );
			}

			if( closestTilePos == (0, 0) ) {
				myplayer.TargetMirror = null;
			}
		}


		public void DrawMirrorOnFullscreenMap( int tileX, int tileY, bool isTarget ) {
			Texture2D tex = this.MirrorTex;
			float myScale = isTarget ? 0.25f : 0.125f;
			float uiScale = 5f;//Main.mapFullscreenScale;
			float scale = uiScale * myScale;

			int wldBaseX = ((tileX + 1) << 4) + 8;
			int wldBaseY = ((tileY + 1) << 4) + 8;
			var overMapData = HUDMapHelpers.GetFullMapScreenPosition( new Vector2(wldBaseX, wldBaseY) );

			if( overMapData.Item2 ) {
				Main.spriteBatch.Draw(
					texture: tex,
					position: overMapData.Item1,
					sourceRectangle: null,
					color: isTarget ? Color.Cyan : Color.White,
					rotation: 0f,
					origin: new Vector2( tex.Width/2, tex.Height/2 ),
					scale: scale,
					effects: SpriteEffects.None,
					layerDepth: 1f
				);
			}
		}
	}
}