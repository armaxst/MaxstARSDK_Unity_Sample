/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections.Generic;
using System.Text;

using maxstAR;

public class MarkerTrackerSample : MonoBehaviour
{
	private Dictionary<int, MarkerTrackerBehaviour> markerTrackableMap =
		new Dictionary<int, MarkerTrackerBehaviour>();
	private bool startTrackerDone = false;
	private bool cameraStartDone = false;

	void Start()
	{
		MarkerTrackerBehaviour[] markerTrackables = FindObjectsOfType<MarkerTrackerBehaviour>();

		foreach (var trackable in markerTrackables)
		{
            trackable.SetMarkerTrackerFileName(trackable.MarkerID, trackable.MarkerSize);
			markerTrackableMap.Add(trackable.MarkerID, trackable);
			Debug.Log("Trackable id: " + trackable.MarkerID);
            Debug.Log(trackable.TrackerDataFileName);
		}
		AddTrackerData();
	}

	private void AddTrackerData()
	{
		foreach (var trackable in markerTrackableMap)
		{
			if (trackable.Value.TrackerDataFileName.Length == 0)
			{
				continue;
			}

			TrackerManager.GetInstance().AddTrackerData(trackable.Value.TrackerDataFileName);
		}

		TrackerManager.GetInstance().LoadTrackerData();
	}

	private void DisableAllTrackables()
	{
		foreach (var trackable in markerTrackableMap)
		{
			trackable.Value.OnTrackFail();
		}
	}

    public void OnClickBackButton()
    {
        SceneStackManager.Instance.LoadPrevious();
    }

	void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			SceneStackManager.Instance.LoadPrevious();
		}

		StartCamera();

		if (!startTrackerDone)
		{
			TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_MARKER);
			startTrackerDone = true;
		}

		DisableAllTrackables();

		TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();
		TrackingResult trackingResult = state.GetTrackingResult();

        string recognizedID = null;
		for (int i = 0; i < trackingResult.GetCount(); i++)
		{
			Trackable trackable = trackingResult.GetTrackable(i);
            int markerId = -1;
            if (int.TryParse(trackable.GetName(), out markerId)) {
                if (markerTrackableMap.ContainsKey(markerId))
                {
                    markerTrackableMap[markerId].OnTrackSuccess(
                        trackable.GetId(), trackable.GetName(), trackable.GetPose());

                    recognizedID += trackable.GetId().ToString() + ", ";
                }
            }
		}

        Debug.Log("Recognized Marker id : " + recognizedID);
	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			TrackerManager.GetInstance().StopTracker();
			startTrackerDone = false;
			StopCamera();
		}
	}

	void OnDestroy()
	{
		markerTrackableMap.Clear();
		TrackerManager.GetInstance().StopTracker();
		TrackerManager.GetInstance().DestroyTracker();
		StopCamera();
	}

	void StartCamera()
	{
		if (!cameraStartDone)
		{
			Debug.Log("Unity StartCamera");
			ResultCode result = CameraDevice.GetInstance().Start();
			if (result == ResultCode.Success)
			{
				cameraStartDone = true;
				//CameraDevice.GetInstance().SetAutoWhiteBalanceLock(true);   // For ODG-R7 preventing camera flickering
			}
		}
	}

	void StopCamera()
	{
		if (cameraStartDone)
		{
			Debug.Log("Unity StopCamera");
			CameraDevice.GetInstance().Stop();
			cameraStartDone = false;
		}
	}
}
