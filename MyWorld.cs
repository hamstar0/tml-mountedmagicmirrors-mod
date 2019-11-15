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
			if( !MountedMagicMirrorsMod.Config.GenerateMountedMirrorsForNewWorlds ) {
				return;
			}

			int mirrors;

			switch( WorldHelpers.GetSize() ) {
			default:
			case WorldSize.SubSmall:
				mirrors = MountedMagicMirrorsMod.Config.TinyWorldMirrors;
				break;
			case WorldSize.Small:
				mirrors = MountedMagicMirrorsMod.Config.SmallWorldMirrors;
				break;
			case WorldSize.Medium:
				mirrors = MountedMagicMirrorsMod.Config.MediumWorldMirrors;
				break;
			case WorldSize.Large:
				mirrors = MountedMagicMirrorsMod.Config.LargeWorldMirrors;
				break;
			case WorldSize.SuperLarge:
				mirrors = MountedMagicMirrorsMod.Config.HugeWorldMirrors;
				break;
			}

			tasks.Add( new MountedMirrorsGenPass(mirrors) );
		}
	}
}
