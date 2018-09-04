/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using System;
using UnityEngine;
using System.Collections;

namespace maxstAR
{
    class AndroidEngine : IDisposable
    {
#if !UNITY_EDITOR
        private AndroidJavaObject currentActivity;
        private AndroidJavaClass maxstARClass;
#endif

        public AndroidEngine()
        {
#if !UNITY_EDITOR
            if (currentActivity == null || maxstARClass == null)
            {
                AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (currentActivity != null)
                {
					string licenseKey = ConfigurationScriptableObject.GetInstance().LicenseKey;
                    maxstARClass = new AndroidJavaClass("com.maxst.ar.MaxstARInitializer");
					maxstARClass.CallStatic("init", currentActivity, licenseKey);
					maxstARClass.CallStatic("setCameraApi", 1);
                }
                else
                {
                    Debug.Log("No Activity");
                }
            }
#endif
		}

        public void Dispose()
        {
#if !UNITY_EDITOR
            if (currentActivity != null && maxstARClass != null)
            {
                maxstARClass.CallStatic("deinit");
                currentActivity.Dispose();
                currentActivity = null;

                maxstARClass.Dispose();
                maxstARClass = null;
            }
#endif
        }
    }
}