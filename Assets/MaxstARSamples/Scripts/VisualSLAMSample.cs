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

public class VisualSLAMSample : ARBehaviour
{
	private string fileName = null;

    public Slider progressSlider;

    private CameraBackgroundBehaviour cameraBackgroundBehaviour = null;

    private Vector3 translatePosition = new Vector3(0, 0, 0);

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
        //Debug.Log("width : " + Screen. + "  height : " + Screen.currentResolution.height);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        Screen.orientation = ScreenOrientation.LandscapeLeft;

		fileName = Application.persistentDataPath + "/3dmap/Sample.3dmap";

		StartCamera();
		TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_SLAM);
	}

	void Update()
	{
		EnableChildrenRenderer(false);

		progressSlider.value = TrackerManager.GetInstance ().GetGuideInfo ().GetInitializingProgress ();

		TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();

        cameraBackgroundBehaviour.UpdateCameraBackgroundImage(state);

		TrackingResult trackingResult = state.GetTrackingResult();
		if (trackingResult.GetCount() == 0)
		{
			return;
		}

        if(Input.GetMouseButton(0)) {
            moveContent(Input.mousePosition);
        }

		EnableChildrenRenderer(true);

		Trackable trackable = trackingResult.GetTrackable(0);

		Matrix4x4 poseMatrix = trackable.GetPose();
		transform.position = MatrixUtils.PositionFromMatrix(poseMatrix);
		transform.rotation = MatrixUtils.QuaternionFromMatrix(poseMatrix);
		transform.localScale = MatrixUtils.ScaleFromMatrix(poseMatrix);

        transform.Translate(translatePosition);
	}

    void moveContent(Vector2 movePosition) 
    {
        Vector3 worldPosition = TrackerManager.GetInstance().GetWorldPositionFromScreenCoordinate(movePosition);
        translatePosition = worldPosition;
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
			TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_SLAM);
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
	}

	public void QuitFindingSurface()
	{
		TrackerManager.GetInstance().QuitFindingSurface();
	}

	public void SaveMap()
	{
		Directory.CreateDirectory(Application.persistentDataPath + "/3dmap");
		SaveSurfaceData(fileName);
		Debug.Log("Save To " + fileName);
	}

	public void LoadMap()
	{
		TrackerManager.GetInstance().StopTracker();
		TrackerManager.GetInstance().DestroyTracker();

		TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_OBJECT);

		TrackerManager.GetInstance().AddTrackerData(fileName);

		TrackerManager.GetInstance().LoadTrackerData();

		Debug.Log("Load From " + fileName);
	}

	void SaveSurfaceData(string fileName)
	{
		SurfaceThumbnail surfaceThumbnail = TrackerManager.GetInstance().SaveSurfaceData(fileName);
		int width = surfaceThumbnail.GetWidth();
		int height = surfaceThumbnail.GetHeight();
		byte[] thumbnailData = surfaceThumbnail.GetData();

		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				int index = y * width + x;
				tex.SetPixel(x, height - y, new Color(thumbnailData[index] / 255.0f, thumbnailData[index] / 255.0f, thumbnailData[index] / 255.0f));
			}
		}
		tex.Apply();

		string imageFileName = fileName.Substring(0, fileName.LastIndexOf("."));
		FileStream fileSave = new FileStream(imageFileName + ".png", FileMode.Create);
		BinaryWriter binary = new BinaryWriter(fileSave);

		binary.Write(tex.EncodeToPNG());
		fileSave.Close();
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

	private void StartCameraInternal()
	{
		StartCamera();
		SensorDevice.GetInstance().Start();
	}

	private void StopCameraInternal()
	{
		StopCamera();
		SensorDevice.GetInstance().Stop();
	}
}