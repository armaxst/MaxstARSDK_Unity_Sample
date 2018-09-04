﻿/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace maxstAR
{
	/// <summary>
	/// Initialize system environment with app key, screen size and orientation
	/// </summary>
	public abstract class AbstractARManager : MonoBehaviour
	{
		private static AbstractARManager instance = null;

		internal static AbstractARManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<AbstractARManager>();
				}

				return instance;
			}
		}

		private string licenseKey = null;
		private int screenWidth = 0;
		private int screenHeight = 0;
		private ScreenOrientation orientation = ScreenOrientation.Unknown;
		private float nearClipPlane = 0.0f;
		private float farClipPlane = 0.0f;
		private Camera arCamera = null;
		private int cameraWidth = 0;
		private int cameraHeight = 0;

		/// <summary>
		/// Intialize sdk
		/// </summary>
		protected void Init()
		{
			InitInternal();

			if (Application.platform == RuntimePlatform.Android ||
				Application.platform == RuntimePlatform.IPhonePlayer)
			{
				MaxstAR.SetScreenOrientation((int)Screen.orientation);
			}
			else
			{
				MaxstAR.SetScreenOrientation((int)ScreenOrientation.LandscapeLeft);
			}

            MaxstAR.OnSurfaceChanged(Screen.width, Screen.height);
		}

		/// <summary>
		/// Set device orientation and surface size
		/// </summary>
		void InitInternal()
		{
			licenseKey = AbstractConfigurationScriptableObject.GetInstance().LicenseKey;

#if UNITY_IOS && !UNITY_EDITOR
            NativeAPI.init(licenseKey);
#endif

			// If CameraBackgroundBehaviour is not activated when start application, projection matrix 
			// can not be made because screen width and height isn't set properly.
			if (screenWidth != Screen.width || screenHeight != Screen.height)
			{
				screenWidth = Screen.width;
				screenHeight = Screen.height;
				MaxstAR.OnSurfaceChanged(screenWidth, screenHeight);
			}

			if (Application.platform == RuntimePlatform.Android ||
				Application.platform == RuntimePlatform.IPhonePlayer)
			{
				MaxstAR.SetScreenOrientation((int)Screen.orientation);
			}
			else
			{
				MaxstAR.SetScreenOrientation((int)ScreenOrientation.LandscapeLeft);
			}

			arCamera = GetComponent<Camera>();
		}

		void Update()
		{
			// If world center is target then tracking pose should be set to main camera's transform
			if (worldCenterMode == WorldCenterMode.TARGET)
			{
				TrackingState trackingState = TrackerManager.GetInstance().GetTrackingState();
				TrackingResult trackingResult = trackingState.GetTrackingResult();
				if (trackingResult.GetCount() > 0)
				{
					Trackable trackable = trackingResult.GetTrackable(0);
					Matrix4x4 targetPose = trackable.GetTargetPose().inverse;

					if (targetPose == Matrix4x4.zero)
						return;

					Quaternion rotation = Quaternion.Euler(90, 0, 0);
					Matrix4x4 m = Matrix4x4.TRS(new Vector3(0, 0, 0), rotation, new Vector3(1, 1, 1));
					targetPose = m * targetPose;

					Camera.main.transform.position = MatrixUtils.PositionFromMatrix(targetPose);
					Camera.main.transform.rotation = MatrixUtils.QuaternionFromMatrix(targetPose);
					Camera.main.transform.localScale = MatrixUtils.ScaleFromMatrix(targetPose);
				}
			}
		}

		/// <summary>
		/// Release sdk
		/// </summary>
		protected void Deinit()
		{
		}

		/// <summary>
		/// The world center mode defines what is the center in game view.
		/// If camera is world center then trackable's transform is changed when tracking success.
		/// If traget is world center then main camera's transform is chagned when tracking success.
		/// </summary>
		public enum WorldCenterMode
		{
			/// <summary>
			/// Camera is world center
			/// </summary>
			CAMERA = 0,
			/// <summary>
			/// Target is world center
			/// </summary>
			TARGET = 1
		}

		[SerializeField]
		private WorldCenterMode worldCenterMode;

		/// <summary>
		/// Get world center mode value
		/// </summary>
		public WorldCenterMode WorldCenterModeSetting
		{
			get
			{
				return worldCenterMode;
			}
		}

        public Camera GetARCamera() 
        {
            return arCamera;
        }

		/// <summary>
		/// Set world center mode
		/// </summary>
		/// <param name="worldCenterMode">World center enum value</param>
		public void SetWorldCenterMode(WorldCenterMode worldCenterMode)
		{
			this.worldCenterMode = worldCenterMode;
		}

        void OnPreRender()
		{
			if (screenWidth != Screen.width || screenHeight != Screen.height)
			{
				screenWidth = Screen.width;
				screenHeight = Screen.height;
				MaxstAR.OnSurfaceChanged(screenWidth, screenHeight);
			}

			if (orientation != Screen.orientation)
			{
				orientation = Screen.orientation;

				if (Application.platform == RuntimePlatform.Android ||
					Application.platform == RuntimePlatform.IPhonePlayer)
				{
					MaxstAR.SetScreenOrientation((int)orientation);
				}
			}

			if (nearClipPlane != arCamera.nearClipPlane || farClipPlane != arCamera.farClipPlane)
			{
				nearClipPlane = arCamera.nearClipPlane;
				farClipPlane = arCamera.farClipPlane;
			}

			int tempCameraWidth = CameraDevice.GetInstance().GetWidth();
			int tempCameraHeight = CameraDevice.GetInstance().GetHeight();

			if (tempCameraWidth == 0 || tempCameraHeight == 0)
			{
				return;
			}

			if (cameraWidth != tempCameraWidth || cameraHeight != tempCameraHeight)
			{
				cameraWidth = tempCameraWidth;
				cameraHeight = tempCameraHeight;
			}

            if (AbstractCameraBackgroundBehaviour.Instance != null)
            {
                TransformBackgroundPlane(arCamera, AbstractCameraBackgroundBehaviour.Instance.transform);
            }
      
			arCamera.projectionMatrix = CameraDevice.GetInstance().GetProjectionMatrix();
		}

		private void TransformBackgroundPlane(Camera camera, Transform planeTransform)
		{
			float widthRatio = (float)Screen.width / cameraWidth;
			float heightRatio = (float)Screen.height / cameraHeight;
			float farClipPlane = camera.farClipPlane * 0.90f;
			float tanFovWidth = (1.0f / (float)Screen.width) * (float)Screen.height;
			float frustumWidth = tanFovWidth * farClipPlane * camera.aspect;
			float viewWidth = (float)frustumWidth / Screen.width;
			float viewHeight = viewWidth * (widthRatio / heightRatio);
			float flipHorizontal = 1.0f;
			float flipVertical = 1.0f;

			if (CameraDevice.GetInstance().IsFlipHorizontal())
			{
				flipHorizontal = -1.0f;
			}

			if (CameraDevice.GetInstance().IsFlipVertical())
			{
				flipVertical = -1.0f;
			}

			if (MaxstARUtils.IsDirectXAPI())
			{
				flipHorizontal = -flipHorizontal;
			}

			if (widthRatio > heightRatio)
			{
				planeTransform.localScale = new Vector3(widthRatio * cameraWidth * viewWidth * flipVertical,
					widthRatio * cameraHeight * viewWidth * flipHorizontal, 0.1f);
				planeTransform.localPosition = new Vector3(0.0f, 0.0f, farClipPlane);
			}
			else
			{
				planeTransform.localScale = new Vector3(heightRatio * cameraWidth * viewHeight * flipVertical,
					heightRatio * cameraHeight * viewHeight * flipHorizontal, 0.1f);
				planeTransform.localPosition = new Vector3(0.0f, 0.0f, farClipPlane);
			}

            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D9 ||
                SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 ||
                SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12)
            {
                planeTransform.localScale = new Vector3(planeTransform.localScale.x, -planeTransform.localScale.y, planeTransform.localScale.z);
            }
		}
	}
}