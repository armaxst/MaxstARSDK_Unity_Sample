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

public class ExraInstantTrackerMultiContents : MonoBehaviour
{
	[SerializeField]
	private Text startBtnText = null;

	public GameObject cube = null;

	private bool startTrackerDone = false;
	private bool cameraStartDone = false;
	private bool findSurfaceDone = false;

	private List<InstantTrackableBehaviour> instantTrackables = new List<InstantTrackableBehaviour>();

	private List<Vector3> touchToWorldPositions = new List<Vector3> ();
	private List<Vector3> touchSumPositions = new List<Vector3> ();

	private int id = 0;

	void Start ()
	{
		instantTrackables.Clear();
		InstantTrackableBehaviour[] trackables = FindObjectsOfType<InstantTrackableBehaviour>();
		foreach (var trackable in trackables)
		{
			trackable.OnTrackFail ();
			instantTrackables.Add (trackable);
			touchToWorldPositions.Add (new Vector3 (0.0f, 0.0f, 0.0f));
			touchSumPositions.Add (new Vector3 (0.0f, 0.0f, 0.0f));
		}
	}

	public void OnClickBackButton ()
	{
		SceneStackManager.Instance.LoadPrevious ();
	}

	void Update ()
	{
		if (Input.GetKey (KeyCode.Escape)) {
			SceneStackManager.Instance.LoadPrevious ();
		}

		StartCamera ();

		if (!startTrackerDone) {
			TrackerManager.GetInstance ().StartTracker (TrackerManager.TRACKER_TYPE_INSTANT);
			SensorDevice.GetInstance ().Start ();
			startTrackerDone = true;
		}

		TrackingState state = TrackerManager.GetInstance ().UpdateTrackingState ();
		TrackingResult trackingResult = state.GetTrackingResult ();

		if (trackingResult.GetCount () == 0) {
			foreach (var trackable in instantTrackables)
			{
				trackable.OnTrackFail ();
			}
			return;
		}	

		#if UNITY_EDITOR
		if (Input.GetMouseButton(0))
		{
			touchSumPositions[id] = TrackerManager.GetInstance().GetWorldPositionFromScreenCoordinate(Input.mousePosition);
		}
		#else
		if (Input.touchCount > 0)
		{
			UpdateTouchPositionDelta(id);
		}
		#endif

		for (int i = 0; i < instantTrackables.Count; i++) {
			Trackable track = trackingResult.GetTrackable (0);
			Matrix4x4 poseMatrix = track.GetPose () * Matrix4x4.Translate (touchSumPositions[i]);
			instantTrackables[i].OnTrackSuccess (track.GetId (), track.GetName (), poseMatrix);
		}
	}

	private void UpdateTouchPositionDelta(int id)
	{
		switch (Input.GetTouch(0).phase)
		{
			case TouchPhase.Began:
				touchToWorldPositions[id] = TrackerManager.GetInstance().GetWorldPositionFromScreenCoordinate(Input.GetTouch(0).position);
				break;
			
			case TouchPhase.Moved:
				Vector3 currentWorldPosition = TrackerManager.GetInstance().GetWorldPositionFromScreenCoordinate(Input.GetTouch(0).position);
				touchSumPositions[id] += (currentWorldPosition - touchToWorldPositions[id]);
				touchToWorldPositions[id] = currentWorldPosition;
			break;
		}
	}

	public void ClickNumber1()
	{
		id = 0;
	}

	public void ClickNumber2()
	{
		id = 1;
	}

	public void ClickNumber3()
	{
		id = 2;
	}

	void OnApplicationPause (bool pause)
	{
		if (pause) {
			SensorDevice.GetInstance ().Stop ();
			TrackerManager.GetInstance ().StopTracker ();
			startTrackerDone = false;
			StopCamera ();
		}
	}

	void OnDestroy ()
	{
		SensorDevice.GetInstance ().Stop ();
		TrackerManager.GetInstance ().StopTracker ();
		TrackerManager.GetInstance ().DestroyTracker ();
		StopCamera ();
	}

	void StartCamera ()
	{
		if (!cameraStartDone) {
			Debug.Log ("Unity StartCamera");
			ResultCode result = CameraDevice.GetInstance ().Start ();
			if (result == ResultCode.Success) {
				cameraStartDone = true;
				//CameraDevice.GetInstance().SetAutoWhiteBalanceLock(true);   // For ODG-R7 preventing camera flickering
			}
		}
	}

	void StopCamera ()
	{
		if (cameraStartDone) {
			Debug.Log ("Unity StopCamera");
			CameraDevice.GetInstance ().Stop ();
			cameraStartDone = false;
		}
	}

	public void OnClickStart ()
	{
		if (!findSurfaceDone) {
			TrackerManager.GetInstance ().FindSurface ();
			if (startBtnText != null) {
				startBtnText.text = "Stop Tracking";
			}
			findSurfaceDone = true;
			for (int i = 0 ; i < touchSumPositions.Count; i++)
			{
				touchSumPositions[i] = Vector3.zero;
			}
		} else {
			TrackerManager.GetInstance ().QuitFindingSurface ();
			if (startBtnText != null) {
				startBtnText.text = "Start Tracking";
			}
			findSurfaceDone = false;
		}
	}
}
