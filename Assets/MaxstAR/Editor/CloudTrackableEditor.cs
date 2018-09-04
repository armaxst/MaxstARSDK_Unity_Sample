using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace maxstAR
{
    [CustomEditor(typeof(CloudTrackableBehaviour))]
    public class CloudTrackableEditor : Editor
    {
        private CloudTrackableBehaviour trackableBehaviour;
        private const int maxHeight = 25;

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

            trackableBehaviour = (CloudTrackableBehaviour)target;

            EditorGUILayout.Separator();

            CloudType oldType = trackableBehaviour.CloudNameType;
            CloudType newType = (CloudType)EditorGUILayout.EnumPopup("Object of Recognition", trackableBehaviour.CloudNameType);

            if (oldType != newType)
            {
                trackableBehaviour.CloudNameType = newType;
                isDirty = true;

                if(newType == CloudType.Cloud) {
                    trackableBehaviour.CloudName = "_MaxstCloud_";
                } else {
                    trackableBehaviour.CloudName = "";
                }

                trackableBehaviour.OnTrackerCloudName(trackableBehaviour.CloudName);
            }

            if(newType == CloudType.User_Defined) {
                EditorGUILayout.Separator();

                string cloudName = trackableBehaviour.CloudName;
                string newCloudName = EditorGUILayout.TextField("Target Image Name : ", trackableBehaviour.CloudName);

                trackableBehaviour.CloudName = newCloudName;
                isDirty = true;
            } 

            EditorGUILayout.Separator();

            GUIContent content = new GUIContent("Add Cloud Target Image");
            if (GUILayout.Button(content, GUILayout.MaxWidth(Screen.width), GUILayout.MaxHeight(maxHeight)))
            {
                Application.OpenURL("https://developer.maxst.com");
            }
            EditorGUILayout.Separator();

            if (newType == CloudType.User_Defined)
            {
                string cloudName = trackableBehaviour.CloudName;
                if(cloudName == "_MaxstCloud_") {
                    EditorGUILayout.LabelField("Please set a different name.");
                    isDirty = true;
                }
            }
            EditorGUILayout.Separator();

            if (GUI.changed && isDirty)
            {
                EditorUtility.SetDirty(trackableBehaviour);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                SceneManager.Instance.SceneUpdated();
            }
        }
    }
}
