using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using HamstarHelpers.Helpers.TModLoader;
using MountedMagicMirrors.Items;


namespace MountedMagicMirrors.Tiles {
	public class MountedMagicMirrorTile : ModTile {
		public override void SetDefaults() {
			Main.tileFrameImportant[ this.Type ] = true;
			Main.tileLavaDeath[ this.Type ] = true;
			Main.tileLighted[ this.Type ] = true;

			TileObjectData.newTile.CopyFrom( TileObjectData.Style3x3Wall );
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.addTile( this.Type );

			ModTranslation name = this.CreateMapEntryName();
			name.SetDefault( "Mounted Magic Mirror" );

			this.AddMapEntry( new Color(0, 255, 255), name, (string currTileName, int tileX, int tileY) => currTileName );

			this.dustType = 7;
			this.disableSmartCursor = true;
		}


		////////////////

		public override void ModifyLight( int i, int j, ref float r, ref float g, ref float b ) {
			r = 1f;
			g = 1f;
			b = 1f;
		}


		////////////////

		public override bool CanPlace( int i, int j ) {
			Tile tile = Framing.GetTileSafely( i, j );
			return tile.wall != WallID.LihzahrdBrickUnsafe || NPC.downedGolemBoss;
		}

		public override void PlaceInWorld( int i, int j, Item item ) {
			foreach( Action<int, int, Item> action in MountedMagicMirrorsMod.Instance.OnMirrorCreate ) {
				action( i, j, item );
			}
		}


		////////////////

		public override bool CanKillTile( int i, int j, ref bool blockDamaged ) {
			return MMMConfig.Instance.Get<bool>( nameof( MMMConfig.IsMountedMagicMirrorBreakable) );
		}

		public override void KillTile( int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem ) {
			if( !MMMConfig.Instance.Get<bool>( nameof(MMMConfig.IsMountedMagicMirrorBreakable) ) ) {
				fail = true;
				effectOnly = false;
				noItem = true;
			}
		}

		public override void KillMultiTile( int i, int j, int frameX, int frameY ) {
			var config = MMMConfig.Instance;

			if( !config.Get<bool>( nameof(MMMConfig.IsMountedMagicMirrorBreakable) ) ) {
				for( int k=i; k<i+3; k++ ) {
					for( int l=j; l<j+3; l++ ) {
						if( Main.tile[k, l].wall <= 0 ) {
							Main.tile[k, l].wall = 2;
						}
					}
				}
			} else {
				if( config.Get<bool>( nameof(MMMConfig.MountedMagicMirrorDropsItem) ) ) {
					Item.NewItem( i * 16, j * 16, 64, 32, ModContent.ItemType<MountableMagicMirrorTileItem>() );
				}
			}
		}


		////////////////

		public override void MouseOver( int i, int j ) {
			Main.LocalPlayer.showItemIcon = true;
			Main.LocalPlayer.showItemIcon2 = ModContent.ItemType<MountableMagicMirrorTileItem>();

			var myplayer = TmlHelpers.SafelyGetModPlayer<MMMPlayer>( Main.LocalPlayer );
			if( myplayer.AddDiscoveredMirror( i, j ) ) {
				Main.NewText( "Mirror located!", Color.Lime );
			}
		}


		public override bool NewRightClick( int i, int j ) {
			Main.playerInventory = false;
			Main.resetMapFull = true;
			Main.mapEnabled = true;
			Main.mapFullscreen = true;

			var myplayer = TmlHelpers.SafelyGetModPlayer<MMMPlayer>( Main.LocalPlayer );
			myplayer.BeginMapMirrorPicking();

			return true;
		}
	}
}
