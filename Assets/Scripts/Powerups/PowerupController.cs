using UnityEngine;

public class PowerupController : MonoBehaviour
{
    [SerializeField] private DataClass.PowerUp powerupType;
    // Start is called before the first frame update
    public DataClass.PowerUp GetPowerupType() {
        return powerupType;
    }
    public void SelfDestruct() {
        Debug.Log("COLLECTED");
        transform.GetComponent<Animator>().SetTrigger("Collect");
    }
    public void DestroyCollider() {
        transform.GetComponent<CapsuleCollider>().enabled = false;
    }
    public void DestroyGameObject()
    {
        Debug.Log("DESTROYING POWERUP");
        gameObject.SetActive(false);
    }
    public void ReviveObject()
    {
        Debug.Log("Reviving");
        gameObject.SetActive(true);
        transform.GetComponent<CapsuleCollider>().enabled = true;
        transform.GetComponent<Animator>().SetTrigger("Revive");
    }
    void Start() // ideally we should find a way for PlayRecord to get the powerups, and not PowerupController to send it to PlayRecord, but the order of start function execution prevents this :(
    {
        PlayRecord playRecord = FindObjectOfType<PlayRecord>();
        playRecord.AddPowerupToList(this.gameObject);
    }
}
