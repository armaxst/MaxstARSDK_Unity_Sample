/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

namespace maxstAR
{
	[CustomEditor(typeof(MapViewerBehaviour))]
	public class MapViewerEditor : Editor
	{
		private MapViewerBehaviour mapViewerBehaviour = null;
		private bool autoCamera = false;

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

			mapViewerBehaviour = (MapViewerBehaviour)target;

			int keyframeIndex = EditorGUILayout.IntSlider("Keyframe index", mapViewerBehaviour.KeyframeIndex, 
				0, mapViewerBehaviour.MaxKeyframeCount - 1);
			bool showMesh = EditorGUILayout.Toggle("Show Mesh", mapViewerBehaviour.ShowMesh);
			autoCamera = EditorGUILayout.Toggle("Auto Camera", mapViewerBehaviour.AutoCamera);
			bool transparent = EditorGUILayout.Toggle("Transparent", mapViewerBehaviour.Transparent);

			if (mapViewerBehaviour.KeyframeIndex != keyframeIndex)
			{
				mapViewerBehaviour.KeyframeIndex = keyframeIndex;
				isDirty = true;
			}

			if (mapViewerBehaviour.ShowMesh != showMesh)
			{
				mapViewerBehaviour.ShowMesh = showMesh;
				isDirty = true;
			}

			if (mapViewerBehaviour.AutoCamera != autoCamera)
			{
				mapViewerBehaviour.AutoCamera = autoCamera;
				isDirty = true;
			}

			if (mapViewerBehaviour.Transparent != transparent)
			{
				mapViewerBehaviour.Transparent = transparent;
				isDirty = true;
			}

			if (GUI.changed && isDirty)
			{
				EditorUtility.SetDirty(mapViewerBehaviour);
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}

		void OnSceneGUI()
		{
			if (Event.current.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.ScrollWheel)
			{
				if (autoCamera)
				{
					Vector3 position = SceneView.lastActiveSceneView.camera.transform.position;
					Quaternion rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
					mapViewerBehaviour.ApplyViewCamera(position, rotation);
					mapViewerBehaviour.UpdateMapViewer();
				}
			}
		}
	}
}