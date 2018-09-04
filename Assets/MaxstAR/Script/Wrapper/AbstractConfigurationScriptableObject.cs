/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;

namespace maxstAR
{
    /// <summary>
    /// Control the configuration asset file
    /// </summary>
    public class AbstractConfigurationScriptableObject : ScriptableObject
    {
		/// <summary>
		/// App signature key
		/// </summary>
		[SerializeField]
		public string LicenseKey = null;

		/// <summary>
		/// Select webcam type
		/// </summary>
        [SerializeField]
		public int WebcamType = 0;

		/// <summary>
		/// Select camera type
		/// </summary>
        [SerializeField]
		public CameraDevice.CameraType CameraType = CameraDevice.CameraType.Rear;

		/// <summary>
		/// Select camera resolution
		/// </summary>
        [SerializeField]
		public CameraDevice.CameraResolution CameraResolution = CameraDevice.CameraResolution.Resolution640x480;

		/// <summary>
		/// Select wearable device type
		/// </summary>
        [SerializeField]
		public WearableCalibration.WearableType WearableType = WearableCalibration.WearableType.None;

        private static AbstractConfigurationScriptableObject configuration = null;

        /// <summary>
        /// Get configuration asset instance
        /// </summary>
        /// <returns>Configuration scriptable object</returns>
        public static AbstractConfigurationScriptableObject GetInstance()
        {
            if (configuration == null)
            {
                configuration = Resources.Load<AbstractConfigurationScriptableObject>("MaxstAR/Configuration");

                // To Create Asset
                //configuration = CreateInstance<ConfigurationScriptableObject>();
                //AssetDatabase.CreateAsset(configuration, "Assets/Resources/Configuration.asset");
                //AssetDatabase.Refresh();
            }

            if (configuration == null)
            {
                Debug.LogError("Configuration is null");
            }

            return configuration;
        }
    }
}