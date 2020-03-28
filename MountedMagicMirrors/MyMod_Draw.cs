using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.TModLoader;
using HamstarHelpers.Helpers.HUD;


namespace MountedMagicMirrors {
	partial class MountedMagicMirrorsMod : Mod {
		public const int MaxTileClickDistance = 16;



		////////////////

		public override void PostDrawFullscreenMap( ref string mouseText ) {
			var myplayer = TmlHelpers.SafelyGetModPlayer<MMMPlayer>( Main.LocalPlayer );
			if( !myplayer.IsMapMirrorPicking ) {
				return;
			}

			int maxDistSqr = MountedMagicMirrorsMod.MaxTileClickDistance * MountedMagicMirrorsMod.MaxTileClickDistance;

			int mouseX = Main.mouseX - (Main.screenWidth / 2);
			mouseX = (int)((float)mouseX * Main.UIScale);
			mouseX = (Main.screenWidth / 2) + mouseX;
			int mouseY = Main.mouseY - (Main.screenHeight / 2);
			mouseY = (int)((float)mouseY * Main.UIScale);
			mouseY = (Main.screenHeight / 2) + mouseY;

			(int x, int y) mouseTile;
			HUDMapHelpers.GetFullscreenMapTileOfScreenPosition( mouseX, mouseY, out mouseTile );

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
			var wldPos = new Vector2(wldBaseX, wldBaseY);

			Tuple<Vector2, bool> overMapData = HUDMapHelpers.GetFullMapScreenPosition( wldPos );

			if( overMapData.Item2 ) {
				Vector2 scrPos = overMapData.Item1;

				Main.spriteBatch.Draw(
					texture: tex,
					position: scrPos,
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