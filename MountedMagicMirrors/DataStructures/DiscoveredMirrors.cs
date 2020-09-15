using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Terraria;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;


namespace MountedMagicMirrors.DataStructures {
	class DiscoveredMirrors : ConcurrentDictionary<int, ISet<int>> {
		public static bool WorldMirrorsEquals(
					IDictionary<string, DiscoveredMirrors> world1,
					IDictionary<string, DiscoveredMirrors> world2 ) {
			if( world1.Count != world2.Count ) {
				return false;
			}

			foreach( (string worldUid, DiscoveredMirrors mirrors) in world1 ) {
				if( !world2.ContainsKey(worldUid) ) {
					return false;
				}
				if( !mirrors.DeepEquals( world2[worldUid] ) ) {
					return false;
				}
			}
			return true;
		}



		////////////////

		public DiscoveredMirrors() : base() { }
		//public DiscoveredMirrors( int capacity ) : base( capacity ) { }
		public DiscoveredMirrors( IEqualityComparer<int> comparer )
			: base( comparer ) { }
		public DiscoveredMirrors( IDictionary<int, ISet<int>> dictionary )
			: base( dictionary ) { }
		public DiscoveredMirrors( int capacity, IEqualityComparer<int> comparer )
			: base( comparer ) { }
		public DiscoveredMirrors( IDictionary<int, ISet<int>> dictionary, IEqualityComparer<int> comparer )
			: base( dictionary, comparer ) { }
		//protected DiscoveredMirrors( SerializationInfo info, StreamingContext context )
		//	: base( info, context ) { }

		//

		public DiscoveredMirrors( IEnumerable<KeyValuePair<int, ISet<int>>> collection )
			: base( collection ) { }

		public DiscoveredMirrors( int concurrencyLevel, int capacity )
			: base( concurrencyLevel, capacity ) { }

		public DiscoveredMirrors( IEnumerable<KeyValuePair<int, ISet<int>>> collection, IEqualityComparer<int> comparer )
			: base( collection, comparer ) { }

		public DiscoveredMirrors(
					int concurrencyLevel,
					IEnumerable<KeyValuePair<int, ISet<int>>> collection,
					IEqualityComparer<int> comparer )
			: base( concurrencyLevel, collection, comparer ) { }

		public DiscoveredMirrors( int concurrencyLevel, int capacity, IEqualityComparer<int> comparer )
			: base( concurrencyLevel, capacity, comparer ) { }



		////////////////

		public bool DeepEquals( DiscoveredMirrors mirrors ) {
			foreach( (int tileX, ISet<int> tileYs) in this ) {
				if( !mirrors.ContainsKey(tileX) ) {
					return false;
				}
				if( !mirrors[tileX].SetEquals(tileYs) ) {
					return false;
				}
			}

			return true;
		}
	}
}
