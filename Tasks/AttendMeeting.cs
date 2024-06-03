using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttendMeeting : TaskBase
{
    public LocationMeetingArea meetingArea;
    private float timeToMove = 0;
    private Vector3 originalPos;

    protected override void OnEnable()
    {
        if (npc == null)
            return;
        base.OnEnable();
        originalPos = npc.transform.position;
        npc.movement.Teleport(meetingArea.GetPosition());
    }

    protected override void Update ()
    {
        timeToMove -= Time.deltaTime;

        if (timeToMove > 0)
            return;
        
        timeToMove = Random.Range(2, 10);

        npc.movement.GoTo(meetingArea.GetPosition(),true);
    }

    protected override void OnDisable()
    {
        if (npc == null)
            return;
        base.OnDisable();
        npc.movement.Teleport(originalPos);
    }
}
