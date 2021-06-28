using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.volumes;
using UnityEngine;
using umi3d.common.volume;

public class VolumeTrackingTester : MonoBehaviour
{
    public VolumeTracker tracker;

    // Start is called before the first frame update
    void Start()
    {
        tracker.SubscribeToVolumeEntrance(id =>
        {
            UMI3DClientServer.SendData(new VolumeUserTransitDto() { direction = true, volumeId = id}, true);
        });

        tracker.SubscribeToVolumeExit(id =>
        {
            UMI3DClientServer.SendData(new VolumeUserTransitDto() { direction = false, volumeId = id }, true);
        });
    }

    private void Update()
    {
        tracker.volumesToTrack = VolumeSliceGroupManager.Instance.GetVolumeSliceGroups().ConvertAll(group => group as AbstractVolumeCell);
        tracker.volumesToTrack.AddRange(VolumePrimitiveManager.Instance.GetPrimitives());
    }
}
