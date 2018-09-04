using UnityEngine;
using System.Collections.Generic;
using System.Text;

using maxstAR;

public class CloudTrackerSample : ARBehaviour {

    private Dictionary<string, List<CloudTrackableBehaviour>> cloudTrackablesMap = new Dictionary<string, List<CloudTrackableBehaviour>>();
    private CameraBackgroundBehaviour cameraBackgroundBehaviour = null;
    
    void Awake()
    {
        Init();
        cameraBackgroundBehaviour = FindObjectOfType<CameraBackgroundBehaviour>();
        if (cameraBackgroundBehaviour == null)
        {
            Debug.LogError("Can't find CameraBackgroundBehaviour.");
            return;
        }
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        cloudTrackablesMap.Clear();
        CloudTrackableBehaviour[] cloudTrackables = FindObjectsOfType<CloudTrackableBehaviour>();
        foreach (var trackable in cloudTrackables)
        {
            if (trackable.CloudName == "") 
            {
                Debug.LogError("Defined Name is must be named.");
                continue;
            }

            if(cloudTrackablesMap.ContainsKey(trackable.CloudName)) {
                List<CloudTrackableBehaviour> cloudTrackableList = cloudTrackablesMap[trackable.CloudName];
                cloudTrackableList.Add(trackable);
                cloudTrackablesMap.Add(trackable.CloudName, cloudTrackableList);
            } else {
                List<CloudTrackableBehaviour> cloudTrackableList = new List<CloudTrackableBehaviour>();
                cloudTrackableList.Add(trackable);
                cloudTrackablesMap.Add(trackable.CloudName, cloudTrackableList);
            }
        }

        StartCamera();

        // Add SecretId and SecretKey.
        TrackerManager.GetInstance().setCloudRecognitionSecretIdAndSecretKey("", "");
        TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_CLOUD_RECOGNIZER);

        // For see through smart glass setting
        if (ConfigurationScriptableObject.GetInstance().WearableType == WearableCalibration.WearableType.OpticalSeeThrough)
        {
            WearableManager.GetInstance().GetDeviceController().SetStereoMode(true);

            CameraBackgroundBehaviour cameraBackground = FindObjectOfType<CameraBackgroundBehaviour>();
            cameraBackground.gameObject.SetActive(false);

            WearableManager.GetInstance().GetCalibration().CreateWearableEye(Camera.main.transform);

            // BT-300 screen is splited in half size, but R-7 screen is doubled.
            if (WearableManager.GetInstance().GetDeviceController().IsSideBySideType() == true)
            {
                // Do something here. For example resize gui to fit ratio
            }
        }
    }

    private void DisableAllTrackables()
    {
        foreach (var trackableLists in cloudTrackablesMap)
        {
            foreach(var trackable in trackableLists.Value) 
            {
                trackable.OnTrackFail();
            }
        }
    }

    void Update()
    {
        DisableAllTrackables();

        TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();
        TrackedImage image = state.GetImage();

        cameraBackgroundBehaviour.UpdateCameraBackgroundImage(state);

        TrackingResult trackingResult = state.GetTrackingResult();

        if(trackingResult.GetCount() > 0) {
            Trackable trackable = trackingResult.GetTrackable(0);
            if(cloudTrackablesMap.ContainsKey(trackable.GetCloudName())) {
                foreach (var cloudTrackable in cloudTrackablesMap[trackable.GetCloudName()])
                {
                    cloudTrackable.OnTrackSuccess(trackable.GetId(), trackable.GetName(), trackable.GetPose());
                }
            } else {
                if(cloudTrackablesMap.ContainsKey("_MaxstCloud_")) {
                    foreach (var cloudTrackable in cloudTrackablesMap["_MaxstCloud_"])
                    {
                        cloudTrackable.OnTrackSuccess(trackable.GetId(), trackable.GetName(), trackable.GetPose());
                    }
                }
            }
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            TrackerManager.GetInstance().StopTracker();
            StopCamera();
        }
        else
        {
            StartCamera();
            TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_CLOUD_RECOGNIZER);
        }
    }

    void OnDestroy()
    {
        TrackerManager.GetInstance().StopTracker();
        TrackerManager.GetInstance().DestroyTracker();
        StopCamera();
    }
}