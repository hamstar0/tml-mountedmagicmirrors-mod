using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;


namespace MountedMagicMirrors {
	public class MountedMagicMirrorsConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ServerSide;


		////

		public bool DebugModeInfo { get; set; } = false;

		////

		[Header( "\n " )]
		[DefaultValue( true )]
		public bool EnableMountedMagicMirrorRecipe { get; set; } = true;

		[DefaultValue( true )]
		public bool IsMountedMagicMirrorBreakable { get; set; } = true;

		[DefaultValue( true )]
		public bool MountedMagicMirrorDropsItem { get; set; } = true;
	}
}
