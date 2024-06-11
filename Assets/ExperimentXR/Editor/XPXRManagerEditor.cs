using UnityEngine;
using UnityEditor;
using XPXR;

[CustomEditor(typeof(XPXRManager))]
class XPXRManagerEditor : Editor
{

    GameObject gameObject;
    Editor gameObjectEditor;

    SerializedProperty config;

    void OnEnable()
    {
        config = serializedObject.FindProperty(nameof(XPXRManager.config));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.ObjectField(config, typeof(XPXRConfig), new GUIContent("Configuration File"));
        serializedObject.ApplyModifiedProperties();
        GUILayout.Space(10);
        GUILayout.Label("Objects Tracked in the scene: ");
        var style = new GUIStyle();
        style.margin.left = 42;
        GUILayout.BeginVertical(style);
        XPXRObjectTracker[] objectTrackers = GameObject.FindObjectsOfType<XPXRObjectTracker>();
        GUI.enabled = false;
        // string[] obj = new string[objectTrackers.Length];
        // for (int i = 0; i < objectTrackers.Length; i++)
        // {
        //     obj[i] = objectTrackers[i].GetObjectName() + " : tracking enabled(" + objectTrackers[i].TracingEnabled + ")";
        // }
        foreach (var objt in objectTrackers)
        {
            string frequency = (objt.TraceFrequency == 0)?"": objt.TraceFrequency.ToString();
            GUILayout.Toggle(objt.TracingEnabled, $"{objt.GetObjectName()} - each {frequency} frames");
        }
        GUILayout.EndVertical();
        GUI.enabled = true;
        GUILayout.Space(6);
        if (GUILayout.Button("Refresh objects list"))
        {
            this.OnInspectorGUI();
        }
    }
}
