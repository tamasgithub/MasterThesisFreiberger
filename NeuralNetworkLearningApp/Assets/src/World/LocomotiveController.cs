using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LocomotiveController : MonoBehaviour
{
    public Transform track;
    public TrackController trackController;
    public float heightAboveGround = 0;
    public int direction = 1;
    public float remainingHaltingTime = 5f;
    public float speed = 1;
    private bool halting = false;
    private float trackProgress = 0.1f;

    private void Start()
    {
        trackController = track.GetComponent<TrackController>();
    }
    // Update is called once per frame
    void Update()
    {
        if (halting)
        {
            return;
        }
        float lastTrackProgress = trackProgress;
        trackProgress += Time.deltaTime * speed * direction;
        if (trackProgress < 0 || trackProgress >= 1)
        {
            // reset the track progress to last frame's value so that the wagons reading the value
            // and then using it to get their position, don't get an OOBE
            trackProgress -= Time.deltaTime * speed * direction;
            halting = true;
            StartCoroutine(WaitToLeaveStation());
            return;
        }

        transform.position = trackController.GetPositionOnTrack(trackProgress) + heightAboveGround * Vector3.up;
        transform.rotation = trackController.GetRotationOnTrack(trackProgress);

        if (direction == -1)
        {
            transform.Rotate(180 * Vector3.up);
        }

        
    }

    public int GetDirection()
    {
        return direction;
    }

    public float GetTrackProgress()
    {
        return trackProgress;
    }

    public TrackController GetTrackController()
    {
        return trackController;
    }

    IEnumerator WaitToLeaveStation()
    {
        while (remainingHaltingTime > 0)
        {
            remainingHaltingTime -= Time.deltaTime;
            yield return null;
        }
        remainingHaltingTime = 5;
        halting = false;
        float numberOfWagons = transform.parent.childCount - 1;
        float startProgress = numberOfWagons / (track.childCount-1);
        direction *= -1;
        trackProgress = direction > 0 ? startProgress : 1 - startProgress;
        
    }
}

