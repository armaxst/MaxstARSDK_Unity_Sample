/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;

namespace maxstAR
{
	public class ARManager : AbstractARManager
	{
#if UNITY_ANDROID
		private AndroidEngine androidEngine = null;
#endif

		void Awake()
		{
            base.Init();

#if UNITY_ANDROID
			androidEngine = new AndroidEngine();
#endif

#if UNITY_EDITOR
			// Find arsdk_version.txt
			// if not exist file create file and write arsdk version
			// if file exist write
			System.IO.File.WriteAllText(System.IO.Path.Combine(Application.streamingAssetsPath, "arsdk_version.txt"), MaxstAR.GetVersion());
#endif
        }

		void OnDestroy()
		{
			base.Deinit();

#if UNITY_ANDROID
			androidEngine = null;
#endif
		}
	}
}