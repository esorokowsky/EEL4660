using UnityEngine;
using System.Collections;

public class GamePiece 
{
    public GameObject Object;
    public int MarkerNum;

    public Vector2 CurLoc;
    public Vector2 PrevLoc;

    public int Player;

    public bool CanReverse = false;

    public GamePiece()
    {
    }

    public GamePiece(GameObject gameObject, Vector2 startLoc, Material material, int id, int player)
	{
        Object = gameObject;
		CurLoc = startLoc;
        MarkerNum = id;
        Player = player;

		if(material != null)
            Object.GetComponent<Renderer>().material = material;

		MovePiece(startLoc);
	}

	public void MovePiece(Vector2 newLoc)
	{
		PrevLoc = CurLoc;
		CurLoc = newLoc;

        Object.transform.position = Utilities.IndexToWorld(newLoc);

        //For checkers you can move backwards after getting to the other side
        if(Player == 1)
        {
            if (CurLoc.x == 7)
                CanReverse = true;
        }
        else if(Player == 2)
        {
            if (CurLoc.x == 0)
                CanReverse = true;
        }

		//Debug.Log(Model.name + " " + newLoc.ToString() + " " + Model.transform.position.ToString());
	}
}
