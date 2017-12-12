using UnityEngine;
using System.IO;
using System.Collections.Generic;

using maxstAR;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SurfaceMeshBehaviour : MonoBehaviour
{
	[SerializeField]
	private Texture2D texture = null;

	private List<Vector3> vertexList = new List<Vector3>();
	private List<int> indexList = new List<int>();
	private List<Vector2> uvList = new List<Vector2>();
	private Mesh surface = null;
	private GameObject hitObject = null;
	private Vector3 cast = new Vector3(0.0f, 0.0f, 1.0f);
	private Vector3 contentSize = new Vector3(0.1f, 0.1f, 0.1f);

	void Start()
	{
		if (texture)
		{
			GetComponent<Renderer>().material.mainTexture = texture;
			GetComponent<Renderer>().material.shader = Shader.Find("Custom/StandardSurface");
		}
		else
		{
			GetComponent<Renderer>().material.color = Color.clear;
			GetComponent<Renderer>().material.shader = Shader.Find("Custom/TransparentSurface");
		}
	}

	void Update()
	{
		TrackingState state = TrackerManager.GetInstance().GetTrackingState();
		TrackingResult trackingResult = TrackerManager.GetInstance().GetTrackingResult(state);
		if (trackingResult.GetCount() > 0)
		{
			SurfaceMesh surfaceMesh = TrackerManager.GetInstance().GetSurfaceMesh();

			//float progress = surfaceMesh.GetInitializingProgress();
			float[] vertices = surfaceMesh.GetVertexBuffer();
			short[] indices = surfaceMesh.GetIndexBuffer();

			vertexList.Clear();
			uvList.Clear();
			for (int i = 0; i < surfaceMesh.GetVertexCount(); i++)
			{
				vertexList.Add(new Vector3(vertices[3 * i + 0], -vertices[3 * i + 1], vertices[3 * i + 2]));
				uvList.Add(new Vector2(vertices[3 * i + 0] + 0.5f, -vertices[3 * i + 1] + 0.5f));
			}

			indexList.Clear();
			for (int i = 0; i < surfaceMesh.GetIndexCount(); i++)
			{
				indexList.Add(indices[i]);
			}

			if (surface == null)
			{
				surface = new Mesh();
			}

			surface.Clear();
			surface.SetVertices(vertexList);
			surface.SetTriangles(indexList, 0);
			surface.SetUVs(0, uvList);
			surface.RecalculateNormals();
			surface.MarkDynamic();

			GetComponent<MeshFilter>().sharedMesh = surface;
			GetComponent<MeshCollider>().sharedMesh = surface;
		}
	}

	void OnGUI()
	{
		cast.z = 1.0f;

#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0))
		{
			cast = Input.mousePosition;
		}
#else
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began)
			{
				cast = touch.position;
			}
		}
#endif

		if (cast.z != 1.0f)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(cast);
			if (Physics.Raycast(ray, out hit))
			{
				if (hitObject == null)
				{
					hitObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
					hitObject.transform.parent = transform;
					hitObject.transform.localScale = contentSize;
				}

				hitObject.transform.position = hit.point;
				hitObject.transform.rotation = Quaternion.LookRotation(hit.normal);
			}
		}
	}

	void OnDisable()
	{
		if (surface)
		{
			surface.Clear();
		}
	}

	void OnDestroy()
	{
		if (surface)
		{
			surface.Clear();
		}
	}
}