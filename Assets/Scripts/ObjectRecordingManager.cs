using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PlaybackDirection
{
    REVERSE = -1,
    STOP,
    FORWARD,
    count
}

[System.Serializable]
public enum PlaybackEndBehaviour
{
    STOP = 0,
    LOOP, 
    PING_PONG,
    count
}

public class ObjectRecordingManager : MonoBehaviour
{
    [SerializeField]
    private List<ObjectRecorder> recorders;

    [SerializeField]
    private bool recording = false;

    public string saveFile = "objectRecordings.txt";

    public float targetFrameRate = 5.0f;
    private float targetTimeStep;

    public float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        //  Our timestep in the inverse of our target framerate
        targetTimeStep = 1 / targetFrameRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (recording)
        {
            // Add the time since the last update to our elapsed time
            elapsedTime += Time.deltaTime;

            // If enough time has elapsed for us to make a new keyframe, record one
            while (elapsedTime >= targetTimeStep)
            {
                // Record a new keyframe in each recorder
                foreach (ObjectRecorder recorder in recorders)
                {
                    recorder.RecordFrameNow();
                }

                // Now that we've recorded, wait again until we've excceded our timestep
                elapsedTime -= targetTimeStep;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !recording)
        {
            Debug.Log("Starting Recording");
            StartRecording();
        }
        else if (Input.GetKeyDown(KeyCode.R) && recording)
        {
            Debug.Log("Stopping Recording");
            StopRecording();
        }
        else if (!recording)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SetPlaybackDirection(PlaybackDirection.FORWARD);
                SetEndBehaviour(PlaybackEndBehaviour.LOOP);
                for (int i = 0; i < recorders.Count; i++)
                {
                    recorders[i].ResetPlayback();
                }
            }
        }
    }

    public void StartRecording()
    {
        for (int i = 0; i < recorders.Count; i++)
        {
            SetPlaybackDirection(PlaybackDirection.STOP);
            recorders[i].StartRecording();
        }
        recording = true;
    }

    public void StopRecording()
    {
        recording = false;
        for (int i = 0; i < recorders.Count; i++)
        {
            recorders[i].ResetPlayback();
            // ****TODO: Save recording to text file
        }
    }

    public void SetPlaybackDirection(PlaybackDirection dir)
    {
        for (int i = 0; i < recorders.Count; i++)
        {
            recorders[i].playbackDirection = dir;
        }
    }

    public void SetEndBehaviour(PlaybackEndBehaviour end)
    {
        for (int i = 0; i < recorders.Count; i++)
        {
            recorders[i].endBehaviour = end;
        }
    }
}
