using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ItemCountController : MonoBehaviour
{
    protected int countOfItem = 0;
    protected Text countText;
    protected CatMovement catMovement;
    // Start is called before the first frame update
    void Start()
    {
        countText = GetComponent<Text>();
        catMovement = FindObjectOfType<CatMovement>();
        Button thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(() => {UseBerry();}); 
        UpdateCountText();
    }
    public bool AttemptDeductCountOfItem() { // returns true and deducts item if player has it; returns false otherwise  
        if (countOfItem <= 0) {
            Debug.Log("Player does not have any more of this powerup");
            return false;
        }
        countOfItem--;
        UpdateCountText();
        return true;
    }
    public void AddCountOfItem() {
        countOfItem++;
        UpdateCountText();
    }
    private void UpdateCountText() {
        countText.text = countOfItem.ToString();
    }
    protected abstract void UseBerry();
    public abstract DataClass.PowerUp GetPowerupType();
}
