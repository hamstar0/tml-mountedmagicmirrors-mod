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
			var mymod = (MountedMagicMirrorsMod)this.mod;
			if( !mymod.Config.GenerateMountedMirrorsForNewWorlds ) {
				return;
			}

			int mirrors;

			switch( WorldHelpers.GetSize() ) {
			default:
			case WorldSize.SubSmall:
				mirrors = mymod.Config.TinyWorldMirrors;
				break;
			case WorldSize.Small:
				mirrors = mymod.Config.SmallWorldMirrors;
				break;
			case WorldSize.Medium:
				mirrors = mymod.Config.MediumWorldMirrors;
				break;
			case WorldSize.Large:
				mirrors = mymod.Config.LargeWorldMirrors;
				break;
			case WorldSize.SuperLarge:
				mirrors = mymod.Config.HugeWorldMirrors;
				break;
			}

			tasks.Add( new MountedMirrorsGenPass(mirrors) );
		}
	}
}
