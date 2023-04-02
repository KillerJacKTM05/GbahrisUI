using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafeZone;
using TMPro;
public class InGameUI : UIPanels
{
    [SerializeField] private TMP_Text hourText;
    private string hourString;
    // Start is called before the first frame update
    public void UpdateHourString(int[] hour, int[] minute)
    {
        if(GameManager.Instance.GetHour() < 12 && GameManager.Instance.GetHour() >= 0)
        {
            hourString = hour[1].ToString() + hour[0].ToString() + ":" + minute[1].ToString() + minute[0].ToString() + " AM";
        }
        else if(GameManager.Instance.GetHour() == 12)
        {
            hourString = hour[1].ToString() + hour[0].ToString() + ":" + minute[1].ToString() + minute[0].ToString() + " PM";
        }
        else if(GameManager.Instance.GetHour() > 12 && GameManager.Instance.GetHour() < 24)
        {
            var newHour = GameManager.Instance.GetHour() - 12;
            var newHour1 = newHour / 10;
            var newHour2 = newHour % 10;
            hourString = newHour1.ToString() + newHour2.ToString() + ":" + minute[1].ToString() + minute[0].ToString() + " PM";
        }

        hourText.text = hourString;
    }
    public string GetHourString()
    {
        return hourString;
    }
}
