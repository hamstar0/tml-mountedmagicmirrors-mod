using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.Debug;
using Microsoft.Xna.Framework.Graphics;
using MountedMagicMirrors.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace MountedMagicMirrors {
	partial class MountedMagicMirrorsMod : Mod {
		public static MountedMagicMirrorsMod Instance { get; private set; }

		////////////////


		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-mountedmagicmirrors-mod";



		////////////////

		public TilePattern MMMTilePattern { get; private set; }

		public string MagicMirrorsRecipeGroupName { get; private set; }

		public MMMConfig Config => this.GetConfig<MMMConfig>();


		////////////////

		private Texture2D MirrorTex;



		////////////////

		public MountedMagicMirrorsMod() {
			MountedMagicMirrorsMod.Instance = this;
		}

		public override void Load() {
			this.MMMTilePattern = new TilePattern( new TilePatternBuilder {
				IsAnyOfType = new HashSet<int> { this.TileType<MountedMagicMirrorTile>() }
			} );
		}

		public override void PostSetupContent() {
			if( Main.netMode != 2 && !Main.dedServ ) {
				this.MirrorTex = this.GetTexture( "Items/MountableMagicMirrorTileItem" );
			}
		}

		public override void Unload() {
			MountedMagicMirrorsMod.Instance = null;
		}


		////

		public override void AddRecipeGroups() {
			string grpName = Language.GetTextValue("LegacyMisc.37")+" "+Lang.GetItemNameValue(ItemID.MagicMirror);

			var group = new RecipeGroup( () => grpName,
				(int)ItemID.MagicMirror, (int)ItemID.IceMirror
			);

			this.MagicMirrorsRecipeGroupName = this.GetType().Name + ":AnyMagicMirror";
			RecipeGroup.RegisterGroup( this.MagicMirrorsRecipeGroupName, group );
		}
	}
}