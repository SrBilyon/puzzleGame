using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMain : MonoBehaviour
{
    /// <summary>
    /// Tile prefab. This are instantiated to create the grid
    /// </summary>
    public GameObject tile;

    /// <summary>
    /// All tiles get parented to this
    /// </summary>
    public GameObject gridContainer;

    /// <summary>
    /// The grid that the mouse is hovered to
    /// </summary>
    public static GameObject selectedEdge;

    /// <summary>
    /// Container for the current or next egg
    /// </summary>
    public static GameObject currentEggHolder, nextEggHolder;

    /// <summary>
    /// The current and next egg to be used
    /// </summary>
    public static SimpleEgg currentEgg, nextEgg;

    /// <summary>
    /// Variables for creating the grid [x,y]
    /// </summary>
    public static int rowCount = 8, colCount = 5;

    /// <summary>
    ///The initial eggs to be spawned at game start
    /// </summary>
    public int initialEggs = 8;

    /// <summary>
    /// The gap between all the tiles
    /// </summary>
    public float space = 1.5f;

    /// <summary>
    /// The container for all the slot locations on the grid
    /// </summary>
    public static Vector2[] coords;

    public static Vector2 currentEggDropCoords;

    void Awake()
    {
        selectedEdge = GameObject.Find("TopEdge 2");
        print(selectedEdge);
    }

    // Use this for initialization
    void Start()
    {
        //Create the Coordinate Vector and size it by rows * columns
        coords = new Vector2[rowCount * colCount];

        currentEggHolder = GameObject.Find("Current");
        nextEggHolder = GameObject.Find("Next");

        //Create the actual grid
        CreateGrid();

        //Create the first Egg
        currentEgg = Instantiate(EggUtility.CreateNewEgg()) as SimpleEgg;
        nextEgg = Instantiate(EggUtility.CreateNewEgg()) as SimpleEgg;

        currentEgg.transform.position = new Vector3(currentEggHolder.transform.position.x, currentEggHolder.transform.position.y, -3);
        nextEgg.transform.position = new Vector3(nextEggHolder.transform.position.x, nextEggHolder.transform.position.y, -3);
    }

    //void Update()
    //{
    //    CheckForHorizontalNeighbors();
    //}

    //void CheckForHorizontalNeighbors()
    //{
    //    List<GameObject> horEggs = EggUtility.GetNeighborHorizontal(0);
    //    print(horEggs.Capacity);

    //    int count = 0;
    //    SimpleEgg.EggType type;

    //    for (int i = 0; i < horEggs.Capacity; i++)
    //    {
    //        type = horEggs[i].GetComponent<SimpleEgg>().eggType;

    //        if (horEggs[i].GetComponent<SimpleEgg>().eggType == type)
    //        {
    //            count++;

    //            if (count >= 3)
    //            {
    //                print("Three Similar eggs found!");
    //                break;
    //            }
    //        }
    //        else
    //            break;

    //    }
    //}

    /// <summary>
    /// Create the initial eggs
    /// </summary>
    void CreateInitialEggs()
    {
        for (int i = 0; i < initialEggs; i++)
            Instantiate(EggUtility.CreateNewEgg());
    }

    /// <summary>
    /// Create the Grid
    /// </summary>
    void CreateGrid()
    {
        int i = 0; //Current column
        int j = 0; //Current row

        //The k is the array index
        int k = 0;

        //The i counter is for columns
        for (i = 0; i < colCount; i++)
        {
            //While the j counter is for rows
            for (j = 0; j < rowCount; j++)
            {
                #region Creating the cube
                GameObject newTile = Instantiate(tile) as GameObject;
                newTile.name = "Grid" + i + "," + j;
                newTile.transform.position = new Vector3(i * space, j * space, 0);
                newTile.transform.parent = gridContainer.transform;

                coords[k] = new Vector2(i, j);
                #endregion

                #region Coloring the Cubes
                if (j % 2 == 0)
                {
                    if (i % 2 == 0) //If even on the x and even on the y -> Red
                        newTile.renderer.material.mainTexture = (Texture)Resources.Load("Textures/Red");
                    else            //If even on the x and odd on the y -> Brown
                        newTile.renderer.material.mainTexture = (Texture)Resources.Load("Textures/Brown");
                }
                else
                {
                    if (i % 2 != 0) //If Odd on the x and Odd on the y -> Red
                        newTile.renderer.material.mainTexture = (Texture)Resources.Load("Textures/Red");
                    else
                        newTile.renderer.material.mainTexture = (Texture)Resources.Load("Textures/Brown");
                }
                #endregion
                k++;
            }
        }
    }

    /// <summary>
    /// Select a random Coordinate
    /// </summary>
    /// <returns></returns>
    public static Vector2 SelectRandomCoord()
    {
        //Grab a random coord
        int k = Random.Range(0, coords.Length);

        var selectedCoord = coords[k];

        //print(selectedCoord);
        return selectedCoord;
    }

    /// <summary>
    /// Get a grid coordinate
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static Vector2 GetGridCoord(int index)
    {
        return new Vector2(coords[index].x, coords[index].y);
    }

    /// <summary>
    /// Get the slot number on a grid
    /// </summary>
    /// <returns></returns>
    public static int GetGridSlotIndex(Vector2 gridCoords)
    {
        //The int that will hold the index
        int slot = 0;

        int i = 0; //Current column
        int j = 0; //Current row
        int k = 0; //Current index

        //The i counter is for columns
        for (i = 0; i < colCount; i++)
        {
            //While the j counter is for rows
            for (j = 0; j < rowCount; j++)
            {
                if (i == gridCoords.x && j == gridCoords.y)
                {
                    slot = k;
                    return slot;
                }
                k++;
            }
        }
        print("Uh... I shouldn't be called");
        return slot;
    }
}

