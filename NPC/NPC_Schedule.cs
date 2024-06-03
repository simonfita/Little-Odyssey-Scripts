using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class NPC_Schedule : MonoBehaviour
{
    public Schedule schedule;

    private TaskBase overwriteTask;

    [SerializeField,HideInInspector]
    public List<TaskBase> tasks;

    [SerializeField, HideInInspector]
    public TaskBase currentTask;

    [SerializeField, HideInInspector]
    private string _oldSchedule;

    private NPC self;

    private void Start()
    {
        self = GetComponent<NPC>();
  
        if (!Application.isPlaying) //Execute Always fix
            return;       
        foreach (TaskBase task in tasks)
            task.enabled = false;

        currentTask = GetTaskForTime(WorldTime.CurrentDayTime);
        currentTask.enabled = true;
        
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            if ((schedule!=null ? schedule.name : "Null") != _oldSchedule)
            {
                UpdateComponents();
            }
        }
#endif


    }

    public void FollowSchedule()
    {

        TaskBase shouldDo;

        if (overwriteTask != null)
            shouldDo = overwriteTask;
        else
            shouldDo = GetTaskForTime(WorldTime.CurrentDayTime);
        
        if (shouldDo == currentTask) return;     

        if (!currentTask.CanEndTask())
        {
            currentTask.wantsToEnd = true;
        }
        else
        {
            if(currentTask !=null)
                currentTask.enabled = false;
            currentTask = shouldDo;
            currentTask.enabled = true;
        }
    }

    private TaskBase GetTaskForTime(float t)
    {

        for (int i = tasks.Count - 1; i >= 0; i--)
        {
            if (t > tasks[i].startTime)
                return tasks[i];
        }

        return tasks[tasks.Count - 1]; //returns last daily task if it's too early for anything

    }
#if UNITY_EDITOR
    private void UpdateComponents()
    {
        Debug.Log("Added/Removed components - schedule change");
        _oldSchedule = schedule != null ? schedule.name : "Null";

        TaskBase[] currentTasks = GetComponents<TaskBase>();

        for (int i = 0; i < currentTasks.Length; i++) 
        {
            DestroyImmediate(currentTasks[i]);
        }
        tasks = new List<TaskBase>();
        if (schedule != null)
        {

            
            foreach (Schedule.TaskAndTime task in schedule.taskWithTimes)
            {
                TaskBase newTask =  (TaskBase)gameObject.AddComponent(task.component.Type);
                newTask.Initialize(self, task.startTime);
                tasks.Add(newTask);
                
            }
        }
        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        foreach(TaskBase task in tasks)
            PrefabUtility.RecordPrefabInstancePropertyModifications(task);

    }
#endif


    public T AddOverwrideTask<T>() where T : TaskBase
    {
        if (overwriteTask != null)
            Debug.LogError("Added second Overwrite task!");

        overwriteTask = gameObject.AddComponent<T>();
        overwriteTask.enabled = false;  
        overwriteTask.Initialize(self, WorldTime.CurrentDayTime);
        Invoke(nameof(LateOverwriteEnable), 0);//hacky solution

        return (T)overwriteTask;

    }
    private void LateOverwriteEnable()
    {
        overwriteTask.enabled = true;
    }

    public void RemoveOverrideTask()
    {
        if (overwriteTask == null)
            Debug.LogError("Tried to remove non existing override!");

        Destroy(overwriteTask);
    }


}
