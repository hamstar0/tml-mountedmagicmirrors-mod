using System;
using Terraria.ModLoader;
using Terraria;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;


namespace MountedMagicMirrors {
	partial class MMMPlayer : ModPlayer {
		public override void PreUpdate() {
			if( this.player.whoAmI == Main.myPlayer ) {
				if( MMMConfig.Instance.DebugModePosition ) {
					DebugHelpers.Print( "WhereAmI", (this.player.Center/16).ToShortString()+" ("+this.player.Center.ToString()+")" );
				}

				if( this.IsMapMirrorPicking ) {
					this.UpdateMapMirrorPicking();
				}
			}
		}

		////

		private void UpdateMapMirrorPicking() {
			if( !Main.mapFullscreen ) {
				this.EndMapMirrorPicking();
				return;
			}

			bool isClick =	(Main.mouseRight && Main.mouseRightRelease) ||
							(Main.mouseLeft && Main.mouseLeftRelease);

			if( isClick ) {
				if( !this.ClickSafetyLock ) {
					if( this.TargetMirror.HasValue ) {
						(int TileX, int TileY) target = this.TargetMirror.Value;

						if( this.CurrentWorldDiscoveredMirrorTiles.Contains2D(target.TileX, target.TileY) ) {
							if( this.TeleportToMirror( target.TileX, target.TileY ) ) {
								this.EndMapMirrorPicking();
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
