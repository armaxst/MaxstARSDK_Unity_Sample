using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace maxstAR {
    public class CloudTrackableBehaviour : AbstractCloudTrackableBehaviour
    {
        public void OnTrackSuccess(string id, string name, Matrix4x4 poseMatrix)
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

            // Enable renderers
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = true;
            }

            // Enable colliders
            foreach (Collider component in colliderComponents)
            {
                component.enabled = true;
            }

            transform.position = MatrixUtils.PositionFromMatrix(poseMatrix);
            transform.rotation = MatrixUtils.QuaternionFromMatrix(poseMatrix);
        }

        public void OnTrackFail()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

            // Disable renderer
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = false;
            }

            // Disable collider
            foreach (Collider component in colliderComponents)
            {
                component.enabled = false;
            }
        }
    }
}

