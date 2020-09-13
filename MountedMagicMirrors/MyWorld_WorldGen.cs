using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.World;


namespace MountedMagicMirrors {
	partial class MMMWorld : ModWorld {
		public override void ModifyWorldGenTasks( List<GenPass> tasks, ref float totalWeight ) {
			var config = MMMConfig.Instance;

			if( !config.Get<bool>( nameof(MMMConfig.GenerateMountedMirrorsForNewWorlds) ) ) {
				return;
			}

			int mirrors;

			switch( WorldHelpers.GetSize() ) {
			default:
			case WorldSize.SubSmall:
				mirrors = config.Get<int>( nameof(MMMConfig.TinyWorldMirrors) );
				break;
			case WorldSize.Small:
				mirrors = config.Get<int>( nameof(MMMConfig.SmallWorldMirrors) );
				break;
			case WorldSize.Medium:
				mirrors = config.Get<int>( nameof(MMMConfig.MediumWorldMirrors) );
				break;
			case WorldSize.Large:
				mirrors = config.Get<int>( nameof(MMMConfig.LargeWorldMirrors) );
				break;
			case WorldSize.SuperLarge:
				mirrors = config.Get<int>( nameof(MMMConfig.HugeWorldMirrors) );
				break;
			}

			tasks.Add( new MountedMirrorsGenPass( mirrors ) );
		}
	}
}
