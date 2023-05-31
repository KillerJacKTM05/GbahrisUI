using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SafeZone;

public class Menupause : UIPanels
{
    public TMP_Text title;
    public Textbox titleTextBox;
    public override void RewriteMyLanguage()
    {
        titleTextBox.RewriteTargetText(title, GameManager.Instance.GetGameLanguage());
        base.RewriteMyLanguage();
    }
}
