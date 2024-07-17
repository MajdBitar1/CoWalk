using System;
using System.Collections;
using System.Linq;
using Fusion;
using Oculus.Avatar2;
using Oculus.Platform;
using UnityEngine;

public class AvatarNetworkManager : NetworkBehaviour, IPlayerLeft
{
    private SampleAvatarEntity _avatar;
    public GameObject LocalEntityAvatarPrefab;
    public GameObject RemoteEntityAvatarPrefab;
    

    //MINE
    public delegate void MetaAvatarSetup();
    public static event MetaAvatarSetup OnMetaAvatarSetup;

    [Networked(OnChanged = nameof(OnAvatarDataChanged)), Capacity(1200)] public NetworkArray<byte> AvatarData { get; }
    [Networked] public int RecordStreamLength { get; set; }

    [Networked(OnChanged = nameof(OnAvatarUserIdChanged))] public ulong OculusUserId { get; set; }

    private IEnumerator GetOculusID()
    {
        while (OvrPlatformInit.status != OvrPlatformInitStatus.Succeeded)
        {
            if (OvrPlatformInit.status == OvrPlatformInitStatus.Failed)
            {
                Debug.LogError($"XPXR.MetaAvatar-Fusion: Error initializing OvrPlatform. Falling back to local avatar");
                yield break;
            }

            yield return null;
        }

        Users.GetLoggedInUser().OnComplete(message =>
        {
            if (!message.IsError)
            {
                this.OculusUserId = message.Data.ID;
            }
            else
            {
                var e = message.GetError();
                Debug.LogError($"XPXR.MetaAvatar-Fusion: Error loading CDN avatar: {e.Message}. Falling back to local avatar");
            }
        });
    }
    public void Start()
    {
        //base.Spawned();
        // Create the avatar entity 
        GameObject avatarEntity = (base.HasStateAuthority) ? this.LocalEntityAvatarPrefab : this.RemoteEntityAvatarPrefab;
        avatarEntity = GameObject.Instantiate(avatarEntity, base.transform);
        this._avatar = avatarEntity.GetComponentInChildren<SampleAvatarEntity>();
        avatarEntity.transform.parent.gameObject.tag = "Player";
        GameManager.UpdatePlayerList();

        // Init of the OVRPlatform if not started
        if (OvrPlatformInit.status == OvrPlatformInitStatus.NotStarted)
        {
            OvrPlatformInit.InitializeOvrPlatform();
        }
        if (base.HasStateAuthority)
        {
            StartCoroutine(nameof(this.GetOculusID)); // Get the Oculus ID to send to the other users
            // Setup the AvatartEntity
            try
            {
                GameObject avatatSDKManager = GameObject.Find("AvatarSdkManagerHorizon");
                this._avatar.SetBodyTracking(avatatSDKManager.GetComponent<SampleInputManager>());
            }
            catch
            {
                Debug.LogError("XPXR.MetaAvatar-Fusion: AvatarSdkManagerHorizon not present in the scene");
            }
            OvrAvatarFacePoseBehavior face = base.gameObject.AddComponent<OvrAvatarFaceTrackingBehaviorOvrPlugin>();
            OvrAvatarEyePoseBehavior eye = base.gameObject.AddComponent<OvrAvatarEyeTrackingBehaviorOvrPlugin>();
            this._avatar.SetFacePoseProvider(face);
            this._avatar.SetEyePoseProvider(eye);
            GameObject Parent = GameManager.GetOrigin();
            if (Parent is null)
            {
                throw new System.Exception("XPXR.MetaAvatar: the \"OVRCameraRig\" GameObject is not inside the scene or not correctly named. For using Meta Avatar, please add the OVRCameraRig Prefab inside the scene.");
            }
            else 
            {
                gameObject.transform.position = Parent.transform.position;
                gameObject.transform.rotation = Parent.transform.rotation;
                Parent.transform.parent = gameObject.transform;
                OnMetaAvatarSetup();
            }
        }
    }
    private void LateUpdate()
    {
        if (base.HasStateAuthority)
        {
            // Get the stream data of the local entity
            byte[] record = this._avatar.RecordStreamData(OvrAvatarEntity.StreamLOD.Medium);
            if (record == null)
            {
                Debug.LogWarning("XPXR.MetaAvatar-Fusion: Cannot record stream data until entity has loaded a skeleton");
                return;
            }
            RecordStreamLength = record.Length;
            this.AvatarData.CopyFrom(record, 0, record.Length);
        }
    }

    public void UpdateAvatar()
    {
        if (base.HasStateAuthority)
        {
            return;
        }
        //Debug.Log("XPXR.MetaAvatar-Fusion: Avatar value updated");
        if (this._avatar is null)
        {
            this._avatar = base.GetComponentInChildren<SampleAvatarEntity>();
            if (this._avatar is null)
            {
                Debug.LogError("XPXR.MetaAvatar-Fusion: No avatar entity found");
                return;
            }
        }
        // this._avatar.SetPlaybackTimeDelay(Runner.DeltaTime); // For Playback Interpolation
        this._avatar.ApplyStreamData(this.AvatarData.Take(this.RecordStreamLength).ToArray());
    }

    public void LoadUserAvatar()
    {
        if (base.HasStateAuthority)
        {
            return;
        }
        Debug.Log($"XPXR.MetaAvatar-Fusion: Load of the distant user with his OculusUserID {this.OculusUserId.ToString()}");
        if (this.OculusUserId != 0 && OvrPlatformInit.status == OvrPlatformInitStatus.Succeeded)
        {
            this._avatar.LoadRemoteUserCdnAvatar(this.OculusUserId);
        }
        else
        {
            this._avatar.LoadPreset(1);
        }
    }

    public static void OnAvatarDataChanged(Changed<AvatarNetworkManager> changed)
    {
        changed.Behaviour.UpdateAvatar();
    }

    public static void OnAvatarUserIdChanged(Changed<AvatarNetworkManager> changed)
    {
        Debug.Log($"XPXR.MetaAvatar-Fusion: Load of the distant user with his OculusUserID {changed.Behaviour.OculusUserId.ToString()}");
        changed.Behaviour.LoadUserAvatar();
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (base.Object.StateAuthority.PlayerId != base.Object.InputAuthority.PlayerId)
        {
            base.Object.RequestStateAuthority();
            base.Runner.Despawn(this.Object);
        }
    }
}

