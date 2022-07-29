using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public event Action moveCompleted;
    [SerializeField] 
    private int maxBlockStackHeight = 5; // lower value means less stack before bugs, higher value means slower before next move can be made
    private List<GameObject> blocks = new List<GameObject>();
    private GroundManager groundManager;
    private List<GameObject> unmovableBlocksOfCurrentMove = new List<GameObject>(); // populated when groundManager's PopulateBlockManagerCurrentMove is called
    private CatMovement catMovement;
    private PlayRecord playRecord;
    void Start()
    {
        groundManager = FindObjectOfType<GroundManager>();
        catMovement = FindObjectOfType<CatMovement>();
        playRecord = FindObjectOfType<PlayRecord>();
        foreach (Transform children in transform) {
            blocks.Add(children.gameObject);
        }
        FindObjectOfType<PlayRecord>().InitializeBlockManager(this, blocks); // terrible model of calling a class to reference it. TODO: find a better solution to reference this properly in PlayRecord despite not being initialized during PlayRecord's start
        StartCoroutine(BlockFallSimulation());
        catMovement.CatMoveAction += (Vector3 direction) => BlockFallManagement();
        playRecord.UndoEvent += ResetBlocksToState;
    }

    private void ResetBlocksToState(PlayRecord.MoveState moveState)
    {
        List<PlayRecord.Block> blockMetadata = moveState.blockMetadata;
        IList<GameObject> allGameBlocks = playRecord.GetAllBlocksInGame();
        List<GameObject> blocksRerolled = new List<GameObject>();
        for (int i = 0; i < blockMetadata.Count; i++)
        {
            PlayRecord.Block currentBlock = blockMetadata[i];
            if (currentBlock.isActive)
            {
                allGameBlocks[i].SetActive(true);
                allGameBlocks[i].transform.position = currentBlock.position;
                blocksRerolled.Add(allGameBlocks[i]);
            } else
            {
                allGameBlocks[i].SetActive(false);
            }
        }
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
        List <GameObject> toBeDeletedBlocks = new List<GameObject>();
        for (int i = 0; i < maxBlockStackHeight; i++) {
            bool hasChange = false;
            foreach (GameObject block in blocks) { // a possible optimization is only considering movableBlocks, but introduces strong function coupling
                DataClass.BlockState blockState = block.GetComponent<BlockController>().FallAndResetRestingPlatforms(); // if you get a null reference exception in this line, you forgot to put a blockcontroller object on a block
                if (blockState != DataClass.BlockState.HasNotFallen) {
                    hasChange = true;
                    yield return new WaitForFixedUpdate();
                }
                if (blockState == DataClass.BlockState.ToBeDeleted)
                    toBeDeletedBlocks.Add(block);
            }
            if (!hasChange) // break early when no more blocks are moving
                break;
            foreach (GameObject block in toBeDeletedBlocks) { // delete all blocks labelled as TobeDeleted
                block.GetComponent<BlockController>().SelfDestruct();
                blocks.Remove(block);
            }
            toBeDeletedBlocks.Clear();
        }
        moveCompleted?.Invoke();
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
    public IList<GameObject> GetCurrentBlocksState()
    {
        return blocks.AsReadOnly();
    }
}
