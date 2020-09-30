using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InfoBox : MonoBehaviour
{
    public Text Title;
    public Text Text;
    public Text IdText;
    public Text ButtonText;
    public Button OkButton;

    public void Initialize(string title, string text, string idText, string buttonText, Action buttonAction)
    {
        Title.text = title;
        Text.text = text;
        IdText.text = idText;
        ButtonText.text = buttonText;
        OkButton.onClick.AddListener(() => DestroySelf());
        if (buttonAction != null) OkButton.onClick.AddListener(() => buttonAction.Invoke() );
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
