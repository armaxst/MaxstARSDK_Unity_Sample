/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace maxstAR
{
    /// <summary>
    /// Control device sensor
    /// </summary>
    public class SensorDevice
    {
        private static SensorDevice instance = null;

        /// <summary>
        /// Get a SensorDevice instance.
        /// </summary>
        /// <returns>Return the SensorDevice instance</returns>
        public static SensorDevice GetInstance()
        {
            if (instance == null)
            {
                instance = new SensorDevice();
            }
            return instance;
        }

        private SensorDevice()
        {
        }

        /// <summary>
        /// Start device sensor
        /// </summary>
        public void Start()
        {
            NativeAPI.startSensor();
        }

        /// <summary>
        /// Stop device sensor
        /// </summary>
        public void Stop()
        {
            NativeAPI.stopSensor();
        }
    }
}