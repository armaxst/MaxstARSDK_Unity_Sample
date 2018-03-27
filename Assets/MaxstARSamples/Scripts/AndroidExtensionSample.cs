/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections.Generic;
using System.Text;

using maxstAR;

public class AndroidExtensionSample : MonoBehaviour
{
	private AndroidJavaObject currentActivity;
	private AndroidJavaClass AndroidExtensionClass;
	private AndroidJavaObject AndroidExtensionObject;

	private bool cameraStartDone = false;
	private bool startTrackerDone = false;
	private bool resizeSurfaceToggle = false;

	private ImageTrackableBehaviour imageTrackable;

	void Start()
	{
#if UNITY_ANDROID
		AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidExtensionClass = new AndroidJavaClass("com.maxst.ar.android_extension.AndroidExtension");
		AndroidExtensionObject = AndroidExtensionClass.CallStatic<AndroidJavaObject>("init", currentActivity);
#endif

		imageTrackable = FindObjectOfType<ImageTrackableBehaviour>();

		if (Application.platform == RuntimePlatform.Android)
		{
			TrackerManager.GetInstance().AddTrackerData(imageTrackable.TrackerDataFileName, true);
		}
		else
		{
			TrackerManager.GetInstance().AddTrackerData(Application.streamingAssetsPath + "/" + imageTrackable.TrackerDataFileName);
		}

		TrackerManager.GetInstance().LoadTrackerData();
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
			TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_IMAGE);
			startTrackerDone = true;
		}

		TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();
		TrackingResult trackingResult = state.GetTrackingResult();

		if (trackingResult.GetCount() == 0)
		{
			imageTrackable.OnTrackFail();
		}

		for (int i = 0; i < trackingResult.GetCount(); i++)
		{
			Trackable trackable = trackingResult.GetTrackable(i);
			imageTrackable.OnTrackSuccess(
				trackable.GetId(), trackable.GetName(), trackable.GetPose());
		}
	}

	public void ResizeSurfaceView()
	{
		Debug.Log("Resize surface toggle");
#if UNITY_ANDROID
		resizeSurfaceToggle = !resizeSurfaceToggle;
		AndroidExtensionObject.Call("resizeSurface", resizeSurfaceToggle);
#endif
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
		TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.NORMAL_TRACKING);
		TrackerManager.GetInstance().StopTracker();
		TrackerManager.GetInstance().DestroyTracker();

		StopCamera();

		if (currentActivity != null)
		{
			currentActivity.Dispose();
			currentActivity = null;
		}

		if (AndroidExtensionClass != null)
		{
			AndroidExtensionClass.Dispose();
			AndroidExtensionClass = null;
		}

		if (AndroidExtensionObject != null)
		{
			AndroidExtensionObject.Call("deinit");
			AndroidExtensionObject.Dispose();
			AndroidExtensionObject = null;
		}
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