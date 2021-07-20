using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowCountController : ItemCountController
{
    public override DataClass.PowerUp GetPowerupType() {
        return DataClass.PowerUp.Grow;
    }
    protected override void UseBerry() {
        catMovement.AttemptGrowth();
    }
}
