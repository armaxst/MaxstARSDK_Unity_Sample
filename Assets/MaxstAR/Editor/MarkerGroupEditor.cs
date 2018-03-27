/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace maxstAR
{
	[CustomEditor(typeof(MarkerGroupBehaviour))]
	public class MarkerGroupEditor : Editor
	{
		private MarkerGroupBehaviour markerGroupBehaviour;
        private MarkerTrackerBehaviour[] markerTrackerBehaviour;

		public void OnEnable()
		{
            markerTrackerBehaviour = FindObjectsOfType<MarkerTrackerBehaviour>();

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

			bool isDirty = false;

            markerGroupBehaviour = (MarkerGroupBehaviour)target;

            EditorGUILayout.Separator();

            float oldMarkerSize = markerGroupBehaviour.MarkerGroupSize;
            float newMarkerSize = EditorGUILayout.FloatField("Marker Size : ", markerGroupBehaviour.MarkerGroupSize);

			if (oldMarkerSize != newMarkerSize)
			{
                markerGroupBehaviour.MarkerGroupSize = newMarkerSize;
				isDirty = true;
			}

			EditorGUILayout.Separator();

            bool oldApplyAll = markerGroupBehaviour.ApplyAll;
            bool newApplyAll = EditorGUILayout.Toggle("Apply All : ", markerGroupBehaviour.ApplyAll);

            if (oldApplyAll != newApplyAll)
            {
                markerGroupBehaviour.ApplyAll = newApplyAll;
                isDirty = true;
            }

			if (GUI.changed && isDirty)
			{
                if (markerGroupBehaviour.ApplyAll) {
                    foreach (var tracker in markerTrackerBehaviour)
                    {
                        tracker.MarkerSize = markerGroupBehaviour.MarkerGroupSize;
                        EditorUtility.SetDirty(tracker);
                    }
                }

				EditorUtility.SetDirty(markerGroupBehaviour);
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
				SceneManager.Instance.SceneUpdated();
			}
		}
	}
}