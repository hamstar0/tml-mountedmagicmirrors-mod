using HamstarHelpers.Helpers.World;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;
using HamstarHelpers.Helpers.Debug;


namespace MountedMagicMirrors {
	class MMMWorld : ModWorld {
		public override void ModifyWorldGenTasks( List<GenPass> tasks, ref float totalWeight ) {
			if( !MMMConfig.Instance.GenerateMountedMirrorsForNewWorlds ) {
				return;
			}

			int mirrors;

			switch( WorldHelpers.GetSize() ) {
			default:
			case WorldSize.SubSmall:
				mirrors = MMMConfig.Instance.TinyWorldMirrors;
				break;
			case WorldSize.Small:
				mirrors = MMMConfig.Instance.SmallWorldMirrors;
				break;
			case WorldSize.Medium:
				mirrors = MMMConfig.Instance.MediumWorldMirrors;
				break;
			case WorldSize.Large:
				mirrors = MMMConfig.Instance.LargeWorldMirrors;
				break;
			case WorldSize.SuperLarge:
				mirrors = MMMConfig.Instance.HugeWorldMirrors;
				break;
			}

			tasks.Add( new MountedMirrorsGenPass(mirrors) );
		}
	}
}
