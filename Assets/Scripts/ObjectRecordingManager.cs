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
    private List<ObjectRecorder> recorders;

    public bool recording = false;

    public string saveFile = "objectRecordings.txt";

    public float targetFrameRate = 0.0f;
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
    }
}
