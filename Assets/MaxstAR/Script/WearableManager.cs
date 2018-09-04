using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace maxstAR
{
	public class WearableManager
	{
		private static WearableManager Instance = null;
		public static WearableManager GetInstance()
		{
			if (Instance == null)
			{
				Instance = new WearableManager();
			}

			return Instance;
		}

		private WearableDeviceController wearableDeviceController;
		private WearableCalibration wearableCalibration;

		private WearableManager() 
		{
			wearableDeviceController = new WearableDeviceController();
			wearableDeviceController.Init();
			bool supportedDevice = wearableDeviceController.IsSupportedWearableDevice();
			string deviceName = wearableDeviceController.GetModelName();
			Debug.Log(deviceName);
			if (supportedDevice)
			{
				Debug.Log("Supported wearable device");
				wearableCalibration = new WearableCalibration();
				if (wearableCalibration.Init(deviceName, Screen.width, Screen.height))
				{
					string profileName = wearableCalibration.activeProfile;
					Debug.Log("WearableCalibration init success. Profile name : " + profileName);
				}
			}
		}

		public WearableDeviceController GetDeviceController()
		{
			return wearableDeviceController;
		}

		public WearableCalibration GetCalibration()
		{
			return wearableCalibration;
		}
	}
}
