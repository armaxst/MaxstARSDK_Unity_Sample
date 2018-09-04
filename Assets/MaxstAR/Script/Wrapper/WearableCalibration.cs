/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace maxstAR
{
    /// <summary>
    /// API for wearable calibration.
    /// </summary>
	public class WearableCalibration
    {
        /// <summary>
        /// Types of HMD eyes
        /// </summary>
        public enum EyeType
        {
            /// <summary>
            /// Left HMD eye
            /// </summary>
            EYE_LEFT = 0,

            /// <summary>
            /// Right HMD eye
            /// </summary>
            EYE_RIGHT = 1,

            /// <summary>
            /// Number of eyes
            /// </summary>
            EYE_NUM = 2,
        };

        /// <summary>
        /// Wearable Device Type
        /// </summary>
        public enum WearableType
        {
            /// <summary>
            /// None Wearable Device
            /// </summary>
            None = 0,

            /// <summary>
            /// Optical See-throught Wearable Device
            /// </summary>
            OpticalSeeThrough = 1,
        };

		/// <summary>
		/// Default constructor
		/// </summary>
		public WearableCalibration()
		{

		}

		/// <summary>
		/// Get active calibration profile name
		/// </summary>
		public string activeProfile { get; set; }

		private GameObject eyeLeft = null;
		private GameObject eyeRight = null;

        /// <summary>
        /// Confirm that the HMD unit is initialized.
        /// </summary>
        /// <returns>Result of device initialize</returns>
        public bool IsActivated()
        {
            return NativeAPI.WearableCalibration_isActivated();
        }

        /// <summary>
        /// Initialize the HMD device.
        /// </summary>
        /// <param name="modelName">Device name</param>
        /// <param name="width">Device screen width</param>
        /// <param name="height">Device screen height</param>
        /// <returns>Result of device initialize</returns>
        public bool Init(string modelName, int width, int height)
        {
            bool result = false;

            result = NativeAPI.WearableCalibration_init(modelName);

            Debug.Log("Wearable Init is " + result.ToString());

            AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaClass javaClass = new AndroidJavaClass("com.maxst.ar.WearableCalibration");
            AndroidJavaObject javaObject = javaClass.CallStatic<AndroidJavaObject>("getInstance");

            if (!javaObject.Call<bool>("readActiveProfile", currentActivity, modelName))
            {
                Debug.Log("Read Default Profile.");
            }

			activeProfile = javaObject.Call<string>("getActiveProfileName");

			//javaObject.Call("setSurfaceSize", width, height);

            currentActivity.Dispose();
            javaUnityPlayer.Dispose();
            return result;
        }

        /// <summary>
        /// Deinitialize the HMD device.
        /// </summary>
		public void Deinit()
        {
            NativeAPI.WearableCalibration_deinit();

            if (eyeLeft != null)
			{
				GameObject.DestroyImmediate(eyeLeft);
			}

			if (eyeRight != null)
			{
				GameObject.DestroyImmediate(eyeRight);
			}
        }

        /// <summary>
        /// Get HMD screen viewport.
        /// </summary>
        /// <param name="eyeType">Types of HMD eyes</param>
        /// <returns>Viewport array(float type array, size 4).</returns>
        public float[] GetViewport(EyeType eyeType)
        {
            float[] viewport = new float[4];
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                viewport[0] = 0.0f;
                viewport[1] = 0.0f;
                viewport[2] = 0.5f;
                viewport[3] = 1.0f;
            }
            else
            {
                viewport[0] = 0.5f;
                viewport[1] = 0.0f;
                viewport[2] = 0.5f;
                viewport[3] = 1.0f;
            }
            return viewport;
        }

        /// <summary>
        /// Get calibrated HMD projection matrix.
        /// </summary>
        /// <param name="eyeType">Types of HMD eyes</param>
        /// <returns>Projection matrix array(float type array, size 16).</returns>
        public float[] GetProjectionMatrix(EyeType eyeType)
        {
            float[] projection = new float[16];
            NativeAPI.WearableCalibration_getProjectionMatrix(projection, (int)eyeType);
            return projection;
        }

		/// <summary>
		/// Create sterescopic camera
		/// </summary>
		/// <param name="cameraTransform"></param>
		public void CreateWearableEye(Transform cameraTransform)
		{
			if (eyeLeft == null)
			{
				eyeLeft = new GameObject("EyeLeft");
				eyeLeft.AddComponent<Camera>();
			}

			if (eyeRight == null)
			{
				eyeRight = new GameObject("EyeRight");
				eyeRight.AddComponent<Camera>();
			}

			eyeLeft.transform.parent = cameraTransform;
			eyeRight.transform.parent = cameraTransform;

			eyeLeft.transform.localPosition = Vector3.zero;
			eyeLeft.transform.localRotation = Quaternion.identity;
			eyeLeft.transform.localScale = Vector3.one;

			eyeRight.transform.localPosition = Vector3.zero;
			eyeRight.transform.localRotation = Quaternion.identity;
			eyeRight.transform.localScale = Vector3.one;

			Camera mainCamera = cameraTransform.GetComponent<Camera>();
			Camera leftCamera = eyeLeft.GetComponent<Camera>();
			Camera rightCamera = eyeRight.GetComponent<Camera>();

			CameraClearFlags clearFlags = mainCamera.clearFlags;
			Color backgroundColor = mainCamera.backgroundColor;
			float depth = mainCamera.depth + 1;
			float nearClipPlane = mainCamera.nearClipPlane;
			float farClipPlane = mainCamera.farClipPlane;

			float[] projectionMatrixEyeLeftPtr = GetProjectionMatrix(WearableCalibration.EyeType.EYE_LEFT);
			float[] projectionMatrixEyeRightPtr = GetProjectionMatrix(WearableCalibration.EyeType.EYE_RIGHT);

			Matrix4x4 projectionEyeLeft = MatrixUtils.ConvertGLProjectionToUnityProjection(projectionMatrixEyeLeftPtr);
			Matrix4x4 projectionEyeRight = MatrixUtils.ConvertGLProjectionToUnityProjection(projectionMatrixEyeRightPtr);

			leftCamera.clearFlags = clearFlags;
			leftCamera.backgroundColor = backgroundColor;
			leftCamera.depth = depth;
			leftCamera.nearClipPlane = nearClipPlane;
			leftCamera.farClipPlane = farClipPlane;
			leftCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
			leftCamera.projectionMatrix = projectionEyeLeft;

			rightCamera.clearFlags = clearFlags;
			rightCamera.backgroundColor = backgroundColor;
			rightCamera.depth = depth;
			rightCamera.nearClipPlane = nearClipPlane;
			rightCamera.farClipPlane = farClipPlane;
			rightCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
			rightCamera.projectionMatrix = projectionEyeRight;
		}
    }
}