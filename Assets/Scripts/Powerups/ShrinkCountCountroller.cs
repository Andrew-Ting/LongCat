using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkCountCountroller : ItemCountController
{
    void Start()
    {
        base.Start();
        playRecord.UndoEvent += moveState => UpdateItemCountWithUndoIndex(moveState, 1);
    }
    public override DataClass.PowerUp GetPowerupType() {
        return DataClass.PowerUp.Shrink;
    }
    protected override void UseBerry() {
        catMovement.AttemptShrink();
    }

}
