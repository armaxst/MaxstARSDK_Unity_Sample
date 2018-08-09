/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.UI;

using maxstAR;

public class InstantTrackerSample : ARBehaviour
{
	[SerializeField]
	private Text startBtnText = null;

	private Vector3 touchToWorldPosition = Vector3.zero;
	private Vector3 touchSumPosition = Vector3.zero;
	private bool findSurfaceDone = false;

	private InstantTrackableBehaviour instantTrackable = null;
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
		instantTrackable = FindObjectOfType<InstantTrackableBehaviour>();
		if (instantTrackable == null)
		{
			return;
		}

		instantTrackable.OnTrackFail();
		StartCamera();

		TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_INSTANT);
		SensorDevice.GetInstance().Start();
	}

	void Update()
	{
		if (instantTrackable == null)
		{
			return;
		}

		TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();

        cameraBackgroundBehaviour.UpdateCameraBackgroundImage(state);

		TrackingResult trackingResult = state.GetTrackingResult();

		if (trackingResult.GetCount() == 0)
		{
			instantTrackable.OnTrackFail();
			return;
		}		

		if (Input.touchCount > 0)
		{
			UpdateTouchDelta(Input.GetTouch(0).position);
		}

		Trackable trackable = trackingResult.GetTrackable(0);
		Matrix4x4 poseMatrix = trackable.GetPose() * Matrix4x4.Translate(touchSumPosition);
		instantTrackable.OnTrackSuccess(trackable.GetId(), trackable.GetName(), poseMatrix);
	}

	private void UpdateTouchDelta(Vector2 touchPosition)
	{
		switch (Input.GetTouch(0).phase)
		{
			case TouchPhase.Began:
				touchToWorldPosition = TrackerManager.GetInstance().GetWorldPositionFromScreenCoordinate(touchPosition);
				break;

			case TouchPhase.Moved:
				Vector3 currentWorldPosition = TrackerManager.GetInstance().GetWorldPositionFromScreenCoordinate(touchPosition);
				touchSumPosition += (currentWorldPosition - touchToWorldPosition);
				touchToWorldPosition = currentWorldPosition;
				break;
		}
	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			SensorDevice.GetInstance().Stop();
			TrackerManager.GetInstance().StopTracker();
			StopCamera();
		}
		else
		{
			StartCamera();
			TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_INSTANT);
			SensorDevice.GetInstance().Start();
		}
	}

	void OnDestroy()
	{
		SensorDevice.GetInstance().Stop();
		TrackerManager.GetInstance().StopTracker();
		TrackerManager.GetInstance().DestroyTracker();
		StopCamera();
	}

	public void OnClickStart()
	{
		if (!findSurfaceDone)
		{
			TrackerManager.GetInstance().FindSurface();
			if (startBtnText != null)
			{
				startBtnText.text = "Stop Tracking";
			}
			findSurfaceDone = true;
			touchSumPosition = Vector3.zero;
		}
		else
		{
			TrackerManager.GetInstance().QuitFindingSurface();
			if (startBtnText != null)
			{
				startBtnText.text = "Start Tracking";
			}
			findSurfaceDone = false;
		}
	}
}
