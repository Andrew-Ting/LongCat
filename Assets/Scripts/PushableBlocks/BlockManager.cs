using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [SerializeField] 
    private int maxBlockStackHeight = 5; // lower value means less stack before bugs, higher value means slower before next move can be made
    private List<GameObject> blocks = new List<GameObject>();
    private GroundManager groundManager;
    private List<GameObject> unmovableBlocksOfCurrentMove = new List<GameObject>(); // populated when groundManager's PopulateBlockManagerCurrentMove is called
    private CatMovement catMovement;
    void Start()
    {
        groundManager = FindObjectOfType<GroundManager>();
        catMovement = FindObjectOfType<CatMovement>();
        foreach (Transform children in transform) {
            blocks.Add(children.gameObject);
        }
        StartCoroutine(BlockFallSimulation());
        catMovement.CatMoveAction += (Vector3 direction) => BlockFallManagement();
    }

    public List<GameObject> GetAllBlocks() {
        return blocks;
    }
    public int GetNumberOfTotalBlocks() {
        return blocks.Count;
    }
    public void BlockFallManagement() { // has side effect of moving cat; use BlockFallSimulation for the function without side effect
        StartCoroutine(BlockFallSimulation());
        catMovement.ReadyForMovement();
    }
    private IEnumerator BlockFallSimulation() {
        catMovement.SetAreBlocksMoving(true);
        yield return new WaitForFixedUpdate(); // gives a frame for block colliders to move horizontally
        for (int i = 0; i < maxBlockStackHeight; i++) {
            bool hasChange = false;
            foreach (GameObject block in blocks) { // a possible optimization is only considering movableBlocks, but introduces strong function coupling
                hasChange = hasChange || block.GetComponent<BlockController>().FallAndResetRestingPlatforms();
                yield return new WaitForFixedUpdate();
            }
            if (!hasChange) // break early when no more blocks are moving
                break;
        }
        catMovement.SetAreBlocksMoving(false);
    }
    public void SetAllBlockMovableStateTo(bool newState) {
        foreach (GameObject block in blocks) {
            block.GetComponent<BlockController>().SetIsPushableTo(newState);
        }
    }
    public void AddToCurrentUnmovableBlocks(GameObject block) {
        unmovableBlocksOfCurrentMove.Add(block);
    }
    public bool IsSetAsUnmovableBlock(GameObject block) {
        return unmovableBlocksOfCurrentMove.Contains(block);
    }

    public List<GameObject> GetAllMovableBlocks(List<GameObject> blocksOnGroundExcluded) { // has side effect, use carefully
        unmovableBlocksOfCurrentMove.Clear();
        List<GameObject> movedBlocks = new List<GameObject>(blocks);
        groundManager.PopulateBlockManagerCurrentMove(blocksOnGroundExcluded); 
        foreach (GameObject block in unmovableBlocksOfCurrentMove) { // this is O(N^2), but block count is really small anyway xd
            movedBlocks.Remove(block);
        }
        return movedBlocks;
    }
}
