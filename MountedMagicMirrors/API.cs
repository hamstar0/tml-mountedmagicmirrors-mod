using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;


namespace MountedMagicMirrors {
	public class MountedMagicMirrorsAPI {
		public static void OnMirrorCreate( Action<int, int, Item> action ) {
			MountedMagicMirrorsMod.Instance.OnMirrorCreate.Add( action );
		}


		public static IList<(int tileX, int tileY)> GetDiscoveredMirrors( Player player ) {
			var myplayer = player.GetModPlayer<MMMPlayer>();

			IEnumerable<(int, int)> tiles = myplayer.CurrentWorldDiscoveredMirrorTiles?.SelectMany(
				kv => kv.Value.Select( y => (kv.Key, y) )
			);
			return tiles?.ToList() ?? new List<(int, int)>();
		}
	}
}
