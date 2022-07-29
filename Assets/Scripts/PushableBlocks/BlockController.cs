using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public event Action<List<GameObject>> SetBlocksOnTopAsUnmovable;
    private CatMovement catMovement;
    [SerializeField]
    private int blockMaxFallHeight = 30; // if the block falls further than this, it is assumed to have fallen off the world and is deleted
    [SerializeField]
    LayerMask groundLayer = (1 << 6);
    [SerializeField]
    LayerMask blocksLayer = (1 << 7);
    [SerializeField]
    int groundLayerNumber = 6;
    [SerializeField]
    int blocksLayerNumber = 7;
    private List<GameObject> restingPlatforms = new List<GameObject>(); // gameobjects the block is resting on
    private BlockManager blockManager;
    private GroundManager groundManager;
    private bool isPushableThisMove = false;
    private bool isTouchingGroundThisMove = false;
    void Awake() {
        catMovement = FindObjectOfType<CatMovement>();
        blockManager = FindObjectOfType<BlockManager>();
        groundManager = FindObjectOfType<GroundManager>();
    }
    void OnEnable() {
        catMovement.CatMoveAction += BlockPush;
    }
    void OnDisable() {
        catMovement.CatMoveAction -= BlockPush;
    }
    public bool TestDirectBlockPush(Vector3 directionOfMovement) { // test if cat can push piece of the block in given direction
        foreach (Transform child in transform) {
            RaycastHit hitObject;
            bool getCollision = Physics.Raycast(child.position, directionOfMovement, out hitObject, 1, groundLayer | blocksLayer);
            if (getCollision && (hitObject.transform.gameObject.layer == groundLayerNumber || 
                    (hitObject.transform.gameObject.layer == blocksLayerNumber && blockManager.IsSetAsUnmovableBlock(hitObject.transform.parent.gameObject))))
                return false;
        }
        return true;
    }
    void SendBlockToManagerAndRecurseIfMoved(List<GameObject> excludedBlocks) {
        if (excludedBlocks.Contains(gameObject)) // blocks on the ground must move if directly pushed by player
            return;
        if (blockManager.IsSetAsUnmovableBlock(gameObject)) // prevent cyclic action calling
            return;
        blockManager.AddToCurrentUnmovableBlocks(gameObject);
        isPushableThisMove = false;
        SetBlocksOnTopAsUnmovable?.Invoke(excludedBlocks);
    }
    void BlockPush(Vector3 directionOfMovement) { // cat pushes a piece of the block and moves in this direction
        if (isPushableThisMove) 
            isPushableThisMove = false; // reset isPushable variable       
        else return;
        transform.position += new Vector3(directionOfMovement.x, 0, directionOfMovement.z); // only move blocks horizontally; vertical movement handled by BlockManager
    }
    public void SetIsPushableTo(bool newState) {
        isPushableThisMove = newState;
    }
    public DataClass.BlockState FallAndResetRestingPlatforms() { // drops the block until it lands on a platform/block and sets all its resting platforms
        // returns whether the block has fallen or not in this function call. If it falls far enough, it is returned with a mark for deletion (deleted in BlockManager)
        foreach (GameObject lowerBlock in restingPlatforms) {      
            lowerBlock.GetComponent<BlockController>().SetBlocksOnTopAsUnmovable -= SendBlockToManagerAndRecurseIfMoved;
        }       
        if (isTouchingGroundThisMove) {
            isTouchingGroundThisMove = false;
            groundManager.CurrentMoveBlockPopulate -= SendBlockToManagerAndRecurseIfMoved;
        }
        restingPlatforms.Clear();
        int unitsOfFalling = 0; 
        while (unitsOfFalling < blockMaxFallHeight) {
            bool landsOnSomething = false;
            foreach (Transform child in transform) {
                RaycastHit hit;
                if (Physics.Raycast(child.position, -Vector3.up, out hit, 1, groundLayer | blocksLayer)) {
                    GameObject hitObject = hit.transform.gameObject;
                    if (hitObject.layer == groundLayerNumber && !isTouchingGroundThisMove) {
                        isTouchingGroundThisMove = true;
                        landsOnSomething = true;
                        groundManager.CurrentMoveBlockPopulate += SendBlockToManagerAndRecurseIfMoved;
                    }
                    else if (hitObject.layer == blocksLayerNumber && !restingPlatforms.Contains(hitObject.transform.parent.gameObject) && hitObject.transform.parent.gameObject != gameObject) {
                        landsOnSomething = true;
                        restingPlatforms.Add(hitObject.transform.parent.gameObject);
                        hitObject.transform.parent.gameObject.GetComponent<BlockController>().SetBlocksOnTopAsUnmovable += SendBlockToManagerAndRecurseIfMoved;
                        
                    }
                }
            }
            if (landsOnSomething)
                break;
            transform.position -= Vector3.up;
            unitsOfFalling++;
        }
        if (unitsOfFalling == 0)
            return DataClass.BlockState.HasNotFallen;
        if (unitsOfFalling == blockMaxFallHeight)
            return DataClass.BlockState.ToBeDeleted;
        return DataClass.BlockState.HasFallen;
    }
    public void SelfDestruct() {
        gameObject.SetActive(false); // sike; the object is not actually destroyed bc it can be revived with an undo
    }
}
