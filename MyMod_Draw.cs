using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.HUD;
using HamstarHelpers.Helpers.TModLoader;
using HamstarHelpers.Services.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;


namespace MountedMagicMirrors {
	partial class MountedMagicMirrorsMod : Mod {
		private (int TileX, int TileY) _LastMirror = (0,0);



		////////////////

		public override void PostDrawFullscreenMap( ref string mouseText ) {
			var myplayer = TmlHelpers.SafelyGetModPlayer<MMMPlayer>( Main.LocalPlayer );
			if( !myplayer.IsMapMirrorPicking ) {
				return;
			}

			bool isNowTargetting = Timers.GetTimerTickDuration( "MMMIsMapMirrorPickingNow" ) > 0;
			bool isNew = false;

			if( MMMConfig.Instance.DebugModeInfo ) {
				isNew = myplayer.TargetMirror.HasValue &&
						( myplayer.TargetMirror.Value.TileX != this._LastMirror.TileX ||
						myplayer.TargetMirror.Value.TileY != this._LastMirror.TileY );
			}

			foreach( (int tileX, int tileY) in myplayer.GetDiscoveredMirrors() ) {
				bool isTarget = myplayer.TargetMirror.HasValue &&
								myplayer.TargetMirror.Value.TileX == tileX &&
								myplayer.TargetMirror.Value.TileY == tileY;

				if( MMMConfig.Instance.DebugModeInfo ) {
					if( myplayer.TargetMirror.HasValue && isNew ) {
						this._LastMirror = myplayer.TargetMirror.Value;
						Main.NewText( "is target? " + myplayer.TargetMirror + " vs " + tileX + "," + tileY );
					}
				}

				this.DrawMirrorOnFullscreenMap( tileX, tileY, isNowTargetting && isTarget );
			}
		}


		public void DrawMirrorOnFullscreenMap( int tileX, int tileY, bool isTarget ) {
			Texture2D tex = this.MirrorTex;
			float myScale = isTarget ? 0.25f : 0.125f;
			float uiScale = Main.mapFullscreenScale;//( isZoomed ? Main.mapFullscreenScale : 1f ) * scale;

			int wldBaseX = ((tileX + 1) << 4) + 8;
			int wldBaseY = ((tileY + 1) << 4) + 8;
			int wldX = wldBaseX - (int)( (float)tex.Width * 8f * myScale );
			int wldY = wldBaseY - (int)( (float)tex.Height * 8f * myScale );
			int wid = (int)( (float)tex.Width * 16f * myScale );
			int hei = (int)( (float)tex.Height * 16f * myScale );

			var wldRect = new Rectangle( wldX, wldY, wid, hei );
			var overMapData = HUDMapHelpers.GetFullMapScreenPosition( wldRect );

			//DebugHelpers.Print( "mapdraw", "tileX:"+tileX+", tileY:"+tileY+
			//	", plrpos: " + (int)Main.LocalPlayer.Center.X+":"+(int)Main.LocalPlayer.Center.Y+
			//	", wldRect:" + wldRect+", overMapData:" + overMapData.Item1, 20 );
			if( overMapData.Item2 ) {
				Main.spriteBatch.Draw(
					texture: tex,
					position: overMapData.Item1,
					sourceRectangle: null,
					color: isTarget ? Color.Cyan : Color.White,
					rotation: 0f,
					origin: default( Vector2 ),
					scale: uiScale * myScale,
					effects: SpriteEffects.None,
					layerDepth: 1f
				);
			}
		}
	}
}