using System;
using System.Runtime.InteropServices;

namespace maxstAR
{
    internal class NativeAPI
    {
#if UNITY_IOS && !UNITY_EDITOR
        const string MaxstARLibName = "__Internal";
#else
        const string MaxstARLibName = "MaxstAR";
#endif

        #region -- System setting
        [DllImport(MaxstARLibName)]
        public static extern void init(string licenseKey);
        #endregion

        #region -- MaxstAR setting
        [DllImport(MaxstARLibName)]
        public static extern void getVersion(byte[] versionBytes, int bytesLength);

        [DllImport(MaxstARLibName)]
        public static extern void onSurfaceChanged(int surfaceWidth, int surfaceHeight);

        [DllImport(MaxstARLibName)]
        public static extern void setScreenOrientation(int orientation);
        #endregion

        #region -- Camera device setting
        [DllImport(MaxstARLibName)]
        public static extern int CameraDevice_start(int cameraId, int preferredWidth, int preferredHeight);

        [DllImport(MaxstARLibName)]
        public static extern int CameraDevice_stop();

        [DllImport(MaxstARLibName)]
        public static extern bool CameraDevice_setNewFrame(byte[] data, int length, int width, int height, int format);

        [DllImport(MaxstARLibName)]
        public static extern bool CameraDevice_setNewFramePtr(ulong data, int length, int width, int height, int format);

        [DllImport(MaxstARLibName)]
        public static extern bool CameraDevice_setNewFrameAndTimestamp(byte[] data, int length, int width, int height, int format, ulong timestamp);

        [DllImport(MaxstARLibName)]
        public static extern bool CameraDevice_setNewFramePtrAndTimestamp(ulong data, int length, int width, int height, int format, ulong timestamp);

        [DllImport(MaxstARLibName)]
        public static extern bool CameraDevice_setFocusMode(int focusMode);

        [DllImport(MaxstARLibName)]
        public static extern bool CameraDevice_setFlashLightMode(bool toggle);

        [DllImport(MaxstARLibName)]
        public static extern bool CameraDevice_setAutoWhiteBalanceLock(bool toggle);

        [DllImport(MaxstARLibName)]
        public static extern bool CameraDevice_flipVideo(int direction, bool toggle);

        [DllImport(MaxstARLibName)]
        public static extern int CameraDevice_getParamList();

        [DllImport(MaxstARLibName)]
        public static extern int CameraDevice_Param_getKeyLength(int index);

        [DllImport(MaxstARLibName)]
        public static extern void CameraDevice_Param_getKey(int index, byte[] key);

        [DllImport(MaxstARLibName)]
        public static extern bool CameraDevice_setBoolTypeParameter(string key, bool boolValue);

        [DllImport(MaxstARLibName)]
        public static extern bool CameraDevice_setIntTypeParameter(string key, int intValue);

        [DllImport(MaxstARLibName)]
        public static extern bool CameraDevice_setRangeTypeParameter(string key, int min, int max);

        [DllImport(MaxstARLibName)]
        public static extern bool CameraDevice_setStringTypeParameter(string key, string stringValue);

        [DllImport(MaxstARLibName)]
        public static extern int CameraDevice_getWidth();

        [DllImport(MaxstARLibName)]
        public static extern int CameraDevice_getHeight();

        [DllImport(MaxstARLibName)]
        public static extern void CameraDevice_getProjectionMatrix(float[] matrix);

        [DllImport(MaxstARLibName)]
        public static extern bool CameraDevice_checkCameraMove(ulong TrackedImage_cPtr);
        #endregion

        #region -- TrackerManager settings
        [DllImport(MaxstARLibName)]
        public static extern void TrackerManager_startTracker(int trackerMask);

        [DllImport(MaxstARLibName)]
        public static extern void TrackerManager_stopTracker();

        [DllImport(MaxstARLibName)]
        public static extern void TrackerManager_destroyTracker();

        [DllImport(MaxstARLibName)]
        public static extern void TrackerManager_addTrackerData(string trackingFileName, bool isAndroidAssetFile = false);

        [DllImport(MaxstARLibName)]
        public static extern void TrackerManager_removeTrackerData(string trackingFileName = "");

        [DllImport(MaxstARLibName)]
        public static extern void TrackerManager_loadTrackerData();

        [DllImport(MaxstARLibName)]
        public static extern void TrackerManager_setTrackingOption(int option);

        [DllImport(MaxstARLibName)]
        public static extern bool TrackerManager_isTrackerDataLoadCompleted();

        [DllImport(MaxstARLibName)]
        public static extern ulong TrackerManager_updateTrackingState();

        [DllImport(MaxstARLibName)]
        public static extern void TrackerManager_findSurface();

        [DllImport(MaxstARLibName)]
        public static extern void TrackerManager_quitFindingSurface();

        [DllImport(MaxstARLibName)]
        public static extern ulong TrackerManager_getGuideInfo();

        [DllImport(MaxstARLibName)]
        public static extern ulong TrackerManager_saveSurfaceData(string outputFileName);

        [DllImport(MaxstARLibName)]
        public static extern void TrackerManager_getWorldPositionFromScreenCoordinate(float[] screen, float[] world);

        [DllImport(MaxstARLibName)]
        public static extern bool CloudManager_GetFeatureClient(ulong TrackedImage_cPtr, byte[] descriptData, int[] resultLength);

        [DllImport(MaxstARLibName)]
        public static extern string CloudManager_JWTEncode(string secretKey, string payloadString);
        #endregion

        #region -- TrackingResult
        [DllImport(MaxstARLibName)]
        public static extern int TrackingResult_getCount(ulong TrackingResult_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern ulong TrackingResult_getTrackable(ulong TrackingResult_cPtr, int index);
        #endregion

        #region -- Trackable
        [DllImport(MaxstARLibName)]
        public static extern void Trackable_getId(ulong Trackable_cPtr, byte[] id);

        [DllImport(MaxstARLibName)]
        public static extern void Trackable_getName(ulong Trackable_cPtr, byte[] name);

        [DllImport(MaxstARLibName)]
        public static extern void Trackable_getCloudName(ulong Trackable_cPtr, byte[] cloudName);

        [DllImport(MaxstARLibName)]
        public static extern void Trackable_getCloudMeta(ulong Trackable_cPtr, byte[] cloudMeta, int[] length);

        [DllImport(MaxstARLibName)]
        public static extern void Trackable_getPose(ulong Trackable_cPtr, float[] pose);

        [DllImport(MaxstARLibName)]
        public static extern float Trackable_getWidth(ulong Trackable_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern float Trackable_getHeight(ulong Trackable_cPtr);
        #endregion

        #region -- TrackingState
        [DllImport(MaxstARLibName)]
        public static extern ulong TrackingState_getTrackingResult(ulong TrackingState_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern ulong TrackingState_getImage(ulong TrackingState_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern int TrackingState_getCodeScanResultLength(ulong TrackingState_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern void TrackingState_getCodeScanResult(ulong TrackingState_cPtr, byte[] result, int length);
        #endregion

        #region -- Guide Info
        [DllImport(MaxstARLibName)]
        public static extern float GuideInfo_getInitializingProgress(ulong GuideInfo_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern int GuideInfo_getKeyframeCount(ulong GuideInfo_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern int GuideInfo_getFeatureCount(ulong GuideInfo_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern void GuideInfo_getFeatureBuffer(ulong GuideInfo_cPtr, float[] data, int length);
        #endregion

        #region -- SurfaceThumbnail
        [DllImport(MaxstARLibName)]
        public static extern int SurfaceThumbnail_getWidth(ulong SurfaceThumbnail_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern int SurfaceThumbnail_getHeight(ulong SurfaceThumbnail_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern int SurfaceThumbnail_getLength(ulong SurfaceThumbnail_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern int SurfaceThumbnail_getBpp(ulong SurfaceThumbnail_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern int SurfaceThumbnail_getData(ulong SurfaceThumbnail_cPtr, byte[] data, int length);
        #endregion

        #region -- SensorDevice
        [DllImport(MaxstARLibName)]
        public static extern void startSensor();

        [DllImport(MaxstARLibName)]
        public static extern void stopSensor();
        #endregion


        #region -- MapViewer
        [DllImport(MaxstARLibName)]
        public static extern bool MapViewer_initialize(string fileName);

        [DllImport(MaxstARLibName)]
        public static extern void MapViewer_deInitialize();

        [DllImport(MaxstARLibName)]
        public static extern IntPtr MapViewer_getJson();

        [DllImport(MaxstARLibName)]
        public static extern int MapViewer_create(int idx);

        [DllImport(MaxstARLibName)]
        public static extern void MapViewer_getIndices(out int indices);

        [DllImport(MaxstARLibName)]
        public static extern void MapViewer_getTexCoords(out float texCoords);

        [DllImport(MaxstARLibName)]
        public static extern int MapViewer_getImageSize(int idx);

        [DllImport(MaxstARLibName)]
        public static extern void MapViewer_getImage(int idx, out byte image);
        #endregion

        #region -- Wearable Calibration
        [DllImport(MaxstARLibName)]
        public static extern bool WearableCalibration_isActivated();

        [DllImport(MaxstARLibName)]
        public static extern bool WearableCalibration_init(string modelName);

        [DllImport(MaxstARLibName)]
        public static extern void WearableCalibration_deinit();

        [DllImport(MaxstARLibName)]
        public static extern void WearableCalibration_setSurfaceSize(int width, int height);

        [DllImport(MaxstARLibName)]
        public static extern void WearableCalibration_getProjectionMatrix(float[] projection, int eyeType);
        #endregion

        #region -- Image Extractor
        [DllImport(MaxstARLibName)]
        public static extern int TrackedImage_getIndex(ulong Image_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern int TrackedImage_getWidth(ulong Image_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern int TrackedImage_getHeight(ulong Image_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern int TrackedImage_getLength(ulong Image_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern int TrackedImage_getFormat(ulong Image_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern void TrackedImage_getData(ulong Image_cPtr, byte[] buffer, int size);

        [DllImport(MaxstARLibName)]
        public static extern ulong TrackedImage_getDataPtr(ulong Image_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern ulong TrackedImage_getDataYuv420spSplitPtr(ulong Image_cPtr);

        [DllImport(MaxstARLibName)]
        public static extern ulong TrackedImage_getDataYuv420_888SplitPtr(ulong Image_cPtr);
        #endregion

    }
}
