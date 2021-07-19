using UnityEngine;
using System;
using System.Collections.Generic;

public class CatMovement : MonoBehaviour
{
    [SerializeField]
    private int catHeight;
    [SerializeField]
    LayerMask blocksLayer = (1 << 7);
    [SerializeField]
    int blocksLayerNumber = 7;
    [SerializeField]
    LayerMask objects = (1 << 6 | 1 << 7);
    public event Action<Vector3> CatMoveAction;
    private CameraController cameraController;
    private BlockManager blockManager;
    private Vector3 moveCatVector; // direction cat will move once it is ready for movement
    private bool areBlocksMoving = false;
    public void MoveCat(DataClass.Directions dirIndex)
    {
        if (areBlocksMoving) // you don't want the cat to be able to move as blocks are falling; opens a can of worms in logic
            return;
        int p = ((int)dirIndex + (int)cameraController.GetCameraView()) % 4;
        int dir = p < 2 ? 1 : -1;
        Vector3 newDirection = p % 2 == 0 ? dir * Vector3.forward : dir * Vector3.right;
        FaceDirection(newDirection);
        Vector3 newMoveDirection = newDirection; // this is relative
        if (!Physics.Raycast(transform.position + Vector3.up * catHeight + transform.forward, -Vector3.up, catHeight, objects))
        {
            Ray ray = new Ray(transform.position + transform.forward, -Vector3.up);
            RaycastHit hit;
            //nothing is in front of it
            if (Physics.Raycast(ray, out hit, 2, objects))
            {
                //hits something
                if(hit.transform.position.y + 1 != transform.position.y)
                {
                    newMoveDirection -= Vector3.up;
                }
                //else make newMoveDirection forward(but since it's already set as that anyway)
            }
            else
            {
                //can't move
                newMoveDirection = Vector3.zero;
            }
        }
        else
        {
            //hits something / something is blocking the way
            bool canClimb = false;
            Vector3 blockClimb = Vector3.zero;
            for(int i = 0; i < catHeight - 1; i++)
            {
                Vector3 blockCheck = transform.position + transform.forward + Vector3.up * i;
                if (!Physics.Raycast(blockCheck, Vector3.up, 1, objects) && Physics.Raycast(blockCheck + Vector3.up, -Vector3.up, 1, objects))
                {
                    canClimb = true;
                    blockClimb = blockCheck;
                    break;
                }
            }
            if(canClimb)
            {
                Vector3 finalPos = newDirection * (catHeight + (int)transform.position.y - 1 - (int)blockClimb.y) - Vector3.up * ((int)transform.position.y - 1 - (int)blockClimb.y);
                Vector3 catHeightClimb = new Vector3(transform.position.x, finalPos.y + transform.position.y, transform.position.z);
                float dist = Vector3.Distance(catHeightClimb, transform.position + finalPos);
                if (!Physics.Raycast(catHeightClimb, newDirection, dist, objects)) //check if climb space is not occupied horizontally
                {
                    if (Physics.Raycast(transform.position + finalPos, -Vector3.up, 1, objects)) // check if theres ground at final space 
                    {
                        if(!Physics.Raycast(transform.position + finalPos, Vector3.up, catHeight -1, objects)) // height at final pos is enough
                        {
                            newMoveDirection = finalPos;
                        }
                        else
                        {
                            //add animation for no height room?
                            newMoveDirection = Vector3.zero;
                            Debug.Log("final space not enough height");
                        }
                    }
                    else
                    {
                        //add animation for no ground at final space?
                        newMoveDirection = Vector3.zero;
                        Debug.Log("no block below to land on");
                    }
                }
                else
                {
                    //add animation for block along the way?
                    newMoveDirection = Vector3.zero;
                    Debug.Log("Something is blocking on space to land on");
                }
            } 
            // if the block can't be climbed, can it be pushed?
            else
            {

                if (PushConditionMet()) { 
                    // can be pushed
                    GameObject catLandingPositionSameHeight = GetHitObjectAt(transform.position + transform.forward - Vector3.up, objects);
                    if (NotGoodLandingSpot(catLandingPositionSameHeight)) // if push conditions are met and player does not maintain its height, it must go down
                        newMoveDirection -= Vector3.up;

                }
                else {
                    newMoveDirection = Vector3.zero;
                    blockManager.SetAllBlockMovableStateTo(false);
                }
            }
        }
        moveCatVector = newMoveDirection;
        CatMoveAction?.Invoke(newMoveDirection);
    }

    public void ReadyForMovement() { // called by BlockManager when all blocks have moved to fixed position
        transform.position += moveCatVector;
    }
    public void SetAreBlocksMoving(bool newState) {
        areBlocksMoving = newState;
    }
    bool PushConditionMet() {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position + Vector3.up * catHeight + transform.forward, -Vector3.up, catHeight, blocksLayer);
        if (hits.Length != catHeight) // the blocks don't span the height of the player
            return false;
        if (Physics.Raycast(transform.position + Vector3.up * (catHeight - 1) + transform.forward, Vector3.up, blocksLayer)) // the block structure is taller than player
            return false;
        List<GameObject> directlyPushedBlocksList = new List<GameObject>();
        foreach (RaycastHit hit in hits) {
            if (!directlyPushedBlocksList.Contains(hit.transform.parent.gameObject))
                directlyPushedBlocksList.Add(hit.transform.parent.gameObject);
        }
        blockManager.SetAllBlockMovableStateTo(true);
        List<GameObject> allMovedBlocks = blockManager.GetAllMovableBlocks(directlyPushedBlocksList); // gets all blocks that move given the ones directly pushed
        foreach (GameObject block in allMovedBlocks) // loop through all blocks that must be moved and check if anything is in the way
        {
            if (!block.GetComponent<BlockController>().TestDirectBlockPush(transform.forward)) {// something is in the way for this block
                return false;
            }
        }
        GameObject floorBelowCat = GetHitObjectAt(transform.position - Vector3.up, objects);
        // don't push a block if you're also standing on it, or it pushes the block you're standing on
        if (floorBelowCat.layer == blocksLayerNumber && !blockManager.IsSetAsUnmovableBlock(floorBelowCat.transform.parent.gameObject))
            return false;
        GameObject catLandingPositionSameHeight = GetHitObjectAt(transform.position + transform.forward - Vector3.up, objects);
        GameObject catLandingPositionMoveDown = GetHitObjectAt(transform.position + transform.forward - (Vector3.up * 2), objects);
        if (NotGoodLandingSpot(catLandingPositionMoveDown) && NotGoodLandingSpot(catLandingPositionSameHeight))
            return false;
        return true;
    }
    GameObject GetHitObjectAt(Vector3 position, LayerMask layerMask) {
        RaycastHit hitObject;
        bool hitAnObject = Physics.Raycast(position - Vector3.up, Vector3.up, out hitObject, layerMask);
        if (!hitAnObject)
            return null;
        return hitObject.transform.gameObject;
    }
    bool NotGoodLandingSpot(GameObject landPosition) {
        if (landPosition == null)
            return true;
        if (landPosition.layer == blocksLayerNumber && !blockManager.IsSetAsUnmovableBlock(landPosition.transform.parent.gameObject))
            return true;
        return false;
    }
    void FaceDirection(Vector3 direction)
    {
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        transform.Rotate(Vector3.up, angle);
    }

    void Awake()
    {
        cameraController = FindObjectOfType<CameraController>();
        blockManager = FindObjectOfType<BlockManager>();
    }

    void Start()
    {

    }
}
