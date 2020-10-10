using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Event_InfoBox : MonoBehaviour
{
    public Text Title;
    public Text Text;
    public Text IdText;
    public Text ButtonText;
    public UI_Button OkButton;

    public void Initialize(string title, string text, string idText, string buttonText, Action buttonAction)
    {
        Title.text = title;
        Text.text = text;
        IdText.text = idText;
        ButtonText.text = buttonText;
        OkButton.Button.onClick.AddListener(() => DisableButton());
        if (buttonAction != null) OkButton.Button.onClick.AddListener(() => buttonAction.Invoke() );
    }

    private void DisableButton()
    {
        OkButton.SetEnabled(false);
    }
}
