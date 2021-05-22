using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;
using MountedMagicMirrors.Tiles;


namespace MountedMagicMirrors {
	class MountedMirrorsGenPass : GenPass {
		public static (int minTileX, int maxTileX, int minTileY, int maxTileY) GetTileBoundsForWorld() {
			int minTileX = 64;
			int maxTileX = Main.maxTilesX - minTileX;
			if( Main.maxTilesX < 64 || maxTileX <= minTileX ) {
				minTileX = 0;
				maxTileX = Main.maxTilesX;
			}

			int minTileY = (int)Main.worldSurface;
			int maxTileY = Main.maxTilesY - 220;
			if( Main.maxTilesY <= 220 || maxTileY <= minTileY ) {
				minTileY = 0;
				maxTileY = Main.maxTilesY;
			}

			return (minTileX, maxTileX, minTileY, maxTileY);
		}



		////////////////

		private TilePattern MirrorSpacePattern;
		private int NeededMirrors;

		private IDictionary<int, ISet<int>> MirrorPositions = new Dictionary<int, ISet<int>>();



		////////////////

		public MountedMirrorsGenPass( int mirrors ) : base( "PopulateMountedMirrors", 1f ) {
			this.MirrorSpacePattern = new TilePattern( new TilePatternBuilder {
				AreaFromCenter = new Rectangle( -1, -1, 3, 3 ),
				HasWall = true,
				HasLava = false,
				HasSolidProperties = false,
				IsPlatform = false,
				IsActuated = false,
				IsNotAnyOfWallType = new HashSet<int> {
					WallID.SpiderUnsafe,
					WallID.HiveUnsafe,
					WallID.CorruptionUnsafe1,
					WallID.CorruptionUnsafe2,
					WallID.CorruptionUnsafe3,
					WallID.CorruptionUnsafe4,
					WallID.CorruptGrassUnsafe,
					WallID.CrimsonUnsafe1,
					WallID.CrimsonUnsafe2,
					WallID.CrimsonUnsafe3,
					WallID.CrimsonUnsafe4,
					WallID.CrimstoneUnsafe,
					WallID.CrimsonGrassUnsafe,
				}
			} );
			this.NeededMirrors = mirrors;
		}


		////////////////

		public override void Apply( GenerationProgress progress ) {
			if( progress != null ) {
				progress.Message = "Pre-placing Mounted Magic Mirrors: %";
			}

			int minTileDist = MMMConfig.Instance.Get<int>( nameof(MMMConfig.MinimumMirrorTileSpacing) );
			if( Main.maxTilesX <= (minTileDist + 4) || Main.maxTilesY <= (minTileDist + 4) ) {
				LogHelpers.Warn( "Invalid world size." );
				return;
			}

			for( int i = 0; i < 1000; i++ ) {
				WorldGen.genRand.Next();    // Desyncs this from Wormholes?
			}

			(int TileX, int TileY)? myRandCenterTile;
			(int TileX, int TileY) randCenterTile;

			try {
				for( int i = 0; i < this.NeededMirrors; i++ ) {
					myRandCenterTile = this.GetRandomOpenMirrorableCenterTile( 1000 );
					if( !myRandCenterTile.HasValue ) {
						break;
					}

					randCenterTile = myRandCenterTile.Value;

					this.MirrorPositions.Set2D( randCenterTile.TileX, randCenterTile.TileY );

					this.SpawnMirror( randCenterTile.TileX, randCenterTile.TileY );

					progress?.Set( (float)i / (float)this.NeededMirrors );
				}
			} catch( Exception e ) {
				throw new ModHelpersException( "Mounted Mirrors world gen failed.", e );
			}
		}


		////////////////

		private (int TileX, int TileY)? GetRandomOpenMirrorableCenterTile( int maxAttempts ) {
			(int TileX, int TileY)? myRandTileCenter = null;
			int attempts = 0;

			do {
				myRandTileCenter = this.GetRandomMirrorableCenterTile( maxAttempts );
				if( !myRandTileCenter.HasValue ) {
					break;
				}

				if( !this.HasNearbyMirrors(myRandTileCenter.Value.TileX, myRandTileCenter.Value.TileY) ) {
					return myRandTileCenter;
				}
			} while( attempts++ < maxAttempts );

			return null;
		}


		////////////////

		private (int TileX, int TileY)? GetRandomMirrorableCenterTile( int maxAttempts ) {
			int attempts = 0;
			int randTileX, randTileY;
			var bounds = MountedMirrorsGenPass.GetTileBoundsForWorld();

			do {
				randTileX = WorldGen.genRand.Next( bounds.minTileX, bounds.maxTileX );
				randTileY = WorldGen.genRand.Next( bounds.minTileY, bounds.maxTileY );

				if( this.MirrorSpacePattern.Check( randTileX, randTileY ) ) {
					return (randTileX, randTileY);
				}
			} while( attempts++ < maxAttempts );

			return null;
		}


		private bool HasNearbyMirrors( int tileX, int tileY ) {
			int minTileDist = MMMConfig.Instance.Get<int>( nameof(MMMConfig.MinimumMirrorTileSpacing) );
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
			ushort mmmTile = (ushort)ModContent.TileType<MountedMagicMirrorTile>();

			WorldGen.Place3x3Wall( centerTileX - 1, centerTileY - 1, mmmTile, 0 );
			
			foreach( Action<int, int, Item> action in mymod.OnMirrorCreate ) {
				action( centerTileX - 1, centerTileY - 1, null );
			}

			if( MMMConfig.Instance.DebugModeWorldGen ) {
				LogHelpers.Log( "Placed mounted magic mirror (" + this.MirrorPositions.Count + " of " + this.NeededMirrors + ")" +
					" at " + centerTileX + "," + centerTileY +
					" (" + ( centerTileX << 4 ) + "," + ( centerTileY << 4 ) + ")"
				);
			}
		}
	}
}
