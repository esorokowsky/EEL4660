using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    public Dictionary<int, GamePiece> Player1Pieces = new Dictionary<int, GamePiece>();
    public Dictionary<int, GamePiece> Player2Pieces = new Dictionary<int,GamePiece>();

    private Dictionary<int, Utilities.Marker> _Markers = new Dictionary<int, Utilities.Marker>();

    //scallar offsets from marker 1
    private float _XOffset = 0.147f;
    private float _YOffset = 0.0529f;

	public Material Player1Mat;
	public Material Player2Mat;

	private GameObject _GameBoard;
	private ClientSocket _ClientSocket;

    //1: Chess 2P, 2: Checkers 1P, 3: Checkers 2P
    private int _GameMode = -1;

	// Use this for initialization
	void Start () 
	{
		_GameBoard = GameObject.Find("ChessBoard");
	}

    public void InitChess2P()
    {
        _GameMode = 1;
        InitChess();

        GameObject gameChooser = GameObject.Find("GameChooser");
        gameChooser.SetActive(false);

        GameObject ai = GameObject.Find("NextTurnButton").transform.Find("Button").gameObject;
        ai.SetActive(true);
    }

    public void InitCheckers1P()
    {
        _GameMode = 2;
        InitCheckers();

        GameObject gameChooser = GameObject.Find("GameChooser");
        gameChooser.SetActive(false);

        GameObject ai = GameObject.Find("NextTurnButton").transform.Find("Button").gameObject;
        ai.SetActive(true);
    }

    public void InitCheckers2P()
    {
        _GameMode = 3;
        InitCheckers();

        GameObject gameChooser = GameObject.Find("GameChooser");
        gameChooser.SetActive(false);

        GameObject ai = GameObject.Find("NextTurnButton").transform.Find("Button").gameObject;
        ai.SetActive(true);
    }
	
    void InitChess()
    {
        GameObject Player1 = GameObject.Find("Player 1 Pieces");
        GameObject Player2 = GameObject.Find("Player 2 Pieces");

        int markerNum = 5;

        //Player 1
        {
            //Init all pawns
            for (int i = 0; i < 8; i++)
            {
                Vector2 loc = new Vector2(0, i);

                //Player 1
                loc.x = 1;
                GameObject curObject = Player1.transform.Find("Hi_Pawn " + (i + 1)).gameObject;
                Player1Pieces.Add(markerNum, new GamePiece(curObject, loc, Player1Mat, markerNum++, 1));
            }

            //King
            GameObject curPiece = Player1.transform.Find("Hi_King").gameObject;
            Player1Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(0, 4), Player1Mat, markerNum++, 1));

            //Queen
            curPiece = Player1.transform.Find("Hi_Queen").gameObject;
            Player1Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(0, 3), Player1Mat, markerNum++, 1));

            //Bishops
            curPiece = Player1.transform.Find("Hi_Bishop 1").gameObject;
            Player1Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(0, 2), Player1Mat, markerNum++, 1));

            curPiece = Player1.transform.Find("Hi_Bishop 2").gameObject;
            Player1Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(0, 5), Player1Mat, markerNum++, 1));

            //Knights
            curPiece = Player1.transform.Find("Hi_Knight 1").gameObject;
            Player1Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(0, 1), Player1Mat, markerNum++, 1));

            curPiece = Player1.transform.Find("Hi_Knight 2").gameObject;
            Player1Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(0, 6), Player1Mat, markerNum++, 1));

            //Rook
            curPiece = Player1.transform.Find("Hi_Rook 1").gameObject;
            Player1Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(0, 0), Player1Mat, markerNum++, 1));

            curPiece = Player1.transform.Find("Hi_Rook 2").gameObject;
            Player1Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(0, 7), Player1Mat, markerNum++, 1));
        }


        //Player 2
        {
            //Init all pawns
            for (int i = 0; i < 8; i++)
            {
                Vector2 loc = new Vector2(0, i);

                //Player 2
                loc.x = 6;
                GameObject curObject = Player2.transform.Find("Hi_Pawn " + (i + 1)).gameObject;
                Player2Pieces.Add(markerNum, new GamePiece(curObject, loc, Player2Mat, markerNum++, 2));
            }

            //King
            GameObject curPiece = Player2.transform.Find("Hi_King").gameObject;
            Player2Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(7, 4), Player2Mat, markerNum++, 2));

            //Queen
            curPiece = Player2.transform.Find("Hi_Queen").gameObject;
            Player2Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(7, 3), Player2Mat, markerNum++, 2));

            //Bishops
            curPiece = Player2.transform.Find("Hi_Bishop 1").gameObject;
            Player2Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(7, 2), Player2Mat, markerNum++, 2));

            curPiece = Player2.transform.Find("Hi_Bishop 2").gameObject;
            Player2Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(7, 5), Player2Mat, markerNum++, 2));

            //Knights
            curPiece = Player2.transform.Find("Hi_Knight 1").gameObject;
            Player2Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(7, 1), Player2Mat, markerNum++, 2));

            curPiece = Player2.transform.Find("Hi_Knight 2").gameObject;
            Player2Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(7, 6), Player2Mat, markerNum++, 2));

            //Rooks
            curPiece = Player2.transform.Find("Hi_Rook 1").gameObject;
            Player2Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(7, 0), Player2Mat, markerNum++, 2));

            curPiece = Player2.transform.Find("Hi_Rook 2").gameObject;
            Player2Pieces.Add(markerNum, new GamePiece(curPiece, new Vector2(7, 7), Player2Mat, markerNum++, 2));
        }

        _ClientSocket = new ClientSocket();
        _ClientSocket.Init();
    }

    void InitCheckers()
    {
        GameObject Player1 = GameObject.Find("Player 1 Pieces");
        GameObject Player2 = GameObject.Find("Player 2 Pieces");

        int markerNum = 5;

        //Player 1
        for (int i = 0; i < 12; i++)
        {
            Vector2 loc1 = new Vector2(0, (i * 2));
            if(i >= 8)
            {
                loc1.y = (i - 8) * 2;
            }
            else if(i >= 4)
            {
                loc1.y = (i - 4) * 2+1;
            }
            else
            {
                loc1.y = (i * 2);
            }

            loc1.x = i/4;
            GameObject curObject = Player1.transform.Find("Checker Light " + (i + 1)).gameObject;
            Player1Pieces.Add(markerNum, new GamePiece(curObject, loc1, Player1Mat, markerNum++, 1));
        }

        //Player 2
        for (int i = 0; i < 12; i++)
        {
            Vector2 loc2 = new Vector2(0, (i * 2));
            if (i >= 8)
            {
                loc2.y = (i - 8) * 2 + 1;
            }
            else if (i >= 4)
            {
                loc2.y = (i - 4) * 2;
            }
            else
            {
                loc2.y = (i * 2) + 1;
            }

            loc2.x = 7 - i / 4;
            GameObject curObject = Player2.transform.Find("Checker Dark " + (i + 1)).gameObject;
            Player2Pieces.Add(markerNum, new GamePiece(curObject, loc2, Player2Mat, markerNum++, 2));
        }

        _ClientSocket = new ClientSocket();
        _ClientSocket.Init();
    }

    public void ProccessTurn()
    {
        if(_GameMode == 1)      //Chess 2P
        {

        }
        else if(_GameMode == 2) //Checkers 1P
        {
            GamePiece killedPiece = Checkers.MoveAI(Player1Pieces, Player2Pieces);
            if (killedPiece != null)
            {
                killedPiece.Object.SetActive(false);
                Player1Pieces.Remove(killedPiece.MarkerNum);
            }

            GamePiece killedPiece2 = Checkers.MoveAI(Player2Pieces, Player1Pieces);
            if (killedPiece2 != null)
            {
                killedPiece2.Object.SetActive(false);
                Player1Pieces.Remove(killedPiece2.MarkerNum);
            }
        }
        else if(_GameMode == 3) //Checkers 1P
        {

        }
    }

	// Update is called once per frame
	void Update () 
	{
        List<Utilities.Marker> curMarkers = GetMovedMarkers();

        //Go through the moved markers and check to see if a piece was killed
        foreach(Utilities.Marker curMarker in curMarkers)
        {
            //The piece that we need to remove if any
            GamePiece killedPiece = null;
            int killedPlayer = -1;

            if (_GameMode == 1) //Chess
            {
                //Check to see if we landed on a game piece and kill it if we did otherwise just move
                foreach(GamePiece curPiece in Player1Pieces.Values)
                {
                    if(curPiece.CurLoc == curMarker.ScreenLoc)
                    {
                        killedPiece = curPiece;
                        killedPlayer = 1;
                        break;
                    }
                }

                //If no piece was found in player1 pieces then check player 2
                if (killedPiece == null)
                {
                    foreach (GamePiece curPiece in Player2Pieces.Values)
                    {
                        if (curPiece.CurLoc == curMarker.ScreenLoc)
                        {
                            killedPiece = curPiece;
                            killedPlayer = 2;
                            break;
                        }
                    }
                }
            }
            else //Checkers
            {
                //Find the space between the new and old location which is the kill location
                GamePiece refPiece = Player1Pieces[curMarker.ID];
                Vector2 killPos = refPiece.CurLoc + curMarker.ScreenLoc;
                killPos = killPos / 2;

                //Check to see if we landed on a game piece and kill it if we did otherwise just move
                foreach (GamePiece curPiece in Player1Pieces.Values)
                {
                    if (curPiece.CurLoc == killPos)
                    {
                        killedPiece = curPiece;
                        killedPlayer = 1;
                        break;
                    }
                }

                //If no piece was found in player1 pieces then check player 2
                if (killedPiece == null)
                {
                    //Find the space between the new and old location which is the kill location
                    refPiece = Player1Pieces[curMarker.ID];
                    killPos = refPiece.CurLoc + curMarker.ScreenLoc;
                    killPos = killPos / 2;

                    foreach (GamePiece curPiece in Player2Pieces.Values)
                    {
                        if (curPiece.CurLoc == killPos)
                        {
                            killedPiece = curPiece;
                            killedPlayer = 2;
                            break;
                        }
                    }
                }
            }

            //If we found a piece to kill then hide it and remove it from the valid pieces
            if (killedPiece != null)
            {
                killedPiece.Object.SetActive(false);
                if (killedPlayer == 1)
                {
                    Player1Pieces.Remove(killedPiece.MarkerNum);
                }
                else
                {
                    Player2Pieces.Remove(killedPiece.MarkerNum);
                }
            }

            //Move the game piece
            if (Player1Pieces.ContainsKey(curMarker.ID))
            {
                GamePiece curPiece = Player1Pieces[curMarker.ID];
                curPiece.MovePiece(curMarker.ScreenLoc);
            }
            else if (Player2Pieces.ContainsKey(curMarker.ID))
            {
                GamePiece curPiece = Player2Pieces[curMarker.ID];
                curPiece.MovePiece(curMarker.ScreenLoc);
            }
        }


	}

    //pull in markers from Aruco
    List<Utilities.Marker> GetMovedMarkers()
    {
        List<Utilities.Marker> retList = new List<Utilities.Marker>();

        if (_ClientSocket == null)
            return retList;

        List<Utilities.Marker> markers = _ClientSocket.GetMarkers();

        foreach (Utilities.Marker curMarker in markers)
        {
            Utilities.Marker temp = new Utilities.Marker();

            temp.ScreenLoc.y = (int)(curMarker.ScreenLoc.x / 50);
            temp.ScreenLoc.x = (int)(curMarker.ScreenLoc.y / 50);
            temp.ID = curMarker.ID;

            //Only proccess valid markers
            if (Player1Pieces.ContainsKey(curMarker.ID))
            {
                GamePiece curPiece = Player1Pieces[curMarker.ID];

                if (curPiece.CurLoc != temp.ScreenLoc)
                    retList.Add(temp);
            }
            else if (Player2Pieces.ContainsKey(curMarker.ID))
            {
                GamePiece curPiece = Player2Pieces[curMarker.ID];

                if (curPiece.CurLoc != temp.ScreenLoc)
                    retList.Add(temp);
            }
            else
            {
                Debug.Log("Marker " + curMarker.ID + " does not exist!!!");
            }
        }

        return retList;
    }
}
