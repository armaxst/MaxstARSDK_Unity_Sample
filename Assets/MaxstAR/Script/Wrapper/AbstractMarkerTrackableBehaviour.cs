/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.IO;
using JsonFx.Json;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using UnityEngine.Rendering;

namespace maxstAR
{
	/// <summary>
	/// Serve frame marker tracking. This is not working current version!
	/// </summary>
    public abstract class AbstractMarkerTrackableBehaviour : AbstractTrackableBehaviour
    {
        void Start()
        {
            Renderer myRenderer = GetComponent<Renderer>();
            myRenderer.enabled = false;
            Destroy(myRenderer);
        }

        /// <summary>
        /// Marker name list in temporary
        /// </summary>
        public readonly string[] TrackableList = new string[10]
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
        };

        /// <summary>
        /// Notify tracking file changed and change target image texture
        /// </summary>
        /// <param name="trackerFileName"></param>
        protected override void OnTrackerDataFileChanged(string trackerFileName)
        {
            Debug.Log("OnTrackerDataFileChanged : " + trackerFileName);
        }

        private void SetTargetTexture(string textureName)
        {
            if (File.Exists(textureName))
            {
                StartCoroutine(MaxstUtil.loadImageFromFileWithSizeAndTexture(Application.streamingAssetsPath + "/../../" + textureName, (width, height, texture) => {
                    Texture2D localTexture = texture;
                    if (localTexture)
                    {

                    MeshFilter imagePlaneMeshFilter = gameObject.GetComponent<MeshFilter>();
                    if (imagePlaneMeshFilter.sharedMesh == null)
                    {
                        imagePlaneMeshFilter.sharedMesh = new Mesh();
                        imagePlaneMeshFilter.sharedMesh.name = "ImagePlane";
                    }

                    float localWidth = (float)width * 0.5f;
                    float localHeight = (float)height * 0.5f;
                    imagePlaneMeshFilter.sharedMesh.vertices = new Vector3[]
                    {
                        new Vector3(-localWidth, -localHeight, 0.0f),
                        new Vector3(-localWidth, localHeight, 0.0f),
                        new Vector3(localWidth, -localHeight, 0.0f),
                        new Vector3(localWidth, localHeight, 0.0f)
                    };

                    imagePlaneMeshFilter.sharedMesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
                    imagePlaneMeshFilter.sharedMesh.uv = new Vector2[]
                    {
                            new Vector2(0, 0),
                            new Vector2(0, 1),
                            new Vector2(1, 0),
                            new Vector2(1, 1),
                    };

                    if (gameObject.GetComponent<MeshRenderer>().sharedMaterial == null)
                    {
                        gameObject.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Unlit/Texture"));
                    }

                    gameObject.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", texture);
                }
                }));
            }
        }
    }
}
