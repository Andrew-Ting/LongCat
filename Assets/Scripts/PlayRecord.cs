using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayRecord : MonoBehaviour
{
    // script in charge of tracking player's every move and resulting state (to allow undoing)
   
    public class Block
    {
        public Vector3 position { get; }
        public bool isActive { get; }
        public Block(Vector3 position, bool isActive)
        {
            this.position = position; // current position of the Block
            this.isActive = isActive; // a block is active iff it hasn't been pushed off into the abyss
        }
    }
    public class MoveState
    {
        public Vector3 catPosition { get; }
        public DataClass.Directions catDirection { get; }
        public int catHeight { get; }
        public List<Block> blockMetadata { get; } // stores metadata about the ith gameobject in allBlocksInGame (which is a block), see the Block type above
        public List<int> powerupQuantity { get; } // {grow powerup count, shrink powerup count}
        public List<bool> isPowerupCollected { get; } // stores whether the ith gameobject in allPowerupsInGame (which is a powerup) has been collected or not
        public MoveState(Vector3 catPosition, DataClass.Directions catDirection, int catHeight, List<Block> blockMetadata, List<bool> isPowerupCollected, List<int> powerupQuantity) 
        {
            this.catPosition = catPosition;
            this.catDirection = catDirection;
            this.catHeight = catHeight;
            this.blockMetadata = blockMetadata;
            this.isPowerupCollected = isPowerupCollected;
            this.powerupQuantity = powerupQuantity;
        }
    }
    public event Action<MoveState> UndoEvent;
    Stack<MoveState> moves;
    CatMovement catMovement;
    BlockManager blockManager;
    private List<GameObject> allBlocksInGame;
    private List<GameObject> allPowerupsInGame;

    void Start()
    {
        moves = new Stack<MoveState>();
        allPowerupsInGame = new List<GameObject>();
        catMovement = FindObjectOfType<CatMovement>();
        catMovement.CatHeightChangeAction += RecordHeightChange;
        UndoEvent += ResetFieldPowerups;
    }

    // ran at BlockManager's start function. This is bad, ideally we want PlayRecord to take this info from BlockManager, not for BlockManager to supply it

    public void InitializeBlockManager(BlockManager blockManager, List<GameObject> blocks) {
        this.blockManager = blockManager;
        blockManager.moveCompleted += () => RecordMoveChange();
        allBlocksInGame = new List<GameObject>(blocks);
    }

     // ideally we should find a way for PlayRecord to get the powerups, and not PowerupController to send it to PlayRecord, but the order of start function execution prevents this :(
    public void AddPowerupToList(GameObject powerupObject)
    {
        allPowerupsInGame.Add(powerupObject);
    }

    private void RecordHeightChange(int newHeight) // records actions of the player using growth/shrink powerups
    {
        if (moves.Count > 0)
        {
            // sanity check
            if (catMovement.GetCatPosition() != moves.Peek().catPosition)
            {
                Debug.LogError("The cat position has changed after growing/shrinking. This shouldn't happen");
                Debug.LogError("EXPECTED: " + moves.Peek().catPosition + " BUT GOT " + catMovement.GetCatPosition());
            }
            if (catMovement.GetCatDirection() != moves.Peek().catDirection)
            {
                Debug.LogError("The cat's faced direction has changed after growing/shrinking. This shouldn't happen");
                Debug.LogError("EXPECTED: " + moves.Peek().catDirection + " BUT GOT " + catMovement.GetCatDirection());
            }
        }
        StartCoroutine(RecordChange(false, false, true, false, true, true));
    }

    private void RecordMoveChange() // records player movement and corresponding world state changes
    {
        StartCoroutine(RecordChange(true, true, false, true, true, true));
    }

    // for optimization purposes; set to true only the parameters that could have changed from a move
    private IEnumerator RecordChange (bool catPositionChanged, bool catDirectionChanged, bool catHeightChanged, bool blockMetadataChanged, bool isPowerupCollectedChanged, bool powerupQuantityChanged) 
    {
        yield return null; // wait a frame before recording changes to ensure new game state has been applied; no race conditions
        // state variables
        MoveState latestState;
        Vector3 catPosition;
        DataClass.Directions catDirection;
        int catHeight;
        List<Block> blockMetadata;
        List<bool> isPowerupCollected;
        List<int> powerupQuantity;

        if (moves.Count == 0) // no previously stored data to copy-optimize. Get everything
        {
            catPosition = catMovement.GetCatPosition();
            catDirection = catMovement.GetCatDirection();
            catHeight = catMovement.GetCatHeight();
            blockMetadata = ParseBlockMetadata(blockManager.GetAllBlocks());
            isPowerupCollected = ParseIsPowerupCollected();
            powerupQuantity = ParsePowerupQuantity();
        }
        else // copy data from the previous state wherever possible instead of recomputing it
        {
            latestState = moves.Peek();
            catPosition = !catPositionChanged ? latestState.catPosition : catMovement.GetCatPosition();
            catDirection = !catDirectionChanged ? latestState.catDirection : catMovement.GetCatDirection();
            catHeight = !catHeightChanged ? latestState.catHeight : catMovement.GetCatHeight();
            blockMetadata = !blockMetadataChanged ? latestState.blockMetadata : ParseBlockMetadata(blockManager.GetAllBlocks());
            isPowerupCollected = !isPowerupCollectedChanged ? latestState.isPowerupCollected : ParseIsPowerupCollected();
            powerupQuantity = !powerupQuantityChanged ? latestState.powerupQuantity : ParsePowerupQuantity();
        }
        moves.Push(new MoveState(catPosition, catDirection, catHeight, blockMetadata, isPowerupCollected, powerupQuantity));
    }

    private List<Block> ParseBlockMetadata(IList<GameObject> blockList)
    {
        List<Block> blockMetadata = new List<Block>();
        foreach(GameObject block in allBlocksInGame)
        {
            if (blockList.Contains(block))
            {
                blockMetadata.Add(new Block(block.transform.position, true));
            }
            else blockMetadata.Add(new Block(Vector3.zero, false));
        }
        return blockMetadata;
    }

    private List<bool> ParseIsPowerupCollected()
    {
        List<bool> isPowerupCollected = new List<bool>();
        foreach(GameObject powerup in allPowerupsInGame)
        {
          isPowerupCollected.Add(!powerup.activeSelf || !powerup.GetComponent<CapsuleCollider>().enabled); // the powerup is collected iff its collider is disabled, or the powerup itself is inactive
        }
        return isPowerupCollected;
    }

    private List<int> ParsePowerupQuantity()
    {
        List<int> powerupQuantity = new List<int>();
        powerupQuantity.Add(FindObjectOfType<GrowCountController>().GetCountOfItem());
        powerupQuantity.Add(FindObjectOfType<ShrinkCountCountroller>().GetCountOfItem());
        return powerupQuantity;
    }

    public void UndoMove() // function called by undo button on click
    {
        if (moves.Count > 1) { // disable undoing beyond first move
            moves.Pop();
            UndoEvent.Invoke(moves.Peek());
        }
        else
        {
            Debug.Log("Already at first move. Cannot undo further");
        }
    }
    public IList<GameObject> GetAllBlocksInGame()
    {
        return allBlocksInGame.AsReadOnly(); 
    }
    public void ResetFieldPowerups(MoveState moveState) // would ideally be placed in a powerupmanager class, but it isn't worth making one just for this
    {
        for (int i = 0; i < allPowerupsInGame.Count; i++)
        {
            if (!moveState.isPowerupCollected[i] && !allPowerupsInGame[i].activeSelf)
            {
                allPowerupsInGame[i].GetComponent<PowerupController>().ReviveObject();
            } 
        }
    }
}
