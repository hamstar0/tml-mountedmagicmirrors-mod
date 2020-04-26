using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.TModLoader.Mods;
using HamstarHelpers.Services.Hooks.LoadHooks;
using HamstarHelpers.Services.Network;
using MountedMagicMirrors.Tiles;


namespace MountedMagicMirrors {
	partial class MountedMagicMirrorsMod : Mod {
		public static MountedMagicMirrorsMod Instance { get; private set; }


		////////////////

		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-mountedmagicmirrors-mod";



		////////////////

		public TilePattern MMMTilePattern { get; private set; }

		public string MagicMirrorsRecipeGroupName { get; private set; }

		internal IList<Action<int, int, Item>> OnMirrorCreate = new List<Action<int, int, Item>>();



		////////////////

		private Texture2D MirrorTex;



		////////////////

		public MountedMagicMirrorsMod() {
			MountedMagicMirrorsMod.Instance = this;
		}

		public override void Load() {
			this.MMMTilePattern = new TilePattern( new TilePatternBuilder {
				IsActive = true,
				IsAnyOfType = new HashSet<int> { ModContent.TileType<MountedMagicMirrorTile>() }
			} );
		}

		public override void PostSetupContent() {
			if( Main.netMode != 2 && !Main.dedServ ) {
				this.MirrorTex = this.GetTexture( "Items/MountableMagicMirrorTileItem" );
			}

			//

			void onTileSectionPacketGet( int tileX, int tileY, int width, int height, BinaryReader data ) {
				var myworld = ModContent.GetInstance<MMMWorld>();
				myworld.RegisterTileSectionTile( tileX, tileY );
			}

			//

			Client.SubscribeToTileSectionPackets( onTileSectionPacketGet );

			LoadHooks.AddPostWorldUnloadEachHook( () => {
				var myworld = ModContent.GetInstance<MMMWorld>();
				myworld?.UnregisterTileSections();
			} );
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


		////////////////

		public override object Call( params object[] args ) {
			return ModBoilerplateHelpers.HandleModCall( typeof(MountedMagicMirrorsAPI), args );
		}
	}
}