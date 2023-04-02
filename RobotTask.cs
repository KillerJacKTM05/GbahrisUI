using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafeZone;

public class RobotTask : MonoBehaviour
{
    public bool isActiveNow = false;
    public string Description = "No task.";
    [Tooltip("if it will be a task that has an activation timer, fill below. Else, you can leave it as it is.")]
    public int activationHour = 25;
    public int activationMinute = 60;
    [Tooltip("Urgent Task will be display different.")]
    public bool urgency = false;
    [Tooltip("If there is a successor.")]
    public bool isChainQuest = false;
    public RobotTask nextQuest;
    [Tooltip("If the quest will affect the environment.")]
    public List<TaskInteractedObject> interactionObject = new List<TaskInteractedObject>();
    public bool activate = false;

    private Transform spawnedCursor;
    private Coroutine waitYourTurn;
    private void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        if (isActiveNow)
        {
            spawnedCursor = Instantiate(GameManager.Instance.GetGameSettings().taskDestinationVisualizer) as Transform;
            spawnedCursor.transform.position = transform.position;
            spawnedCursor.SetParent(transform);
        }
        else if(activationHour != 25 && activationMinute != 60)
        {
            waitYourTurn = StartCoroutine(WaitYourTurnRoutine());
        }
    }
    public void CompleteQuestTriggerChain()
    {
        int myPosition = InterfaceManager.Instance.objectivePanelUI.SearchAmongActiveTasks(this);
        nextQuest.isActiveNow = true;
        nextQuest.Initialize();
        DataManager.Instance.UpdatePlayerData(Description, InterfaceManager.Instance.InGameUI.GetHourString(), urgency);
        if (interactionObject.Count > 0)
        {
            for(int i = 0; i < interactionObject.Count; i++)
            {
                if(interactionObject[i].GetInteractionStatus() == false)
                {
                    interactionObject[i].SetActiveStatus(activate);
                }
                else
                {
                    if (interactionObject[i].IsAudioExists())
                    {
                        interactionObject[i].PlayRandomAudio();
                    }
                    else
                    {

                    }
                }
            }
        }

        AudioManager.Instance.PlayRandomClipFromTaskPool();
        InterfaceManager.Instance.objectivePanelUI.UpdateThis(nextQuest, myPosition);
        GameManager.Instance.RemoveAQuestFromTheList(this);
    }
    public void CompleteQuestWithoutChain()
    {
        int myPosition = InterfaceManager.Instance.objectivePanelUI.SearchAmongActiveTasks(this);
        DataManager.Instance.UpdatePlayerData(Description, InterfaceManager.Instance.InGameUI.GetHourString(), urgency);
        if(interactionObject.Count > 0)
        {
            for (int i = 0; i < interactionObject.Count; i++)
            {
                if (interactionObject[i].GetInteractionStatus() == false)
                {
                    interactionObject[i].SetActiveStatus(activate);
                }
                else
                {
                    if (interactionObject[i].IsAudioExists())
                    {
                        interactionObject[i].PlayRandomAudio();
                    }
                    else
                    {

                    }
                }
            }
        }

        AudioManager.Instance.PlayRandomClipFromTaskPool();
        InterfaceManager.Instance.objectivePanelUI.ReleaseContent(myPosition);
        GameManager.Instance.RemoveAQuestFromTheList(this);
    }
    public void Terminate()
    {
        Destroy(spawnedCursor.gameObject);
        Destroy(gameObject);
    }

    //this is only for freeRoaming.
    public void FreeRoam()
    {
        isActiveNow = true;
        Description = "Free Roaming.";

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isChainQuest && isActiveNow)
        {
            CompleteQuestTriggerChain();
        }
        else if(other.CompareTag("Player") && !isChainQuest && isActiveNow)
        {
            CompleteQuestWithoutChain();
        }
    }
    private IEnumerator WaitYourTurnRoutine()
    {
        while (true)
        {
            if(GameManager.Instance.GetHour() >= activationHour && GameManager.Instance.GetMinute() >= activationMinute)
            {
                isActiveNow = true;
                Initialize();
                InterfaceManager.Instance.objectivePanelUI.AddTask(this);
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(60f / GameManager.Instance.GetGameSettings().realWorldTimeForEachGameHour);
            }
        }
    }
}
