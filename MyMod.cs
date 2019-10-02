using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace MountedMagicMirrors {
	public class MountedMagicMirrorsMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-mountedmagicmirrors-mod";


		////////////////

		public static MountedMagicMirrorsMod Instance { get; private set; }



		////////////////

		public string MagicMirrorsRecipeGroupName { get; private set; }

		public MountedMagicMirrorsConfig Config => this.GetConfig<MountedMagicMirrorsConfig>();



		////////////////

		public MountedMagicMirrorsMod() {
			MountedMagicMirrorsMod.Instance = this;
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
	}
}