/*
Copyright 2019 - 2021 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using inetum.unityUtils;
using System.Collections.Generic;
using System.Threading;
using umi3d.common;
using UnityEngine;
using UnityOpus;
using System.Linq;
using System;
using System.Collections;
using MainThreadDispatcher;

namespace umi3d.cdk.collaboration
{
    [RequireComponent(typeof(AudioSource))]
    public class MicrophoneListener : Singleton<MicrophoneListener>
    {
        /// <summary>
        /// Whether the microphone is running
        /// </summary>
        public static bool IsMute
        {
            get { return Exists ? Instance.muted : false; }
            set
            {
                if (Exists)
                {
                    if (Instance.muted != value)
                    {
                        if (value) Instance.StopRecording();
                        else Instance.StartRecording();
                    }

                    Instance.muted = value;
                }
            }
        }

        private void Start()
        {
            IsMute = IsMute;
        }

        public static void UpdateFrequency(int frequency)
        {
            Instance.samplingFrequency = frequency;
            Debug.Log($"update frequency to {frequency} [{Instance.reading}]");
            if (Instance.reading)
            {
                Instance.OnDisable();
                Instance.OnEnable();
                Instance.StopRecording();
                Instance.StartRecording();
            }
            else
            {
                Instance.OnDisable();
                Instance.OnEnable();
            }
        }

        /// <summary>
        /// Starts to stream the input of the current Mic device
        /// </summary>
        void StartRecording()
        {
            reading = true;

            frameSize = samplingFrequency / 100; //at least frequency/100
            outputBufferSize = frameSize * sizeof(float); // at least frameSize * sizeof(float)
            pcmQueue = new Queue<float>();
            frameBuffer = new float[frameSize];
            outputBuffer = new byte[outputBufferSize];
            microphoneBuffer = new float[lengthSeconds * samplingFrequency];

            if(!IsAValidDevices(microphoneLabel))
                microphoneLabel = Microphone.devices[0];

            clip = Microphone.Start(microphoneLabel, true, lengthSeconds, samplingFrequency);
            lock (pcmQueue)
                pcmQueue.Clear();
            if (thread == null)
                thread = new Thread(ThreadUpdate);
            if (!thread.IsAlive)
                thread.Start();
        }

        /// <summary>
        /// Ends the Mic stream.
        /// </summary>
        void StopRecording()
        {
            reading = false;
            Destroy(clip);
            Microphone.End(microphoneLabel);
        }

        #region ReadMicrophone

        /// <summary>
        /// 
        /// </summary>
        [SerializeField, EditorReadOnly]
        bool muted = false;
        bool reading = false;

        public string microphoneLabel { get; private set; }

        int samplingFrequency = 12000;

        const int lengthSeconds = 1;

        AudioClip clip;
        int head = 0;
        float[] microphoneBuffer;


        private Thread thread;
        int sleepTimeMiliseconde = 5;

        float db;
        object dbLocker = new object();
        float DB
        {
            get
            {
                lock (dbLocker)
                    return db;
            }
            set
            {
                bool lowDB = false;
                lock (dbLocker) {
                    db = value;
                    lowDB = db < minDbToSend;
                }
                if (lowDB)
                    UnityMainThreadDispatcher.Instance().Enqueue(TurnMicOff());
            }
        }

        float minDbToSend = -100;
        bool shouldSend;
        object shouldSendLocker = new object();
        bool TurnMicOffRunning;
        bool ShouldSend
        {
            get
            {
                var db = DB >= minDbToSend;
                lock (shouldSendLocker)
                {
                    shouldSend |= db;
                    return shouldSend;
                }
            }
            set
            {
                lock (shouldSendLocker)
                {
                    shouldSend = value;
                }
            }
        }

        const float refValue = 0.1f; // RMS value for 0 dB

        float timeToTurnOff = 1f;
        IEnumerator TurnMicOff()
        {
            if (TurnMicOffRunning)
                yield break;
            TurnMicOffRunning = true;
            var time = Time.time + timeToTurnOff;

            while (DB < minDbToSend)
            {
                if (time <= Time.time)
                {
                    ShouldSend = false;
                    TurnMicOffRunning = false;
                    yield break;
                }
                yield return null;
            }
            ShouldSend = true;
            TurnMicOffRunning = false;
        }

        public void ChangeMinDB(bool up)
        {
            if (up)
                minDbToSend += 5;
            else
                minDbToSend -= 5;
        }

        public void ChangeBitrate(bool up)
        {
            if (up)
                bitrate += 500;
            else
                bitrate -= 500;
            encoder.Bitrate = bitrate;
        }

        public void ChangeTimeToTurnOff(bool up)
        {
            if (up)
                timeToTurnOff += 0.5f;
            else
                timeToTurnOff -= 0.5f;
        }

        public string[] getDevices() => Microphone.devices;

        public void NextDevices()
        {
            
            var devices = getDevices();
            var i = Array.IndexOf(devices, microphoneLabel) + 1;
            Debug.Log(i);
            if (i < 0 || i >= devices.Length)
                i = 0;
            Debug.Log(i);
            Debug.Log(devices[i]);
            setDevices(devices[i]);
            Debug.Log(microphoneLabel);
        }

        public bool setDevices(string name)
        {
            if(IsAValidDevices(name))
            {
                if (reading)
                {
                    StopRecording();
                    microphoneLabel = name;
                    StartRecording();
                }
                else
                    microphoneLabel = name;
                return true;
            }
            return false;
        }

        public bool IsAValidDevices(string name)
        {
            if (name == null) return false;
            return getDevices().Contains(name);
        }

        public List<string> GetInfo()
        {
            var infos = new List<string>();

            if(reading)
            {
                infos.Add("Current Microphone : " + microphoneLabel);
                infos.Add($" Sampling Frequency : { samplingFrequency} Hz");
                infos.Add($" Bitrate : {bitrate} b/s" );
                infos.Add($" Frame Size : {frameSize} float");
                infos.Add($" Output Buffer Size : {outputBufferSize} bytes");
                lock (pcmQueue)
                    infos.Add(" PCM Queue : " + pcmQueue.Count.ToString());
                infos.Add($" DB : {DB} [min:{minDbToSend} => send:{ShouldSend}] ");
                infos.Add($" Time to turn off : {timeToTurnOff} s ");
                infos.Add(" Microphone List : ");
                getDevices().ForEach(a => infos.Add("    " + a));
            }
            else
                infos.Add("Microphone is muted");

            return infos;
        }

        void Update()
        {
            if (!reading) return;

            var position = Microphone.GetPosition(null);
            if (position < 0 || head == position)
            {
                return;
            }

            clip.GetData(microphoneBuffer, 0);
            if (!muted)
            {
                if (head < position)
                {
                    lock (pcmQueue)
                    {
                        for (int i = head; i < position; i++)
                        {
                            pcmQueue.Enqueue(microphoneBuffer[i]);
                        }
                    }
                }
                else
                {
                    lock (pcmQueue)
                    {
                        //head -> length
                        for (int i = head; i < microphoneBuffer.Length; i++)
                        {
                            pcmQueue.Enqueue(microphoneBuffer[i]);
                        }
                        //0->position
                        for (int i = 0; i < position; i++)
                        {
                            pcmQueue.Enqueue(microphoneBuffer[i]);
                        }
                    }
                }
            }
            head = position;
        }

        #endregion

        #region Encoder

        int bitrate = 96000;
        int frameSize; //at least frequency/100
        int outputBufferSize; // at least frameSize * sizeof(float)

        Encoder encoder;
        Queue<float> pcmQueue;
        float[] frameBuffer;
        byte[] outputBuffer;

        void OnEnable()
        {
            var samp = (SamplingFrequency)samplingFrequency;
            encoder = new Encoder(
                samp,
                NumChannels.Mono,
                OpusApplication.Audio)
            {
                Bitrate = bitrate,
                Complexity = 10,
                Signal = OpusSignal.Voice
            };
        }

        void OnDisable()
        {
            encoder.Dispose();
            encoder = null;
            pcmQueue?.Clear();
            reading = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            reading = false;
        }

        void ThreadUpdate()
        {
            while (reading)
            {
                bool ok = false;
                lock (pcmQueue)
                {
                    ok = pcmQueue.Count >= frameSize;
                }
                if (ok)
                {
                    float sum = 0;
                    lock (pcmQueue)
                    {
                        
                        for (int i = 0; i < frameSize; i++)
                        {
                            var v = pcmQueue.Dequeue();
                            frameBuffer[i] = v;
                            sum += v * v;
                            //if (peak < wavePeak)
                            //    peak = wavePeak;
                        }
                        //db = 20 * Mathf.Log10(Mathf.Abs(peak));

                    }
                    var rmsValue = Mathf.Sqrt(sum / frameSize);
                    DB = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB

                    if (ShouldSend) 
                    {
                        var encodedLength = encoder.Encode(frameBuffer, outputBuffer);
                        if (UMI3DCollaborationClientServer.Exists
                            && UMI3DCollaborationClientServer.Instance?.ForgeClient != null
                            && UMI3DCollaborationClientServer.UserDto.dto.status == StatusType.ACTIVE)
                        {
                            UMI3DCollaborationClientServer.Instance.ForgeClient.SendVOIP(encodedLength, outputBuffer);
                        }
                    }
                }
                Thread.Sleep(sleepTimeMiliseconde);
            }
            thread = null;
        }
        #endregion
    }
}
