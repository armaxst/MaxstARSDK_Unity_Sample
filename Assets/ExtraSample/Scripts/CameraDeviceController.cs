using UnityEngine;
using System.Collections.Generic;
using System.Text;

using maxstAR;

public class CameraDeviceController : MonoBehaviour
{
	private bool cameraStartDone = false;

	void OnEnable()
	{
		StartCamera();
	}

	void OnDisable()
	{
		StopCamera();
	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			StopCamera();
		}
		else
		{
			StartCamera();
		}
	}

	void OnDestroy()
	{
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