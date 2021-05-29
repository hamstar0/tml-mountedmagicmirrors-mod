using System;
using Terraria;
using Terraria.ID;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Classes.PlayerData;


namespace MountedMagicMirrors {
	partial class MMMCustomPlayer : CustomPlayerData {
		protected override void OnEnter( bool isCurrentPlayer, object data ) {
			if( isCurrentPlayer && Main.netMode == NetmodeID.MultiplayerClient ) {
				var myplayer = this.Player.GetModPlayer<MMMPlayer>();

				myplayer.OnCurrentClientEnter();
			}
		}
	}
}
