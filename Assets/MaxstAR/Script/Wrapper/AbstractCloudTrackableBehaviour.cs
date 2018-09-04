using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maxstAR;

namespace maxstAR {

    public enum CloudType
    {
        Cloud,
        User_Defined
    }

    public class AbstractCloudTrackableBehaviour : MonoBehaviour {

        [SerializeField]
        public CloudType CloudNameType = CloudType.Cloud;

        [SerializeField]
        public string CloudName = "_MaxstCloud_";


        void Start()
        {
            Renderer currentRenderer = GetComponent<Renderer>();
            currentRenderer.enabled = false;
            Destroy(currentRenderer);
        }

        public void OnTrackerCloudName(string trackerCloudName)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.OSXEditor)
            {
                MeshFilter imagePlaneMeshFilter = gameObject.GetComponent<MeshFilter>();
                if (imagePlaneMeshFilter.sharedMesh == null)
                {
                    imagePlaneMeshFilter.sharedMesh = new Mesh();
                    imagePlaneMeshFilter.sharedMesh.name = "ImagePlane";
                }

                float imageW = 1.0f;
                float imageH = 1.0f;

                float vertexWidth = imageW * 0.5f * 0.1f;
                float vertexHeight = imageH * 0.5f * 0.1f;
                imagePlaneMeshFilter.sharedMesh.vertices = new Vector3[]
                {
                            new Vector3(-vertexWidth, 0.0f, -vertexHeight),
                            new Vector3(-vertexWidth, 0.0f, vertexHeight),
                            new Vector3(vertexWidth, 0.0f, -vertexHeight),
                            new Vector3(vertexWidth, 0.0f, vertexHeight)
                };

                imagePlaneMeshFilter.sharedMesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
                imagePlaneMeshFilter.sharedMesh.uv = new Vector2[]
                {
                            new Vector2(0, 0),
                            new Vector2(0, 1),
                            new Vector2(1, 0),
                            new Vector2(1, 1),
                };

                Material cloudTrackerMaterial = null;
                if(trackerCloudName == "_MaxstCloud_") {
                    cloudTrackerMaterial = Resources.Load<Material>("MaxstAR/Contents/CloudTracker");
                } else {
                    cloudTrackerMaterial = Resources.Load<Material>("MaxstAR/Contents/DefinedTracker");
                }

                gameObject.GetComponent<MeshRenderer>().material = cloudTrackerMaterial;

            }
        }
    }
}
