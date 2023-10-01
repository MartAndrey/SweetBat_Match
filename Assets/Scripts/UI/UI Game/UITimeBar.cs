using UnityEngine;
using UnityEngine.UI;

public class UITimeBar : MonoBehaviour
{
    public static UITimeBar Instance;

    [SerializeField] Gradient gradient;
    [SerializeField] Image fillColor;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Method to change the time bar fill based on the factor.
    /// </summary>
    /// <param name="factor">The factor to change the time bar fill.</param>
    public void ChangeTimeBar(float factor)
    {
        factor = 1 - factor;
        fillColor.color = gradient.Evaluate(factor);
        fillColor.fillAmount = factor;
    }
}