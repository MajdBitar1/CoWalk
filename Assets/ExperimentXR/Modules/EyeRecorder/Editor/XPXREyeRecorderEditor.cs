// TODO DeCommenter quand les tests Eyetracking terminé
// using UnityEngine;
// using UnityEditor;
// using XPXR;

// [CustomEditor(typeof(XPXREyeRecorder))]
// public class XPXREyeRecorderEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         XPXREyeRecorder tracker = (target as XPXREyeRecorder);
//         EditorGUI.BeginChangeCheck();
//         bool TracingEnabled = EditorGUILayout.Toggle("Tracing Enabled", tracker.TracingEnabled);
//         GUILayout.Space(10);
//         float ConfidenceThreshold = EditorGUILayout.Slider("Confidence Threshold", tracker.ConfidenceThreshold,0f,1f);
//         GUI.enabled = false;
//         GUILayout.Label("No record wil be done if detected eye state confidence is below this threshold.");
//         GUI.enabled = true;
//         GUILayout.Space(10);
//         int TraceFrequency = EditorGUILayout.IntSlider("Trace frequency", tracker.TraceFrequency, 0, 100);
//         GUI.enabled = false;
//         GUILayout.Label("For every x frames a record is made (0 = each frame)");
//         GUI.enabled = true;
//         if (EditorGUI.EndChangeCheck())
//         {

//             Undo.RecordObject(target, "Edit one or mors values of XPXRObjectTracker");
//             // EditorUtility.SetDirty(target);
//             tracker.TracingEnabled = TracingEnabled;
//             tracker.TraceFrequency = TraceFrequency;
//             tracker.ConfidenceThreshold = ConfidenceThreshold;

//             PrefabUtility.RecordPrefabInstancePropertyModifications(tracker);
//         }
//     }
// }
