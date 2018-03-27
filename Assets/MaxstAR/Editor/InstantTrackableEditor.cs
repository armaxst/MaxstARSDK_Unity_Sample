/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace maxstAR
{
	[CustomEditor(typeof(InstantTrackableBehaviour))]
	public class InstantTrackableEditor : Editor
	{
		//private InstantTrackableBehaviour trackableBehaviour = null;

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

			//trackableBehaviour = (InstantTrackableBehaviour)target;
		}
	}
}