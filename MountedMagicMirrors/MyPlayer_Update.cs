using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Libraries.DotNET.Extensions;


namespace MountedMagicMirrors {
	partial class MMMPlayer : ModPlayer {
		private void UpdateMapMirrorPicking() {
			if( !Main.mapFullscreen ) {
				this.EndMapMirrorPicking();
				return;
			}

			bool isLeftClick = Main.mouseLeft && Main.mouseLeftRelease;
			bool isRightClick = Main.mouseRight && Main.mouseRightRelease;

			if( isLeftClick || isRightClick ) {
				if( !this.ClickSafetyLock ) {
					if( this.TargetMirror.HasValue ) {
						(int TileX, int TileY) target = this.TargetMirror.Value;
						
						if( this.CurrentWorldDiscoveredMirrorTiles?.Contains2D(target.TileX, target.TileY) ?? false ) {
							var config = MMMConfig.Instance;

							if( isRightClick && config.Get<bool>( nameof(MMMConfig.RightClickToUndiscover) ) ) {
								Main.NewText( "Mirror removed.", Color.Yellow );
								this.CurrentWorldDiscoveredMirrorTiles.Remove2D(target.TileX, target.TileY);
							} else {
								if( this.TeleportToMirror( target.TileX, target.TileY ) ) {
									this.EndMapMirrorPicking();
								}
							}
						}
					}
				}
			} else {
				//if( (!Main.mouseLeft && !Main.mouseLeftRelease) && (!Main.mouseRight && !Main.mouseRightRelease) ) {
				this.ClickSafetyLock = false;
				//}
			}
		}


		////////////////

		public void BeginMapMirrorPicking() {
			this.IsMapMirrorPicking = true;
			this.ClickSafetyLock = true;
		}

		public void EndMapMirrorPicking() {
			this.IsMapMirrorPicking = false;
			this.ClickSafetyLock = false;
			Main.mapFullscreen = false;
		}
	}
}
