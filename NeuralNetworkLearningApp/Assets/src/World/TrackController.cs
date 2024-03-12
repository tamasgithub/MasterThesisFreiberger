using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class TrackController : MonoBehaviour
{
    public float heightAboveGround = 0f;
    private Terrain terrain;
    public float trackLength = 24f;

    // Start is called before the first frame update
    void Start()
    {
        terrain = Terrain.activeTerrain;
        foreach (Transform track in transform)
        {
            Quaternion existingRotation = track.localRotation;


            Vector3 trackPosition = track.position;
            float leftHeight = terrain.SampleHeight(trackPosition + track.TransformDirection(trackLength / 2 * Vector3.left));
            float rightHeight = terrain.SampleHeight(trackPosition + track.TransformDirection(trackLength / 2 * Vector3.right));
            float gradient = (leftHeight - rightHeight) / trackLength;
            trackPosition.y = (leftHeight + rightHeight) / 2 + heightAboveGround;
            track.position = trackPosition;
            // Calculate the new z-axis rotation based on the terrain slope
            float newZRotation = -Mathf.Atan(gradient) * Mathf.Rad2Deg;

            // Apply the new rotation while preserving existing y-axis rotation
            track.rotation = Quaternion.Euler(new Vector3(0, track.eulerAngles.y, newZRotation));
            track.localScale = new Vector3(1 / Mathf.Cos(newZRotation * Mathf.Deg2Rad), 1, 1);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public Vector3 GetPositionOnTrack(float trackProgress)
    {
        return GetValueOnTrack(trackProgress, "position");
    }

    public Quaternion GetRotationOnTrack(float trackProgress)
    {
        return Quaternion.Euler(GetValueOnTrack(trackProgress, "rotation"));
    }

    // @params trackProgress: the progress between 0 inclusive and 1 exclusive of the whole track
    public Vector3 GetValueOnTrack(float trackProgress, string value) {
        if (trackProgress < 0 || trackProgress >= 1)
        {
            Debug.LogException(new Exception("trackProgress outside of [0;1) range: " + trackProgress));
        }
        float mappedTrackProgress = trackProgress * (transform.childCount - 1);
        int lastTrackIndex = Mathf.FloorToInt(mappedTrackProgress);
        Transform lastTrack = transform.GetChild(lastTrackIndex);
        Transform nextTrack = transform.GetChild(lastTrackIndex + 1);
        float interpolatingVariable = mappedTrackProgress - lastTrackIndex;
        switch (value)
        {
            case "position":
                return Vector3.Lerp(lastTrack.position, nextTrack.position, interpolatingVariable);
                
            case "rotation":
                return Quaternion.Lerp(lastTrack.rotation, nextTrack.rotation, interpolatingVariable).eulerAngles;
                
        }
        return Vector3.zero;
    }


}
