using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

using maxstAR;

public class VisualSLAMSample : MonoBehaviour
{
	private string fileName = null;

	private bool cameraStartDone = false;

	void Start()
	{
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		fileName = Application.persistentDataPath + "/3dmap/Sample.3dmap";

		TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_SLAM);
		BackgroundRenderer.GetInstance().SetRenderingOption(
			BackgroundRenderer.RenderingOption.FEATURE_RENDERER,
			BackgroundRenderer.RenderingOption.PROGRESS_RENDERER,
			BackgroundRenderer.RenderingOption.SURFACE_MESH_RENDERER);
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			SceneStackManager.Instance.LoadPrevious();
		}

		StartCamera();

		EnableChildrenRenderer(false);

		TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();
		TrackingResult trackingResult = state.GetTrackingResult();
		if (trackingResult.GetCount() == 0)
		{
			return;
		}

		EnableChildrenRenderer(true);

		Trackable trackable = trackingResult.GetTrackable(0);

		Matrix4x4 poseMatrix = trackable.GetPose();
		transform.position = MatrixUtils.PositionFromMatrix(poseMatrix);
		transform.rotation = MatrixUtils.QuaternionFromMatrix(poseMatrix);
		transform.localScale = MatrixUtils.ScaleFromMatrix(poseMatrix);
	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			TrackerManager.GetInstance().StopTracker();
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
		BackgroundRenderer.GetInstance().SetRenderingOption(
			BackgroundRenderer.RenderingOption.FEATURE_RENDERER,
			BackgroundRenderer.RenderingOption.PROGRESS_RENDERER,
			BackgroundRenderer.RenderingOption.SURFACE_MESH_RENDERER);

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
		BackgroundRenderer.GetInstance().SetRenderingOption(
				BackgroundRenderer.RenderingOption.FEATURE_RENDERER,
				BackgroundRenderer.RenderingOption.PROGRESS_RENDERER,
				BackgroundRenderer.RenderingOption.SURFACE_MESH_RENDERER);

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

	void StartCamera()
	{
		if (!cameraStartDone)
		{
			Debug.Log("Unity StartCamera");
			ResultCode result = CameraDevice.GetInstance().Start();
			if (result == ResultCode.Success)
			{
				cameraStartDone = true;
				SensorDevice.GetInstance().Start();
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
			SensorDevice.GetInstance().Stop();
		}
	}
}