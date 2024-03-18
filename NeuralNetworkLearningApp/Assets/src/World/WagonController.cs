using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VehicleInteraction))]
public class WagonController: MonoBehaviour
{
    private LocomotiveController locController;
    private TrackController trackController;
    private VehicleInteraction vehicleInteraction;
    
    public float wagonLength = 14.2f;
    public float heightAboveGround = 1;
    private float wagonToTrackRatio;
    private bool halting;
    
    private void Start()
    {
        locController = transform.parent.GetChild(0).GetComponent<LocomotiveController>();
        trackController = locController.GetTrackController();
        vehicleInteraction = transform.GetComponent<VehicleInteraction>();
        wagonToTrackRatio = wagonLength / trackController.GetTrackPieceLength() / trackController.transform.childCount;
        locController.startEvent += () => vehicleInteraction.enabled = false;
        locController.stopEvent += () => vehicleInteraction.enabled = true;
    }

    private void Update()
    {
    }

    public void FollowLocAt(float progress, int locDirection) {
        if (Time.frameCount % 500 == 0)
        {
            print("wagon follows to " + progress);
        }
        float wagonTrackProgress = progress - locDirection * (transform.GetSiblingIndex() * wagonToTrackRatio + 0.003f);
        transform.position = trackController.GetPositionOnTrack(wagonTrackProgress) + heightAboveGround * Vector3.up;
        transform.rotation = trackController.GetRotationOnTrack(wagonTrackProgress);

        transform.Rotate(Vector3.up * (180 - locDirection * 90));
    }

    public float GetWagonToTrackRatio()
    {
        print("first returning " + wagonToTrackRatio);
        return wagonToTrackRatio;
    }
}
