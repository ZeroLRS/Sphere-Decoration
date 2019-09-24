using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Keyframe
{
    public float duration;
    public Vector3 location, rotation, scale;
}

public class ObjectRecorder : MonoBehaviour
{    
    [SerializeField]
    private List<Keyframe> keyframes;

    [SerializeField]
    public List<GameObject> enableWhenRecording;

    [SerializeField]
    private int keyframeIndex = 0;

    private float lastKeyframeRecordTime = 0.0f;

    public PlaybackDirection playbackDirection;
    public PlaybackEndBehaviour endBehaviour;
    public float currentPlaytime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (playbackDirection != PlaybackDirection.STOP)
        {
            UpdatePlayback();
            UpdateTransform();
        }
    }

    private void UpdateTransform()
    {
        // Get the current keyframe and update our transform values based on it
        Keyframe currentFrame = keyframes[keyframeIndex];
        gameObject.transform.localPosition = currentFrame.location;
        gameObject.transform.rotation = Quaternion.Euler(currentFrame.rotation);
        gameObject.transform.localScale = currentFrame.scale;
    }

    public void ResetPlayback()
    {
        keyframeIndex = 0;
        currentPlaytime = 0.0f;
        UpdateTransform();
    }

    private void UpdatePlayback()
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

        if (playbackDirection == PlaybackDirection.STOP)
        {
            return;
        }

        if (playbackDirection == PlaybackDirection.FORWARD)
        {
            if (currentPlaytime > keyframes[keyframeIndex].duration)
            {
                int nextIndex = keyframeIndex + 1;
                if (keyframes.Count <= nextIndex)
                {
                    HandleEndBehaviour();
                }
                else
                {
                    currentPlaytime -= keyframes[keyframeIndex].duration;
                    keyframeIndex = nextIndex;
                }
            }
        }
        else if (playbackDirection == PlaybackDirection.REVERSE)
        {
            if (currentPlaytime <= 0)
            {
                int nextIndex = keyframeIndex - 1;
                if (nextIndex < 0)
                {
                    HandleEndBehaviour();
                }
                else
                {
                    currentPlaytime += keyframes[keyframeIndex].duration;
                    keyframeIndex = nextIndex;
                }
            }
        }

    }

    private void HandleEndBehaviour()
    {
        // If we are looping, set the index to the farthest end of the direction we start from
        if (endBehaviour == PlaybackEndBehaviour.LOOP)
        {
            if (playbackDirection == PlaybackDirection.FORWARD)
            {
                keyframeIndex = 0;
            }
            else if (playbackDirection == PlaybackDirection.REVERSE)
            {
                keyframeIndex = keyframes.Count - 1;
            }
        }
        // If we are ping-ponging, reverse playback direction
        else if (endBehaviour == PlaybackEndBehaviour.PING_PONG)
        {
            if (playbackDirection == PlaybackDirection.FORWARD)
            {
                playbackDirection = PlaybackDirection.REVERSE;
            }
            else if (playbackDirection == PlaybackDirection.REVERSE)
            {
                playbackDirection = PlaybackDirection.FORWARD;
            }
        }
        // If we are stopping playback... stop playback
        else if (endBehaviour == PlaybackEndBehaviour.STOP)
        {
            playbackDirection = PlaybackDirection.STOP;
        }
    }

    public void StartRecording()
    {
        // Reset values to start recording fresh
        keyframes.Clear();
        keyframeIndex = 0;
        currentPlaytime = 0.0f;

        // So that the first frame doesn't have a huge duration, set our last keyframe time to now
        lastKeyframeRecordTime = Time.time;
    }

    public void RecordFrameNow()
    {
        // Create a new keyframe
        Keyframe newFrame;
        // The duration for the frame should be how long since the last one
        newFrame.duration = Time.time - lastKeyframeRecordTime;

        // Record all of the information about the object's transform
        newFrame.location = transform.position;
        newFrame.rotation = transform.rotation.eulerAngles;
        newFrame.scale = transform.localScale;

        // Set our last record time to now
        lastKeyframeRecordTime = Time.time;

        // Add this keyframe to the list
        keyframes.Add(newFrame);
    }
}
