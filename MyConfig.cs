using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;


namespace MountedMagicMirrors {
	public class MMMConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ServerSide;


		////

		[Header( "Debug settings" )]
		public bool DebugModeInfo { get; set; } = false;

		////

		[Header( "\n Mounted mirror settings" )]
		[DefaultValue( true )]
		public bool EnableMountedMagicMirrorRecipe { get; set; } = true;
		[DefaultValue( false )]
		[ReloadRequired]
		public bool EnableMountedMagicMirrorEasyModeRecipe { get; set; } = false;

		[DefaultValue( true )]
		public bool IsMountedMagicMirrorBreakable { get; set; } = true;

		[DefaultValue( true )]
		public bool MountedMagicMirrorDropsItem { get; set; } = true;


		[Header( "World generation settings" )]
		[DefaultValue( true )]
		public bool GenerateMountedMirrorsForNewWorlds = true;

		[DefaultValue( 64 )]
		public int MinimumMirrorTileSpacing = 64;

		[DefaultValue( 4 )]
		public int TinyWorldMirrors = 4;    // SmallWorldPortals / 2

		[DefaultValue( 8 )]
		public int SmallWorldMirrors = 8;  // 4200 x 1200 = 5040000

		[DefaultValue( 14 )]
		public int MediumWorldMirrors = 14; // 6400 x 1800 = 11520000

		[DefaultValue( 20 )]
		public int LargeWorldMirrors = 20;  // 8400 x 2400 = 20160000

		[DefaultValue( 27 )]
		public int HugeWorldMirrors = 27;
	}
}
