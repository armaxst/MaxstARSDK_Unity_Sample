﻿/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;

namespace maxstAR
{
    /// <summary>
    /// To check the special case of android file is in assets folder
    /// </summary>
    public enum StorageType
    {
        /// <summary>
        /// File is located in android assets folder
        /// </summary>
        StreamingAssets,
        /// <summary>
        /// Normal file
        /// </summary>
        AbsolutePath
    }

    /// <summary>
    /// Parent class of all TrackableBehaviour. Save tracking file's id (uuid), name, path etc.
    /// All setting datas are for editor mode and runtime tracking result.
    /// </summary>
    public abstract class AbstractTrackableBehaviour : MonoBehaviour
    {
        [SerializeField]
        private StorageType storageType = StorageType.StreamingAssets;

        [SerializeField]
        private UnityEngine.Object trackerDataFileObject = null;

        [SerializeField]
        private string trackerDataFileName = null;

        [SerializeField]
        private string trackableId = null;

        [SerializeField]
        private string trackableName = null;

        //[SerializeField]
        //protected int trackableIndex = 0;

        /// <summary>
        /// If file is located in android assets folder or not
        /// </summary>
        public StorageType StorageType
        {
            get { return storageType; }
            set { storageType = value; }
        }

        /// <summary>
        /// Save tracking file object name in StreamingAssets folder (For editor mode)
        /// </summary>
        public UnityEngine.Object TrackerDataFileObject
        {
            get { return trackerDataFileObject; }
            set
            {
                trackerDataFileObject = value;
            }
        }

        /// <summary>
        /// Tracking file name
        /// </summary>
        public string TrackerDataFileName
        {
            get { return trackerDataFileName; }
            set
            {
                trackerDataFileName = value;
                OnTrackerDataFileChanged(trackerDataFileName);
            }
        }

        /// <summary>
        /// Tracking file uuid. This value is addressed in tracking file.
        /// </summary>
        public string TrackableId
        {
            get { return trackableId; }
            set { trackableId = value; }
        }

        /// <summary>
        /// Tracking file name only without extention.
        /// </summary>
        public string TrackableName
        {
            get { return trackableName; }
            set { trackableName = value; }
        }

        /// <summary>
        /// To notify tracker file changed (Editor mode only)
        /// </summary>
        /// <param name="trackerFileName"></param>
        protected virtual void OnTrackerDataFileChanged(string trackerFileName)
        {
        }

        /// <summary>
        /// Common interface to notify this target lost tracking. Child class can override this method
        /// to receive tracking lost event
        /// </summary>
        public virtual void OnTrackFail()
        {
        }

        /// <summary>
        /// Common interface to notify this target tracking success. Child class can override this method
        /// to receive tracking success event
        /// </summary>
        /// <param name="id">target's id (uuid)</param>
        /// <param name="name">target's name (map file name without extension)</param>
        /// <param name="poseMatrix">tracked pose matrix</param>
        public virtual void OnTrackSuccess(string id, string name, Matrix4x4 poseMatrix)
        {
        }
    }
}