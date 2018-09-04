/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using System;
using UnityEngine;
using System.Collections;

namespace maxstAR
{
	public class WearableDeviceController
	{
		private AndroidJavaObject javaObject = null;

		public WearableDeviceController() { }

		public void Init()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

				AndroidJavaClass javaClass = new AndroidJavaClass("com.maxst.ar.WearableDeviceController");
				javaObject = javaClass.CallStatic<AndroidJavaObject>("createDeviceController", currentActivity);

				currentActivity.Dispose();
				javaUnityPlayer.Dispose();
				javaClass.Dispose();
			}
		}

		public void DeInit()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				javaObject.Dispose();
				javaObject = null;
			}
		}

		public bool IsSupportedWearableDevice()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return javaObject.Call<bool>("isSupportedWearableDevice");
			}
			else
			{
				return false;
			}
		}

		public string GetModelName()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return javaObject.Call<string>("getModelName");
			}
			else
			{
				return "";
			}
		}

		public void SetStereoMode(bool toggle)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				javaObject.Call("setStereoMode", toggle);
			}
		}

		public bool IsStereoEnabled()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return javaObject.Call<bool>("isStereoEnabled");
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Check if this device support 3d mode as screen split mode or surface extended mode.
		/// Epson HMD is side by side type and ODG-R7 is screen extend type
		/// </summary>
		/// <returns></returns>
		public bool IsSideBySideType()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return javaObject.Call<bool>("isSideBySideType");
			}
			else
			{
				return false;
			}
		}
	}
}