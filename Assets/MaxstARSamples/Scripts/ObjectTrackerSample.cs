/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections.Generic;
using System.Text;

using maxstAR;

public class ObjectTrackerSample : ARBehaviour
{
	private Dictionary<string, ObjectTrackableBehaviour> objectTrackablesMap =
	new Dictionary<string, ObjectTrackableBehaviour>();

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
		objectTrackablesMap.Clear();
		ObjectTrackableBehaviour[] objectTrackables = FindObjectsOfType<ObjectTrackableBehaviour>();
		foreach (var trackable in objectTrackables)
		{
			objectTrackablesMap.Add(trackable.TrackableName, trackable);
			Debug.Log("Trackable add: " + trackable.TrackableName);
		}

		AddTrackerData();
		StartCamera();
		TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_OBJECT);
	}

	private void AddTrackerData()
	{
		foreach (var trackable in objectTrackablesMap)
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
					Debug.Log("trackable.Value.TrackerDataFileName:" + trackable.Value.TrackerDataFileName);
					TrackerManager.GetInstance().AddTrackerData(trackable.Value.TrackerDataFileName, true);
				}
				else
				{
					Debug.Log("trackable.Value.TrackerDataFileName:" + trackable.Value.TrackerDataFileName);
					TrackerManager.GetInstance().AddTrackerData(Application.streamingAssetsPath + "/" + trackable.Value.TrackerDataFileName);
				}
			}
		}

		TrackerManager.GetInstance().LoadTrackerData();
	}

	private void DisableAllTrackables()
	{
		foreach (var trackable in objectTrackablesMap)
		{
			trackable.Value.OnTrackFail();
		}
	}

	void Update()
	{
		DisableAllTrackables();

		TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();

        cameraBackgroundBehaviour.UpdateCameraBackgroundImage(state);
		TrackingResult trackingResult = state.GetTrackingResult();

		for (int i = 0; i < trackingResult.GetCount(); i++)
		{
			Trackable trackable = trackingResult.GetTrackable(i);

			if (!objectTrackablesMap.ContainsKey(trackable.GetName()))
			{
				return;
			}

			objectTrackablesMap[trackable.GetName()].OnTrackSuccess(trackable.GetId(), trackable.GetName(),
																   trackable.GetPose());
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
			TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_OBJECT);
		}
	}

	void OnDestroy()
	{
		objectTrackablesMap.Clear();
		TrackerManager.GetInstance().StopTracker();
		TrackerManager.GetInstance().DestroyTracker();
		StopCamera();
	}
}