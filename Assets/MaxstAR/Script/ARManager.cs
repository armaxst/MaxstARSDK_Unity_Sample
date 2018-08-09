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