using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupData : MonoBehaviour
{
    [SerializeField] private DataClass.PowerUp powerupType;
    // Start is called before the first frame update
    public DataClass.PowerUp GetPowerupType() {
        return powerupType;
    }
}
