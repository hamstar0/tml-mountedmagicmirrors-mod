using System;
using Terraria;


namespace MountedMagicMirrors {
	public class MountedMagicMirrorsAPI {
		public static void OnMirrorCreate( Action<int, int, Item> action ) {
			MountedMagicMirrorsMod.Instance.OnMirrorCreate.Add( action );
		}
	}
}
