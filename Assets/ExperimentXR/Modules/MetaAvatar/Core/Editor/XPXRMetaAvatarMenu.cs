using Photon.Voice.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class XPXRMetaAvatarMenu : MonoBehaviour
{
    [MenuItem("ExperimentXR/Modules/Setup Meta Avatar with Fusion")]
    static void AddNetworkMetaAvatar()
    {
        GameObject experimentXR = GameObject.Find("ExperimentXR");
        if (experimentXR == null)
        {
            experimentXR = new GameObject("ExperimentXR");
        }
        if (GameObject.Find("MetaAvatarModule") != null)
        {
            Debug.Log("Meta Avatar Module already setup");
            return;
        }
        // Recorder recorder = experimentXR.AddComponent<Recorder>();
        // UnityVoiceClient unityVoiceClient = experimentXR.AddComponent<UnityVoiceClient>();
        // unityVoiceClient.PrimaryRecorder = recorder;
        // unityVoiceClient.SpeakerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ExperimentXR/Modules/MetaAvatar/Core/Resources/Speaker.prefab");
        GameObject avatarModule = (GameObject)PrefabUtility.InstantiatePrefab(
            AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ExperimentXR/Modules/MetaAvatar/Core/Prefabs/MetaAvatarModule.prefab"),
            experimentXR.transform);
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.LogWarning("This is a ALPHA version of XPXR Meta Avatar Module, follow the documentation carefully!");
    }
}
