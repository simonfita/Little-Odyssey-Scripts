using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B83;

[CreateAssetMenu(fileName = "Schedule", menuName = "ScriptableObjects/Schedule")]
public class Schedule : ScriptableObject
{
    [System.Serializable]
    public struct TaskAndTime {
        public SerializableMonoScript component;
        public float startTime;
    }

    public List<TaskAndTime> taskWithTimes;

}
