using UnityEngine;
using Venice;

public class TimeBreak : MonoBehaviour
{
    /// <summary>
    /// Messy debug script I have laying around. Use keys 1-4 to set different game speeds - useful for debugging!
    /// They all toggle individually and the slowest takes priority, so remember to toggle one setting off before toggling another.
    /// </summary>
    void Update()
    {

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ToggleTimeBreak(.01f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ToggleTimeBreak(.05f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ToggleTimeBreak(.1f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ToggleTimeBreak(.2f);
            }
    }

    public void ToggleTimeBreak(float scale)
    {
        if (Time.timeScale == scale)
        {

            SetTimeScale(1);
        }
        else
        {
            SetTimeScale(scale);
        }
    }

    public static void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }
}
