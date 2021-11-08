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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.common;
using UnityEngine;

namespace umi3d.cdk.collaboration
{

    /// <summary>
    /// Singleton use to read AudioDto.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        Dictionary<ulong, AudioReader> GlobalReader = new Dictionary<ulong, AudioReader>();
        Dictionary<ulong, AudioReader> SpacialReader = new Dictionary<ulong, AudioReader>();
        Dictionary<ulong, Coroutine> WaitCoroutine = new Dictionary<ulong, Coroutine>();

        Dictionary<ulong, float> lastMessageTimeDelta;
        Dictionary<ulong, float> lastMessageTime;
        Dictionary<ulong, List<(float, float)>> MessageTime;

        private void Start()
        {
            lastMessageTimeDelta = new Dictionary<ulong, float>();
            lastMessageTime = new Dictionary<ulong, float>();
            MessageTime = new Dictionary<ulong, List<(float, float)>>();

            UMI3DUser.OnNewUser.AddListener(OnAudioChanged);
            UMI3DUser.OnRemoveUser.AddListener(OnUserDisconected);
            UMI3DUser.OnUserAudioUpdated.AddListener(OnAudioChanged);
            UMI3DUser.OnUserAudioFrequencyUpdated.AddListener(OnAudioFrequencyChanged);
        }

        /// <summary>
        /// Read a Voice Dto and dispatched it in the right audioSource.
        /// </summary>
        /// <param name="userId"> the speaking user</param>
        /// <param name="sample"> the voice dto</param>
        public void Read(ulong userId, byte[] sample, ulong timestep)
        {
            MainThreadDispatcher.UnityMainThreadDispatcher.Instance().Enqueue(MarkTime(userId));
            if (SpacialReader.ContainsKey(userId))
                SpacialReader[userId].Read(sample, timestep);
            else if (GlobalReader.ContainsKey(userId))
                GlobalReader[userId].Read(sample, timestep);
        }

        IEnumerator MarkTime(ulong userId)
        {
            var cur = Time.time;
            if (!MessageTime.ContainsKey(userId))
            {
                MessageTime[userId] = new List<(float, float)>();
                lastMessageTime[userId] = cur;
                lastMessageTimeDelta[userId] = 0;
                yield break;
            }
            MessageTime[userId].RemoveAll(e => e.Item1 < cur - 3);
            var last = lastMessageTime[userId];
            if (last > cur - 3)
            {
                var delta = cur - last;
                MessageTime[userId].Add((cur, delta));
                lastMessageTimeDelta[userId] = MessageTime[userId].Select(e => e.Item2).Aggregate((a, b) => a + b) / MessageTime[userId].Count;
            }
            else
                lastMessageTimeDelta[userId] = 0;
            lastMessageTime[userId] = cur;
            yield break;
        }

        public List<string> GetInfo()
        {
            var infos = new List<string>();
            infos.Add("Audio Reader : ");
            infos.Add("     Global Reader : " + GlobalReader.Count);
            GlobalReader.Keys.ForEach(e => infos.Add("       " + e.ToString()));
            infos.Add("     Spatial Reader : " + SpacialReader.Count);
            SpacialReader.Keys.ForEach(e => infos.Add("       " + e.ToString()));
            infos.Add("     Average Last Message Delta (3s): ");
            lastMessageTimeDelta.ForEach(e => infos.Add($"       {e.Key} : {e.Value}"));
            return infos;
        }

        /// <summary>
        /// MAnage user update
        /// </summary>
        /// <param name="user"></param>
        void OnUserDisconected(UMI3DUser user)
        {
            if (WaitCoroutine.ContainsKey(user.id))
            {
                StopCoroutine(WaitCoroutine[user.id]);
                WaitCoroutine.Remove(user.id);
            }
            if (GlobalReader.ContainsKey(user.id))
            {
                Destroy(GlobalReader[user.id].gameObject);
                GlobalReader.Remove(user.id);
            }
            if (SpacialReader.ContainsKey(user.id))
                SpacialReader.Remove(user.id);
        }

        /// <summary>
        /// MAnage user update
        /// </summary>
        /// <param name="user"></param>
        void OnAudioFrequencyChanged(UMI3DUser user)
        {
            if (user.id == UMI3DCollaborationClientServer.UserDto.dto.id)
                MicrophoneListener.UpdateFrequency(user.audioFrequency);
            if (GlobalReader.ContainsKey(user.id))
                GlobalReader[user.id].UpdateFrequency(user.audioFrequency);
            if (SpacialReader.ContainsKey(user.id))
                SpacialReader[user.id].UpdateFrequency(user.audioFrequency);
        }

        /// <summary>
        /// Manage user update
        /// </summary>
        /// <param name="user"></param>
        void OnAudioChanged(UMI3DUser user)
        {
            if (WaitCoroutine.ContainsKey(user.id))
            {
                StopCoroutine(WaitCoroutine[user.id]);
                WaitCoroutine.Remove(user.id);
            }
            var audioPlayer = user?.audioplayer?.audioSource?.gameObject;
            if (audioPlayer != null)
            {
                var reader = audioPlayer.GetOrAddComponent<AudioReader>();
                SpacialReader[user.id] = reader;
                if (GlobalReader.ContainsKey(user.id))
                {
                    Destroy(GlobalReader[user.id].gameObject);
                    GlobalReader.Remove(user.id);
                }
            }
            else
            {
                if (user.audioPlayerId != 0)
                    WaitCoroutine[user.id] = StartCoroutine(WaitForAudioCreation(user));
                else
                {
                    if (SpacialReader.ContainsKey(user.id))
                        SpacialReader.Remove(user.id);
                    if (!GlobalReader.ContainsKey(user.id))
                    {
                        var g = new GameObject();
                        g.name = $"user_{user.id}_audio_reader";
                        GlobalReader[user.id] = g.AddComponent<AudioReader>();
                    }
                }
            }
        }

        IEnumerator WaitForAudioCreation(UMI3DUser user)
        {
            yield return new WaitUntil(() => user?.audioplayer?.audioSource?.gameObject != null);
            OnAudioChanged(user);
        }
    }
}