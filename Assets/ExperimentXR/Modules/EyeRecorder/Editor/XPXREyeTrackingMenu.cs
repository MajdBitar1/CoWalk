using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XPXR;

class XPXREyeTrackingMenu : MonoBehaviour
{

    [MenuItem("ExperimentXR/Modules/Setup the look area", false, 10)]
    public static void LookArea()
    {
        string newLayerName = XPXRLookAreaRecorder.DefaultLayerName;

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProperty = tagManager.FindProperty("layers");
        int layerAdded = LayerMask.NameToLayer(newLayerName);
        if (layerAdded == -1)
        {
            // Create the layer
            for (int i = 8; i < layersProperty.arraySize; i++)
            {
                SerializedProperty layerProperty = layersProperty.GetArrayElementAtIndex(i);
                if (layerProperty.stringValue == "")
                {
                    layerProperty.stringValue = newLayerName;
                    layerAdded = i;
                    break;
                }
            }
        }

        if (layerAdded == -1)
        {
            throw new XPXRException("Failed to add layer. Maximum limit reached.");
            // Debug.LogError("XRXP : Failed to add layer. Maximum limit reached.");
            // return;
        }

        tagManager.ApplyModifiedProperties();
        // Debug.Log("Layer added successfully!");
        if (GameObject.Find("ExperimentXR") == null)
        {
            XPXRMenu.SetupTheScene();
        }
        GameObject gm = GameObject.Find("ExperimentXR");
        if (gm.GetComponent<XPXRLookAreaRecorder>() == null)
        {
            XPXRLookAreaRecorder areaRecorder = gm.AddComponent<XPXRLookAreaRecorder>();
            areaRecorder.AreaMask = 1 << layerAdded;
        }
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());


    }
}