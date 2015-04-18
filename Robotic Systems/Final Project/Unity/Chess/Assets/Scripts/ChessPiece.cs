using UnityEngine;
using System.Collections;

public class ChessPiece
{
	public GameObject Model;
	public int MarkerNumber;
    public int Player;
	public Vector2 CurLocation;
	public Vector2 PrevLocation;

	public ChessPiece(GameObject model, Vector2 startLoc, Material material, int player)
	{
		Model = model;
		CurLocation = startLoc;
        Player = player;

		if(material != null)
			Model.GetComponent<Renderer>().material = material;

		MovePiece(startLoc);
	}

	public void MovePiece(Vector2 newLoc)
	{
		PrevLocation = CurLocation;
		CurLocation = newLoc;

		Model.transform.position = Utilities.IndexToWorld(newLoc);

		//Debug.Log(Model.name + " " + newLoc.ToString() + " " + Model.transform.position.ToString());
	}
}
