using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InfoBlob : MonoBehaviour
{
    private float StartHeight = 1f;
    private float EndHeight = 2.5f;
    private float Duration = 2f;

    private float CurrentDuration;

    public GameObject Target; // Blob will be visible above this object
    public Text BlobText;
    public Image Background;

    public void Initialize(GameObject target, string blobText, Color textColor, Color bgColor)
    {
        Target = target;
        BlobText.text = blobText;
        BlobText.color = textColor;
        Background.color = bgColor;
        transform.position = Camera.main.WorldToScreenPoint(new Vector3(Target.transform.position.x, StartHeight, Target.transform.position.z));
        CurrentDuration = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentDuration += Time.deltaTime;
        float height = Mathf.Lerp(StartHeight, EndHeight, CurrentDuration / Duration);
        transform.position = Camera.main.WorldToScreenPoint(new Vector3(Target.transform.position.x, height, Target.transform.position.z));
        if (CurrentDuration > Duration) Destroy(gameObject);
    }
}
