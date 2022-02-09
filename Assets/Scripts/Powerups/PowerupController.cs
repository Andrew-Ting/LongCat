using UnityEngine;

public class PowerupController : MonoBehaviour
{
    [SerializeField] private DataClass.PowerUp powerupType;
    // Start is called before the first frame update
    public DataClass.PowerUp GetPowerupType() {
        return powerupType;
    }
    public void SelfDestruct() {

        transform.GetComponent<Animator>().SetTrigger("Collect");
    }
    public void DestroyCollider() {
        Destroy(transform.GetComponent<CapsuleCollider>());
    }
    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
