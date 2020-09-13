using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Terraria.ModLoader.Config;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Helpers.DotNET.Reflection;


namespace MountedMagicMirrors {
	public partial class MMMConfig : ModConfig {
		private IDictionary<string, object> Overrides = new ConcurrentDictionary<string, object>();



		////////////////

		public T Get<T>( string propName ) {
			if( !this.Overrides.TryGetValue( propName, out object val ) ) {
				if( !ReflectionHelpers.Get( this, propName, out T myval ) ) {
					throw new ModHelpersException( "Invalid property " + propName + " of type " + typeof( T ).Name );
				}
				return myval;
			}

			if( val.GetType() != typeof( T ) ) {
				throw new ModHelpersException( "Invalid type (" + typeof( T ).Name + ") of property " + propName + "." );
			}
			return (T)val;
		}

		////

		public void SetOverride<T>( string propName, T value ) {
			if( !ReflectionHelpers.Get( this, propName, out T _ ) ) {
				throw new ModHelpersException( "Invalid property " + propName + " of type " + typeof( T ).Name );
			}
			this.Overrides[propName] = value;
		}
	}
}
