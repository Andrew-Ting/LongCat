using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkCountCountroller : ItemCountController
{
    public override DataClass.PowerUp GetPowerupType() {
        return DataClass.PowerUp.Shrink;
    }
    protected override void UseBerry() {
        catMovement.AttemptShrink();
    }
}
