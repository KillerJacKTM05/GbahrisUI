using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SafeZone;

public class ObjectivePanel : UIPanels
{
    [SerializeField] private List<TaskContainer> createdTasks = new List<TaskContainer>();
    
    public override void InitializeThis()
    {
        TaskContainer spawnedNewTask = Instantiate(GameManager.Instance.GetGameSettings().taskContainer);
        spawnedNewTask.InitializeThis();
        spawnedNewTask.transform.SetParent(this.transform.GetChild(0));
        createdTasks.Add(spawnedNewTask);
        if(spawnedNewTask.GetRelatedTask() == null)
        {
            ReleaseContent(spawnedNewTask);
        }
        base.InitializeThis();
    }
    public void AddTask(RobotTask task)
    {
        TaskContainer spawned = Instantiate(GameManager.Instance.GetGameSettings().taskContainer);
        spawned.InitializeThis(task);
        createdTasks.Add(spawned);
        spawned.transform.SetParent(this.transform.GetChild(0));
        ReorganizeTheTasks();
    }
    public void UpdateThis(RobotTask newTask, int index)
    {
        createdTasks[index].InitializeThis(newTask);
    }
    private void ReorganizeTheTasks()
    {
        //urgent tasks will be on top, chain tasks will be suprior than standard tasks.
        for (int i = 0; i < createdTasks.Count; i++)
        {
            for (int j = createdTasks.Count - 1; j > 0; j--)
            {

                if (createdTasks[j].GetRelatedTask().urgency && !createdTasks[j - 1].GetRelatedTask().urgency)
                {
                    TaskContainer temp = createdTasks[j - 1];
                    createdTasks[j - 1] = createdTasks[j];
                    createdTasks[j] = temp;
                }
                else if(createdTasks[j].GetRelatedTask().isChainQuest && !createdTasks[j - 1].GetRelatedTask().isChainQuest)
                {
                    TaskContainer temp = createdTasks[j - 1];
                    createdTasks[j - 1] = createdTasks[j];
                    createdTasks[j] = temp;
                }
            }
        }
    }
    public void ReleaseContent(TaskContainer target)
    {
        for(int i = 0; i < createdTasks.Count; i++)
        {
            if(target == createdTasks[i])
            {
                GameObject temp = createdTasks[i].gameObject;
                createdTasks.RemoveAt(i);
                Destroy(temp);
            }
            else
            {
                continue;
            }
        }
    }
    public void ReleaseContent(int index)
    {
        GameObject temp = createdTasks[index].gameObject;
        createdTasks.RemoveAt(index);
        Destroy(temp);
    }
    public int SearchAmongActiveTasks(RobotTask currTask)
    {
        int index = 0;
        for(int i = 0; i < createdTasks.Count; i++)
        {
            if(createdTasks[i].GetRelatedTask() == currTask)
            {
                index = i;
            }
        }

        return index;
    }
    public RobotTask GetRelatedTaskFromList(int index)
    {
        if(createdTasks.Count > index)
        {
            return createdTasks[index].GetRelatedTask();
        }
        else
        {
            return null;
        }
    }
    public int GetTaskUIObjectCount()
    {
        return createdTasks.Count;
    }
}
