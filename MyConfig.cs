﻿using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;


namespace MountedMagicMirrors {
	public class MMMConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ServerSide;


		////

		[Header( "Debug settings" )]
		public bool DebugModeInfo { get; set; } = false;
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

		////

		[Header( "World generation settings" )]
		[DefaultValue( true )]
		public bool GenerateMountedMirrorsForNewWorlds { get; set; } = true;


		[Range( 0, 10000 )]
		[DefaultValue( 64 )]
		public int MinimumMirrorTileSpacing { get; set; } = 64;


		[Range( 0, 10000 )]
		[DefaultValue( 12 )]
		public int TinyWorldMirrors { get; set; } = 6;    // SmallWorldPortals / 2

		[Range( 0, 10000 )]
		[DefaultValue( 24 )]
		public int SmallWorldMirrors { get; set; } = 10;  // 4200 x 1200 = 5040000

		[Range( 0, 10000 )]
		[DefaultValue( 32 )]
		public int MediumWorldMirrors { get; set; } = 16; // 6400 x 1800 = 11520000

		[Range( 0, 10000 )]
		[DefaultValue( 64 )]
		public int LargeWorldMirrors { get; set; } = 24;  // 8400 x 2400 = 20160000

		[Range( 0, 10000 )]
		[DefaultValue( 96 )]
		public int HugeWorldMirrors { get; set; } = 30;
	}
}
