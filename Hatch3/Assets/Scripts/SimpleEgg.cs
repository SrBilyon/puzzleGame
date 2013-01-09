using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SimpleEgg : MonoBehaviour
{
    #region Members
    /// <summary>
    /// The Type of Egg.
    /// </summary>
    public enum EggType { Red, Blue, Green, Yellow, Pink, Black, White }
    public EggType eggType;

    /// <summary>
    /// The Current State of the egg. 
    /// Waiting - The Egg is in the waiting phase
    /// Active - The Egg is in the current box
    /// Selected - The Egg has been selected
    /// Resting - The Egg is on the map idle
    /// Falling - The Egg is falling down
    /// Bumped - The Egg is being pushed
    /// </summary>
    /// 
    public enum EggState { Waiting, Active, Selected, Resting, Falling, Sliding }
    public EggState eggState = EggState.Waiting;

    public enum BumpDirection { Up, Down, Left, Right }
    public BumpDirection bumpDirection;

    public enum MoveDirection { Up, Down, Left, Right }
    public MoveDirection moveDirection = MoveDirection.Down;

    /// <summary>
    /// This indicates that the egg is making it's first movement. This is for making sure that I dont bump
    /// </summary>
    bool cantBump;

    public bool freshEgg = true, dead = false, atEdge = false, trappedLeft, trappedRight;

    /// <summary>
    /// The container for all the slot locations on the grid
    /// </summary>
    public Vector2 gridCoord;

    /// <summary>
    /// Store the surrounding neighbor
    /// </summary>
    public SimpleEgg nLeft, nRight, nUp, ndown;

    /// <summary>
    /// Gets the index of the current slot you are on
    /// </summary>
    public int gridIndex;

    /// <summary>
    /// Stores the face object in the child
    /// </summary>
    public GameObject face;
    #endregion

    void Start()
    {
        //PositionToGrid(gridCoord);
        eggState = EggState.Waiting;

        if (GameMain.currentEgg == this)
            eggState = EggState.Active;

    }

    /// <summary>
    /// Update the Egg
    /// </summary>
    void Update()
    {
        CheckAllNeighbors();

        switch (eggState)
        {
            case EggState.Falling:
                //Gravity Function
                //StartCoroutine("Drop", gridCoord);
                Drop(gridCoord);
                break;

            case EggState.Resting:
                CheckAllNeighbors();
                DestroyMatching(gridCoord);
                break;

            case EggState.Selected:
                if (GameMain.selectedEdge != null)
                {
                    transform.position = GameMain.selectedEdge.transform.position;
                }
                break;

            case EggState.Waiting:
                break;

            case EggState.Active:
                break;

            case EggState.Sliding:
                CheckAllNeighbors();
                Slide();
                break;
        }

        if (eggState != EggState.Waiting && eggState != EggState.Active && eggState != EggState.Selected && eggState != EggState.Sliding)
        {
            if (!CheckIfGrounded())
            {
                eggState = EggState.Falling;
            }
            else
            {
                eggState = EggState.Resting;
            }

        }
    }

    /// <summary>
    /// An event called when the mouse or touch hits this object
    /// </summary>
    public void OnMouseDown()
    {
        //Make sure the egg I'm clicking 
        if (GameMain.currentEgg == this)
            eggState = EggState.Active;

        //Be able to select the egg only if it's the selected egg
        if (eggState == EggState.Active)
        {
            eggState = EggState.Selected;
        }
    }

    /// <summary>
    /// An event called when the user unselects the mouse buttom
    /// </summary>
    public void OnMouseUp()
    {
        //If this egg is the selected one...
        if (eggState == EggState.Selected)
        {
            //1. Get the slot that the egg should be dropping into
            var gridInt = GameMain.GetGridSlotIndex(GameMain.currentEggDropCoords);
            gridCoord = GameMain.GetGridCoord(gridInt);

            //2. Set the grid coord to the position we are wanting to get in
            PositionToGrid(new Vector2(gridCoord.x, gridCoord.y));

            //3. Check to see if there is an egg in the slot we are trying to place
            //this egg in
            if (EggUtility.IsEggInSlot(gridCoord))
            {
                if (gridCoord.x == 0)
                {
                    if (nRight.trappedLeft)
                    {
                        ReturnToCurrentSlot();
                    } 
                    
                }
                else if (gridCoord.x == GameMain.colCount - 1)
                {
                    if (nLeft.trappedRight)
                    {
                        ReturnToCurrentSlot();
                    } 
                }
                else
                ReturnToCurrentSlot();

                return;
            }

            //4. Tag the egg. This egg can now be searched using GetEggs()
            tag = "Egg";

            //5. Start the egg off in a falling state
            if (!CheckIfGrounded())
            {
                eggState = EggState.Falling;
            }
            else
                //6. Check to see where the egg is going
                if (gridCoord.x == 0 && CheckIfGrounded()) //Left Side
                {
                    trappedLeft = true; trappedRight = false;
                    //The direction the egg is coming in from
                    moveDirection = MoveDirection.Right;
                    eggState = EggState.Sliding;
                }
                else //If on the right side
                    if (gridCoord.x == GameMain.colCount - 1 && CheckIfGrounded())
                    {
                        trappedRight = true; trappedLeft = false;
                        moveDirection = MoveDirection.Left;
                        eggState = EggState.Sliding;
                    }
                    else if (gridCoord.y != 0)
                    {
                        eggState = EggState.Falling;
                    }

            //Replace the current egg with the one in the next egg pool
            GameMain.currentEgg = GameMain.nextEgg;

            //Create a new next egg
            GameMain.nextEgg = Instantiate(EggUtility.CreateNewEgg()) as SimpleEgg;

            //Place all the eggs in the right position
            GameMain.currentEgg.transform.position = new Vector3(GameMain.currentEggHolder.transform.position.x, GameMain.currentEggHolder.transform.position.y, -3);
            GameMain.nextEgg.transform.position = new Vector3(GameMain.nextEggHolder.transform.position.x, GameMain.nextEggHolder.transform.position.y, -3);
        }
    }

    /// <summary>
    /// Move this egg to a new location
    /// </summary>
    public void PositionToGrid(Vector2 grid)
    {
        gridCoord = grid;
        gridIndex = GameMain.GetGridSlotIndex(gridCoord);
        transform.position = new Vector3(gridCoord.x, gridCoord.y, -5);
    }

    private bool CheckIfGrounded()
    {
        if (gridCoord.y == 0 || IsSomethingUnderneath(gridCoord))
            return true;
        else
            return false;
    }

    private bool CheckIfOnLastRow()
    {
        //If the egg is going to land on the last row
        if (gridCoord.y != GameMain.rowCount - 1 && gridCoord.y != 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Return the egg to the current slot if the egg isn't able to be dropped
    /// </summary>
    public void ReturnToCurrentSlot()
    {
        Debug.LogWarning("There is an egg in the slot. Try again");
        eggState = EggState.Waiting;
        gridCoord = new Vector2(-1, -1);
        GameMain.currentEgg.transform.position = new Vector3(GameMain.currentEggHolder.transform.position.x, GameMain.currentEggHolder.transform.position.y, -3);
    }

    #region Check Surrounding Eggs
    public void CheckAllNeighbors()
    {
        List<GameObject> eggs = EggUtility.GetEggs();

        for (int i = 0; i < eggs.Capacity; i++)
        {
            //Returns an egg directly below in the same x position on the grid
            if ((eggs[i].GetComponent<SimpleEgg>().gridCoord.y == gridCoord.y - 1) && (eggs[i].GetComponent<SimpleEgg>().gridCoord.x == gridCoord.x))
            {
                ndown = eggs[i].GetComponent<SimpleEgg>();
            }
            else
                //Returns an egg above below in the same x position on the grid
                if ((eggs[i].GetComponent<SimpleEgg>().gridCoord.y == gridCoord.y + 1) && (eggs[i].GetComponent<SimpleEgg>().gridCoord.x == gridCoord.x))
                {
                    nUp = eggs[i].GetComponent<SimpleEgg>();
                }
                else
                    //Returns an egg directly below in the same x position on the grid
                    if ((eggs[i].GetComponent<SimpleEgg>().gridCoord.x == gridCoord.x - 1) && (eggs[i].GetComponent<SimpleEgg>().gridCoord.y == gridCoord.y))
                    {
                        nLeft = eggs[i].GetComponent<SimpleEgg>();
                    }
                    else
                        //Returns an egg directly below in the same x position on the grid
                        if ((eggs[i].GetComponent<SimpleEgg>().gridCoord.x == gridCoord.x + 1) && (eggs[i].GetComponent<SimpleEgg>().gridCoord.y == gridCoord.y))
                        {
                            nRight = eggs[i].GetComponent<SimpleEgg>();
                        }
        }
    }

    /// <summary>
    /// Check to see if there is an egg underneath me
    /// </summary>
    /// <param name="grid"></param>
    public bool IsSomethingUnderneath(Vector2 grid)
    {
        List<GameObject> eggs = EggUtility.GetEggs();

        for (int i = 0; i < eggs.Capacity; i++)
        {
            //Returns an egg directly below in the same x position on the grid
            if ((eggs[i].GetComponent<SimpleEgg>().gridCoord.y == gridCoord.y - 1) && (eggs[i].GetComponent<SimpleEgg>().gridCoord.x == gridCoord.x))
            {
                ndown = eggs[i].GetComponent<SimpleEgg>();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check to see if there is an egg underneath me
    /// </summary>
    /// <param name="grid"></param>
    public bool IsSomethingAbove(Vector2 grid)
    {
        List<GameObject> eggs = EggUtility.GetEggs();

        for (int i = 0; i < eggs.Capacity; i++)
        {
            //Returns an egg directly below in the same x position on the grid
            if ((eggs[i].GetComponent<SimpleEgg>().gridCoord.y == gridCoord.y + 1) && (eggs[i].GetComponent<SimpleEgg>().gridCoord.x == gridCoord.x))
            {
                nUp = eggs[i].GetComponent<SimpleEgg>();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check to see if there is an egg underneath me
    /// </summary>
    /// <param name="grid"></param>
    public bool IsSomethingIsNextToMe(Vector2 grid)
    {
        List<GameObject> eggs = EggUtility.GetEggs();

        for (int i = 0; i < eggs.Capacity; i++)
        {
            //Returns an egg directly below in the same x position on the grid
            if ((eggs[i].GetComponent<SimpleEgg>().gridCoord.x == gridCoord.x + 1 || eggs[i].GetComponent<SimpleEgg>().gridCoord.x == gridCoord.x - 1) && (eggs[i].GetComponent<SimpleEgg>().gridCoord.y == gridCoord.y))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check to see if there is an egg underneath me
    /// </summary>
    /// <param name="grid"></param>
    public bool IsSomethingLeftOfMe(Vector2 grid)
    {
        List<GameObject> eggs = EggUtility.GetEggs();

        for (int i = 0; i < eggs.Capacity; i++)
        {
            //Returns an egg directly below in the same x position on the grid
            if ((eggs[i].GetComponent<SimpleEgg>().gridCoord.x == gridCoord.x - 1) && (eggs[i].GetComponent<SimpleEgg>().gridCoord.y == gridCoord.y))
            {
                nLeft = eggs[i].GetComponent<SimpleEgg>();
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Check to see if there is an egg underneath me
    /// </summary>
    /// <param name="grid"></param>
    public bool IsSomethingRightOfMe(Vector2 grid)
    {
        List<GameObject> eggs = EggUtility.GetEggs();

        for (int i = 0; i < eggs.Capacity; i++)
        {
            //Returns an egg directly below in the same x position on the grid
            if ((eggs[i].GetComponent<SimpleEgg>().gridCoord.x == gridCoord.x + 1) && (eggs[i].GetComponent<SimpleEgg>().gridCoord.y == gridCoord.y))
            {
                nRight = eggs[i].GetComponent<SimpleEgg>();
                return true;
            }
        }
        return false;
    }
    #endregion

    /// <summary>
    /// Drop the egg down like gravity
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    /// 
    void Drop(Vector2 grid)
    {
        //Not in the last row
        if (!CheckIfOnLastRow())
        {
            // Not grounded
            if (!CheckIfGrounded())
            {
                //If I'm not on the ground and there is nothing under me
                PositionToGrid(new Vector2(gridCoord.x, gridCoord.y - 1));
            }
            else
            {
                //Grounded
                if (freshEgg)
                    freshEgg = false;

                eggState = EggState.Resting;
            }
        }
        else if (CheckIfGrounded())
        {
            eggState = EggState.Resting;

            if (!CheckIfGrounded())
            {
                PositionToGrid(new Vector2(gridCoord.x, gridCoord.y));
            }
        }

        //If I'm on the very top and nothing is under me
        else if (CheckIfOnLastRow())
        {
            if (CheckIfGrounded())
            {
                //Grounded
                if (freshEgg)
                    freshEgg = false;

                PositionToGrid(new Vector2(gridCoord.x, gridCoord.y));
                bumpDirection = BumpDirection.Down;
                eggState = EggState.Resting;
            }
            PositionToGrid(new Vector2(gridCoord.x, gridCoord.y - 1));
        }
    }

    /// <summary>
    /// Push the eggs over if not falling
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="grid"></param>
    public void Toss(BumpDirection direction, Vector2 grid)
    {
        switch (nLeft.bumpDirection)
        {
            case BumpDirection.Left:
                if (!IsSomethingLeftOfMe(gridCoord))
                    nLeft.PositionToGrid(new Vector2(grid.x - 1, grid.y));
                break;
        }
        switch (nRight.bumpDirection)
        {
            case BumpDirection.Right:
                if (!IsSomethingRightOfMe(gridCoord))
                    nRight.PositionToGrid(new Vector2(grid.x + 1, grid.y));
                break;
        }
    }

    public void Slide()
    {
        //if (freshEgg)
        //    return;
        if (!CheckIfGrounded())
        {
            eggState = EggState.Falling;
            return;
        }

        switch (moveDirection)
        {
            case MoveDirection.Left:
                if (!IsSomethingLeftOfMe(gridCoord) && gridCoord.x > 0)
                    PositionToGrid(new Vector2(gridCoord.x - 1, gridCoord.y));
                else if (IsSomethingLeftOfMe(gridCoord))
                {
                    if (!nLeft.trappedLeft)
                    {
                        nLeft.eggState = EggState.Sliding;
                        nLeft.moveDirection = MoveDirection.Left;

                        if (!nLeft.IsSomethingLeftOfMe(gridCoord) && nLeft.gridCoord.x > 0)
                            nLeft.PositionToGrid(new Vector2(nLeft.gridCoord.x - 1, nLeft.gridCoord.y));
                    }
                    else
                    {
                        trappedLeft = true; trappedRight = false;
                    }
                }
                break;
            case MoveDirection.Right:
                if (!IsSomethingRightOfMe(gridCoord) && gridCoord.x < GameMain.colCount - 1)
                    PositionToGrid(new Vector2(gridCoord.x + 1, gridCoord.y));
                else if (IsSomethingRightOfMe(gridCoord))
                {
                    if (!nRight.trappedRight)
                    {
                        nRight.eggState = EggState.Sliding;
                        nRight.moveDirection = MoveDirection.Right;

                        if (!nRight.IsSomethingRightOfMe(gridCoord) && nRight.gridCoord.x < GameMain.colCount - 1)
                            nRight.PositionToGrid(new Vector2(nRight.gridCoord.x + 1, nRight.gridCoord.y));
                    }
                    else
                    {
                        trappedLeft = false; trappedRight = true;
                    }
                }
                break;
        }
    }

    #region Matching
    /// <summary>
    /// The container of similar neighbors on the x axis
    /// </summary>
    public List<GameObject> listOfObjectsToRemoveX = new List<GameObject>();

    /// <summary>
    /// The container of similar neighbors on the y axis
    /// </summary>
    public List<GameObject> listOfObjectsToRemoveY = new List<GameObject>();

    /// <summary>
    /// The container of similar neighbors on the y axis
    /// </summary>
    public List<GameObject> listOfObjectsToBump = new List<GameObject>();

    /// <summary>
    /// Number of objects need to count as a match
    /// </summary>
    private int minimumNoOfObjects = 3;

    /// <summary>
    /// Check the neighbors of this egg and seek for a series of similar eggs
    /// </summary>
    /// <param name="start"></param>
    private void DestroyMatching(Vector2 start)
    {
        #region X Check
        listOfObjectsToRemoveX.Clear();
        listOfObjectsToRemoveY.Clear();

        //If the list doesn't contain this, add it
        if (!listOfObjectsToRemoveX.Contains(this.gameObject))
            listOfObjectsToRemoveX.Add(this.gameObject);

        //Check for horizontal neighbours
        CheckForSimilarNeighbors(true, false);

        //Go to each neighbor egg and check it's neighbor
        for (int i = 0; i < listOfObjectsToRemoveX.Count; i++)
        {
            CheckForSimilarNeighbors(true, false);
        }

        //If the amount of eggs in the similar neighbors list = 3
        if (listOfObjectsToRemoveX.Count >= minimumNoOfObjects)
        {
            //Destory the horizontal eggs
            for (int i = 0; i < listOfObjectsToRemoveX.Count; i++)
            {
                Destroy(listOfObjectsToRemoveX[i]);
            }
            //Destroy the vertical eggs
            for (int i = 0; i < listOfObjectsToRemoveY.Count; i++)
            {
                Destroy(listOfObjectsToRemoveY[i]);
            }
            //Clear both of the holder arrays
            listOfObjectsToRemoveX.Clear();
            listOfObjectsToRemoveY.Clear();

        }
        #endregion

        #region Y Check
        //If the list doesn't contain this, add it
        if (!listOfObjectsToRemoveY.Contains(this.gameObject))
            listOfObjectsToRemoveY.Add(this.gameObject);

        //Check for horizontal neighbours
        CheckForSimilarNeighbors(false, true);

        //Go to each neighbor egg and check it's neighbor
        for (int i = 0; i < listOfObjectsToRemoveY.Count; i++)
        {
            CheckForSimilarNeighbors(false, true);
        }

        //If the amount of eggs in the similar neighbors list = 3
        if (listOfObjectsToRemoveY.Count >= minimumNoOfObjects)
        {
            //Destory the vertical eggs
            for (int i = 0; i < listOfObjectsToRemoveY.Count; i++)
            {
                Destroy(listOfObjectsToRemoveY[i]);
            }
            //Destroy the horizontal eggs
            for (int i = 0; i < listOfObjectsToRemoveX.Count; i++)
            {
                Destroy(listOfObjectsToRemoveX[i]);
            }
            //Clear both of the holder arrays
            listOfObjectsToRemoveY.Clear();
            listOfObjectsToRemoveX.Clear();
        }
        #endregion
    }

    /// <summary>
    /// Check for similar neighbors and store them into a list
    /// </summary>
    /// <param name="checkHor"></param>
    /// <param name="checkVert"></param>
    private void CheckForSimilarNeighbors(bool checkHor, bool checkVert)
    {
        // Check all four neighbours (vertically and horizontally)
        // and make sure the previous start object is ignored if found...  

        if (checkHor)
        {
            if (nLeft != null)
            {
                if (nLeft.eggType == this.eggType && !listOfObjectsToRemoveX.Contains(nLeft.gameObject)) //nLeft != previous)
                {
                    listOfObjectsToRemoveX.Add(nLeft.gameObject);
                }
            }
            if (nRight != null)
            {
                if (nRight.eggType == this.eggType && !listOfObjectsToRemoveX.Contains(nRight.gameObject))//nRight != previous)
                {
                    listOfObjectsToRemoveX.Add(nRight.gameObject);
                }
            }
        }

        if (checkVert)
        {
            if (nUp != null)
            {
                if (nUp.eggType == this.eggType && !listOfObjectsToRemoveY.Contains(nUp.gameObject))//nUp != previous)
                {
                    listOfObjectsToRemoveY.Add(nUp.gameObject);
                }
            }
            if (ndown != null)
            {
                if (ndown.eggType == this.eggType && !listOfObjectsToRemoveY.Contains(ndown.gameObject))//ndown != previous)
                {
                    listOfObjectsToRemoveY.Add(ndown.gameObject);
                }
            }
        }

    }



    private void CheckForBumpNeighbors()
    {
        // Check all four neighbours (vertically and horizontally)
        // and make sure the previous start object is ignored if found...  

        if (nLeft != null)
        {
            if (nLeft.gridCoord.x != 0 && !listOfObjectsToRemoveX.Contains(nLeft.gameObject)) //nLeft != previous)
            {
                listOfObjectsToBump.Add(nLeft.gameObject);
            }
        }
        if (nRight != null)
        {
            if (nRight.gridCoord.x != GameMain.colCount - 1 && !listOfObjectsToRemoveX.Contains(nRight.gameObject))//nRight != previous)
            {
                listOfObjectsToBump.Add(nRight.gameObject);
            }
        }
    }
    #endregion
}
