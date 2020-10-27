using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Terraria;
using HamstarHelpers.Services.Network.NetIO;
using HamstarHelpers.Services.Network.NetIO.PayloadTypes;
using MountedMagicMirrors.DataStructures;


namespace MountedMagicMirrors.Net {
	[Serializable]
	class PlayerDataProtocol : NetIOBroadcastPayload {
		public static void Broadcast( IDictionary<string, DiscoveredMirrors> mirrors ) {
			var protocol = new PlayerDataProtocol( Main.myPlayer, mirrors );
			NetIO.Broadcast( protocol );
		}

		/*public static void SendToClients(
					int toWho,
					int fromWho,
					IDictionary<string, DiscoveredMirrors> mirrors ) {
			var protocol = new PlayerDataProtocol( fromWho, mirrors );
			NetIO.SendToClients( protocol, toWho, fromWho );
		}*/



		////////////////

		public int PlayerWho;
		public Dictionary<string, Dictionary<int, HashSet<int>>> DiscoveredMirrorTilesPerWorld;



		////////////////
		
		private PlayerDataProtocol() { }

		private PlayerDataProtocol( int playerWho, IDictionary<string, DiscoveredMirrors> mirrors ) {
			this.PlayerWho = playerWho;
			this.DiscoveredMirrorTilesPerWorld = mirrors.ToDictionary(
				(kv) => kv.Key,
				(kv) => kv.Value.ToDictionary(
					kv2 => kv2.Key,
					kv2 => kv2.Value as HashSet<int>
				)
			);
		}


		////////////////

		public override bool ReceiveOnServerBeforeRebroadcast( int fromWho ) {
			this.Receive();
			return true;
		}

		public override void ReceiveBroadcastOnClient() {
			this.Receive();
		}

		////

		private void Receive() {
			Player plr = Main.player[this.PlayerWho];
			var myplayer = plr.GetModPlayer<MMMPlayer>();

			IDictionary<string, DiscoveredMirrors> mirrors = this.DiscoveredMirrorTilesPerWorld.ToDictionary(
				( kv ) => kv.Key,
				( kv ) => new DiscoveredMirrors( kv.Value.ToDictionary(
					kv2 => kv2.Key,
					kv2 => kv2.Value as ISet<int>
				) )
			);
			var concurMirrors = new ConcurrentDictionary<string, DiscoveredMirrors>( mirrors );

			myplayer.SetDiscoveredMirrorsFromNetwork( concurMirrors );
		}
	}
}
