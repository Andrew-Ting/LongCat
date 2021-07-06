using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [SerializeField] 
    private int maxBlockStackHeight = 10; // lower value means less stack before bugs, higher value means slower before next move can be made
    private List<GameObject> blocks = new List<GameObject>();
    private GroundManager groundManager;
    private List<GameObject> unmovableBlocksOfCurrentMove = new List<GameObject>(); // populated when groundManager's PopulateBlockManagerCurrentMove is called
    private int blockHorizontallyMovedCount = 0;
    private CatMovement catMovement;
    void Start()
    {
        groundManager = FindObjectOfType<GroundManager>();
        catMovement = FindObjectOfType<CatMovement>();
        foreach (Transform children in transform) {
            blocks.Add(children.gameObject);
        }
        StartCoroutine(BlockFallSimulation());
    }

    public List<GameObject> GetAllBlocks() {
        return blocks;
    }
    public int GetNumberOfTotalBlocks() {
        return blocks.Count;
    }
    public void BlockFallManagement() { // has side effect of moving cat; use BlockFallSimulation for the function without side effect
        blockHorizontallyMovedCount++;
        if (blockHorizontallyMovedCount != blocks.Count - unmovableBlocksOfCurrentMove.Count) // pseudo mutex lock; only let gravity take place when all blocks have moved horizontally
            return;
        blockHorizontallyMovedCount = 0;
        StartCoroutine(BlockFallSimulation());
        catMovement.ReadyForMovement();
    }
    private IEnumerator BlockFallSimulation() {
        catMovement.SetAreBlocksMoving(true);
        yield return new WaitForSeconds(0); // gives a frame for blocks to move horizontally
        for (int i = 0; i < maxBlockStackHeight; i++) {
            foreach (GameObject block in blocks) { // a possible optimization is only considering movableBlocks. Easily done by making a copy in this class taken from GetAllMovableBlocks, but introduces strong function coupling
                block.GetComponent<BlockController>().FallAndResetRestingPlatforms();
                yield return new WaitForSeconds(0);
            }
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
        // add mutex lock here
        foreach (GameObject block in unmovableBlocksOfCurrentMove) { // this is O(N^2), but block count is really small anyway xd
            movedBlocks.Remove(block);
        }
        return movedBlocks;
    }
}
