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
	public class MarkerGroupBehaviour : MonoBehaviour
    {
        [SerializeField]
        private float markerGroupSize = 1.0f;

        private float defaultSize = 1.0f;

        [SerializeField]
        private bool applyAll = true;

        public float MarkerGroupSize
        {
            get
            {
                return this.markerGroupSize;
            }

            set
            {
                this.markerGroupSize = value;
            }
        }

        public bool ApplyAll
        {
            get
            {
                return applyAll;
            }

            set
            {
                applyAll = value;
            }
        }


        private void Start()
        {
            if (applyAll)
            {
                TrackerManager.GetInstance().AddTrackerData("All : " + markerGroupSize);
            }
            else
            {
                TrackerManager.GetInstance().AddTrackerData("All : " + defaultSize);
            }
        }
    }
}
