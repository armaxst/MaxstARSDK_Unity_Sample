/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections.Generic;
using System.Text;

using maxstAR;

public class CameraConfigurationSample : ARBehaviour
{
	private Dictionary<string, ImageTrackableBehaviour> imageTrackablesMap =
		new Dictionary<string, ImageTrackableBehaviour>();

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
		imageTrackablesMap.Clear();
		ImageTrackableBehaviour[] imageTrackables = FindObjectsOfType<ImageTrackableBehaviour>();
		foreach (var trackable in imageTrackables)
		{
			imageTrackablesMap.Add(trackable.TrackableName, trackable);
			Debug.Log("Trackable add: " + trackable.TrackableName);
		}

		AddTrackerData();

		StartCamera();
		TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_IMAGE);
	}

	private void AddTrackerData()
	{
		foreach (var trackable in imageTrackablesMap)
		{
			if (trackable.Value.TrackerDataFileName.Length == 0)
			{
				continue;
			}

			if (trackable.Value.StorageType == StorageType.AbsolutePath)
			{
				TrackerManager.GetInstance().AddTrackerData(trackable.Value.TrackerDataFileName);
			}
			else
			{
				if (Application.platform == RuntimePlatform.Android)
				{
					TrackerManager.GetInstance().AddTrackerData(trackable.Value.TrackerDataFileName, true);
				}
				else
				{
					TrackerManager.GetInstance().AddTrackerData(Application.streamingAssetsPath + "/" + trackable.Value.TrackerDataFileName);
				}
			}
		}

		TrackerManager.GetInstance().LoadTrackerData();
	}

	private void DisableAllTrackables()
	{
		foreach (var trackable in imageTrackablesMap)
		{
			trackable.Value.OnTrackFail();
		}
	}

	void Update()
	{
		DisableAllTrackables();

		TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();
		TrackingResult trackingResult = state.GetTrackingResult();
		cameraBackgroundBehaviour.UpdateCameraBackgroundImage(state);

		for (int i = 0; i < trackingResult.GetCount(); i++)
		{
			Trackable trackable = trackingResult.GetTrackable(i);
			imageTrackablesMap[trackable.GetName()].OnTrackSuccess(
				trackable.GetId(), trackable.GetName(), trackable.GetPose());
		}
	}

	bool flashOn = false;
	bool horizontalFlip = false;
	bool verticalFlip = false;
	bool focusContinuous = true;
	bool whiteBalanceLock = false;

	public void SetFlashLightMode()
	{
		flashOn = !flashOn;
		CameraDevice.GetInstance().SetFlashLightMode(flashOn);
	}

	public void FlipHorizontal()
	{
		horizontalFlip = !horizontalFlip;
		CameraDevice.GetInstance().FlipVideo(CameraDevice.FlipDirection.HORIZONTAL, horizontalFlip);
	}

	public void FlipVertical()
	{
		verticalFlip = !verticalFlip;
		CameraDevice.GetInstance().FlipVideo(CameraDevice.FlipDirection.VERTICAL, verticalFlip);
	}

	public void WhiteBalanceLock()
	{
		whiteBalanceLock = !whiteBalanceLock;
		CameraDevice.GetInstance().SetAutoWhiteBalanceLock(whiteBalanceLock);
	}

	public void SetContinuousFocus()
	{
		focusContinuous = !focusContinuous;
		if (focusContinuous)
		{
			CameraDevice.GetInstance().SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUS_AUTO);
		}
		else
		{
			CameraDevice.GetInstance().SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_AUTO);
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
			TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_IMAGE);
		}
	}

	void OnDestroy()
	{
		CameraDevice.GetInstance().SetFlashLightMode(false);

		imageTrackablesMap.Clear();
		TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.NORMAL_TRACKING);
		TrackerManager.GetInstance().StopTracker();
		TrackerManager.GetInstance().DestroyTracker();
		StopCamera();
	}
}