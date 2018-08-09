using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maxstAR;

public class ARBehaviour : MonoBehaviour
{
	private bool cameraStartDone = false;

	protected void Init()
	{
		BackKeyHandler backKeyHandler = BackKeyHandler.Instance;
	}

	public void OnClickBackButton()
	{
		SceneStackManager.Instance.LoadPrevious();
	}

	public void StartCamera()
	{
		if (!cameraStartDone)
		{
			ResultCode result = CameraDevice.GetInstance().Start();
			cameraStartDone = true;
			Debug.Log("Unity StartCamera. result : " + result);
		}
	}

	public void StopCamera()
	{
		if (cameraStartDone)
		{
			ResultCode result = CameraDevice.GetInstance().Stop();
			Debug.Log("Unity StopCamera. result : " + result);
			cameraStartDone = false;
		}
	}
}
