using UnityEngine;

public class Fish : MonoBehaviour
{
    bool collected = false; // for when fish is collected (to not let player collect it twice)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollect() //call this function when fish is gonna be collected
    {
        collected = true;
        transform.GetComponent<Animator>().SetTrigger("Fished");
    }
}
