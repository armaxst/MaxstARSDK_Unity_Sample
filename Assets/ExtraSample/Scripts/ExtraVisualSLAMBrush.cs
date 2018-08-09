/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.UI;

using maxstAR;

public class ExtraVisualSLAMBrush : ARBehaviour
{
	[SerializeField]
	private Text startBtnText = null;

    private bool startTrackerDone = false;

	private Vector3 [] linePoint = new Vector3[100];
	private int linePointCount = 0;

	private LineRenderer lineRenderer = null;

	private GameObject anchor = null;

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

        Screen.orientation = ScreenOrientation.LandscapeLeft;
		TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_SLAM);

		lineRenderer = GetComponent<LineRenderer> ();

		anchor = GameObject.Find ("AnchorPoint");
	}

	void Update()
	{
		StartCamera();

        if (!startTrackerDone)
        {
            TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_SLAM);
            startTrackerDone = true;
        }

		EnableChildrenRenderer(false);

		TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();
		TrackingResult trackingResult = state.GetTrackingResult();
		cameraBackgroundBehaviour.UpdateCameraBackgroundImage(state);

		if (trackingResult.GetCount() == 0)
		{
			return;
		}

		#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0))
		{
			if (linePointCount < 100) {
				linePoint [linePointCount++] = anchor.transform.position;
				lineRenderer.positionCount = linePointCount;
				lineRenderer.SetPositions (linePoint);
			}
		}
		#else

		if (Input.touchCount > 0)
		{
			if (linePointCount < 100) {
		linePoint [linePointCount++] = anchor.transform.position;
				lineRenderer.positionCount = linePointCount;
				lineRenderer.SetPositions (linePoint);
			}
		}

		#endif


		EnableChildrenRenderer(true);
	}

	public void Reset()
	{
		linePointCount = 0;
		if (startBtnText != null) {
			startBtnText.text = "Start Tracking";
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
		Screen.orientation = ScreenOrientation.AutoRotation;
		TrackerManager.GetInstance().StopTracker();
		TrackerManager.GetInstance().DestroyTracker();
		StopCamera();
	}

	public void FindSurface()
	{
		TrackerManager.GetInstance().StopTracker();
		TrackerManager.GetInstance().DestroyTracker();

		TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_SLAM);
		TrackerManager.GetInstance().FindSurface();

		if (startBtnText != null) {
			startBtnText.text = "Stop Tracking";
		}
	}

	public void QuitFindingSurface()
	{
		TrackerManager.GetInstance().QuitFindingSurface();
	}

	private void EnableChildrenRenderer(bool activate)
	{
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();

		// Disable renderer
		foreach (Renderer component in rendererComponents)
		{
			component.enabled = activate;
		}
	}
}