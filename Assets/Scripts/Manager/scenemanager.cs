using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scenemanager : NetworkSceneManagerDefault
{
    [SerializeField] NetworkRunner m_runner;
    [SerializeField] DemoNetwork m_NetworkManager;
    [SerializeField] GameObject XPXR;

    public void Start()
    {

    }
    public void LoadParkScene()
    {
        // LOADING SCREEN ACTIVATE

        // DELETE RUNNER
        m_runner.SetActiveScene("ParkScene");
        //m_NetworkManager.ReloadRunner(1);
        // ACTIVATE NEW SCENE

        //m_runner.SetActiveScene("ParkScene");
    }
}
