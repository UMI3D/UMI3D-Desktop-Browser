/*
Copyright 2019 - 2022 Inetum

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


using Mumble;
using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.cdk.collaboration
{
    /// <summary>
    /// Custom microphone for <see cref="MumbleClient"/> not based on Unity interface for mics 
    /// but on <see cref="https://github.com/naudio/NAudio"/>. 
    /// </summary>
    public class NAudioMicrophone : MumbleMicrophone
    {
        #region Fields

        /// <summary>
        /// Filter to improve voice recording.
        /// </summary>
        private BiQuadFilter filter;

        /// <summary>
        /// Use <see cref="filter"/> to improve record quality ?
        /// </summary>
        private bool useFilter = false;

        private bool debugMode = false;

        #region Mic data

        /// <summary>
        /// Index of the current input device choosen to record audio.
        /// </summary>
        private int currentMicIndex = -1;

        /// <summary>
        /// Current sample rate used to record audio.
        /// </summary>
        private int currentMicSampleRate = -1;

        /// <summary>
        /// Two channels are recorded by default because some microphones only record data in one channel.
        /// (But a mono audio input is sent to murmure)
        /// </summary>
        private int numberOfChannel = 2;

        /// <summary>
        /// Input audio channel which is sent to the server. 0 = right channel, 1 = left channel.
        /// </summary>
        private int channelChoosen = 0;

        /// <summary>
        /// Is <see cref="channelChoosen"/> set up ?
        /// </summary>
        private bool isChannelChoosen = false;

        /// <summary>
        /// Audio data to process;
        /// </summary>
        private PcmArray newData;

        #endregion

        #region Record data

        /// <summary>
        /// Sends audio to Murmure server ?
        /// </summary>
        private bool shouldSendAudioToServer = false;

        /// <summary>
        /// Does microphone needs to record data.
        /// </summary>
        private bool needToRecord = false;

        /// <summary>
        /// Voice samples recorded from microphone.
        /// </summary>
        private List<float> data = new List<float>();

        /// <summary>
        /// Microphone recorder.
        /// </summary>
        private WaveInEvent waveIn;

        /// <summary>
        /// Last <see cref="MicType"/> used by the microphone last frame.
        /// </summary>
        private MicType lastMicrophoneMode;

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int InitializeMic()
        {
            if (MicNumberToUse == currentMicIndex)
            {
                Debug.Log("Mic already init " + GetCurrentMicName());
                return currentMicSampleRate;
            }

            //Make sure there are are microphones connected.
            if (WaveInEvent.DeviceCount <= 0 || Microphone.devices.Length <= 0)
            {
                Debug.Log("No microphone connected!");
                return -1;
            }

            currentMicIndex = MicNumberToUse;
            isChannelChoosen = false;

            Microphone.GetDeviceCaps(Microphone.devices[MicNumberToUse], out int minFreq, out int maxFreq);


            currentMicSampleRate = MumbleClient.GetNearestSupportedSampleRate(maxFreq);
            NumSamplesPerOutgoingPacket = MumbleConstants.NUM_FRAMES_PER_OUTGOING_PACKET * currentMicSampleRate / 100;

            if (currentMicSampleRate != 48000)
                Debug.LogWarning("Using a possibly unsupported sample rate of " + currentMicSampleRate + " things might get weird");

            InitializeInternalMic(currentMicSampleRate);

            filter = BiQuadFilter.LowPassFilter(currentMicSampleRate, 10000, 1);

            return currentMicSampleRate;
        }

        /// <summary>
        /// Inits <see cref="waveIn"/>.
        /// </summary>
        /// <param name="micSampleRate"></param>
        protected void InitializeInternalMic(int micSampleRate)
        {
            if (waveIn == null)
            {
                waveIn = new WaveInEvent();

                waveIn.DataAvailable += (s, a) =>
                {
                    if (!shouldSendAudioToServer)
                        return;

                    byte[] buffer = a.Buffer;

                    lock (data)
                    {
                        for (int index = 0; index < a.BytesRecorded; index += 2 * numberOfChannel)
                        {
                            short sample = (short)((buffer[channelChoosen + index + 1] << 8) | buffer[channelChoosen + index]);

                            if (useFilter && filter!= null)
                                data.Add(filter.Transform(sample / 32768f));
                            else
                                data.Add(sample / 32768f);
                        }
                    }

                    if (!isChannelChoosen)
                        ChooseChannel(buffer, a.BytesRecorded, micSampleRate);

                };
            } 

            waveIn.WaveFormat = new WaveFormat(micSampleRate, numberOfChannel);
            waveIn.DeviceNumber = MicNumberToUse;
        }

        /// <summary>
        /// Sets up <see cref="channelChoosen"/>, meaning choose the audio input channel to send to the server.
        /// </summary>
        /// <returns></returns>
        private void ChooseChannel(byte[] buffer, int nbByte, int sampleRate)
        {
            if (!isChannelChoosen)
            {
                int right = 0;
                int left = 0;

                for (int i = 0; i < nbByte; i += 4)
                {
                    right = right + Mathf.Abs((short)((buffer[i + 1] << 8) | buffer[i]));
                    left = left + Mathf.Abs((short)((buffer[i + 3] << 8) | buffer[i + 2]));
                }

                var diff = Mathf.Abs(right - left);

                if (diff > 100000)
                {
                    isChannelChoosen = true;
                    channelChoosen = (right - left > 0) ? 0 : 2;
                    Debug.Log("Mic canal choosen " + channelChoosen);
                }
                else if (diff < 1000)
                {
                    isChannelChoosen = true;
                    channelChoosen = 0;
                    Debug.Log("Mic canal choosen " + channelChoosen);
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void SendVoiceIfReady()
        {
            lock (data)
            {
                bool sendData = VoiceSendingType == MicType.AlwaysSend;

                while (data.Count >= NumSamplesPerOutgoingPacket)
                {
                    newData = _mumbleClient.GetAvailablePcmArray();
                    newData.Pcm = data.GetRange(0, NumSamplesPerOutgoingPacket).ToArray();

                    if (VoiceSendingType == MicType.Amplitude && !AmplitudeHigherThan(MinAmplitude, newData.Pcm))
                    {
                        sendData = false;

                        //Drop data with low amplitude
                        if (data.Count > NumSamplesPerOutgoingPacket)
                            data = data.GetRange(NumSamplesPerOutgoingPacket + 1, data.Count - NumSamplesPerOutgoingPacket - 1);
                        else
                            data.Clear();
                    } else
                    {
                        sendData = true;
                    }

                    if (sendData)
                    {
                        if (data.Count > NumSamplesPerOutgoingPacket)
                            data = data.GetRange(NumSamplesPerOutgoingPacket + 1, data.Count - NumSamplesPerOutgoingPacket - 1);

                        if (_writePositionalDataFunc != null)
                            _writePositionalDataFunc(ref newData.PositionalData, ref newData.PositionalDataLength);
                        else
                            newData.PositionalDataLength = 0;

                        _mumbleClient.SendVoicePacket(newData);
                    }
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="sampleRate"></param>
        public override void StartSendingAudio(int sampleRate)
        {
            data.Clear();

            shouldSendAudioToServer = true;
        }

        /// <summary>
        /// Starts recording user's voice.
        /// </summary>
        private void StartRecording()
        {
            if (!needToRecord)
            {
                try
                {
                    waveIn.StartRecording();
                    needToRecord = true;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error, cannot start recording audio with : {GetCurrentMicName()} \n" + ex.Message);

                    needToRecord = false;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void StopSendingAudio()
        {
            data.Clear();

            shouldSendAudioToServer = false;
        }

        /// <summary>
        /// Stops recording user's voice.
        /// </summary>
        private void StopRecording()
        {
            if (needToRecord)
            {
                needToRecord = false;
                waveIn.StopRecording();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Update()
        {
            if (_mumbleClient == null || !_mumbleClient.ConnectionSetupFinished)
                return;

            if (VoiceSendingType == MicType.PushToTalk)
            {
                if (lastMicrophoneMode != MicType.PushToTalk)
                    StopRecording();

                if (Input.GetKeyDown(PushToTalkKeycode))
                    StartRecording();
                else if (Input.GetKeyUp(PushToTalkKeycode))
                    StopRecording();
            } else
            {
                if (!_mumbleClient.IsSelfMuted())
                {
                    StartRecording();
                }
                else if (needToRecord)
                {
                    StopRecording();
                }
            }

            if (needToRecord && shouldSendAudioToServer)
                SendVoiceIfReady();

            lastMicrophoneMode = VoiceSendingType;

            if (Input.GetKeyDown(KeyCode.P))
                debugMode = !debugMode;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override bool HasMic()
        {
            return currentMicIndex != -1;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string GetCurrentMicName()
        {
            if (HasMic())
                return WaveInEvent.GetCapabilities(currentMicIndex).ProductName;
            else
                return string.Empty;
        }

        protected void OnDestroy()
        {
            waveIn?.Dispose();
        }

        private void OnGUI()
        {
            if (debugMode)
            {
                GUI.Box(new Rect(0, Screen.height - 120, 425, 80), "");

                if (GUI.Button(new Rect(0, Screen.height - 115, 100, 70), "Reset filter"))
                    useFilter = false;
                if (GUI.Button(new Rect(105, Screen.height - 115, 100, 70), "Low pass filter"))
                {
                    lock (filter)
                    {
                        filter = BiQuadFilter.LowPassFilter(currentMicSampleRate, 10000, 1);
                        useFilter = true;
                        Debug.Log("Apply low pass filter");
                    }
                }
                if (GUI.Button(new Rect(210, Screen.height - 115, 100, 70), "High pass filter"))
                {
                    lock (filter)
                    {
                        filter = BiQuadFilter.HighPassFilter(currentMicSampleRate, 300, 1);
                        useFilter = true;
                        Debug.Log("Apply high pass filter");
                    }
                }
                if (GUI.Button(new Rect(320, Screen.height - 115, 100, 70), "Change audio \n canal" + channelChoosen))
                {
                    if (channelChoosen == 2)
                        channelChoosen = 0;
                    else
                        channelChoosen = 2;
                }
            }
        }

        #endregion
    }
}

