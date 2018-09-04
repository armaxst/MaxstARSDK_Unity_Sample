/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Linq;
using System;
using System.Runtime.InteropServices;

namespace maxstAR
{
    /// <summary>
    /// Handles native background rendering. Background rendering includes camera image
    /// </summary>
    public class AbstractCameraBackgroundBehaviour : MonoBehaviour
    {
        private const string RGBShaderName = "MaxstAR/RGBBackground";
        private const string Yuv420ShaderName = "MaxstAR/Yuv420Background";
        private const string AndroidYuv420ShaderName = "MaxstAR/AndroidYuv420Background";
        private const string Yuv420_888ShaderName = "MaxstAR/Yuv420_888Background";
        private const string Yuv420NonRG16ShaderName = "MaxstAR/Yuv420NonRG16Background";

        //public Color32 pauseColor = new Color32(255, 255, 255, 255);
        private static AbstractCameraBackgroundBehaviour instance = null;
        private int prevImageIndex = int.MaxValue;
        private bool textureCreateDone = false;

        internal static AbstractCameraBackgroundBehaviour Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AbstractCameraBackgroundBehaviour>();
                }

                return instance;
            }
        }

        private bool keepRendering = false;

        private Texture2D rgbTexture = null;
        private Texture2D yTexture = null;
        private Texture2D uvTexture = null;

        private Texture2D uTexture = null;
        private Texture2D vTexture = null;

        private Texture2D u16bitTexture = null;
        private Texture2D v16bitTexture = null;

        bool rg16SupportGpuAndUnityVersionCheck = false;

        void Awake()
        {
            Debug.Log("Current graphics device type is " + SystemInfo.graphicsDeviceType + ".");

            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                MaxstAR.SetScreenOrientation((int)Screen.orientation);
            }
            else
            {
                MaxstAR.SetScreenOrientation((int)ScreenOrientation.LandscapeLeft);
            }

            rg16SupportGpuAndUnityVersionCheck = false;

            try
            {
                if (SystemInfo.SupportsTextureFormat((TextureFormat)62))
				{
					Debug.Log("RG16 Support");
					rg16SupportGpuAndUnityVersionCheck = true;
				}
				else
				{
					if (Application.platform == RuntimePlatform.OSXEditor)
					{
						Debug.Log("RG16 Support");
						rg16SupportGpuAndUnityVersionCheck = true;
					}
					else
					{
						Debug.Log("RG16 not Support");
						rg16SupportGpuAndUnityVersionCheck = false;
					}
				}
			}
			catch (Exception e)  // CS0168
			{
				Debug.Log("Exception! " + e.Message);
				rg16SupportGpuAndUnityVersionCheck = false;
			}
		}

		void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				StopRendering();
			}
			else
			{
				StartRendering();
			}
		}

		void OnEnable()
		{
			StartRendering();
		}

		void OnDisable()
		{
			StopRendering();
		}

		void OnDestroy()
		{
			StopRendering();
		}

		private void StartRendering()
		{
			//GetComponent<Renderer>().material.SetFloat("pauseMode", 0.0f);
			Debug.Log("StartRendering");
			keepRendering = true;
		}

		private void StopRendering()
		{
			//GetComponent<Renderer>().material.SetFloat("pauseMode", 1.0f);
			Debug.Log("StopRendering");
			keepRendering = false;
		}

		private void CreateCameraTexture(int imageWidth, int imageHeight, ColorFormat imageFormat)
		{
			Debug.Log("Create texture. Color format : " + imageFormat);

			if (imageFormat == ColorFormat.RGB888)
			{
				rgbTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
				GetComponent<Renderer>().material = new Material(Shader.Find(RGBShaderName));
				GetComponent<Renderer>().material.mainTexture = rgbTexture;
				Debug.Log("Texture Make RGB888");
			}
			else if (imageFormat == ColorFormat.YUV420sp && rg16SupportGpuAndUnityVersionCheck)
			{
				yTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.Alpha8, false);
				uvTexture = new Texture2D(imageWidth / 2, imageHeight / 2, (TextureFormat)62, false);

				if (Application.platform == RuntimePlatform.Android)
				{
                    GetComponent<Renderer>().material = new Material(Shader.Find(AndroidYuv420ShaderName));
				}
				else
				{
					GetComponent<Renderer>().material = new Material(Shader.Find(Yuv420ShaderName));
				}

				yTexture.filterMode = FilterMode.Point;
				uvTexture.filterMode = FilterMode.Point;
				yTexture.wrapMode = TextureWrapMode.Clamp;
				uvTexture.wrapMode = TextureWrapMode.Clamp;

				GetComponent<Renderer>().material.SetTexture("_YTex", yTexture);
				GetComponent<Renderer>().material.SetTexture("_UVTex", uvTexture);
				Debug.Log("Texture Make YUV420sp");
			}
			else if (imageFormat == ColorFormat.YUV420sp && !rg16SupportGpuAndUnityVersionCheck)
			{
				yTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.Alpha8, false);
				uTexture = new Texture2D(imageWidth / 2, imageHeight / 2, TextureFormat.Alpha8, false);
				vTexture = new Texture2D(imageWidth / 2, imageHeight / 2, TextureFormat.Alpha8, false);

				yTexture.filterMode = FilterMode.Point;
				uTexture.filterMode = FilterMode.Point;
				vTexture.filterMode = FilterMode.Point;
				yTexture.wrapMode = TextureWrapMode.Clamp;
				uTexture.wrapMode = TextureWrapMode.Clamp;
				vTexture.wrapMode = TextureWrapMode.Clamp;
				GetComponent<Renderer>().material = new Material(Shader.Find(Yuv420NonRG16ShaderName));
				GetComponent<Renderer>().material.SetTexture("_YTex", yTexture);
				if (Application.platform == RuntimePlatform.Android)
				{
					GetComponent<Renderer>().material.SetTexture("_UTex", vTexture);
					GetComponent<Renderer>().material.SetTexture("_VTex", uTexture);
				}
				else
				{
					GetComponent<Renderer>().material.SetTexture("_UTex", uTexture);
					GetComponent<Renderer>().material.SetTexture("_VTex", vTexture);
				}

				Debug.Log("Texture Make YUV420sp like YUV420");
			}
			else if (imageFormat == ColorFormat.YUV420_888 && rg16SupportGpuAndUnityVersionCheck)
			{
				yTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.Alpha8, false);
				u16bitTexture = new Texture2D(imageWidth / 2, imageHeight / 2, (TextureFormat)62, false);
				v16bitTexture = new Texture2D(imageWidth / 2, imageHeight / 2, (TextureFormat)62, false);

				if (Application.platform == RuntimePlatform.Android)
				{
					GetComponent<Renderer>().material = new Material(Shader.Find(Yuv420_888ShaderName));
				}
				else
				{
					GetComponent<Renderer>().material = new Material(Shader.Find(Yuv420_888ShaderName));
				}

				yTexture.filterMode = FilterMode.Point;
				yTexture.wrapMode = TextureWrapMode.Clamp;

				u16bitTexture.filterMode = FilterMode.Point;
				u16bitTexture.wrapMode = TextureWrapMode.Clamp;
	
				v16bitTexture.filterMode = FilterMode.Point;
				v16bitTexture.wrapMode = TextureWrapMode.Clamp;

				GetComponent<Renderer>().material.SetTexture("_YTex", yTexture);
				if (Application.platform == RuntimePlatform.Android)
				{
					GetComponent<Renderer>().material.SetTexture("_VTex", u16bitTexture);
					GetComponent<Renderer>().material.SetTexture("_UTex", v16bitTexture);
				}
				else
				{
					GetComponent<Renderer>().material.SetTexture("_UTex", u16bitTexture);
					GetComponent<Renderer>().material.SetTexture("_VTex", v16bitTexture);
				}
				Debug.Log("Texture Make YUV420_888");
			}
			else if (imageFormat == ColorFormat.YUV420_888 && !rg16SupportGpuAndUnityVersionCheck)
			{
				yTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.Alpha8, false);
				uTexture = new Texture2D(imageWidth / 2, imageHeight / 2, TextureFormat.Alpha8, false);
				vTexture = new Texture2D(imageWidth / 2, imageHeight / 2, TextureFormat.Alpha8, false);

				yTexture.filterMode = FilterMode.Point;
				uTexture.filterMode = FilterMode.Point;
				vTexture.filterMode = FilterMode.Point;
				yTexture.wrapMode = TextureWrapMode.Clamp;
				uTexture.wrapMode = TextureWrapMode.Clamp;
				vTexture.wrapMode = TextureWrapMode.Clamp;
				GetComponent<Renderer>().material = new Material(Shader.Find(Yuv420NonRG16ShaderName));
				GetComponent<Renderer>().material.SetTexture("_YTex", yTexture);
				if (Application.platform == RuntimePlatform.Android)
				{
					GetComponent<Renderer>().material.SetTexture("_UTex", vTexture);
					GetComponent<Renderer>().material.SetTexture("_VTex", uTexture);
				}
				else
				{
					GetComponent<Renderer>().material.SetTexture("_UTex", uTexture);
					GetComponent<Renderer>().material.SetTexture("_VTex", vTexture);
				}

				Debug.Log("Texture Make YUV420_888 like YUV420");
			}

			Mesh mesh = GetComponent<MeshFilter>().mesh;
			Vector3[] vertices = mesh.vertices;
			Vector2[] uvs = new Vector2[vertices.Length];

			uvs[0].x = 0; uvs[0].y = 1;
			uvs[1].x = 1; uvs[1].y = 0;
			uvs[2].x = 1; uvs[2].y = 1;
			uvs[3].x = 0; uvs[3].y = 0;
			mesh.uv = uvs;

			//GetComponent<Renderer>().material.SetColor("_PauseColor", pauseColor);
			GetComponent<Renderer>().material.renderQueue = 1500;
			Debug.Log("Create background texture done");
		}

		/// <summary>
		/// Render camera background image with tracked frame
		/// </summary>
		/// <param name="state">TrackingState</param>
		public void UpdateCameraBackgroundImage(TrackingState state)
		{
			if (!keepRendering)
			{
				return;
			}

			TrackedImage image = state.GetImage(!rg16SupportGpuAndUnityVersionCheck);

			if (prevImageIndex == image.GetIndex())
			{
				return;
			}

			prevImageIndex = image.GetIndex();

			if (image.GetWidth() == 0 && image.GetHeight() == 0)
			{
				return;
			}

			if (!textureCreateDone)
			{
				CreateCameraTexture(image.GetWidth(), image.GetHeight(), image.GetFormat());
				textureCreateDone = true;
			}

			UpdateInternal(image);
		}

		void UpdateInternal(TrackedImage image)
		{
			IntPtr cameraFramePtr = image.GetDataPtr();

			switch (image.GetFormat())
			{
				case ColorFormat.RGB888:
					if (rgbTexture != null)
					{
						rgbTexture.LoadRawTextureData(cameraFramePtr, image.GetWidth() * image.GetHeight() * 3);
						rgbTexture.Apply();
					}
					break;

				case ColorFormat.YUV420sp:
					if (uvTexture != null) // It means that RG16 texture format is supported
					{
						IntPtr yPtr;
						IntPtr uvPtr;
						unsafe
						{
							byte* pointer = (byte*)cameraFramePtr.ToPointer();
							yPtr = (IntPtr)pointer;
							pointer += image.GetWidth() * image.GetHeight();
							uvPtr = (IntPtr)pointer;
						}

						yTexture.LoadRawTextureData(yPtr, image.GetWidth() * image.GetHeight());
						yTexture.Apply();
						uvTexture.LoadRawTextureData(uvPtr, image.GetWidth() * image.GetHeight() / 2);
						uvTexture.Apply();
					}
					else if (yTexture != null && uTexture != null && vTexture != null)
					{
						IntPtr yPtr;
						IntPtr uPtr;
						IntPtr vPtr;

						int uvSize = image.GetWidth() * image.GetHeight() / 2;

						unsafe
						{
							byte* pointer = (byte*)cameraFramePtr.ToPointer();
							yPtr = (IntPtr)pointer;
							pointer += image.GetWidth() * image.GetHeight();
							uPtr = (IntPtr)pointer;
							pointer += uvSize / 2;
							vPtr = (IntPtr)pointer;
						}

						yTexture.LoadRawTextureData(yPtr, image.GetWidth() * image.GetHeight());
						yTexture.Apply();

						uTexture.LoadRawTextureData(uPtr, uvSize / 2);
						uTexture.Apply();

						vTexture.LoadRawTextureData(vPtr, uvSize / 2);
						vTexture.Apply();
					}
					break;

				case ColorFormat.YUV420_888:
					if (u16bitTexture != null)  // It means that RG16 texture format is supported
					{
						IntPtr yPtr;
						IntPtr uPtr;
						IntPtr vPtr;
						unsafe
						{
							byte* pointer = (byte*)cameraFramePtr.ToPointer();
							yPtr = (IntPtr)pointer;
							pointer += image.GetWidth() * image.GetHeight();
							uPtr = (IntPtr)pointer;
							pointer += image.GetWidth() * image.GetHeight() / 2;
							vPtr = (IntPtr)pointer;
						}

						yTexture.LoadRawTextureData(yPtr, image.GetWidth() * image.GetHeight());
						yTexture.Apply();

						u16bitTexture.LoadRawTextureData(uPtr, image.GetWidth() * image.GetHeight() / 2);
						u16bitTexture.Apply();

						v16bitTexture.LoadRawTextureData(vPtr, image.GetWidth() * image.GetHeight() / 2);
						v16bitTexture.Apply();
					}
					else if (yTexture != null && uTexture != null && vTexture != null)
					{
						IntPtr yPtr;
						IntPtr uPtr;
						IntPtr vPtr;

						int uSize = image.GetWidth() * image.GetHeight() / 2;

						unsafe
						{
							byte* pointer = (byte*)cameraFramePtr.ToPointer();
							yPtr = (IntPtr)pointer;
							pointer += image.GetWidth() * image.GetHeight();
							uPtr = (IntPtr)pointer;
							pointer += uSize;
							vPtr = (IntPtr)pointer;
						}

						yTexture.LoadRawTextureData(yPtr, image.GetWidth() * image.GetHeight());
						yTexture.Apply();

						uTexture.LoadRawTextureData(uPtr, uSize / 2);
						uTexture.Apply();

						vTexture.LoadRawTextureData(vPtr, uSize / 2);
						vTexture.Apply();
					}
					break;
			}
		}

		internal bool RenderingEnabled()
		{
			return keepRendering;
		}
	}
}