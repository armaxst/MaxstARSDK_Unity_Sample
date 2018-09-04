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
    [RequireComponent(typeof(MeshRenderer))]
    public class AbstractFeaturePointBehaviour : MonoBehaviour
    {
        public float FeatureSize = 1.0f;

        private Renderer meshRenderer;

        public Camera arCamera;
        private MeshFilter meshFilter = null;

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                Destroy(this.meshFilter);
            }
        }

        private void Generate(Vector3[] vertex)
        {

            if (this.meshFilter == null)
            {
                this.meshFilter = gameObject.AddComponent<MeshFilter>();
                meshFilter.mesh = new Mesh();
                meshFilter.mesh.name = "Feature Mesh Filter";
                meshRenderer = GetComponent<Renderer>();
                meshRenderer.material.SetFloat("_Type", 1.0f);
                meshRenderer.material.SetFloat("_FeatureSize", FeatureSize);
            }

            Matrix4x4 projectionMatrix = arCamera.projectionMatrix;

            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                Matrix4x4 orientationMatrix = Matrix4x4.Scale(new Vector3(1.0f, 1.0f, 1.0f));
                projectionMatrix = projectionMatrix * orientationMatrix;
            }
            else
            {
                Matrix4x4 orientationMatrix = Matrix4x4.Scale(new Vector3(1.0f, -1.0f, 1.0f));
                projectionMatrix = projectionMatrix * orientationMatrix;
            }

            meshRenderer.material.SetMatrix("projectionMatrix", projectionMatrix);

            Vector3[] vertices = new Vector3[vertex.Length << 2];

            for (int i = 0; i < vertex.Length; i++)
            {
                vertices[0 + (i << 2)] = vertex[i];
                vertices[1 + (i << 2)] = vertex[i];
                vertices[2 + (i << 2)] = vertex[i];
                vertices[3 + (i << 2)] = vertex[i];
            }

            meshFilter.mesh.vertices = vertices;

            int[] triangles = new int[vertex.Length * 6];

            for (int i = 0; i < vertex.Length; i++)
            {
                triangles[0 + (i * 6)] = 0 + (i << 2);
                triangles[1 + (i * 6)] = 1 + (i << 2);
                triangles[2 + (i * 6)] = 2 + (i << 2);
                triangles[3 + (i * 6)] = 3 + (i << 2);
                triangles[4 + (i * 6)] = 2 + (i << 2);
                triangles[5 + (i * 6)] = 1 + (i << 2);
            }

            meshFilter.mesh.triangles = triangles;

            Vector2[] uv = new Vector2[vertex.Length << 2];

            for (int i = 0; i < vertex.Length; i++)
            {
                uv[0 + (i << 2)] = new Vector2(0, 0);
                uv[1 + (i << 2)] = new Vector2(1, 0);
                uv[2 + (i << 2)] = new Vector2(0, 1);
                uv[3 + (i << 2)] = new Vector2(1, 1);
            }

            meshFilter.mesh.uv = uv;
        }

        private Vector3[] convertFloatToVertex3(float[] vertex, int count)
        {
            if (count == 0)
            {
                return null;
            }

            Vector3[] tempVertex = new Vector3[count];

            for (int i = 0; i < count; i++)
            {
                float temp1 = vertex[0 + (i * 3)];
                float temp2 = vertex[1 + (i * 3)];
                float temp3 = vertex[2 + (i * 3)];


                if (temp1 > 100000 || temp1 < -100000) {
                    temp1 = 0.0f;
                }

                if (temp2 > 100000 || temp2 < -100000) {
                    temp2 = -100.0f;
                }

                if (temp3 > 100000 || temp3 < -100000) {
                    temp3 = 0.0f;
                }
                                     
                tempVertex[i] = new Vector3(temp1, temp2, temp3);
            }

            return tempVertex;
        }

        void Start()
        {
            GetComponent<Renderer>().material.renderQueue = 1600;
            transform.localScale = arCamera.transform.localScale;
            transform.localRotation = arCamera.transform.localRotation;
            transform.localPosition = arCamera.transform.localPosition;

            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D9 ||
                SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 ||
                SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12 ||
                SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore)
            {
                transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
            }
        }

        private void Update()
        {
            if (this.meshFilter != null)
            {
                meshFilter.mesh.Clear();
            }

            GuideInfo guideInfo = TrackerManager.GetInstance().GetGuideInfo();

            int featureCount = guideInfo.GetFeatureCount();
            if (featureCount == 0)
            {
                return;
            }

            float[] featureBuffer = guideInfo.GetFeatureBuffer();

            if (featureBuffer.Length > 0)
            {
                Vector3[] vertexVector3Array = convertFloatToVertex3(featureBuffer, featureCount);

                Generate(vertexVector3Array);
            }
        }

    }
}