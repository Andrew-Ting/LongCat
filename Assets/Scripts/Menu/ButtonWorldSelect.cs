using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonWorldSelect : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public GameObject worldSelector;
    public Transform levelSelector;
    [Tooltip("world start index is 0")]
    public int levelSelectorChildIndex;
    [Header("Within self")]
    public GameObject lockGui;

    private PlayerManager playerManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        worldSelector.SetActive(false);
        FindObjectOfType<MenuCamera>().MoveCamera(FindObjectOfType<MenuManager>().cameraViews[(int)DataClass.MenuCameraViews.noView2]);
        foreach(Transform child in levelSelector.Find("Worlds"))
        {
            child.gameObject.SetActive(false);
        }
        levelSelector.Find("Worlds").GetChild(levelSelectorChildIndex).gameObject.SetActive(true);
        levelSelector.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        //check if world is unlocked
        lockGui.SetActive(!playerManager.IsUnlockedWorld(levelSelectorChildIndex));
    }
}
