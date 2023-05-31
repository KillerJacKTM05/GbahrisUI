using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SafeZone;

public class ScenarioEnd : UIPanels
{
    public List<TMP_Text> descriptions = new List<TMP_Text>();
    public List<Textbox> languageBoxes = new List<Textbox>();

    public override void RewriteMyLanguage()
    {
        for(int i = 0; i < descriptions.Count; i++)
        {
            languageBoxes[i].RewriteTargetText(descriptions[i], GameManager.Instance.GetGameLanguage());
        }
        base.RewriteMyLanguage();
    }
}
