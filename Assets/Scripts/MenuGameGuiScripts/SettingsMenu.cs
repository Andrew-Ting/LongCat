using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider BGMslider;
    public Slider SFXslider;

    void Start()
    {
        BGMslider.value = PlayerPrefs.GetFloat("BGM", 1f);
        SFXslider.value = PlayerPrefs.GetFloat("SFX", 1f);

        BGMslider.onValueChanged.AddListener(delegate { PlayerPrefs.SetFloat("BGM", BGMslider.value); FindObjectOfType<AudioManager>().UpdateBGMVolume(); });
        SFXslider.onValueChanged.AddListener(delegate { PlayerPrefs.SetFloat("SFX", SFXslider.value); });
    }

    public void DeletePlayerFiles()
    {
        SaveLoadManager.DeletePlayer();
        SaveLoadManager.DeleteGameData();
        LevelLoader.ReturnHome();
    }

    public void GoHomeFunc()
    {
        LevelLoader.ReturnHome();
    }
}
