using BeardedManStudios.Forge.Networking.Unity;
using inetum.unityUtils;
using UnityEngine;

public class Debugger : PersistentSingleBehaviour<Debugger>
{
    public bool DisplayInfo = false;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F8))
        //    DisplayInfo = !DisplayInfo;

        //if (Input.GetKeyDown(KeyCode.F2) && umi3d.cdk.collaboration.MicrophoneListener.Exists)
        //    umi3d.cdk.collaboration.MicrophoneListener.NextDevices();

        //if (Input.GetKeyDown(KeyCode.F3) && umi3d.cdk.collaboration.MicrophoneListener.Exists)
        //    if (!Input.GetKey(KeyCode.LeftShift))
        //        umi3d.cdk.collaboration.MicrophoneListener.ChangeThreshold(true);
        //    else
        //        umi3d.cdk.collaboration.MicrophoneListener.ChangeThreshold(false);

        //if (Input.GetKeyDown(KeyCode.F4) && umi3d.cdk.collaboration.MicrophoneListener.Exists)
        //    if (!Input.GetKey(KeyCode.LeftShift))
        //        umi3d.cdk.collaboration.MicrophoneListener.ChangeTimeToTurnOff(true);
        //    else
        //        umi3d.cdk.collaboration.MicrophoneListener.ChangeTimeToTurnOff(false);

        //if (Input.GetKeyDown(KeyCode.F5) && umi3d.cdk.collaboration.MicrophoneListener.Exists)
        //    if (!Input.GetKey(KeyCode.LeftShift))
        //        umi3d.cdk.collaboration.MicrophoneListener.ChangeBitrate(true);
        //    else
        //        umi3d.cdk.collaboration.MicrophoneListener.ChangeBitrate(false);

        //if (Input.GetKeyDown(KeyCode.F6) && umi3d.cdk.collaboration.MicrophoneListener.Exists)
        //    if (!Input.GetKey(KeyCode.LeftShift))
        //        umi3d.cdk.collaboration.MicrophoneListener.Gain += 0.1f;
        //    else
        //        umi3d.cdk.collaboration.MicrophoneListener.Gain -= 0.1f;
        //if (Input.GetKeyDown(KeyCode.F7) && umi3d.cdk.collaboration.MicrophoneListener.Exists)
        //    umi3d.cdk.collaboration.MicrophoneListener.LoopBack = !umi3d.cdk.collaboration.MicrophoneListener.LoopBack;
    }



    private void WriteLabel(Rect rect, string message)
    {
        GUI.color = Color.black;
        GUI.Label(rect, message);
        // Do the same thing as above but make the above UI look like a solid
        // shadow so that the text is readable on any contrast screen
        GUI.color = Color.white;
        GUI.Label(rect, message);
    }

    int line = 0;
    int getLine() { var l = line; line += 14; return l; }

    private void OnGUI()
    {
        if (!DisplayInfo || !Exists)
            return;

        line = 50;
        var row = 1000;

        WriteLabel(new Rect(14, getLine(), row, 25), "Connected: " + umi3d.cdk.collaboration.UMI3DCollaborationClientServer.Connected());
        WriteLabel(new Rect(14, getLine(), row, 25), "F2:Next Mic|F3:+ Noise Treshold [Shift:-]|F4: + time to turn mic off [Shift:-]| F5: + bitrate [Shift:-]| F6: + gain [Shift:-]");
        WriteLabel(new Rect(14, getLine(), row, 25), "----------");
        if (NetworkManager.Instance?.Networker != null)
        {
            WriteLabel(new Rect(14, getLine(), row, 25), "Time: " + NetworkManager.Instance.Networker.Time.Timestep);
            WriteLabel(new Rect(14, getLine(), row, 25), $"Bandwidth In: {NetworkManager.Instance.Networker.BandwidthIn} bytes");
            WriteLabel(new Rect(14, getLine(), row, 25), $"Bandwidth Out: {NetworkManager.Instance.Networker.BandwidthOut} bytes");
        }
        if(umi3d.cdk.collaboration.UMI3DCollaborationClientServer.Exists)
            WriteLabel(new Rect(14, getLine(), row, 25), "Round Trip Latency (ms): " + umi3d.cdk.UMI3DClientServer.Instance.GetRoundTripLAtency());
        WriteLabel(new Rect(14, getLine(), row, 25), "----------");
        //if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
        //    umi3d.cdk.collaboration.MicrophoneListener.Instance.GetInfo().ForEach(e => WriteLabel(new Rect(14, getLine(), row, 25), e));
        //WriteLabel(new Rect(14, getLine(), row, 25), "----------");
        //if (umi3d.cdk.collaboration.AudioManager.Exists)
        //    umi3d.cdk.collaboration.AudioManager.Instance.GetInfo().ForEach(e => WriteLabel(new Rect(14, getLine(), row, 25), e));
    }
}
