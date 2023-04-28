using inetum.unityUtils;
using System.Collections;
using System.Collections.Generic;

using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.userCapture;
using umi3d.common;
using umi3d.common.userCapture;
using UnityEngine;

namespace umi3dbrowser.module.lipsync
{
    /// <summary>
    /// Manages lips synchronization on users.
    /// </summary>
    public class LipSyncManager : SingleBehaviour<LipSyncManager>
    {
        //private Dictionary<ulong, LipsControllerInfo> userLipsSyncControllers = new Dictionary<ulong, LipsControllerInfo>();
        ///// <summary>
        ///// Store loading coroutine not to start the same one two times
        ///// </summary>
        //private Dictionary<ulong, Coroutine> waitingForAvatarCoroutines = new Dictionary<ulong, Coroutine> ();

        //private LipsControllerInfo ownLipsSyncController;

        ///// <summary>
        ///// Cached camera reference for optimization.
        ///// </summary>
        //private Camera userCamera;

        //#region OptimizationParameters

        ///// <summary>
        ///// Maximum distance between the camera and the tracked user.
        ///// </summary>
        ///// Users beyond this distance won't have lipSync. Set to negative for no distance checking.
        //[Tooltip("Maximum distance between the camera and the tracked user. " +
        //    "Users beyond this distance won't have lipSync.\n" +
        //    "Set to negative for no distance checking.")]
        //public float maxDistance = 10f;

        ///// <summary>
        ///// Maximum angle between the camera and the (negative) foward axis of the tracked head in degrees.
        ///// </summary>
        ///// Set to 180 degrees for no angular checking.
        //[Tooltip("Maximum angle between the camera and the (negative) foward axis of the tracked head in degrees. " +
        //        "Users beyond this angle won't have lipSync.\n" +
        //        "Set to 180 degrees for no angular checking.")]
        //public float maxAngle = 135f;

        ///// <summary>
        ///// Maximum users that are have lips synchronization at the same time.
        ///// </summary>
        ///// Having lots of synchronized users impacts performance by a lot. Set to negative for no limit.
        //[Tooltip("Maximum users that are have lips synchronization at the same time. " +
        //        "Having lots of synchronized users impacts performance by a lot. Set to negative for no limit.")]
        //public int maxSyncedUsers = 1;

        ///// <summary>
        ///// Should your own avatar have lipSync if possible?
        ///// </summary>
        ///// Not recommended since you don't usually see your own avatar.
        //[Tooltip("Should your own avatar have lipSync if possible?")]
        //public bool activateOwnLipSync;

        //#endregion OptimizationParameters

        //private class LipsControllerInfo
        //{
        //    public ulong userId;
        //    public AbstractLipSyncController controller;
        //    public bool isSpeaking;
        //    public UserAvatar userAvatar;
        //    public Transform viewTransform;
        //    public AudioSource audioSource;
        //}

        //#region lifecycle

        //private void Start()
        //{
        //    UMI3DClientUserTracking.Instance.avatarEvent.AddListener(LaunchLipSyncAfterLoading);

        //    UMI3DCollaborationClientServer.Instance.OnLeavingEnvironment.AddListener(() => userLipsSyncControllers.Clear());
        //    UMI3DCollaborationClientServer.Instance.OnRedirectionStarted.AddListener(() => userLipsSyncControllers.Clear());

        //    AudioManager.Instance.OnUserSpeaking.AddListener(ChangeSpeechState);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="userId"></param>
        //public void LaunchLipSyncAfterLoading(ulong userId)
        //{
        //    if (userLipsSyncControllers.ContainsKey(userId) || waitingForAvatarCoroutines.ContainsKey(userId))
        //        return;

        //    UserAvatar userAvatar = UMI3DClientUserTracking.Instance.embodimentDict[userId];
        //    var coroutine = StartCoroutine(WaitForAvatarAndStart(userAvatar));
        //    waitingForAvatarCoroutines.Add(userId, coroutine);
        //}

        ///// <summary>
        ///// Update the associated lip controller state based on the speaking state
        ///// </summary>
        ///// <param name="user"></param>
        ///// <param name="isSpeaking"></param>
        //private void ChangeSpeechState(UMI3DUser user, bool isSpeaking)
        //{
        //    if (!userLipsSyncControllers.ContainsKey(user.id)) //is another existing user
        //        return;

        //    if (userLipsSyncControllers[user.id].audioSource == null)
        //    {
        //        SetUpAudioListening(userLipsSyncControllers[user.id]);
        //        userLipsSyncControllers[user.id].controller.StartLipSync();
        //        userLipsSyncControllers[user.id].controller.PauseLipSync();
        //    }

        //    userLipsSyncControllers[user.id].isSpeaking = isSpeaking;
        //}

        //private void Update()
        //{
        //    if (userLipsSyncControllers.Count == 0)
        //        return;

        //    List<LipsControllerInfo> choseonControllers = new List<LipsControllerInfo>();

        //    //Optimization constraints are applied here

        //    foreach (var lipsController in userLipsSyncControllers)
        //    {
        //        LipsControllerInfo lipsInfo = lipsController.Value;
        //        if (!lipsInfo.controller.IsLipSyncStarted())
        //            continue;

        //        if (lipsInfo.viewTransform == null) // avatar cannot be found
        //            continue;

        //        // disable all lipsController that does not match the optimization constraints
        //        if (!lipsInfo.isSpeaking // only analyse phonemes when speaking
        //            || (Vector3.Distance(userCamera.transform.position, lipsInfo.viewTransform.position) > maxDistance && maxDistance > 0) // user is not too far
        //            || Vector3.Dot(userCamera.transform.forward, lipsInfo.viewTransform.forward) > Mathf.Cos(Mathf.Deg2Rad * (180f - maxAngle)) // face should be looking to the other face
        //            || IsOutsideOfCamera(userCamera, lipsInfo.viewTransform.position) // point should be in the view frustum
        //            || IsSoundNeglectable(lipsInfo?.audioSource)) // the user must be speaking above the minimum level
        //        {
        //            if (!lipsInfo.controller.IsLipSyncPaused())
        //                lipsInfo.controller.PauseLipSync();
        //        }
        //        else
        //            choseonControllers.Add(lipsInfo);
        //    }

        //    if (maxSyncedUsers <= 0)
        //    {
        //        foreach (var controllerInfo in choseonControllers)
        //        {
        //            if (controllerInfo.controller.IsLipSyncPaused())
        //                controllerInfo.controller.ResumeLipSync();
        //        }
        //        return; // no need for comparison if number of user optimization is disabled
        //    }

        //    // left lips controllers are compared on their distance to the user
        //    int Compare(LipsControllerInfo l1, LipsControllerInfo l2)
        //    {
        //        var dist1 = Vector3.Distance(userCamera.transform.position, l1.viewTransform.position);
        //        var dist2 = Vector3.Distance(userCamera.transform.position, l2.viewTransform.position);
        //        if (dist1 < dist2)
        //            return -1;
        //        else
        //            return 1;
        //    }
        //    choseonControllers.Sort(Compare);
        //    for (var i = 0; i < choseonControllers.Count; i++)
        //    {
        //        if (i < maxSyncedUsers) // closer lipsController are enabled / maintained playing
        //        {
        //            if (choseonControllers[i].controller.IsLipSyncPaused())
        //                choseonControllers[i].controller.ResumeLipSync();
        //        }
        //        else
        //        {
        //            if (!choseonControllers[i].controller.IsLipSyncPaused())
        //                choseonControllers[i].controller.PauseLipSync();
        //        }
        //    }
        //}

        //#endregion lifecycle

        //private static bool IsOutsideOfCamera(Camera camera, Vector3 point)
        //{
        //    var projected = camera.WorldToViewportPoint(point);
        //    return projected.x < 0 || projected.x > 1
        //            || projected.y < 0 || projected.y > 1
        //            || projected.z < 0;
        //}

        ///// <summary>
        ///// Above this threshold the received sound is considered as intended sound0.
        ///// </summary>
        ///// Determined empirically.
        //public float SOUND_THRESHOLD = 0.507f;

        ///// <summary>
        ///// Compute a metric of the volume emitted by an audio source
        ///// </summary>
        ///// The metric is between 0.5 and 1 and corresponds to the max value of samples
        ///// <param name="audioSource"></param>
        ///// <returns></returns>
        //private float GetMaxAmplitude(AudioSource audioSource)
        //{
        //    var samples = new float[1024];
        //    audioSource.GetOutputData(samples, 0);
        //    float maxAmplitude = 0f;
        //    foreach (var sample in samples)
        //    {
        //        var samplePos = (sample + 1f) / 2f;
        //        if (samplePos > maxAmplitude) maxAmplitude = samplePos;
        //    }
        //    return maxAmplitude;
        //}

        //private bool IsSoundNeglectable(AudioSource audioSource)
        //{
        //    if (audioSource == null)
        //        return true;
        //    else
        //        return GetMaxAmplitude(audioSource) < SOUND_THRESHOLD;
        //}

        //#region avatarLoading

        //private bool IsAvatarLoading(UserAvatar userAvatar, bool isYourself)
        //{
        //    var indexOfChild = isYourself ? 0 : 1; // other avatars have 2 childs under embodiment, skeleton and model
        //    return (userAvatar.activeUserBindings && userAvatar.userBindings.Count == 0) //avatar is loaded when binding are set
        //        || userAvatar.transform.childCount < indexOfChild + 1
        //        || (userAvatar.transform.childCount > indexOfChild
        //            && userAvatar.transform.GetChild(indexOfChild).transform.childCount == 0);
        //}

        ///// <summary>
        ///// Wait for the avatar bundle loading
        ///// </summary>
        ///// <param name="userAvatar"></param>
        ///// <returns></returns>
        //private IEnumerator WaitForAvatarAndStart(UserAvatar userAvatar) //todo: merge with the emote waiting
        //{
        //    var isYourself = userAvatar.userId == UMI3DClientServer.Instance.GetUserId();

        //    if (isYourself && !activateOwnLipSync)
        //    {
        //        waitingForAvatarCoroutines.Remove(userAvatar.userId);
        //        yield break;
        //    }

        //    float timeLimit = 15f; // wait 15s max for avatar loading
        //    float startTime = Time.time;
        //    while (IsAvatarLoading(userAvatar, isYourself)
        //            && (Time.time - startTime) < timeLimit) //wait for avatar model loading
        //        yield return null;

        //    if ((Time.time - startTime) >= timeLimit)
        //    {
        //        waitingForAvatarCoroutines.Remove(userAvatar.userId);
        //        UMI3DLogger.LogError($"Unable to launch LipSync. Avatar model not loaded within the {timeLimit}s time frame.", DebugScope.None);
        //        yield break;
        //    }

        //    Launch(userAvatar);
        //    waitingForAvatarCoroutines.Remove(userAvatar.userId);
        //}

        //#endregion avatarLoading

        //#region setUp

        //private AbstractLipSyncController FindLipsController(UserAvatar userAvatar)
        //{
        //    // looking for animation component (legacy unity animaton system, loaded by glTF)
        //    var animationComponents = userAvatar.gameObject.GetComponentsInChildren<Animation>();
        //    if (animationComponents.Length > 0)
        //    {
        //        foreach (var animationComponent in animationComponents)
        //        {
        //            if (animationComponent.GetClipCount() >= 14) // one clip for each viseme
        //                return animationComponent.gameObject.GetOrAddComponent<LipSyncAnimationController>();
        //        }
        //    }

        //    // looking for blendshapes in the mesh (loaded by glTF)
        //    var skinnedMeshComponents = userAvatar.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        //    if (skinnedMeshComponents.Length > 0)
        //    {
        //        foreach (var skinnedMeshComponent in skinnedMeshComponents)
        //        {
        //            if (skinnedMeshComponent.sharedMesh.blendShapeCount >= 15) // one layer for each viseme
        //                return skinnedMeshComponent.gameObject.GetOrAddComponent<LipSyncBlendshapeController>();
        //        }
        //    }

        //    // looking for a custom attached animator component (mecanim unity animaton system, not supported by glTF)
        //    var animatorComponents = userAvatar.gameObject.GetComponentsInChildren<Animator>();
        //    if (animatorComponents.Length > 0)
        //    {
        //        foreach (var animatorComponent in animatorComponents)
        //        {
        //            if (animatorComponent.layerCount == 14) // one layer for each viseme
        //                return animatorComponent.gameObject.GetOrAddComponent<LipSyncAnimatorController>();
        //        }
        //    }

        //    UMI3DLogger.LogError($"Unable to launch lipSync for user {userAvatar.userId}. No animation component, nor animator, nor blenshapes.", DebugScope.None);
        //    throw new System.NotSupportedException($"Unable to launch lipSync for user {userAvatar.userId}. No animation component, nor animator, nor blenshapes.");
        //}

        //private bool SetUpAudioListening(LipsControllerInfo lipsInfos)
        //{
        //    var user = UMI3DCollaborationEnvironmentLoader.Instance.JoinnedUserList.Find(u => u.id == lipsInfos.userId);
        //    var mumbleAudioNode = AudioManager.Instance.GetMumbleAudioPlayer(user)?.gameObject;
        //    if (mumbleAudioNode == null)
        //        return false;

        //    lipsInfos.audioSource = mumbleAudioNode.GetComponent<AudioSource>();
        //    lipsInfos.controller.OVRLipSyncContextHandler = mumbleAudioNode.GetOrAddComponent<OVRLipSyncContext>();
        //    if (lipsInfos.controller.OVRLipSyncContextHandler is OVRLipSyncContext)
        //        (lipsInfos.controller.OVRLipSyncContextHandler as OVRLipSyncContext).audioLoopback = true;
        //    lipsInfos.controller.OVRLipSyncContextHandler.audioSource = lipsInfos.audioSource;
        //    return true;
        //}

        //#endregion setUp

        ///// <summary>
        ///// Start lips synchronization.
        ///// </summary>
        ///// <param name="userAvatar"></param>
        ///// <exception cref="System.Exception"></exception>
        //public void Launch(UserAvatar userAvatar)
        //{
        //    AbstractLipSyncController lipsAnimationController = FindLipsController(userAvatar);

        //    // cached reference to main camera
        //    userCamera = UMI3DClientUserTrackingBone.instances[BoneType.Viewpoint].GetComponentInChildren<Camera>();

        //    // register all settings
        //    var lipsInfos = new LipsControllerInfo()
        //    {
        //        controller = lipsAnimationController,
        //        userId = userAvatar.userId,
        //        userAvatar = userAvatar,
        //        viewTransform = UMI3DClientUserTracking.Instance.embodimentDict[userAvatar.userId].GetComponentInChildren<UMI3DViewpointHelper>().transform,
        //        isSpeaking = false
        //    };

        //    // look for audio source associated to the user and bind it
        //    var isYourself = userAvatar.userId == UMI3DClientServer.Instance.GetUserId();
        //    if (isYourself && activateOwnLipSync)
        //    {
        //        lipsAnimationController.OVRLipSyncContextHandler = userAvatar.gameObject.GetOrAddComponent<OVRLipSyncContext>();
        //        var mic = userAvatar.gameObject.GetOrAddComponent<OVRLipSyncMicInput>(); //todo: create custom listener of mic
        //        ownLipsSyncController = lipsInfos;
        //    }
        //    else
        //    {
        //        SetUpAudioListening(lipsInfos);
        //        userLipsSyncControllers.Add(userAvatar.userId, lipsInfos);
        //    }

        //    // connect OVR LipSync
        //    lipsAnimationController.StartLipSync();
        //    if (!isYourself)
        //        lipsAnimationController.PauseLipSync();
        //}

        ///// <summary>
        ///// Stop all lips synchronization on a user
        ///// </summary>
        ///// <param name="userAvatar"></param>
        //public void Stop(UserAvatar userAvatar)
        //{
        //    userLipsSyncControllers[userAvatar.userId].controller.StopLipSync();
        //    userLipsSyncControllers.Remove(userAvatar.userId);
        //}
    }
}