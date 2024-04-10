using UnityEngine;

public class LocomotiveController : VehicleController
{
    private Transform track;
    private TrackController trackController;
    public float heightAboveGround = 0;
    private float locToTrackRatio;
    public float locLength;

    private void Awake()
    {
        track = GameObject.Find("Track").transform;
        trackController = track.GetComponent<TrackController>();
        startEvent += () => HandleStartEvent();
        locToTrackRatio = locLength / trackController.GetTrackPieceLength() / trackController.transform.childCount;
    }

    private void OnEnable()
    {
        // activate steam particles
        transform.GetChild(1).gameObject.SetActive(true);
    }
    // Update is called once per frame
    protected override void ProgressTo(float progress)
    {
        transform.position = trackController.GetPositionOnTrack(progress) + heightAboveGround * Vector3.up;
        transform.rotation = trackController.GetRotationOnTrack(progress);

        if (direction == -1)
        {
            transform.Rotate(180 * Vector3.up);
        }

        for (int i = 1; i < transform.parent.childCount; i++)
        {
            transform.parent.GetChild(i).GetComponent<WagonController>().FollowLocAt(progress, direction);
        }
    }

    public TrackController GetTrackController()
    {
        return trackController;
    }

    private void HandleStartEvent()
    {
        float numberOfWagons = transform.parent.childCount - 1;
        float startProgress = locToTrackRatio;
        for (int i = 1; i < transform.parent.childCount; i++)
        {
            startProgress += transform.parent.GetChild(i).GetComponent<WagonController>().GetWagonToTrackRatio();
        }
        SetProgress(direction > 0 ? startProgress : 1 - startProgress);
    }
}

