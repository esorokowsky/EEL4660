using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkers
{
    //Very dumb checkers AI
    public static GamePiece MoveAI(Dictionary<int, GamePiece> PlayerPieces, Dictionary<int, GamePiece> AIPieces)
    {
        //Build the current game board
        Dictionary<Vector2, GamePiece> gameBoard = new Dictionary<Vector2, GamePiece>();

        foreach (GamePiece curPiece in PlayerPieces.Values)
            gameBoard.Add(curPiece.CurLoc, curPiece);

        foreach (GamePiece curPiece in AIPieces.Values)
            gameBoard.Add(curPiece.CurLoc, curPiece);

        int score = -1;
        Vector2 dest = Vector2.zero;
        GamePiece movePiece = null;

        foreach(GamePiece curPiece in AIPieces.Values)
        {
            Vector2 tempDest = Vector2.zero;
            int tempScore = GetBestMove(ref tempDest, curPiece, gameBoard, 0, 3);

            if(tempScore > score)
            {
                score = tempScore;
                dest = tempDest;
                movePiece = curPiece;
            }
        }

        GamePiece killedPiece = null;
        if (score != -1)
        {
            //Find the space between the new and old location which is the kill location
            Vector2 killPos = movePiece.CurLoc + dest;
            killPos = killPos / 2;

            if (gameBoard.ContainsKey(killPos))
            {
                killedPiece = gameBoard[killPos];
            }

            movePiece.MovePiece(dest);
        }

        return killedPiece;
    }

    public static bool ValidMove(Vector2 move)
    {
        return move.x >= 0 && move.y >= 0 && move.x < 8 && move.y < 8;
    }

    public static int GetBestMove(ref Vector2 dest, GamePiece Piece, Dictionary<Vector2, GamePiece> GameBoard, int CurDepth, int MaxDepth)
    {
        if (CurDepth == MaxDepth)
        {
            return 0;
        }

        int leftScore = -1;
        int rightScore = -1;

        Vector2 moveL = new Vector2(-1,1);
        Vector2 moveR = new Vector2(-1,-1);

        if(Piece.Player == 1)
        {
            moveL *= -1;
            moveR *= -1;
        }

        //calculate left move
        Vector2 locL = (Piece.CurLoc + moveL);
        if (!ValidMove(locL))
        {
            leftScore = -1;
        }
        else if(GameBoard.ContainsKey(locL))
        {
            GamePiece temp = GameBoard[(locL)];
            if(temp.Player != Piece.Player)
            {
                locL += moveL;
                if (!ValidMove(locL))
                {
                    leftScore = -1;
                }
                else if (!GameBoard.ContainsKey(locL))
                {
                    moveL = locL;
                    leftScore = 2;
                }
                else
                {
                    leftScore = -1;
                }
            }
            else
            {
                leftScore = -1;
            }
        }
        else
        {
            moveL = locL;
            leftScore = 1;
        }

        //calculate right move
        Vector2 locR = (Piece.CurLoc + moveR);
        if (!ValidMove(locR))
        {
            rightScore = -1;
        }
        else if (GameBoard.ContainsKey(locR))
        {
            GamePiece temp = GameBoard[(locR)];
            if (temp.Player != Piece.Player)
            {
                locR += moveR;
                if (!ValidMove(locR))
                {
                    rightScore = -1;
                }
                else if (!GameBoard.ContainsKey(locR))
                {
                    //We have to take the jump according to the rules
                    moveR = locR;
                    rightScore = 20;
                }
                else
                {
                    rightScore = -1;
                }
            }
            else
            {
                rightScore = -1;
            }
        }
        else
        {
            moveR = locR;
            rightScore = 1;
        }


        GamePiece left = new GamePiece();
        left.CurLoc = moveL;
        left.Player = Piece.Player;

        GamePiece right = new GamePiece();
        right.CurLoc = moveR;
        right.Player = Piece.Player;

        Vector2 lvec = Vector2.zero;
        Vector2 rvec = Vector2.zero;

        if(leftScore != -1)
            leftScore += GetBestMove(ref lvec, left, GameBoard, ++CurDepth, MaxDepth);

        if(rightScore != -1)
            rightScore += GetBestMove(ref rvec, right, GameBoard, ++CurDepth, MaxDepth);
        
        if(leftScore >= rightScore)
        {
            dest = moveL;
        }
        else if(rightScore > leftScore)
        {
            dest = moveR;
        }

        int max = Mathf.Max(leftScore, rightScore);

        if (max == -1 && CurDepth > 0)
            max = 0;

        return max;
    }
}
