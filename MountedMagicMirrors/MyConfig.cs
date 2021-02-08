using System;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;


namespace MountedMagicMirrors {
	public partial class MMMConfig : ModConfig {
		public static MMMConfig Instance => ModContent.GetInstance<MMMConfig>();



		////////////////

		public override ConfigScope Mode => ConfigScope.ServerSide;


		////

		[Header( "Debug settings" )]
		public bool DebugModeInfo { get; set; } = false;
		public bool DebugModeWorldGen { get; set; } = false;
		public bool DebugModePosition { get; set; } = false;

		////

		[Header( "Mounted mirror settings" )]
		[DefaultValue( true )]
		public bool EnableMountedMagicMirrorRecipe { get; set; } = true;

		[DefaultValue( false )]
		[ReloadRequired]
		public bool EnableMountedMagicMirrorEasyModeRecipe { get; set; } = false;


		[DefaultValue( true )]
		public bool IsMountedMagicMirrorBreakable { get; set; } = true;


		[DefaultValue( true )]
		public bool MountedMagicMirrorDropsItem { get; set; } = true;

		[DefaultValue( true )]
		public bool RightClickToUndiscover { get; set; } = true;

		////

		[Header( "World generation settings" )]
		[DefaultValue( true )]
		public bool GenerateMountedMirrorsForNewWorlds { get; set; } = true;


		[Range( 0, 10000 )]
		[DefaultValue( 128 )]
		public int MinimumMirrorTileSpacing { get; set; } = 128;


		[Range( 0, 1000 )]
		[DefaultValue( 16 )]
		public int TinyWorldMirrors { get; set; } = 20;    // SmallWorldPortals / 2

		[Range( 0, 1000 )]
		[DefaultValue( 28 )]
		public int SmallWorldMirrors { get; set; } = 28;  // 4200 x 1200 = 5040000

		[Range( 0, 1000 )]
		[DefaultValue( 54 )]
		public int MediumWorldMirrors { get; set; } = 54; // 6400 x 1800 = 11520000

		[Range( 0, 1000 )]
		[DefaultValue( 108 )]
		public int LargeWorldMirrors { get; set; } = 108;  // 8400 x 2400 = 20160000

		[Range( 0, 10000 )]
		[DefaultValue( 160 )]
		public int HugeWorldMirrors { get; set; } = 160;
	}
}
