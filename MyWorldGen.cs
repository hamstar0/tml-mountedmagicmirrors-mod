using HamstarHelpers.Helpers.DotNET.Extensions;
using HamstarHelpers.Classes.Tiles.TilePattern;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;
using MountedMagicMirrors.Tiles;
using HamstarHelpers.Helpers.Debug;


namespace MountedMagicMirrors {
	class MountedMirrorsGenPass : GenPass {
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
			} );
			this.NeededMirrors = mirrors;
		}


		////////////////

		public override void Apply( GenerationProgress progress ) {
			(int TileX, int TileY) randCenterTile;
			float stepWeight = 1f / (float)this.NeededMirrors;

			if( progress != null ) {
				progress.Message = "Pre-placing Mounted Magic Mirrors: %";
			}

			for( int i = 0; i < this.NeededMirrors; i++ ) {
				progress?.Set( stepWeight * (float)i );

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
			int minTileDist = MountedMagicMirrorsMod.Config.MinimumMirrorTileSpacing;
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

			LogHelpers.Log( "Placed mounted magic mirror ("+this.MirrorPositions.Count+" of "+this.NeededMirrors+")" +
				" at " + centerTileX + "," + centerTileY +
				" (" + (centerTileX << 4) + "," + (centerTileY << 4) + ")"
			);
		}
	}
}
