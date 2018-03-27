/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace maxstAR
{
	[CustomEditor(typeof(ImageTrackableBehaviour))]
	public class ImageTrackableEditor : Editor
	{
		private ImageTrackableBehaviour trackableBehaviour;

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

			bool isDirty = false;

			trackableBehaviour = (ImageTrackableBehaviour)target;

			EditorGUILayout.Separator();

			StorageType oldType = trackableBehaviour.StorageType;
			StorageType newType = (StorageType)EditorGUILayout.EnumPopup("Storage type", trackableBehaviour.StorageType);

			if (oldType != newType)
			{
				trackableBehaviour.StorageType = newType;
				isDirty = true;
			}

			UnityEngine.Object oldDataObject = trackableBehaviour.TrackerDataFileObject;

			if (trackableBehaviour.StorageType == StorageType.StreamingAssets)
			{
				EditorGUILayout.HelpBox("Just drag&drop a *.2dmap file with tracking data from your project view here", MessageType.Info);
				EditorGUILayout.Separator();

				UnityEngine.Object newDataObject = EditorGUILayout.ObjectField(trackableBehaviour.TrackerDataFileObject,
					typeof(UnityEngine.Object), true);

				if (oldDataObject != newDataObject)
				{
					string trackerDataFileName = AssetDatabase.GetAssetPath(newDataObject);
					if (!trackerDataFileName.EndsWith(".2dmap"))
					{
						Debug.Log("trackerDataFileName: " + trackerDataFileName);
						Debug.LogError("It's not proper tracker data file!!. File's extension should be .2dmap");
					}
					else
					{
						trackableBehaviour.TrackerDataFileName =
							trackerDataFileName.Replace("Assets/StreamingAssets/", "");
						trackableBehaviour.TrackerDataFileObject = newDataObject;
						isDirty = true;
					}
				}

				if (oldDataObject != null)
				{
					EditorGUILayout.Separator();

					EditorGUILayout.LabelField("Width : ", trackableBehaviour.TargetWidth.ToString());
					EditorGUILayout.LabelField("Height : ", trackableBehaviour.TargetHeight.ToString());
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