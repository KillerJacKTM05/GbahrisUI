using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SafeZone;

public class TaskContainer : UIPanels
{
    [SerializeField] private TMP_Text description;
    private RobotTask relatedTask;

    public void InitializeThis(RobotTask reference = null)
    {
        if(reference != null)
        {
            relatedTask = reference;
            description.text = relatedTask.Description;
            if (relatedTask.urgency)
            {
                description.color = Color.red;
                description.fontStyle = FontStyles.Bold;
            }
        }
        else
        {
            description.text = "No Tasks given.";
        }
    }
    public string GetTaskDescription()
    {
        return description.text;
    }
    public RobotTask GetRelatedTask()
    {
        return relatedTask;
    }
}
