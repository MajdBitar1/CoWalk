using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using XPXR;


// [System.Serializable]
// #if UNITY_EDITOR
// [UnityEditor.InitializeOnLoad]
// #endif
public class XPXRMenu : MonoBehaviour
{
    [MenuItem("ExperimentXR/Setup the scene", false, 10)]
    public static void SetupTheScene()
    {
        if ( GameObject.Find("ExperimentXR") != null)
        {
            Debug.Log("Scene already setup");
            return;
        }
        if (AssetDatabase.LoadAssetAtPath<XPXRConfig>("Assets/XPXRConfig/XPXRConfig.asset") == null)
        {
            SetupConfig();
        }
        GameObject fm = new GameObject("ExperimentXR");
        var icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ExperimentXR/Editor/Icons/Tracker.png");
        EditorGUIUtility.SetIconForObject(fm, icon);
        fm.AddComponent<XPXRManager>();
        fm.GetComponent<XPXRManager>().config = AssetDatabase.LoadAssetAtPath<XPXRConfig>("Assets/XPXRConfig/XPXRConfig.asset");
        if (fm != null)
        {
            PrefabUtility.InstantiatePrefab(fm);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
        Debug.LogWarning("Be careful, this is a ALPHA version of XPXR, follow the documentation carefully!");
    }

    [MenuItem("ExperimentXR/Add XPXR config", false, 10)]
    static void SetupConfig()
    {
        XPXRConfig config = new XPXRConfig();
        var icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ExperimentXR/Editor/Icons/Tracker.png");
        EditorGUIUtility.SetIconForObject(config, icon);
        AssetDatabase.DeleteAsset("Assets/XPXRConfig");
        AssetDatabase.CreateFolder("Assets", "XPXRConfig");
        AssetDatabase.CreateAsset(config, "Assets/XPXRConfig/XPXRConfig.asset");
        Debug.Log("Config as been created : " + AssetDatabase.GetAssetPath(config));

        if ( GameObject.Find("ExperimentXR") != null)
        {
            GameObject.Find("ExperimentXR").GetComponent<XPXRManager>().config = AssetDatabase.LoadAssetAtPath<XPXRConfig>("Assets/XPXRConfig/XPXRConfig.asset");
        }
    }

    [MenuItem("ExperimentXR/User Guide")]
    static void guide()
    {
        Application.OpenURL("https://espace.science/experimentXRdoc/");
    }

    [MenuItem("ExperimentXR/About")]
    static void About()
    {
        Debug.Log("Beta ExperimentXR 0.0.1");
    }
}
