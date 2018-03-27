/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace maxstAR
{
    [CustomEditor(typeof(ConfigurationScriptableObject))]
    public class ConfigurationScriptableObjectEditor : Editor
    {
        private ConfigurationScriptableObject configuration = null;
        private bool isDirty = false;

        private bool licenseFold = true;
        private bool cameraFold = true;

        private string[] LoadWebcamDeviceList()
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            string[] deviceList = new string[devices.Length];
            for (int i = 0; i < devices.Length; i++)
            {
                deviceList[i] = devices[i].name;
                if (devices[i].name == "")
                {
                    deviceList[i] = "Unknown Device " + i;
                }
            }

            return deviceList;
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

            configuration = (ConfigurationScriptableObject)target;

            isDirty = false;

            licenseFold = EditorGUILayout.Foldout(licenseFold, "License Key Setting");
            if (licenseFold)
            {
                EditorGUILayout.LabelField("License key");
                string licenseKey = configuration.LicenseKey;
                configuration.LicenseKey = EditorGUILayout.TextArea(licenseKey, GUILayout.MaxHeight(40));
                EditorGUILayout.HelpBox("Please register your app at https://developer.maxst.com/.", MessageType.Info);
                EditorGUILayout.Space();
                if (string.Equals(licenseKey, configuration.LicenseKey) == false)
                {
                    isDirty = true;
                }
            }

            cameraFold = EditorGUILayout.Foldout(cameraFold, "Camera Settings");
            if (cameraFold)
            {
                CameraDevice.CameraType cameraType = configuration.CameraType;
                configuration.CameraType = (CameraDevice.CameraType)EditorGUILayout.EnumPopup("Mobile Camera Type", cameraType);
                EditorGUILayout.HelpBox("Camera settings in mobile app", MessageType.Info);
                EditorGUILayout.Space();
                if (string.Equals(cameraType, configuration.CameraType) == false)
                {
                    isDirty = true;
                }

                int webcamType = configuration.WebcamType;
                configuration.WebcamType = EditorGUILayout.Popup("Webcam Type", webcamType, LoadWebcamDeviceList());
                EditorGUILayout.HelpBox("Webcam settings in Editor mode.", MessageType.Info);
                EditorGUILayout.Space();
                if (string.Equals(webcamType, configuration.WebcamType) == false)
                {
                    isDirty = true;
                }

                CameraDevice.CameraResolution cameraResolution = configuration.CameraResolution;
                configuration.CameraResolution = (CameraDevice.CameraResolution)EditorGUILayout.EnumPopup("Camera Resolution", cameraResolution);
                EditorGUILayout.HelpBox("Please select a supported resolution.", MessageType.Info);
                EditorGUILayout.Space();
                if (string.Equals(cameraResolution, configuration.CameraResolution) == false)
                {
                    isDirty = true;
                }

                WearableCalibration.WearableType wearableType = configuration.WearableType;
                configuration.WearableType = (WearableCalibration.WearableType)EditorGUILayout.EnumPopup("Wearable Type", wearableType);
                EditorGUILayout.HelpBox("Please select a wearable type.", MessageType.Info);
                EditorGUILayout.Space();
                if (string.Equals(wearableType, configuration.WearableType) == false)
                {
                    isDirty = true;
                }
            }

            if (GUI.changed && isDirty)
            {
                EditorUtility.SetDirty(configuration);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}