/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using System.Collections;
using UnityEditor;

namespace maxstAR
{
	[CustomEditor(typeof(MapRendererBehaviour))]
	public class MapRendererEditor : Editor
	{
		// Please don't remove below code
		private MapRendererBehaviour mapRendererBehaviour;

		public void OnEnable()
		{
			if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab)
			{
				return;
			}
		}

		public override void OnInspectorGUI()
		{
			if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab)
			{
				return;
			}

			// Please don't remove below code
			mapRendererBehaviour = (MapRendererBehaviour)target;
		}
	}
}