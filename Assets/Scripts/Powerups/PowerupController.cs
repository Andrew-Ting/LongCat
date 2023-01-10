using UnityEngine;

public class PowerupController : MonoBehaviour
{
    [SerializeField] private DataClass.PowerUp powerupType;

    public DataClass.PowerUp GetPowerupType() {
        return powerupType;
    }
    public void SelfDestruct() {
        transform.GetComponent<Animator>().SetTrigger("Collect");
    }
    public void DestroyCollider() {
        transform.GetComponent<CapsuleCollider>().enabled = false;
    }

    public void ForceAnimationComplete()
    {
        transform.GetComponent<Animator>().SetTrigger("AnimComplete");
    }
    public void ReviveObject()
    {
        transform.GetComponent<CapsuleCollider>().enabled = true;
        transform.GetComponent<Animator>().SetTrigger("Revive");
    }
    public bool isActive()
    {
        return transform.GetComponent<CapsuleCollider>().enabled; // a deactivated collider is an uncollectible powerup
    }
    void Start() // ideally we should find a way for PlayRecord to get the powerups, and not PowerupController to send it to PlayRecord, but the order of start function execution prevents this :(
    {
        PlayRecord playRecord = FindObjectOfType<PlayRecord>();
        playRecord.AddPowerupToList(this.gameObject);
        playRecord.UndoEvent += (moveState) => ForceAnimationComplete();
    }
}
