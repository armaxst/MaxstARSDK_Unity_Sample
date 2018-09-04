/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace maxstAR
{
	/// <summary>
	/// Map created by Visual SLAM renderer
	/// </summary>
	public class MapRendererBehaviour : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] groupObjects = null;

		[SerializeField]
		private GameObject[] imageObjects = null;

		[SerializeField]
		private GameObject[] cameraObjects = null;

		[SerializeField]
		private GameObject[] meshObjects = null;

		[SerializeField]
		private GameObject viewCameraObject = null;

		[SerializeField]
		private Vector2 gameViewSize = Vector2.zero;

		[SerializeField]
		private float textureWidth = 0;

		[SerializeField]
		private float textureHeight = 0;

		private const float vx = 2500.0f;
		private const float vy = 1875.0f;
		private const float vz = 5000.0f;

		private const float scaleFactor = 0.85f;

		internal void Create(Vector3[] vertices, Matrix4x4[] cameraMatrices, Material[] materials)
		{
			int materialsLength = materials.Length;
			if (materialsLength == 0)
			{
				return;
			}

			transform.localRotation = Camera.main.transform.localRotation;

			textureWidth = materials[0].mainTexture.width;
			textureHeight = materials[0].mainTexture.height;

			GameObject mapViewerCameraObject = Resources.Load<GameObject>("MaxstAR/Contents/MapViewerCamera");

			groupObjects = new GameObject[materialsLength];
			cameraObjects = new GameObject[materialsLength];
			imageObjects = new GameObject[materialsLength];
			meshObjects = new GameObject[materialsLength];

			for (int i = 0; i < materialsLength; i++)
			{
				groupObjects[i] = new GameObject();
				cameraObjects[i] = Instantiate(mapViewerCameraObject);
				meshObjects[i] = new GameObject();
				imageObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Quad);

				groupObjects[i].name = "Keyframe" + i;
				cameraObjects[i].name = "Camera";
				meshObjects[i].name = "Mesh";
				imageObjects[i].name = "Image";

				groupObjects[i].transform.parent = transform;
				cameraObjects[i].transform.parent = groupObjects[i].transform;
				meshObjects[i].transform.parent = groupObjects[i].transform;
				imageObjects[i].transform.parent = groupObjects[i].transform;

				cameraObjects[i].layer = LayerMask.NameToLayer("Ignore Raycast");
				meshObjects[i].AddComponent(typeof(MeshFilter));
				meshObjects[i].AddComponent(typeof(MeshRenderer));
				meshObjects[i].GetComponent<MeshFilter>().mesh = CreateMapViewerMesh(i, vertices);
				meshObjects[i].GetComponent<Renderer>().material = materials[i];
				imageObjects[i].GetComponent<Renderer>().material = materials[i];

				groupObjects[i].transform.localPosition = Vector3.zero;
				groupObjects[i].transform.localRotation = Quaternion.identity;
				groupObjects[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

				Matrix4x4 m = MatrixUtils.GetUnityPoseMatrix(cameraMatrices[i]);
				cameraObjects[i].transform.localRotation = MatrixUtils.QuaternionFromMatrix(m);
				cameraObjects[i].transform.localPosition = MatrixUtils.PositionFromMatrix(m);
				cameraObjects[i].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

				meshObjects[i].transform.localPosition = Vector3.zero;
				meshObjects[i].transform.localRotation = Quaternion.identity;
				meshObjects[i].transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

				imageObjects[i].transform.localPosition = new Vector3(0.0f, 0.0f, 10.0f);
				imageObjects[i].transform.localRotation = Quaternion.identity;
				imageObjects[i].transform.localScale = new Vector3(10.0f, 10.0f * -textureHeight / textureWidth, 10.0f);
			}

			gameViewSize = GetGameViewSize();
			viewCameraObject = new GameObject();
			viewCameraObject.name = "ViewCamera";
			viewCameraObject.transform.parent = transform;
			Camera camera = viewCameraObject.AddComponent<Camera>();
			camera.ResetProjectionMatrix();
			camera.nearClipPlane = 0.03f;
			camera.farClipPlane = 10000.0f;
			camera.aspect = gameViewSize.x / gameViewSize.y;
			camera.clearFlags = CameraClearFlags.SolidColor;
			camera.backgroundColor = Color.black;
			camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Ignore Raycast"));

			///////////////////////// TODO: check /////////////////////////
			if (gameViewSize.x > gameViewSize.y * textureWidth / textureHeight)
			{
				camera.fieldOfView = 2.0f * Mathf.Atan2(vy, vz) * Mathf.Rad2Deg;
			}
			else
			{
				camera.fieldOfView = 2.0f * Mathf.Atan2(vx * gameViewSize.y / gameViewSize.x, vz) * Mathf.Rad2Deg;
			}

			viewCameraObject.transform.localPosition = cameraObjects[0].transform.localPosition;
			viewCameraObject.transform.localRotation = cameraObjects[0].transform.localRotation;
		}

		private Mesh CreateMapViewerMesh(int idx, Vector3[] vertices)
		{
			int verticesLength = vertices.Length;
			if (verticesLength == 0)
			{
				return null;
			}

			int size = MapViewer.GetInstance().Create(idx);
			if (size == 0)
			{
				return null;
			}

			int[] indices = new int[size];
			float[] texCoords = new float[verticesLength * 2];

			MapViewer.GetInstance().GetIndices(out indices[0]);
			MapViewer.GetInstance().GetTexCoords(out texCoords[0]);

			Vector2[] uvs = new Vector2[verticesLength];
			for (int j = 0; j < verticesLength; j++)
			{
				uvs[j][0] = texCoords[2 * j + 0];
				uvs[j][1] = texCoords[2 * j + 1];
			}

			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = indices;
			mesh.uv = uvs;
			mesh.RecalculateNormals();

			return mesh;
		}

		internal void Clear()
		{
			int childCount = gameObject.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
			}

			if (viewCameraObject != null)
			{
				DestroyImmediate(viewCameraObject);
			}

			if (groupObjects != null)
			{
				int groupObjectsLength = groupObjects.Length;
				for (int i = 0; i < groupObjectsLength; i++)
				{
					DestroyImmediate(groupObjects[i]);
				}

				groupObjects = null;
			}

			if (imageObjects != null)
			{
				int imageObjectsLength = imageObjects.Length;
				for (int i = 0; i < imageObjectsLength; i++)
				{
					DestroyImmediate(imageObjects[i]);
				}

				imageObjects = null;
			}

			if (cameraObjects != null)
			{
				int cameraObjectsLength = cameraObjects.Length;
				for (int i = 0; i < cameraObjectsLength; i++)
				{
					DestroyImmediate(cameraObjects[i]);
				}

				cameraObjects = null;
			}

			if (meshObjects != null)
			{
				int meshObjectLength = meshObjects.Length;
				for (int i = 0; i < meshObjectLength; i++)
				{
					DestroyImmediate(meshObjects[i]);
				}

				meshObjects = null;
			}
		}

		internal void SetActiveImageObject(int index)
		{
			if (cameraObjects == null || imageObjects == null)
			{
				return;
			}

			int cameraObjectsLength = cameraObjects.Length;
			for (int i = 0; i < cameraObjectsLength; i++)
			{
				cameraObjects[i].SetActive(true);
			}

			int imageObjectsLength = imageObjects.Length;
			for (int i = 0; i < imageObjectsLength; i++)
			{
				imageObjects[i].SetActive(false);
			}

			imageObjects[index].SetActive(true);

			viewCameraObject.transform.localPosition = Vector3.zero;
			viewCameraObject.transform.localRotation = Quaternion.identity;
		}

		internal void SetActiveMeshObject(int index)
		{
			if (meshObjects == null)
			{
				return;
			}

			int meshObjectsLength = meshObjects.Length;
			for (int i = 0; i < meshObjectsLength; i++)
			{
				meshObjects[i].SetActive(false);
			}

			meshObjects[index].SetActive(true);

			viewCameraObject.transform.localPosition = cameraObjects[index].transform.localPosition;
			viewCameraObject.transform.localRotation = cameraObjects[index].transform.localRotation;
		}

		internal void SetDeactiveImageObjects()
		{
			if (imageObjects == null)
			{
				return;
			}

			int imageObjectsLength = imageObjects.Length;
			for (int i = 0; i < imageObjectsLength; i++)
			{
				if (imageObjects[i] != null)
				{
					imageObjects[i].SetActive(false);
				}
			}
		}

		internal void SetDeactiveMeshObjects()
		{
			if (meshObjects == null)
			{
				return;
			}

			int meshObjectLength = meshObjects.Length;
			for (int i = 0; i < meshObjectLength; i++)
			{
				if (meshObjects[i] != null)
				{
					meshObjects[i].SetActive(false);
				}
			}
		}

		private void OnRenderObject()
		{
			if (viewCameraObject != null)
			{
				Vector2 tempGameViewSize = GetGameViewSize();
				if (gameViewSize != tempGameViewSize)
				{
					gameViewSize = tempGameViewSize;
					Camera camera = viewCameraObject.GetComponent<Camera>();
					camera.aspect = gameViewSize.x / gameViewSize.y;

					///////////////////////// TODO: check /////////////////////////
					if (gameViewSize.x > gameViewSize.y * textureWidth / textureHeight)
					{
						camera.fieldOfView = 2.0f * Mathf.Atan2(vy, vz) * Mathf.Rad2Deg;
					}
					else
					{
						camera.fieldOfView = 2.0f * Mathf.Atan2(vx * gameViewSize.y / gameViewSize.x, vz) * Mathf.Rad2Deg;
					}
				}
			}
		}

		private Vector2 GetGameViewSize()
		{
			System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
			System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			System.Object Res = GetSizeOfMainGameView.Invoke(null, null);

			return (Vector2)Res;
		}
	}
}
