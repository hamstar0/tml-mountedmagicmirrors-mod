using HamstarHelpers.Helpers.DotNET.Extensions;
using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;
using MountedMagicMirrors.Tiles;
using HamstarHelpers.Helpers.Debug;


namespace MountedMagicMirrors {
	class MountedMirrorGenPass : GenPass {
		private TilePattern MirrorSpacePattern;
		private int NeededMirrors;

		private IDictionary<int, ISet<int>> MirrorPositions = new Dictionary<int, ISet<int>>();



		////////////////

		public MountedMirrorGenPass( int mirrors ) : base( "PopulateMountedMirrors", 1f ) {
			this.MirrorSpacePattern = new TilePattern( new TilePatternBuilder {
				AreaFromCenter = new Rectangle( -1, -1, 3, 3 ),
				HasWall = true,
				HasLava = false,
				IsSolid = false,
				IsPlatform = false,
				IsActuated = false,
			} );
			this.NeededMirrors = mirrors;
		}


		////////////////

		public override void Apply( GenerationProgress progress ) {
			(int TileX, int TileY) randCenterTile;

			for( int i = 0; i < this.NeededMirrors; i++ ) {
				if( !this.GetRandomOpenMirrorableCenterTile(out randCenterTile, 1000) ) {
					break;
				}

				this.MirrorPositions.Set2D( randCenterTile.TileX, randCenterTile.TileY );

				this.SpawnMirror( randCenterTile.TileX, randCenterTile.TileY );
			}
		}


		////////////////

		private bool GetRandomOpenMirrorableCenterTile( out (int TileX, int TileY) randTileCenter, int maxAttempts ) {
			int attempts = 0;

			do {
				randTileCenter = this.GetRandomMirrorableCenterTile( maxAttempts );

				if( !this.HasNearbyMirrors(randTileCenter.TileX, randTileCenter.TileY) ) {
					return true;
				}
			} while( attempts++ < maxAttempts );

			return false;
		}


		private (int TileX, int TileY) GetRandomMirrorableCenterTile( int maxAttempts ) {
			int attempts = 0;
			int randTileX, randTileY;

			do {
				randTileX = Main.rand.Next( 64, Main.maxTilesX - 64 );
				randTileY = Main.rand.Next( (int)Main.worldSurface, Main.maxTilesY - 220 );

				if( this.MirrorSpacePattern.Check( randTileX, randTileY ) ) {
					break;
				}
			} while( attempts++ < maxAttempts );

			return (randTileX, randTileY);
		}


		private bool HasNearbyMirrors( int tileX, int tileY ) {
			int minTileDist = MountedMagicMirrorsMod.Instance.Config.MinimumMirrorTileSpacing;
			int minTileDistSqt = minTileDist * minTileDist;

			foreach( (int otherTileX, ISet<int> otherTileYs) in this.MirrorPositions ) {
				foreach( int otherTileY in otherTileYs ) {
					int xDist = otherTileX - tileX;
					int yDist = otherTileY - tileY;
					int xDistSqr = xDist * xDist;
					int yDistSqr = yDist * yDist;

					if( (xDistSqr + yDistSqr) < minTileDistSqt ) {
						return true;
					}
				}
			}

			return false;
		}


		////////////////

		private void SpawnMirror( int centerTileX, int centerTileY ) {
			var mymod = MountedMagicMirrorsMod.Instance;
			ushort mmmTile = (ushort)mymod.TileType<MountedMagicMirrorTile>();

			WorldGen.Place3x3Wall( centerTileX - 1, centerTileY - 1, mmmTile, 0 );

			LogHelpers.Log( "Placed mounted magic mirror ("+this.MirrorPositions.Count+" of "+this.NeededMirrors+")" +
				" at " + centerTileX + "," + centerTileY +
				" (" + (centerTileX << 4) + "," + (centerTileY << 4) + ")"
			);
		}
	}




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

			tasks.Add( new MountedMirrorGenPass(mirrors) );
		}
	}
}
