using UnityEngine;
using UnityEditor;
using XPXR;

[CustomEditor(typeof(XPXRObjectTracker))]
public class XPXRObjectTrackerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        XPXRObjectTracker tracker = (target as XPXRObjectTracker);
        EditorGUI.BeginChangeCheck();
        bool TracingEnabled = EditorGUILayout.Toggle("Tracing Enabled", tracker.TracingEnabled);
        GUILayout.Space(10);
        string Category = EditorGUILayout.TextField("Category", tracker.Category);
        GUI.enabled = false;
        GUILayout.Label("Type of object tracked (Body, Eye, Object etc)");
        GUI.enabled = true;
        GUILayout.Space(10);
        string ObjectName = EditorGUILayout.TextField("Object Name", tracker.ObjectName);
        GUI.enabled = false;
        GUILayout.Label("Name of the tracker (default: GameObject.Name)");
        GUI.enabled = true;
        GUILayout.Space(10);
        int TraceFrequency = EditorGUILayout.IntSlider("Trace frequency", tracker.TraceFrequency, 0, 100);
        GUI.enabled = false;
        GUILayout.Label("For every x frames a record is made (0 = each frame)");
        GUI.enabled = true;
        if (EditorGUI.EndChangeCheck())
        {

            Undo.RecordObject(target, "Edit one or mors values of XPXRObjectTracker");
            // EditorUtility.SetDirty(target);
            tracker.TracingEnabled = TracingEnabled;
            tracker.Category = Category;
            tracker.ObjectName = ObjectName.Length > 0 ? ObjectName : tracker.gameObject.name;
            tracker.TraceFrequency = TraceFrequency;

            PrefabUtility.RecordPrefabInstancePropertyModifications(tracker);
        }
    }
}
