using UnityEngine;
using UnityEngine.UI;

public class FishCounter : MonoBehaviour
{
    PlayerManager playerManager;
    public Text fishCountText;

    // Start is called before the first frame update
    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }
    private void Start()
    {
        ChangeText(playerManager.GetFishCount());
    }
    void ChangeText(int n)
    {
        fishCountText.text = n.ToString();
    }
    void FishChange(int fishCount)
    {
        ChangeText(fishCount);
    }
    void OnEnable()
    {
        playerManager.fishCounter += FishChange;
        ChangeText(playerManager.GetFishCount());
    }
    void OnDisable()
    {
        playerManager.fishCounter -= FishChange;
    }
}
