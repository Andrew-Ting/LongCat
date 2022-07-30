using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CatMovement : MonoBehaviour
{
    [SerializeField]
    private int catHeight;
    [SerializeField]
    LayerMask blocksLayer = (1 << 7);
    [SerializeField]
    LayerMask powerupLayer = (1 << 8);
    [SerializeField]
    LayerMask fishLayer = (1 << 10);
    [SerializeField]
    int blocksLayerNumber = 7;
    [SerializeField]
    LayerMask objects = (1 << 6 | 1 << 7);
    public event Action<Vector3> CatMoveAction;
    public event Action<bool> CatLoaded; //used in CameraController
    public event Action<int> CatHeightChangeAction;

    private CameraController cameraController;
    private PlayRecord playRecord;
    private BlockManager blockManager;
    private Vector3 moveCatVector; // direction cat will move once it is ready for movement
    private bool areBlocksMoving = false;
    private bool isCatClimbing = false;
    private Dictionary<DataClass.PowerUp, ItemCountController> itemCountController;
    private GameManager gameManager;
    private DataClass.Directions catFacingDirection;
    //for animation
    [Header("For animation")]
    [SerializeField]private float animSpeed = 0.05f;
    [SerializeField] private float rotateSpeed = 0.04f;
    private Transform catModelGameObject;

    [SerializeField] private float climbAnimationTime = 4f;
    public void MoveCat(DataClass.Directions dirIndex)
    {
        if (areBlocksMoving || Time.timeScale == 0 || isCatClimbing) // you don't want the cat to be able to move as blocks are falling; opens a can of worms in logic
            return;
        catFacingDirection = dirIndex;
        Vector3 newDirection = ConvertCatDirectionToVector3(dirIndex);
        StopCoroutine(CatRotate(newDirection));
        StartCoroutine(CatRotate(newDirection));
        Vector3 newMoveDirection = newDirection; // this is relative
        if (!Physics.Raycast(transform.position + Vector3.up * catHeight + transform.forward, -Vector3.up, catHeight, objects)) //nothing is in front of it
        {
            Ray ray = new Ray(transform.position + transform.forward, -Vector3.up);
            if (Physics.Raycast(ray, out RaycastHit hit, 2, objects))
            {
                //hits something
                if (Mathf.RoundToInt(hit.transform.position.y) + 1 != transform.position.y)
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
            for (int i = 0; i < catHeight - 1; i++)
            {
                Vector3 blockCheck = transform.position + transform.forward + Vector3.up * i;
                if (!Physics.Raycast(blockCheck, Vector3.up, 1, objects) && Physics.Raycast(blockCheck + Vector3.up, -Vector3.up, 1, objects))
                {
                    canClimb = true;
                    blockClimb = blockCheck;
                    break;
                }
            }
            if (canClimb)
            {
                Vector3 finalPos = newDirection * (catHeight + (int)transform.position.y - 1 - (int)blockClimb.y) - Vector3.up * ((int)transform.position.y - 1 - (int)blockClimb.y);
                Vector3 catHeightClimb = new Vector3(transform.position.x, finalPos.y + transform.position.y, transform.position.z);
                float dist = Vector3.Distance(catHeightClimb, transform.position + finalPos);
                if (!Physics.Raycast(catHeightClimb, newDirection, dist, objects)) //check if climb space is not occupied horizontally
                {
                    if (Physics.Raycast(transform.position + finalPos, -Vector3.up, 1, objects)) // check if theres ground at final space 
                    {
                        if (!Physics.Raycast(transform.position + finalPos, Vector3.up, catHeight - 1, objects)) // height at final pos is enough
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
                    catModelGameObject.GetChild(catHeight - 1).GetComponent<Animator>()?.SetTrigger("ShakeHead");
                    blockManager.SetAllBlockMovableStateTo(false);
                }
            }
        }
        moveCatVector = newMoveDirection;
        CatMoveAction?.Invoke(newMoveDirection);
    }

    public Vector3 ConvertCatDirectionToVector3(DataClass.Directions dirIndex)
    {
        int p = ((int)dirIndex + (int)cameraController.GetCameraView()) % 4;
        int dir = p < 2 ? 1 : -1;
        Vector3 newDirection = p % 2 == 0 ? dir * Vector3.forward : dir * Vector3.right;
        return newDirection;
    }

    public void ReadyForMovement() { // called by BlockManager when all blocks have moved to fixed position
        CollectAllBerriesAlong(moveCatVector);

        StopCoroutine(CatFlatMove());
        StopCoroutine(CatDownMove());

        bool isClimbing = false;
        //TODO: make going up; make local position - movecatvector instead of doing it individually
        if (Vector3.Magnitude(moveCatVector) == 1) // do animation when only goes forward
        {
            catModelGameObject.localPosition -= Vector3.forward;
            StartCoroutine(CatFlatMove());
        }
        else if(Vector3.Magnitude(moveCatVector) > 1)
        {
            if (moveCatVector.y < 0)//going Down
            {
                catModelGameObject.localPosition -= Vector3.forward - Vector3.up;
                StartCoroutine(CatDownMove());
            }
            else if (moveCatVector.y > 0)//going Up
            {
                transform.position -= moveCatVector;
                // begin climb animation
                isCatClimbing = true; 
                catModelGameObject.GetChild(catHeight - 1).GetComponent<Animator>().SetTrigger("Climb" + moveCatVector.y);
                StartCoroutine(ResetLocalPositionAfterClimbAnimation());
            }
        }
        if (!isClimbing)
        {
            transform.position += moveCatVector;
            moveCatVector = Vector3.zero;
        }
    }
    IEnumerator ResetLocalPositionAfterClimbAnimation() {
        yield return new WaitForSeconds(climbAnimationTime);
        isCatClimbing = false;
        transform.position += moveCatVector;
        moveCatVector = Vector3.zero;
    }
    IEnumerator CatRotate(Vector3 direction, bool hasTransition = true)
    {
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        transform.Rotate(Vector3.up, angle);
        if (hasTransition)
        {
            catModelGameObject.Rotate(Vector3.up, -angle);
            float newAngle = angle * rotateSpeed;
            float progress = 0;
            while (progress <= 1)
            {
                catModelGameObject.transform.Rotate(Vector3.up, newAngle);
                progress += rotateSpeed;
                yield return new WaitForEndOfFrame();
            }
            angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
            catModelGameObject.transform.Rotate(Vector3.up, angle);
        }
    }

    IEnumerator CatFlatMove()
    {
        float progress = 0;
        while (progress <= 1)
        {
            catModelGameObject.localPosition = new Vector3(0, JumpFlatCurve(progress), Vector3.Lerp(-Vector3.forward, Vector3.zero, progress).z);
            progress += animSpeed;
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator CatDownMove()
    {
        float progress = 0;
        while (progress <= 1)
        {
            catModelGameObject.localPosition = new Vector3(0, JumpDownCurve(progress), Vector3.Lerp(-Vector3.forward, Vector3.zero, progress).z);
            progress += animSpeed;
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetAreBlocksMoving(bool newState) {
        areBlocksMoving = newState;
    }
    bool PushConditionMet() {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position + Vector3.up * catHeight + transform.forward, -Vector3.up, catHeight, blocksLayer);
        if (hits.Length != catHeight) // the blocks don't span the height of the player
            return false;
        if (Physics.Raycast(transform.position + Vector3.up * (catHeight - 1) + transform.forward, Vector3.up, 1, blocksLayer))  // the block structure is taller than player
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
    void CollectAllBerriesAlong(Vector3 catMovementDirection) { // takes in the delta vector of the cat movement and collects all berries along that movement
        for (int pos = 0; pos < Mathf.Abs(catMovementDirection.x); pos++) { // collect all powerups along the x offset
            int posWithDirection = pos * (catMovementDirection.x > 0 ? 1 : -1);
            RaycastHit hitPowerup;
            if (Physics.Raycast(transform.position + Vector3.up * (catMovementDirection.y + 1) + Vector3.right * (posWithDirection), -Vector3.up, out hitPowerup, 1, powerupLayer)) {
                itemCountController[hitPowerup.transform.gameObject.GetComponent<PowerupController>().GetPowerupType()].AddCountOfItem();
                hitPowerup.transform.gameObject.GetComponent<PowerupController>().SelfDestruct();
            }
            if(!gameManager.IsFished())
            {
                if(Physics.Raycast(transform.position + Vector3.up * (catMovementDirection.y + 1) + Vector3.right * (posWithDirection), -Vector3.up, out hitPowerup, 1, fishLayer))
                {
                    hitPowerup.transform.GetComponent<Fish>().CollectFish();
                }
            }
        }
        for (int pos = 0; pos < Mathf.Abs(catMovementDirection.z); pos++) { // collect all powerups along the z offset
            int posWithDirection = pos * (catMovementDirection.z > 0 ? 1 : -1);
            RaycastHit hitPowerup;
            if (Physics.Raycast(transform.position + Vector3.up * (catMovementDirection.y + 1) + Vector3.forward * posWithDirection, -Vector3.up, out hitPowerup, 1, powerupLayer)) {
                itemCountController[hitPowerup.transform.gameObject.GetComponent<PowerupController>().GetPowerupType()].AddCountOfItem();
                hitPowerup.transform.gameObject.GetComponent<PowerupController>().SelfDestruct();
            }
            if (!gameManager.IsFished())
            {
                if (Physics.Raycast(transform.position + Vector3.up * (catMovementDirection.y + 1) + Vector3.forward * posWithDirection, -Vector3.up, out hitPowerup, 1, fishLayer))
                {
                    hitPowerup.transform.GetComponent<Fish>().CollectFish();
                }
            }
        }
        for (int pos = 0; pos < catHeight; pos++) { // collect all powerups along the vertical axis the cat will stand at
            RaycastHit hitPowerup;
            if (Physics.Raycast(transform.position + catMovementDirection + Vector3.up * (pos - 1), Vector3.up, out hitPowerup, 1, powerupLayer)) {
                itemCountController[hitPowerup.transform.gameObject.GetComponent<PowerupController>().GetPowerupType()].AddCountOfItem();
                hitPowerup.transform.gameObject.GetComponent<PowerupController>().SelfDestruct();
            }
            if (!gameManager.IsFished())
            {
                if (Physics.Raycast(transform.position + catMovementDirection + Vector3.up * (pos - 1), Vector3.up, out hitPowerup, 1, fishLayer))
                {
                    hitPowerup.transform.GetComponent<Fish>().CollectFish();
                }
            }
        }
    }
    
    public void AttemptGrowth() {
        if (Physics.Raycast(transform.position + ((catHeight - 1) * Vector3.up), Vector3.up, 1, objects)) {
            Debug.Log("Cannot grow at current position");
            return;
        }
        bool growthSuccess = itemCountController[DataClass.PowerUp.Grow].AttemptDeductCountOfItem();
        if (growthSuccess)
        {
            CatHeightChangeAction(catHeight + 1);
            SetCatHeight(catHeight + 1);
        }
    }
    public void AttemptShrink() {
        bool shrinkSuccess = itemCountController[DataClass.PowerUp.Shrink].AttemptDeductCountOfItem();
        if (shrinkSuccess)
        {
            CatHeightChangeAction(catHeight - 1);
            SetCatHeight(catHeight - 1);
        }
    }
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        cameraController = FindObjectOfType<CameraController>();
        ItemCountController[] powerupTypes = FindObjectsOfType<ItemCountController>();
        catModelGameObject = transform.GetChild(0);

        itemCountController = new Dictionary<DataClass.PowerUp, ItemCountController>();
        for (int i = 0; i < powerupTypes.Length; i++) {
            ItemCountController currentUICount = powerupTypes[i];
            itemCountController[currentUICount.GetPowerupType()] = currentUICount;
        }

        playRecord = FindObjectOfType<PlayRecord>();
        playRecord.UndoEvent += ResetCatToState;

        SetCatHeight(catHeight); 
    }

    void ResetCatToState(PlayRecord.MoveState moveState)
    {
        transform.position = moveState.catPosition;
        catFacingDirection = moveState.catDirection;
        StartCoroutine(CatRotate(ConvertCatDirectionToVector3(moveState.catDirection), false));
        SetCatHeight(moveState.catHeight);
    }
    public void StartSetObject()
    {
        //blockManager = GameObject.Find("Map").GetComponentInChildren<BlockManager>();
        StartCoroutine(SetObject());
    }

    public void SetCatHeight(int newHeight) // changes height and corresponding cat mesh
    {
        catHeight = newHeight;
        Transform catModels = transform.GetChild(0);
        int catHeights = catModels.childCount;
        for (int i = 0; i < catHeights; i++) {
            if (i + 1 == catHeight) {
                catModelGameObject.GetChild(i).gameObject.SetActive(true);
            }
            else catModelGameObject.GetChild(i).gameObject.SetActive(false);
        }
    }

    IEnumerator SetObject()
    {
        GameObject map = null;
        while (map == null)
        {
            map = GameObject.Find("Map");
            yield return new WaitForEndOfFrame();
        }
        blockManager = map.GetComponentInChildren<BlockManager>();
        if(map.GetComponentInChildren<StartingBlock>() == null)
        {
            //it doesn't exist
            Debug.Log("starting block not made");
            transform.position = Vector3.zero;
        }
        else
        {
            transform.position = map.GetComponentInChildren<StartingBlock>().GetPos();
        }
            
        CatLoaded?.Invoke(true);
    }

    private float JumpDownCurve(float t) // returns y-axis given t from 0 to 1
    {
        if (t < 0) return 0;
        else if (t > 1) return -1;
        else return -0.95f * t * (1.2013f * t - 0.7f) * (1.1f * t + 1f) + 1;
    }

    private float JumpFlatCurve(float t)
    {
        if (t < 0 || t > 1) return 0;
        else return -1 * (t - 1) * t;
    }

    // getters
    public Vector3 GetCatPosition()
    {
        return gameObject.transform.position + moveCatVector; // cat position goes up by moveCatVector only after climb animation. It's effectively part of the cat's position
    }
    public DataClass.Directions GetCatDirection()
    {
        return catFacingDirection;
    }
    public int GetCatHeight()
    {
        return catHeight;
    }
}
