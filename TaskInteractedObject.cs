using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskInteractedObject : MonoBehaviour
{
    public List<Transform> effectedObject = new List<Transform>();
    public List<AudioClip> effectedClip = new List<AudioClip>();
    [SerializeField]private bool interacted = false;
    [SerializeField]private bool isActiveNow = false;
    private AudioSource thisSource = null;
    public void SetActiveStatus(bool state)
    {
        isActiveNow = state;
        gameObject.SetActive(isActiveNow);
    }
    public void SetInteractionStatus(bool state)
    {
        interacted = state;
    }
    public bool GetActiveStatus()
    {
        return isActiveNow;
    }
    public bool GetInteractionStatus()
    {
        return interacted;
    }
    public bool IsAudioExists()
    {
        if(thisSource == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void SwitchObjectsIfInList(float time)
    {
        StartCoroutine(SlideShow(time));
    }
    public void PlayRandomAudio()
    {
        int rand = Random.Range(0, effectedClip.Count);
        thisSource.clip = effectedClip[rand];
        thisSource.Play();
    }
    void Start()
    {
        if(effectedObject.Count >= 1)
        {
            SwitchObjectsIfInList(5);
        }
        if(effectedClip.Count >= 1)
        {
            thisSource = GetComponent<AudioSource>();
        }
    }
    private IEnumerator SlideShow(float time)
    {
        int counter = 0;
        while (true)
        {
            if(counter > effectedObject.Count - 1)
            {
                counter = 0;
            }
            effectedObject[counter].gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(time);
            effectedObject[counter].gameObject.SetActive(false);
            counter += 1;
        }
    }
}
