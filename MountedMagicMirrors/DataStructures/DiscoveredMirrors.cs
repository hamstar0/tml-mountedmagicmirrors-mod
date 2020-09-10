using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Terraria;
using HamstarHelpers.Helpers.Debug;


namespace MountedMagicMirrors.DataStructures {
	class DiscoveredMirrors : Dictionary<int, ISet<int>> {
		public DiscoveredMirrors() : base() { }
		public DiscoveredMirrors( int capacity ) : base( capacity ) { }
		public DiscoveredMirrors( IEqualityComparer<int> comparer )
			: base( comparer ) { }
		public DiscoveredMirrors( IDictionary<int, ISet<int>> dictionary )
			: base( dictionary ) { }
		public DiscoveredMirrors( int capacity, IEqualityComparer<int> comparer )
			: base( comparer ) { }
		public DiscoveredMirrors( IDictionary<int, ISet<int>> dictionary, IEqualityComparer<int> comparer )
			: base( dictionary, comparer ) { }
		protected DiscoveredMirrors( SerializationInfo info, StreamingContext context )
			: base( info, context ) { }
	}
}
