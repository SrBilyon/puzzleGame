using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SelectionEdgeCheck : MonoBehaviour
{
    public enum Position { Top, Bottom, Left, Right, Corner }
    public Position pos;
    public GameObject adjGrid;
    public int posNumber;


    void Start()
    {
        GetAdjacentCube();
    }

    /// <summary>
    /// Report to GameMain that I'm the edge piece the mouse is hovering over
    /// </summary>
    void OnMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        GameObject horiBar = GameObject.Find("HorizontalBar");
        GameObject vertBar = GameObject.Find("VerticalBar");

        if (Physics.Raycast(ray))
        {
            GameMain.selectedEdge = this.gameObject;

            //Find the grid slot next to me
            GetAdjacentCube();
        }

        //Display the horizontal bar
        if (pos == Position.Left || pos == Position.Right)
        {
            horiBar.renderer.enabled = true;
            horiBar.transform.position = new Vector3(horiBar.transform.position.x, transform.position.y, -3);
            vertBar.renderer.enabled = false;
        }
        else
        //Display the vertical bar
        //Display the horizontal bar
        if (pos == Position.Top || pos == Position.Bottom)
        {
            vertBar.renderer.enabled = true;
            vertBar.transform.position = new Vector3(transform.position.x, vertBar.transform.position.y, -3);
            horiBar.renderer.enabled = false;
        }
    }

    /// <summary>
    /// Find the grid slot that is adjacent to me
    /// </summary>
    public void GetAdjacentCube()
    {
        string[] words = gameObject.name.Split(' ');
        foreach (string n in words)
        {
            int num = 0;
            if (int.TryParse(n, out num))
            {
                posNumber = num;
            }
        }

        switch (pos)
        {
            case Position.Bottom:
                adjGrid = GameObject.Find("Grid" + posNumber + ",0");
                GameMain.currentEggDropCoords = new Vector2(adjGrid.transform.position.x, adjGrid.transform.position.y);
                break;
            case Position.Left:
                adjGrid = GameObject.Find("Grid0" + "," + posNumber);
                GameMain.currentEggDropCoords = new Vector2(adjGrid.transform.position.x, adjGrid.transform.position.y);
                break;
            case Position.Right:
                adjGrid = GameObject.Find("Grid4" + "," + posNumber);
                GameMain.currentEggDropCoords = new Vector2(adjGrid.transform.position.x, adjGrid.transform.position.y);
                break;
            case Position.Top:
                adjGrid = GameObject.Find("Grid" + posNumber + ",7");
                GameMain.currentEggDropCoords = new Vector2(adjGrid.transform.position.x, adjGrid.transform.position.y);
                break;
            case Position.Corner:
                break;
        }
    }
}
