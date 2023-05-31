using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SafeZone;

[CreateAssetMenu(fileName = "Textbox", menuName = "DevelopmentSafeZone/Game/Textbox", order = 2)]
public class Textbox : ScriptableObject
{
    public string textBoxEng;
    public string textBoxTr;

    public void RewriteTargetText(TMP_Text tex, Language lang)
    {
        if(lang == Language.English)
        {
            tex.text = textBoxEng;
        }
        else
        {
            tex.text = textBoxTr;
        }
    }
    public string RewriteTargetText(string tex, Language lang)
    {
        if (lang == Language.English)
        {
            tex = textBoxEng;
        }
        else
        {
            tex = textBoxTr;
        }
        return tex;
    }
}
