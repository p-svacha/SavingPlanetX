using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour
{
    public Button Button;
    public Image Image;
    public Text Text;

    private Image Background;

    void Start()
    {
        ColorBlock transitionColors = Button.colors;
        transitionColors.disabledColor = Color.white;
        Button.colors = transitionColors;
        Background = Button.GetComponent<Image>();
    }

    public void SetEnabled(bool b)
    {
        if (b)
        {
            if(Background != null) Background.color = ColorSettings.Colors.UI_Interactive_Enabled_Back;
            if(Image != null) Image.color = ColorSettings.Colors.UI_Interactive_Enabled_Front;
            if(Text != null) Text.color = ColorSettings.Colors.UI_Interactive_Enabled_Front;
            Button.interactable = true;
        }
        else
        {
            if(Background != null) Background.color = ColorSettings.Colors.UI_Interactive_Disabled_Back;
            if(Image != null) Image.color = ColorSettings.Colors.UI_Interactive_Disabled_Front;
            if(Text != null) Text.color = ColorSettings.Colors.UI_Interactive_Disabled_Front;
            Button.interactable = false;
        }
    }
}
