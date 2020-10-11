using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MR_DisasterContentLine : MonoBehaviour
{
    public Text Name;
    public Text Location;
    public Text Intensity;
    public Text Damage;
    public Text Day;
    public Text State;

    public void Init(string name, string location, string intensity, string damage, string day, string state)
    {
        Name.text = name;
        Location.text = location;
        Intensity.text = intensity;
        Damage.text = damage;
        Day.text = day;
        State.text = state;
    }
}
