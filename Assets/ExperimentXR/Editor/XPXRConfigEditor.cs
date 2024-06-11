using UnityEngine;
using UnityEditor;
using XPXR;

/// <summary>
/// Use de
/// </summary>
[CustomEditor(typeof(XPXRConfig))]
public class XPXRConfigEditor : Editor
{


    public override void OnInspectorGUI()
    {
        XPXRConfig targetConfig = (XPXRConfig)target;
        EditorGUI.BeginChangeCheck();
        GUILayout.Label("General");
        string ExperimentID = EditorGUILayout.TextField("Experimentation ID", targetConfig.ExperimentID);
        GUILayout.Space(10);
        GUILayout.Label("Ways of saving traces");
        bool OnlineMode = EditorGUILayout.Toggle("Online mode", targetConfig.OnlineMode);
        GUI.enabled = false;
        bool BackUpStorageMode = EditorGUILayout.Toggle("Backup mode", targetConfig.BackUpStorageMode);
        GUI.enabled = true;
        GUILayout.Space(10);
        GUILayout.Label("Server addresses");
        string WebSocketServer = EditorGUILayout.TextField("WebSocket Server", targetConfig.WebSocketServer);
        string FileServer = EditorGUILayout.TextField("File Server", targetConfig.FileServer);
        string AuthorizationToken = EditorGUILayout.TextField("Authorization Token", targetConfig.AuthorizationToken);
        // EditorGUILayout.ObjectField(config, typeof(XPXRConfig), new GUIContent("Configuration File"));
        GUILayout.Space(10);
        GUILayout.Label("Coming soon");
        GUI.enabled = false;
        string RESTServer = EditorGUILayout.TextField("REST Server", targetConfig.RESTServer);
        bool LocalStorageMode = EditorGUILayout.Toggle("Local storage mode", targetConfig.LocalStorageMode);
        GUI.enabled = true;
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(targetConfig,"Modification of the XPXR Config");
            targetConfig.ExperimentID = ExperimentID;
            targetConfig.OnlineMode = OnlineMode;
            targetConfig.BackUpStorageMode = BackUpStorageMode;
            targetConfig.WebSocketServer = WebSocketServer;
            targetConfig.FileServer = FileServer;
            targetConfig.AuthorizationToken = AuthorizationToken;
            targetConfig.RESTServer = RESTServer;
            targetConfig.LocalStorageMode = LocalStorageMode;
            EditorUtility.SetDirty(targetConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
