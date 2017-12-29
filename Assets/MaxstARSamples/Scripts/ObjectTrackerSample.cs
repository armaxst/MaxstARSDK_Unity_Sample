using UnityEngine;
using System.Collections.Generic;
using System.Text;

using maxstAR;

public class ObjectTrackerSample : MonoBehaviour
{
	private Dictionary<string, ObjectTrackableBehaviour> objectTrackablesMap =
	new Dictionary<string, ObjectTrackableBehaviour>();
	private bool startTrackerDone = false;
	private bool cameraStartDone = false;

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
		if (Input.GetKey(KeyCode.Escape))
		{
			SceneStackManager.Instance.LoadPrevious();
		}

		StartCamera();

		if (!startTrackerDone)
		{
			TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_OBJECT);
			startTrackerDone = true;
		}

		DisableAllTrackables();

		TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();
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
			startTrackerDone = false;
			StopCamera();
		}
	}

	void OnDestroy()
	{
		objectTrackablesMap.Clear();
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