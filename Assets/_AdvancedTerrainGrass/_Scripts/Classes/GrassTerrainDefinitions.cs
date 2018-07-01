using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AdvancedTerrainGrass {
	[Serializable]
	public class GrassTerrainDefinitions : ScriptableObject {

		[Header("Serialized Grass Data")]
		[SerializeField] public List <DetailLayerMap> DensityMaps;
		public GrassCell[] Cells;
		public GrassCellContent[] CellContent;
		public int maxBucketDensity = 0;
	}

//	Members
	[Serializable] public class DetailLayerMap {
		[SerializeField] public byte[] mapByte;
	}
}
