using UnityEngine;

[System.Serializable]
public class ToastStyle
{
    public Color textColor = Color.white;
    public Color backgroundColor = new(0, 0, 0, 0.7f);
    public float duration = 2f;
    public bool enableBlink = false;
    public Color blinkColor = Color.red;
}
