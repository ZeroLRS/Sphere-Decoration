using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Keyframe
{
    public float timestamp;
    public Vector3 location, rotation, scale;
}

public class ObjectRecorder : MonoBehaviour
{    
    [SerializeField]
    private List<Keyframe> keyframes;

    [SerializeField]
    private int keyframeIndex = 0;
    private float currentFrameDuration;

    public PlaybackDirection playbackDirection;
    public PlaybackEndBehaviour endBehaviour;
    public float currentPlaytime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (keyframes.Count > 1)
        {
            currentFrameDuration = keyframes[1].timestamp - keyframes[0].timestamp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentPlaytime += Time.deltaTime * (int)playbackDirection;

        // 1) Stopped                                             *
        // 2) Forward                                             
        //  a) Has not exceeded duration, do not change keyframe  *
        //  b) Exceeded duration, procced to next frame           *
        //   i) If last frame, do ending behaviour                *
        // 3) Reverse                                             
        //  a) Has not gone below zero, do not change keyframe    *
        //  b) Below zero, procced to next (previous) frame       *
        //   i) If first frame, do ending behaviour               *
    }

    public void RecordFrameNow()
    {
        Keyframe newFrame;
        newFrame.timestamp = Time.time;
        newFrame.location = transform.position;
        newFrame.rotation = transform.rotation.eulerAngles;
        newFrame.scale = transform.localScale;

        keyframes.Add(newFrame);
    }

    public void SortKeyFrames()
    {
        // Sort keyframes by time
    }
}
