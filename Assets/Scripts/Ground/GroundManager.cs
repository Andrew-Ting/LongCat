using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public event Action<List<GameObject>> CurrentMoveBlockPopulate;
    // Start is called before the first frame update

    public void PopulateBlockManagerCurrentMove(List<GameObject> excludedBlocks) { // has side effect of setting blockcontroller's isPushableThisMove state; should ideally be separated when movement is extended
        CurrentMoveBlockPopulate?.Invoke(excludedBlocks);
    }
}
