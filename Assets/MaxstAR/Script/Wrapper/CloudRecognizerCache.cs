using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using System.IO;
using UnityEngine;

namespace maxstAR
{
    class CloudRecognizerCache
    {
        private static CloudRecognizerCache instance = null;

        internal static CloudRecognizerCache GetInstance()
        {
            if (instance == null)
            {
                instance = new CloudRecognizerCache();
            }
            return instance;
        }

        private List<KeyValuePair<string, string>> cloudList = new List<KeyValuePair<string, string>>();
   
        internal void ADD(string name, string cloudJson)
        {
            KeyValuePair<string, string> removeKeyValue = new KeyValuePair<string, string>("","");
            foreach(KeyValuePair<string, string> each in cloudList) {
                if(name == each.Key) {
                    removeKeyValue = each;
                }
            }

            if(removeKeyValue.Key != "") {
                cloudList.Remove(removeKeyValue);
            }

            if (cloudList.Count == 5)
            {
                cloudList.RemoveAt(0);
            }

            cloudList.Add(new KeyValuePair<string, string>(name, cloudJson));

        }

        internal void LOAD()
        {
            List<KeyValuePair<string, string>> removeList = new List<KeyValuePair<string, string>>();
            List<KeyValuePair<string, string>> addList = new List<KeyValuePair<string, string>>();

            foreach (KeyValuePair<string, string> each in cloudList)
            {
                CloudRecognitionLocalData cloudRecognitionLocalData = JsonReader.Deserialize<CloudRecognitionLocalData>(each.Value);
                if (cloudRecognitionLocalData.cloud_image_path != "")
                {
                    string map_path = "";
                    if (cloudRecognitionLocalData.cloud_image_path != "") {
                        map_path = Path.GetDirectoryName(cloudRecognitionLocalData.cloud_image_path)+ "/" + Path.GetFileNameWithoutExtension(cloudRecognitionLocalData.cloud_image_path) + ".2dmap";
                    } else if(cloudRecognitionLocalData.cloud_2dmap_path != "") {
                        map_path = cloudRecognitionLocalData.cloud_2dmap_path;
                    }
                    if (File.Exists(map_path))
                    {
                        removeList.Add(each);
                        string command = "{\"cloud\":\"add_image\",\"cloud_2dmap_path\":\"" + map_path + "\",\"image_width\":" + cloudRecognitionLocalData.image_width + ",\"cloud_name\":\"" + cloudRecognitionLocalData.cloud_name + "\",\"cloud_meta\":\"" + cloudRecognitionLocalData.cloud_meta + "\"}";
                        addList.Add(new KeyValuePair<string, string>(each.Key, command));
                    }
                }
            }

            foreach (KeyValuePair<string, string> each in removeList)
            {
                cloudList.Remove(each);
            }

            foreach (KeyValuePair<string, string> each in addList)
            {
                cloudList.Add(each);
            }

            TrackerManager.GetInstance().RemoveTrackerData("");
            foreach (KeyValuePair<string, string> each in cloudList) 
            {
                TrackerManager.GetInstance().AddTrackerData(each.Value);
            }
            TrackerManager.GetInstance().LoadTrackerData();
        }
    }
}
