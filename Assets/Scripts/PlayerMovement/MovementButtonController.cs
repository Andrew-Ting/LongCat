using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MovementButtonController : MonoBehaviour
{
    private CatMovement catMovement;
    [SerializeField] private DataClass.Directions directionOfMovement;
    void Awake() 
    {
        catMovement = FindObjectOfType<CatMovement>();
    }

    void Start()
    {
       Button thisButton = GetComponent<Button>();
       thisButton.onClick.AddListener(() => {catMovement.MoveCat(directionOfMovement);});
    }
}
