using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using UnityEngine;
using static umi3d.cdk.collaboration.NoiseReducer;

public class TestMic : MonoBehaviour
{
    private NoiseReducer _noiseReducer;

    public AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        _noiseReducer = new NoiseReducer(new NoiseReducerConfig()
        {
            SampleRate = 48000,
            NumChannels = 1,
            Attenuation = 20,
            Model = RnNoiseModel.Speech
        });
    }

    private const int SampleRate = 48000;
    private const int RecordLengthSec = 2;
    private AudioClip _microphoneClip;
    private int _clipHead;
    private readonly float[] _processBuffer = new float[480 * 2];
    private readonly float[] _microphoneBuffer = new float[RecordLengthSec * SampleRate];

    bool isRecording = false;

    [ContextMenu("Record")]
    public void Record()
    {
        data.Clear();
        source.Stop();
        Debug.Log("record");
        _microphoneClip = Microphone.Start(Microphone.devices[0], false, RecordLengthSec, SampleRate);

        isRecording = true;

        StartCoroutine(Listen());
    }

    List<float> data = new List<float>();

    public IEnumerator Listen()
    {
        yield return new WaitForSeconds(RecordLengthSec);

        Debug.Log("PLAY");

        AudioClip clip = AudioClip.Create("Pomme", SampleRate * RecordLengthSec, 1, SampleRate, false);
        clip.SetData(data.ToArray(), 0);

        source.clip = clip;

        source.Play();
    }

    public bool useNoiseReduction = false;

    void Update()
    {
        var position = Microphone.GetPosition(null);
        if (position < 0 || _clipHead == position) return;

        _microphoneClip.GetData(_microphoneBuffer, 0);

        int GetDataLength(int bufferLength, int head, int tail) =>
            head < tail ? tail - head : bufferLength - head + tail;

        while (GetDataLength(_microphoneBuffer.Length, _clipHead, position) > _processBuffer.Length)
        {
            var remain = _microphoneBuffer.Length - _clipHead;
            if (remain < _processBuffer.Length)
            {
                Array.Copy(_microphoneBuffer, _clipHead, _processBuffer, 0, remain);
                Array.Copy(_microphoneBuffer, 0, _processBuffer, 0, _processBuffer.Length - remain);
            }
            else
            {
                Array.Copy(_microphoneBuffer, _clipHead, _processBuffer, 0, _processBuffer.Length);
            }

            if (useNoiseReduction)
                _noiseReducer.ReduceNoiseFloat(_processBuffer, 0);

            data.AddRange(_processBuffer);
            _clipHead += _processBuffer.Length;
            if (_clipHead > _microphoneBuffer.Length)
            {
                _clipHead -= _microphoneBuffer.Length;
            }
        }
    }
}
