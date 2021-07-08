using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonWorldSelect : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public GameObject worldSelector;
    public Transform levelSelector;
    [Tooltip("0 is world 1")]
    public int levelSelectorChildIndex;
    [Header("Within self")]
    public GameObject lockGui;

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

        //check if world is unlocked
    }
}
