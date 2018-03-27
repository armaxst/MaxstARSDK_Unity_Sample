/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace maxstAR
{
	[CustomEditor(typeof(MarkerTrackerBehaviour))]
	public class MarkerTrackableEditor : Editor
	{
		private MarkerTrackerBehaviour trackableBehaviour;
        private MarkerGroupBehaviour markerGroup;
        public void OnEnable()
		{
            markerGroup = FindObjectOfType<MarkerGroupBehaviour>();
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

			trackableBehaviour = (MarkerTrackerBehaviour)target;

			EditorGUILayout.Separator();

			int oldMarkerId = trackableBehaviour.MarkerID;
            int newMarkerId = EditorGUILayout.IntField("Marker ID : ", trackableBehaviour.MarkerID);

			if (oldMarkerId != newMarkerId)
			{
				trackableBehaviour.MarkerID = newMarkerId;
				isDirty = true;
			}

            EditorGUILayout.Separator();

            
            if (markerGroup.ApplyAll)
            {
                trackableBehaviour.MarkerSize = markerGroup.MarkerGroupSize;
                EditorGUILayout.LabelField("Marker Size : ", markerGroup.MarkerGroupSize.ToString());

                EditorGUILayout.Separator();

                EditorGUILayout.HelpBox("If uou checked [Apply All] at Marker Group, Marker Size is set by Marker Group's size", MessageType.Warning);
                isDirty = true;
            }
            else
            {
                float oldMarkerSize = trackableBehaviour.MarkerSize;
                float newMarkerSize = EditorGUILayout.FloatField("Marker Size : ", trackableBehaviour.MarkerSize);

                if (oldMarkerSize != newMarkerSize)
                {
                    trackableBehaviour.MarkerSize = newMarkerSize;
                    isDirty = true;
                }
            }


            if (GUI.changed && isDirty)
			{
				EditorUtility.SetDirty(trackableBehaviour);
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
				SceneManager.Instance.SceneUpdated();
			}
		}
	}
}