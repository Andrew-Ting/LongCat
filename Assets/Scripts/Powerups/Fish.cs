using UnityEngine;

public class Fish : MonoBehaviour
{
    bool collected = false; // for when fish is collected (to not let player collect it twice)

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(!FindObjectOfType<GameManager>().IsFished());
    }
    
    public void CollectFish() //call this function when fish is gonna be collected
    {
        if(!collected)
        {
            collected = true;
            transform.GetComponent<Animator>().SetTrigger("Fished");
            FindObjectOfType<GameManager>().CollectFish();
        }
    }
}
