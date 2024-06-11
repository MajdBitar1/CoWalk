using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoOpenEditor : MonoBehaviour
{
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch | OVRInput.Controller.LHand))
        {
            XPXR.Modules.MetaAvatar.OpenAvatarEditor();
        }
    }
}
