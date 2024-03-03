using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonController: MonoBehaviour
{
    public LocomotiveController locController;
    public TrackController trackController;
    public float trackPieceLength = 23.8f;
    public float wagonLength = 14.2f;
    public float heightAboveGround = 1;
    
    // Update is called once per frame
    void Update()
    {
        float locTrackProgress = locController.GetTrackProgress();
        float wagonTrackProgress = locTrackProgress - locController.GetDirection() * transform.GetSiblingIndex() * wagonLength / trackPieceLength / 25;
        transform.position = trackController.GetPositionOnTrack(wagonTrackProgress) + heightAboveGround * Vector3.up;
        transform.rotation = trackController.GetRotationOnTrack(wagonTrackProgress);
        transform.Rotate(Vector3.up * 90);
    }
}
