/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections.Generic;
using System.Text;

using maxstAR;

public class InstantTraining : ARBehaviour {

    private CameraBackgroundBehaviour cameraBackgroundBehaviour = null;

    public GameObject trackingObject;

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

        AddTrackerData();
        TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_IMAGE);
        StartCamera();

        // For see through smart glass setting
        if (ConfigurationScriptableObject.GetInstance().WearableType == WearableCalibration.WearableType.OpticalSeeThrough)
        {
            WearableManager.GetInstance().GetDeviceController().SetStereoMode(true);

            cameraBackgroundBehaviour.gameObject.SetActive(false);

            WearableManager.GetInstance().GetCalibration().CreateWearableEye(Camera.main.transform);

            // BT-300 screen is splited in half size, but R-7 screen is doubled.
            if (WearableManager.GetInstance().GetDeviceController().IsSideBySideType() == true)
            {
                // Do something here. For example resize gui to fit ratio
            }
        }
    }

    private void AddTrackerData()
    {
        if(Application.platform == RuntimePlatform.Android) {
            TrackerManager.GetInstance().AddTrackerData("{\"image\":\"add_image\",\"image_path\":\"" + "intant_training.jpg" + "\",\"image_width\":0.26}", true);
            TrackerManager.GetInstance().AddTrackerData("{\"image\":\"add_image\",\"image_path\":\"" + "intant_training1.jpg" + "\",\"image_width\":0.26}", true);
        } else {
            TrackerManager.GetInstance().AddTrackerData("{\"image\":\"add_image\",\"image_path\":\"" + Application.streamingAssetsPath + "/intant_training.jpg" + "\",\"image_width\":0.26}", true);
            TrackerManager.GetInstance().AddTrackerData("{\"image\":\"add_image\",\"image_path\":\"" + Application.streamingAssetsPath + "/intant_training1.jpg" + "\",\"image_width\":0.26}", true);
        }
       
        TrackerManager.GetInstance().LoadTrackerData();
    }

    void Update()
    {
        TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();

        cameraBackgroundBehaviour.UpdateCameraBackgroundImage(state);

        TrackingResult trackingResult = state.GetTrackingResult();

        if (trackingResult.GetCount() > 0)
        {
            for (int i = 0; i < trackingResult.GetCount(); i++)
            {
                Trackable trackable = trackingResult.GetTrackable(i);
              
                Matrix4x4 poseMatrix = trackable.GetPose();

                float width = trackable.GetWidth();
                float height = trackable.GetHeight();

                trackingObject.transform.position = MatrixUtils.PositionFromMatrix(poseMatrix);
                trackingObject.transform.rotation = MatrixUtils.QuaternionFromMatrix(poseMatrix);
                trackingObject.transform.localScale = new Vector3(width, height, height);
            }
        }
        else
        {
            trackingObject.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    public void SetNormalMode()
    {
        TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.NORMAL_TRACKING);
    }

    public void SetExtendedMode()
    {
        TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.EXTEND_TRACKING);
    }

    public void SetMultiMode()
    {
        TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.MULTI_TRACKING);
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
            TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_IMAGE);
        }
    }

    void OnDestroy()
    {
        TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.NORMAL_TRACKING);
        TrackerManager.GetInstance().StopTracker();
        TrackerManager.GetInstance().DestroyTracker();
        StopCamera();
    }
}
