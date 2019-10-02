using HamstarHelpers.Helpers.TModLoader;
using Microsoft.Xna.Framework;
using MountedMagicMirrors.Items;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;


namespace MountedMagicMirrors.Tiles {
	class MountedMagicMirrorTile : ModTile {
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

			this.AddMapEntry( new Color(0, 255, 255), name, (string currTileName, int tileX, int tileY) => {
				var myplayer = TmlHelpers.SafelyGetModPlayer<MMMPlayer>( Main.LocalPlayer );
				myplayer.SetTargetMirror( tileX, tileY );
				return currTileName;
			} );

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

		public override bool CanKillTile( int i, int j, ref bool blockDamaged ) {
			var mymod = (MountedMagicMirrorsMod)this.mod;
			return mymod.Config.IsMountedMagicMirrorBreakable;
		}

		public override void KillTile( int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem ) {
			var mymod = (MountedMagicMirrorsMod)this.mod;

			if( !mymod.Config.IsMountedMagicMirrorBreakable ) {
				fail = true;
				effectOnly = false;
				noItem = true;
			}
		}

		public override void KillMultiTile( int i, int j, int frameX, int frameY ) {
			var mymod = (MountedMagicMirrorsMod)this.mod;

			if( !mymod.Config.IsMountedMagicMirrorBreakable ) {
				for( int k=i; k<i+3; k++ ) {
					for( int l=j; l<j+3; l++ ) {
						if( Main.tile[k, l].wall <= 0 ) {
							Main.tile[k, l].wall = 2;
						}
					}
				}
			} else {
				if( mymod.Config.MountedMagicMirrorDropsItem ) {
					Item.NewItem( i * 16, j * 16, 64, 32, this.mod.ItemType<MountableMagicMirrorTileItem>() );
				}
			}
		}


		////////////////

		public override void MouseOver( int i, int j ) {
			Main.LocalPlayer.showItemIcon = true;
			Main.LocalPlayer.showItemIcon2 = this.mod.ItemType<MountableMagicMirrorTileItem>();

			var myplayer = TmlHelpers.SafelyGetModPlayer<MMMPlayer>( Main.LocalPlayer );
			if( myplayer.AddDiscoveredMirror( i, j ) ) {
				Main.NewText( "Mirror located!", Color.Lime );
			}
		}


		public override void RightClick( int i, int j ) {
			Main.resetMapFull = true;
			Main.mapEnabled = true;
			Main.mapFullscreen = true;

			var myplayer = TmlHelpers.SafelyGetModPlayer<MMMPlayer>( Main.LocalPlayer );
			myplayer.BeginFastTravelChoice();
		}
	}
}
