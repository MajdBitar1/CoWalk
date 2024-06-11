using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Unity.Collections;

namespace XPXR
{
    [CreateAssetMenu(fileName = "XPXRConfig", menuName = "ExperimentXR/XPXRConfig", order = 0)]
    public class XPXRConfig : ScriptableObject {
        public bool LocalStorageMode = false;
        public bool OnlineMode = true;
        public bool BackUpStorageMode = true;
        public string AuthorizationToken;
        public string WebSocketServer;
        public string RESTServer = "https://...";
        public string FileServer;
        public string ExperimentID = "Experimentation";
    } 
    
}