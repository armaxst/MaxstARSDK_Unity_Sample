using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.UI;

using maxstAR;

public class ExtraInstantTrackerSample : MonoBehaviour
{
	[SerializeField]
	private Text startBtnText = null;

	[SerializeField]
	private Material lineMaterial = null;

	[SerializeField]
	private RotationController rotationController;

	[SerializeField]
	private ZoomInOut zoomInOut;

	private Vector3 touchToWorldPosition = Vector3.zero;
	private Vector3 touchSumPosition = Vector3.zero;

	private bool startTrackerDone = false;
	private bool cameraStartDone = false;
	private bool findSurfaceDone = false;

	private InstantTrackableBehaviour instantTrackable = null;
	private InstantPlaneGrid instantPlaneGrid;

    private Matrix4x4 planMatrix = Matrix4x4.identity;

	void Start()
	{
        Input.simulateMouseWithTouches = true;

		instantTrackable = FindObjectOfType<InstantTrackableBehaviour>();
		if (instantTrackable == null)
		{
			return;
		}

		instantTrackable.OnTrackFail();
		instantPlaneGrid = new InstantPlaneGrid(lineMaterial);
	}

	void Update()
	{

		if (instantTrackable == null)
		{
			return;
		}

		StartCamera();

		if (!startTrackerDone)
		{
			TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_INSTANT);
			SensorDevice.GetInstance().Start();
			startTrackerDone = true;
		}

		TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();
		TrackingResult trackingResult = TrackerManager.GetInstance().GetTrackingResult(state);
		if (trackingResult.GetCount() == 0)
		{
			instantTrackable.OnTrackFail();
			instantPlaneGrid.EnableDrawing(false);
			return;
		}

		instantPlaneGrid.EnableDrawing(true);

        Trackable trackable = trackingResult.GetTrackable(0);
        planMatrix = trackable.GetPose();
        Matrix4x4 poseMatrix = trackable.GetPose() * Matrix4x4.Translate(touchSumPosition);
        instantTrackable.OnTrackSuccess(trackable.GetId(), trackable.GetName(), poseMatrix);

        if (Input.touchCount > 0 && !rotationController.getRotationState() && !zoomInOut.getScaleState())
		{
            UpdateTouchDelta(Input.GetTouch(0).position);
		}
	}

	void OnRenderObject()
	{
        instantPlaneGrid.Draw(planMatrix);
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
