using UnityEngine;

public class GraphicsQuality : MonoBehaviour
{
    [SerializeField] private int defaultLevel = 2;

    void Start()
    {
        int level = Mathf.Clamp(PlayerPrefs.GetInt("SavedQualityLevel", defaultLevel), 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(level, true);
    }

    public void SetLow()
    {
        SetLevel(0);
    }

    public void SetMedium()
    {
        SetLevel(1);
    }

    public void SetHigh()
    {
        SetLevel(2);
    }

    void SetLevel(int level)
    {
        level = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(level, true);
        PlayerPrefs.SetInt("SavedQualityLevel", level);
        PlayerPrefs.Save();
    }
}
