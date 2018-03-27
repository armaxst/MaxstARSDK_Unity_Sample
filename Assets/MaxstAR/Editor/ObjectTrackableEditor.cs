/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace maxstAR
{
	[CustomEditor(typeof(ObjectTrackableBehaviour))]
	public class ObjectTrackableEditor : Editor
	{
		private ObjectTrackableBehaviour trackableBehaviour = null;

		private bool Load(string fileName)
		{
			MapViewerBehaviour mapViewerBehaviour = FindObjectOfType<MapViewerBehaviour>();
			if (mapViewerBehaviour == null)
			{
				GameObject mapViewer = new GameObject("MapViewer");
				mapViewer.transform.parent = trackableBehaviour.transform;
				mapViewerBehaviour = mapViewer.AddComponent<MapViewerBehaviour>();
			}

			return mapViewerBehaviour.Load(fileName);
		}

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

			trackableBehaviour = (ObjectTrackableBehaviour)target;

			EditorGUILayout.Separator();

			StorageType oldType = trackableBehaviour.StorageType;
			StorageType newType = (StorageType)EditorGUILayout.EnumPopup("Storage type", trackableBehaviour.StorageType);

			if (oldType != newType)
			{
				trackableBehaviour.StorageType = newType;
				isDirty = true;
			}

			if (trackableBehaviour.StorageType == StorageType.StreamingAssets)
			{
				EditorGUILayout.HelpBox("Just drag&drop a *.3dmap file with tracking data from your project view here", MessageType.Info);
				EditorGUILayout.Separator();

				UnityEngine.Object oldDataObject = trackableBehaviour.TrackerDataFileObject;
				UnityEngine.Object newDataObject = EditorGUILayout.ObjectField(trackableBehaviour.TrackerDataFileObject,
					typeof(UnityEngine.Object), true);

				if (oldDataObject != newDataObject)
				{
					if (newDataObject == null)
					{
						isDirty = true;
						trackableBehaviour.TrackerDataFileObject = null;
						trackableBehaviour.TrackerDataFileName = "";
						MapViewerBehaviour mapViewerBehaviour = FindObjectOfType<MapViewerBehaviour>();
						if (mapViewerBehaviour != null)
						{
							DestroyImmediate(mapViewerBehaviour.gameObject);
						}
					}
					else
					{
						string trackerDataFileName = AssetDatabase.GetAssetPath(newDataObject);
						if (!trackerDataFileName.EndsWith(".3dmap"))
						{
							Debug.Log("trackerDataFileName: " + trackerDataFileName);
							Debug.LogError("It's not proper tracker data file!!. File's extension should be .3dmap");
						}
						else
						{
							trackableBehaviour.TrackerDataFileObject = newDataObject;
							trackableBehaviour.TrackerDataFileName =
								trackerDataFileName.Replace("Assets/StreamingAssets/", "");
							isDirty = true;

							Load(trackerDataFileName);
						}
					}
				}
			}

			if (trackableBehaviour.StorageType == StorageType.StreamingAssets)
			{
				GUILayout.BeginHorizontal(GUILayout.Width(0));
				GUILayout.FlexibleSpace();
				GUIContent content = new GUIContent("Load");

				if (GUILayout.Button(content, GUILayout.Width(100)))
				{
					string trackerDataFileName = AssetDatabase.GetAssetPath(trackableBehaviour.TrackerDataFileObject);
					if (System.IO.File.Exists(trackerDataFileName))
					{
						Load(trackerDataFileName);
					}
					else
					{
						EditorGUILayout.HelpBox("Error! : \"File isn't exist\"", MessageType.Error);
					}
				}
				GUILayout.EndHorizontal();
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