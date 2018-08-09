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

public class ExtraInstantTrackerBrush : ARBehaviour
{
	[SerializeField]
	private Text startBtnText = null;

	private bool startTrackerDone = false;
	private bool findSurfaceDone = false;

	private InstantTrackableBehaviour instantTrackable = null;
	private LineRenderer lineRenderer = null;

	private Vector3[] linePoint = new Vector3[100];
	private int linePointCount = 0;

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
		lineRenderer = instantTrackable.GetComponentInChildren<LineRenderer>();
	}

	void Update()
	{
		StartCamera();

		if (!startTrackerDone)
		{
			TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_INSTANT);
			SensorDevice.GetInstance().Start();
			startTrackerDone = true;
		}

		TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();
		TrackingResult trackingResult = state.GetTrackingResult();
		cameraBackgroundBehaviour.UpdateCameraBackgroundImage(state);

		if (trackingResult.GetCount() == 0)
		{
			instantTrackable.OnTrackFail();
			return;
		}

		Trackable track = trackingResult.GetTrackable(0);
		instantTrackable.OnTrackSuccess(track.GetId(), track.GetName(), track.GetPose());

#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0))
		{
			if (linePointCount < 100)
			{
				linePoint[linePointCount++] = TrackerManager.GetInstance().GetWorldPositionFromScreenCoordinate(Input.mousePosition);
				lineRenderer.positionCount = linePointCount;
				lineRenderer.SetPositions(linePoint);
			}
		}
#else
		if (Input.touchCount > 0)
		{
			if (linePointCount < 100) {
				linePoint [linePointCount++] = TrackerManager.GetInstance ().GetWorldPositionFromScreenCoordinate (Input.GetTouch (0).position);
				lineRenderer.positionCount = linePointCount;
				lineRenderer.SetPositions (linePoint);
			}
		}

		if (Input.GetTouch (0).phase == TouchPhase.Ended) {
			linePointCount = 0;
		}
#endif

	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			SensorDevice.GetInstance().Stop();
			TrackerManager.GetInstance().StopTracker();
			startTrackerDone = false;
			StopCamera();
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
