using MountedMagicMirrors.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace MountedMagicMirrors.Items {
	public class MountableMagicMirrorTileItem : ModItem {
		public const int Width = 30;
		public const int Height = 30;



		////////////////

		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault( "Mountable Magic Mirror" );
			this.Tooltip.SetDefault(
				"Mount on a wall to create a fast travel point."+"\n"+
				"Right-click wall mount to fast travel."
			);
		}

		public override void SetDefaults() {
			this.item.width = MountableMagicMirrorTileItem.Width;
			this.item.height = MountableMagicMirrorTileItem.Height;
			this.item.value = Item.buyPrice( 0, 10, 0, 0 );
			this.item.maxStack = 99;
			this.item.useTurn = true;
			this.item.autoReuse = true;
			this.item.useAnimation = 15;
			this.item.useTime = 10;
			this.item.useStyle = 1;
			this.item.consumable = true;
			this.item.createTile = ModContent.TileType<MountedMagicMirrorTile>();
		}


		////

		public override void AddRecipes() {
			var mymod = (MountedMagicMirrorsMod)this.mod;
			var recipe = new MountedMagicMirrorTileItemRecipe( mymod, this );
			recipe.AddRecipe();
		}
	}




	class MountedMagicMirrorTileItemRecipe : ModRecipe {
		public MountedMagicMirrorTileItemRecipe( MountedMagicMirrorsMod mymod, MountableMagicMirrorTileItem myMirror )
				: base( mymod ) {
			this.AddTile( TileID.TinkerersWorkbench );

			this.AddRecipeGroup( mymod.MagicMirrorsRecipeGroupName, 1 );
			this.AddIngredient( ItemID.Wire, 50 );

			if( mymod.Config.EnableMountedMagicMirrorEasyModeRecipe ) {
				this.AddIngredient( ItemID.LargeRuby, 1 );
			} else {
				this.AddIngredient( ItemID.Teleporter, 1 );
				this.AddIngredient( ItemID.SoulofFlight, 10 );
			}

			this.SetResult( myMirror );
		}


		public override bool RecipeAvailable() {
			return ( (MountedMagicMirrorsMod)this.mod ).Config.EnableMountedMagicMirrorRecipe;
		}
	}
}