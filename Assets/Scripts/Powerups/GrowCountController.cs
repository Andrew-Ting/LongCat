using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowCountController : ItemCountController
{
    void Start()
    {
        base.Start();
        playRecord.UndoEvent += moveState => UpdateItemCountWithUndoIndex(moveState, 0);
    }
    public override DataClass.PowerUp GetPowerupType() {
        return DataClass.PowerUp.Grow;
    }
    protected override void UseBerry() {
        catMovement.AttemptGrowth();
    }
}
