using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// Egg Utility contains operations for getting data on other eggs.
/// </summary>
public class EggUtility : MonoBehaviour 
{
    /// <summary>
    /// Gets all the eggs on the grid and returns
    /// </summary>
    /// <returns>GameObject[] eggs</returns>
    public static List<GameObject> GetEggs()
    {
        List<GameObject> eggs;
        eggs = new List < GameObject > (GameObject.FindGameObjectsWithTag("Egg"));
    
        return eggs;
    }

    /// <summary>
    /// Get the amount of eggs that are on the grid
    /// </summary>
    /// <returns>The amount of eggs in the game</returns>
    public static int GetEggCount()
    {
        int eggCount;
        var eggs = GameObject.FindGameObjectsWithTag("Egg");

        eggCount = eggs.Length;
        return eggCount;
    }

    /// <summary>
    /// Gets the neigbors surrounding a particular egg
    /// </summary>
    /// <param name="seeker"></param>
    /// <returns>Either Neighboring egg or nothing</returns>
    public static SimpleEgg GetNeighborBelow(Vector2 seeker)
    {
        //Generate a list of the existing eggs
        List<GameObject> eggs = new List<GameObject>(GetEggs());

        for (int i = 0; i < eggs.Capacity; i++)
        {
            //Returns an egg directly below in the same x position on the grid
            if ((eggs[i].GetComponent<SimpleEgg>().gridCoord.y == seeker.y - 1) && (eggs[i].GetComponent<SimpleEgg>().gridCoord.x == seeker.x))
            {
                return eggs[i].GetComponent<SimpleEgg>();
            }
        }

        //No neighboring eggs
        return null;
    }

    ///// <summary>
    ///// Get the horizontal neigher of the seeking egg
    ///// </summary>
    ///// <param name="seeker"></param>
    ///// <returns></returns>
    //public static List<GameObject> GetNeighborHorizontal(int col)
    //{
    //    List<GameObject> eggs = new List<GameObject>(GetEggs());
    //    List<GameObject> newEggs = new List<GameObject>();

    //    //for (int j = 0; j < GameMain.colCount; j++)
    //    //{
    //        for (int i = 0; i < eggs.Capacity; i++)
    //        {
    //            //This egg is a neighbor to the left or right
    //            if (eggs[i].GetComponent<SimpleEgg>().gridCoord.y == col)
    //                newEggs.Add(eggs[i]);
    //        }
    //    //}
    //    return newEggs;
    //}

    /// <summary>
    /// Create a new egg instance
    /// </summary>
    /// <returns></returns>
    public static SimpleEgg CreateNewEgg()
    {
        //Picking a random type of egg
        var getRandEgg = GetRandomEnum<SimpleEgg.EggType>();

        #region Getting a new egg
        string eggName = "";

        switch(getRandEgg)
        {
            case SimpleEgg.EggType.Black:
            eggName = "Black";
            break;
            case SimpleEgg.EggType.Blue:
            eggName = "Blue";
            break;
            case SimpleEgg.EggType.Green:
            eggName = "Green";
            break;
            case SimpleEgg.EggType.Pink:
            eggName = "Pink";
            break;
            case SimpleEgg.EggType.Red:
            eggName = "Red";
            break;
            case SimpleEgg.EggType.White:
            eggName = "White";
            break;
            case SimpleEgg.EggType.Yellow:
            eggName = "Yellow";
            break;
        }
        //Instantiating that egg from the prefab folder
        GameObject newEgg = (GameObject)Resources.Load("Prefabs/Eggs/"+ eggName);
        SimpleEgg eggData = newEgg.GetComponent<SimpleEgg>();
        #endregion

        //Setting the egg type
        eggData.eggType = getRandEgg;

        //Make the new egg active
        eggData.eggState = SimpleEgg.EggState.Active;
        
        //Give the egg a random location
        eggData.gridCoord = new Vector2(-1, -1);

        //Set the tag as waiting (THIS IS IMPORTANT
        eggData.tag = "WaitingEgg";
        
        return eggData;
    }
    /// <summary>
    /// Select a Random Enum
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    static T GetRandomEnum<T>()
    {
        //Create an array of type <t> (t = Enum)
        System.Array A = System.Enum.GetValues(typeof(T));
        //Select a random enum from a range of 0 and Array(a) length
        T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
        //Return index
        return V;
    }

    /// <summary>
    /// Checks to see if there is an egg in a particular slot
    /// </summary>
    /// <param name="egg"></param>
    /// <returns>True or false</returns>
    public static bool IsEggInSlot(Vector2 egg)
    {
        List<GameObject> eggs = EggUtility.GetEggs();

        for (int i = 0; i < eggs.Capacity; i++)
        {
            //Returns an egg directly below in the same x position on the grid
            if (eggs[i].GetComponent<SimpleEgg>().gridCoord == egg)
            {
                return true;
            }
        }
        return false;
    }

}
