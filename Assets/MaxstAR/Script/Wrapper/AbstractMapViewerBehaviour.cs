/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Text;
using JsonFx.Json;
using System;
using System.Runtime.InteropServices;

namespace maxstAR
{
	/// <summary>
	/// Handles 3D map file for authoring. Map controller includes mappoint controller and keyframe controller.
	/// </summary>
	public abstract class AbstractMapViewerBehaviour : MonoBehaviour
	{
		[SerializeField]
		private int keyframeIndex = 0;
		/// <summary>
		/// Change the keyframe number of imported 3D map.
		/// </summary>
		public int KeyframeIndex
		{
			get { return keyframeIndex; }
			set
			{
				keyframeIndex = value;
				UpdateMapViewer();
			}
		}

		[SerializeField]
		private bool showMesh = false;
		/// <summary>
		/// Reconstruct map point cloud to 3D for authoring.
		/// </summary>
		public bool ShowMesh
		{
			get { return showMesh; }
			set
			{
				showMesh = value;
				UpdateMapViewer();
			}
		}

		[SerializeField]
		private bool autoCamera = false;
		/// <summary>
		/// Change the view point of the scene view. scene viewpoint in editor. Select the keyframe closest to the scene view.
		/// </summary>
		public bool AutoCamera
		{
			get { return autoCamera; }
			set
			{
				autoCamera = value;
				UpdateMapViewer();
			}
		}

		[SerializeField]
		private int maxKeyframeCount = 0;
		/// <summary>
		/// Get the number of keyframes for the imported map.
		/// </summary>
		public int MaxKeyframeCount
		{
			get { return maxKeyframeCount; }
			set { maxKeyframeCount = value; }
		}

		[SerializeField]
		private bool transparent = false;
		/// <summary>
		/// Changes the loaded map to translucent state.
		/// </summary>
		public bool Transparent
		{
			get { return transparent; }
			set
			{
				transparent = value;
				UpdateMapViewer();

				SetTransparent(transparent);
			}
		}

		[SerializeField]
		private Quaternion[] cameraRotations = null;

		[SerializeField]
		private Matrix4x4[] cameraMatrices = null;

		[SerializeField]
		private Vector3[] vertices = null;

		[SerializeField]
		private Material[] materials = null;

		[SerializeField]
		private MapRendererBehaviour mapRendererBehaviour = null;

		/// <summary>
		/// Read map file and create keyframe and mappoint as Unity3d object.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns>MapViewer gameobject</returns>
		public bool Load(string fileName)
		{
			if (!ReadMap(fileName))
			{
				return false;
			}

			Debug.Log("Read Json Success");

			mapRendererBehaviour = gameObject.GetComponent<MapRendererBehaviour>();
			if (mapRendererBehaviour == null)
			{
				mapRendererBehaviour = gameObject.AddComponent<MapRendererBehaviour>();
			}

			mapRendererBehaviour.Clear();
			mapRendererBehaviour.Create(vertices, cameraMatrices, materials);

			SetTransparent(transparent);
			UpdateMapViewer();

			return true;
		}

		private bool ReadMap(string fileName)
		{
			MapViewer.GetInstance().Deinitialize();
			if (!MapViewer.GetInstance().Initialize(fileName))
			{
				return false;
			}

			IntPtr jsonPtr = MapViewer.GetInstance().GetJson();
			string json = Marshal.PtrToStringAnsi(jsonPtr);

			if (json.Length < 1)
			{
				Debug.Log("Map is not opened");
				return false;
			}

			Map3D map3D = JsonReader.Deserialize<Map3D>(json);

			int width = map3D.width;
			int height = map3D.height;
			maxKeyframeCount = map3D.imageCount;

			vertices = new Vector3[map3D.vertexCount];
			for (int i = 0; i < map3D.vertexCount; i++)
			{
				vertices[i] = new Vector3(map3D.vertices[i].x, -map3D.vertices[i].y, map3D.vertices[i].z);
			}

			cameraMatrices = new Matrix4x4[maxKeyframeCount];
			for (int i = 0; i < maxKeyframeCount; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					cameraMatrices[i][j] = map3D.poseMatrices[i][j];
				}
				cameraMatrices[i] = cameraMatrices[i].inverse;
			}

			cameraRotations = new Quaternion[maxKeyframeCount];
			for (int i = 0; i < maxKeyframeCount; i++)
			{
				cameraRotations[i] = MatrixUtils.InvQuaternionFromMatrix(cameraMatrices[i]);
				Vector3 tempR = cameraRotations[i].eulerAngles;
				tempR.x = -tempR.x;
				tempR.y = -tempR.y;
				cameraRotations[i] = Quaternion.Euler(tempR);
			}

			Shader color = Shader.Find("Unlit/Texture");
			materials = new Material[maxKeyframeCount];
			for (int i = 0; i < maxKeyframeCount; i++)
			{
				materials[i] = new Material(color);
				materials[i].mainTexture = GetCameraTexture(i, width, height);
			}

			return true;
		}

		private void SetTransparent(bool transparent)
		{
			if (materials == null)
			{
				return;
			}

			Shader alpha = Shader.Find("Unlit/Transparent");
			Shader color = Shader.Find("Unlit/Texture");

			int materialsLength = materials.Length;
			for (int i = 0; i < materialsLength; i++)
			{
				if (transparent == true)
				{
					materials[i].shader = alpha;
				}
				else
				{
					materials[i].shader = color;
				}
			}
		}

		private Texture2D GetCameraTexture(int index, int width, int height)
		{
			int size = width * height;
			byte[] image = new byte[size];
			MapViewer.GetInstance().GetImage(index, out image[0]);

			byte alpha = 127;
			byte[] rawTextureData = new byte[size * 4];
			for (int j = 0; j < size; j++)
			{
				rawTextureData[j * 4 + 0] = alpha;
				rawTextureData[j * 4 + 1] = image[j];
				rawTextureData[j * 4 + 2] = image[j];
				rawTextureData[j * 4 + 3] = image[j];
			}

			Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
			texture.LoadRawTextureData(rawTextureData);
			texture.Apply();

			return texture;
		}

		/// <summary>
		/// Updated the control changes of the authoring tool in the scene view.
		/// </summary>
		public void UpdateMapViewer()
		{
			if (showMesh == true)
			{
				mapRendererBehaviour.SetDeactiveImageObjects();
				mapRendererBehaviour.SetActiveMeshObject(keyframeIndex);
			}
			else
			{
				mapRendererBehaviour.SetDeactiveMeshObjects();
				mapRendererBehaviour.SetActiveImageObject(keyframeIndex);
			}
		}

		/// <summary>
		/// Select the keyframe closest to the scene view.
		/// </summary>
		/// <param name="position">The position coordinate of the scene view.</param>
		/// <param name="quaternion">The rotation coordinate of the scene view.</param>
		public void ApplyViewCamera(Vector3 position, Quaternion quaternion)
		{
			const float distanceWeight = 1.0f;
			const float angleWeight = 10.0f;

			float minWeightSum = 999999.0f;

			Vector3 tPosition = position;
			Quaternion tQuaternion = quaternion;

			for (int i = 0; i < maxKeyframeCount; i++)
			{
				Matrix4x4 cameraMatrix = cameraMatrices[i];
				Vector3 cameraPosition = new Vector3(cameraMatrix.m03, cameraMatrix.m13, cameraMatrix.m23);

				float calcDistance = Vector3.Distance(tPosition, cameraPosition);
				float calcAngle = Quaternion.Angle(tQuaternion, cameraRotations[i]);
				float calcWeightSum = distanceWeight * calcDistance + angleWeight * (Mathf.Abs(calcAngle));

				if (minWeightSum > calcWeightSum)
				{
					minWeightSum = calcWeightSum;
					keyframeIndex = i;
				}
			}
		}
	}
}