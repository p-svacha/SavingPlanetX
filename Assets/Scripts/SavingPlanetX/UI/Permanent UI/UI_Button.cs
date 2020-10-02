using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour
{
    public Button Button;
    public Image Background;
    public Image Image;

    public ColorSettings Colors;

    private void Start()
    {
        Colors = GameObject.Find("GameModel").GetComponent<GameModel>().ColorSettings;
    }

    public void SetEnabled(bool b)
    {
        if(b)
        {
            Background.color = Colors.UI_Interactive_Enabled_Back;
            Image.color = Colors.UI_Interactive_Enabled_Front;
            Button.interactable = true;
        }
        else
        {
            Background.color = Colors.UI_Interactive_Disabled_Back;
            Image.color = Colors.UI_Interactive_Disabled_Front;
            Button.interactable = false;
        }
    }
}
