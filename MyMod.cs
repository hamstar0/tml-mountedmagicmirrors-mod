using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.HUD;
using HamstarHelpers.Helpers.TModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace MountedMagicMirrors {
	partial class MountedMagicMirrorsMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-mountedmagicmirrors-mod";


		////////////////

		public static MountedMagicMirrorsMod Instance { get; private set; }



		////////////////

		public string MagicMirrorsRecipeGroupName { get; private set; }

		public MMMConfig Config => this.GetConfig<MMMConfig>();


		////////////////

		private Texture2D MirrorTex;



		////////////////

		public MountedMagicMirrorsMod() {
			MountedMagicMirrorsMod.Instance = this;
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
			string mmName = Language.GetTextValue("LegacyMisc.37")+" "+Lang.GetItemNameValue(ItemID.MagicMirror);

			var group = new RecipeGroup( () => mmName,
				(int)ItemID.MagicMirror, (int)ItemID.IceMirror
			);

			this.MagicMirrorsRecipeGroupName = this.GetType().Name + ":AnyMagicMirror";
			RecipeGroup.RegisterGroup( this.MagicMirrorsRecipeGroupName, group );
		}


		////////////////

		public override void PostDrawFullscreenMap( ref string mouseText ) {
			var myplayer = TmlHelpers.SafelyGetModPlayer<MMMPlayer>( Main.LocalPlayer );

			foreach( (int tileX, int tileY) in myplayer.GetDiscoveredMirrors() ) {
				this.DrawMirrorOnFullscreenMap( tileY, tileY );
			}
		}


		public void DrawMirrorOnFullscreenMap( int tileX, int tileY ) {
			Texture2D tex = this.MirrorTex;
			float scale = Main.mapFullscreenScale;//( isZoomed ? Main.mapFullscreenScale : 1f ) * scale;
			float myScale = 1f;

			int wldX = (tileX << 4) - (int)((float)tex.Width * 8f * myScale);
			int wldY = (tileY << 4) - (int)((float)tex.Height * 8f * myScale);
			int wid = (int)( (float)tex.Width / myScale );
			int hei = (int)( (float)tex.Height / myScale );

			var mapRectOrigin = new Rectangle( wldX, wldY, wid, hei );
			var overMapData = HUDMapHelpers.GetFullMapScreenPosition( mapRectOrigin );

			if( overMapData.Item2 ) {
				Main.spriteBatch.Draw(
					tex,
					overMapData.Item1,
					null,
					Color.White,
					0f,
					default(Vector2),
					scale,
					SpriteEffects.None,
					1f
				);
			}
		}
	}
}