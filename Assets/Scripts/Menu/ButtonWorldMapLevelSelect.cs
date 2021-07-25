using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonWorldMapLevelSelect : MonoBehaviour, IPointerClickHandler
{
    [Header("Data")]
    public LevelData level;
    [Header("References")]
    public GameObject lockGui;
    public Text levelNumberText;

    private PlayerManager playerManager;
    private bool locked;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!locked)
        {
            //load scene
            Debug.Log("not locked");
        }
        else
        {
            Debug.Log("locked");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        locked = !playerManager.IsUnlockedID(level.id);
        lockGui.SetActive(locked);
        levelNumberText.text = level.levelNumber.ToString();
    }
}
