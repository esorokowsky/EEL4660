using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	public List<ChessPiece> Player1Pieces = new List<ChessPiece>();
	public List<ChessPiece> Player2Pieces = new List<ChessPiece>();

    private Dictionary<int, Utilities.Marker> _Markers = new Dictionary<int, Utilities.Marker>();

    //scallar offsets from marker 1
    private float _XOffset = 0.147f;
    private float _YOffset = 0.0529f;

	public Material Player1Mat;
	public Material Player2Mat;

	private GameObject _GameBoard;
	private ClientSocket _ClientSocket;

	// Use this for initialization
	void Start () 
	{
		_GameBoard = GameObject.Find("ChessBoard");
	}
	
    public void InitChess()
    {
        GameObject gameChooser = GameObject.Find("GameChooser");
        gameChooser.SetActive(false);

        GameObject Player1 = GameObject.Find("Player 1 Pieces");
        GameObject Player2 = GameObject.Find("Player 2 Pieces");

        //Init all pawns
        for (int i = 0; i < 8; i++)
        {
            Vector2 loc = new Vector2(0, i);

            //Player 1
            loc.x = 1;
            GameObject curObject = Player1.transform.Find("Hi_Pawn " + (i + 1)).gameObject;
            Player1Pieces.Add(new ChessPiece(curObject, loc, Player1Mat));

            //Player 2
            loc.x = 6;
            curObject = Player2.transform.Find("Hi_Pawn " + (i + 1)).gameObject;
            Player2Pieces.Add(new ChessPiece(curObject, loc, Player2Mat));
        }

        //King
        GameObject curPiece = Player1.transform.Find("Hi_King").gameObject;
        Player1Pieces.Add(new ChessPiece(curPiece, new Vector2(0, 4), Player1Mat));

        curPiece = Player2.transform.Find("Hi_King").gameObject;
        Player2Pieces.Add(new ChessPiece(curPiece, new Vector2(7, 4), Player2Mat));

        //Queen
        curPiece = Player1.transform.Find("Hi_Queen").gameObject;
        Player1Pieces.Add(new ChessPiece(curPiece, new Vector2(0, 3), Player1Mat));

        curPiece = Player2.transform.Find("Hi_Queen").gameObject;
        Player2Pieces.Add(new ChessPiece(curPiece, new Vector2(7, 3), Player2Mat));

        //Bishops
        curPiece = Player1.transform.Find("Hi_Bishop 1").gameObject;
        Player1Pieces.Add(new ChessPiece(curPiece, new Vector2(0, 2), Player1Mat));

        curPiece = Player2.transform.Find("Hi_Bishop 1").gameObject;
        Player2Pieces.Add(new ChessPiece(curPiece, new Vector2(7, 2), Player2Mat));

        curPiece = Player1.transform.Find("Hi_Bishop 2").gameObject;
        Player1Pieces.Add(new ChessPiece(curPiece, new Vector2(0, 5), Player1Mat));

        curPiece = Player2.transform.Find("Hi_Bishop 2").gameObject;
        Player2Pieces.Add(new ChessPiece(curPiece, new Vector2(7, 5), Player2Mat));

        //Knights
        curPiece = Player1.transform.Find("Hi_Knight 1").gameObject;
        Player1Pieces.Add(new ChessPiece(curPiece, new Vector2(0, 1), Player1Mat));

        curPiece = Player2.transform.Find("Hi_Knight 1").gameObject;
        Player2Pieces.Add(new ChessPiece(curPiece, new Vector2(7, 1), Player2Mat));

        curPiece = Player1.transform.Find("Hi_Knight 2").gameObject;
        Player1Pieces.Add(new ChessPiece(curPiece, new Vector2(0, 6), Player1Mat));

        curPiece = Player2.transform.Find("Hi_Knight 2").gameObject;
        Player2Pieces.Add(new ChessPiece(curPiece, new Vector2(7, 6), Player2Mat));

        //Rook
        curPiece = Player1.transform.Find("Hi_Rook 1").gameObject;
        Player1Pieces.Add(new ChessPiece(curPiece, new Vector2(0, 0), Player1Mat));

        curPiece = Player2.transform.Find("Hi_Rook 1").gameObject;
        Player2Pieces.Add(new ChessPiece(curPiece, new Vector2(7, 0), Player2Mat));

        curPiece = Player1.transform.Find("Hi_Rook 2").gameObject;
        Player1Pieces.Add(new ChessPiece(curPiece, new Vector2(0, 7), Player1Mat));

        curPiece = Player2.transform.Find("Hi_Rook 2").gameObject;
        Player2Pieces.Add(new ChessPiece(curPiece, new Vector2(7, 7), Player2Mat));

        _ClientSocket = new ClientSocket();
        _ClientSocket.Init();
    }

    public void InitCheckers()
    {
        GameObject gameChooser = GameObject.Find("GameChooser");
        gameChooser.SetActive(false);

        GameObject Player1 = GameObject.Find("Player 1 Pieces");
        GameObject Player2 = GameObject.Find("Player 2 Pieces");

        //Init all pawns
        for (int i = 0; i < 12; i++)
        {
            Vector2 loc1 = new Vector2(0, (i * 2));
            Vector2 loc2 = new Vector2(0, (i * 2));
            if(i >= 8)
            {
                loc1.y = (i - 8) * 2;
                loc2.y = (i - 8) * 2+1;
            }
            else if(i >= 4)
            {
                loc1.y = (i - 4) * 2+1;
                loc2.y = (i - 4) * 2;
            }
            else
            {
                loc1.y = (i * 2);
                loc2.y = (i * 2)+1;
            }

            //Player 1
            loc1.x = i/4;
            GameObject curObject = Player1.transform.Find("Checker Light " + (i + 1)).gameObject;
            Player1Pieces.Add(new ChessPiece(curObject, loc1, Player1Mat));

            //Player 2
            loc2.x = 7-i/4;
            curObject = Player2.transform.Find("Checker Dark " + (i + 1)).gameObject;
            Player2Pieces.Add(new ChessPiece(curObject, loc2, Player2Mat));
        }

        _ClientSocket = new ClientSocket();
        _ClientSocket.Init();
    }

    public void InitSinglePlayer()
    {
        GameObject ai = GameObject.Find("CheckersAI").transform.Find("Button").gameObject;
        ai.SetActive(true);
    }

    public void ProccessAiTurn()
    {
        Checkers.MoveAI(Player1Pieces, Player2Pieces);
    }

	// Update is called once per frame
	void Update () 
	{
        if (_ClientSocket == null)
            return;

		List<Utilities.Marker> markers = _ClientSocket.GetMarkers ();

        foreach (Utilities.Marker curMarker in markers)
        {
            if(_Markers.ContainsKey(curMarker.ID))
            {
                Utilities.Marker temp = _Markers[curMarker.ID];
                temp.FrameCount = 0;
                temp.ScreenLoc.x = curMarker.ScreenLoc.x;
                temp.ScreenLoc.y = curMarker.ScreenLoc.y;
                temp.WorldLoc = curMarker.WorldLoc;
            }
            else
            {
                _Markers.Add(curMarker.ID, curMarker);
            }
        }

        foreach (Utilities.Marker curMarker in _Markers.Values)
        {
            if (curMarker.ID != 1 && curMarker.ID != 2 && curMarker.ID != 3 && curMarker.ID != 4 && curMarker.FrameCount < 5)
            {
                curMarker.FrameCount++;

                Vector2 pos;
                pos.y = curMarker.ScreenLoc.x / 50;
                pos.x = curMarker.ScreenLoc.y / 50;

                Vector3 test = Utilities.IndexToWorld((int)pos.x, (int)pos.y);

                Debug.Log("Marker: " + curMarker.ID + " " + pos.ToString());

                int nID = curMarker.ID-5;
                nID = nID % 16;

                if(curMarker.ID <= 21)
                    Player1Pieces[nID].Model.transform.position = test;
                else
                    Player2Pieces[nID - 16].Model.transform.position = test;
            }
        }
	}
}
