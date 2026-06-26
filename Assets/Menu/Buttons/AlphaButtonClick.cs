using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AlphaButtonClick : MonoBehaviour
{
    [Range(0f, 1f)]
    public float alphaThreshold = 0.1f;

    void Start()
    {
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.alphaHitTestMinimumThreshold = alphaThreshold;
        }
    }
}