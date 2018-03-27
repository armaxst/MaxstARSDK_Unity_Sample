/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace maxstAR
{
    [CustomEditor(typeof(ARManager))]
    public class ARManagerEditor : Editor
    {
        private const int maxHeight = 25;
        private ARManager arManager = null;

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

            arManager = (ARManager)target;

            EditorGUILayout.Separator();

            AbstractARManager.WorldCenterMode oldWorldCenterMode = arManager.WorldCenterModeSetting;
            AbstractARManager.WorldCenterMode newWorldCenterMode =
                (AbstractARManager.WorldCenterMode)EditorGUILayout.EnumPopup("World Center Mode", arManager.WorldCenterModeSetting);

            if (oldWorldCenterMode != newWorldCenterMode)
            {
                arManager.SetWorldCenterMode(newWorldCenterMode);
                isDirty = true;
            }

            EditorGUILayout.Separator();

            GUIContent content = new GUIContent("Configuration");
            if (GUILayout.Button(content, GUILayout.MaxWidth(Screen.width), GUILayout.MaxHeight(maxHeight)))
            {
                Selection.activeObject = ConfigurationScriptableObject.GetInstance();
            }
            GUILayout.Space(10);

            if (GUI.changed && isDirty)
            {
                EditorUtility.SetDirty(arManager);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}