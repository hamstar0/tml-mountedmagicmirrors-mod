using System;
using Terraria;
using Terraria.ID;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Classes.PlayerData;


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
