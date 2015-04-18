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
            int tempScore = GetBestMove(ref tempDest, curPiece, gameBoard, 0, 6);

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

    //Is the move within the gameboard
    static bool ValidMove(Vector2 move)
    {
        return move.x >= 0 && move.y >= 0 && move.x < 8 && move.y < 8;
    }

    //Calculate the score for the proposed move in dir
    static int CalculateMove(ref Vector2 dest, Vector2 dir, GamePiece Piece, Dictionary<Vector2, GamePiece> GameBoard)
    {
        int score = -1;

        Vector2 loc = (Piece.CurLoc + dir);
        if (!ValidMove(loc))
        {
            score = -1;
        }
        else if (GameBoard.ContainsKey(loc))
        {
            GamePiece temp = GameBoard[(loc)];
            if (temp.Player != Piece.Player)
            {
                loc += dir;
                if (!ValidMove(loc))
                {
                    score = -1;
                }
                else if (!GameBoard.ContainsKey(loc))
                {
                    dest = loc;
                    score = 50;
                }
                else
                {
                    score = -1;
                }
            }
            else
            {
                score = -1;
            }
        }
        else
        {
            dest = loc;
            score = 1;
        }

        return score;
    }

    //Recusivly calculate the best posible move for this piece
    static int GetBestMove(ref Vector2 dest, GamePiece Piece, Dictionary<Vector2, GamePiece> GameBoard, int CurDepth, int MaxDepth)
    {
        if (CurDepth >= MaxDepth)
        {
            return 0;
        }

        int leftScoreF = -1;
        int rightScoreF = -1;
        int leftScoreR = -1;
        int rightScoreR = -1;

        Vector2 moveL = new Vector2(-1,1);
        Vector2 moveR = new Vector2(-1, -1);
        Vector2 moveLR = new Vector2(-1, 1);
        Vector2 moveRR = new Vector2(-1, -1);

        if (Piece.Player == 1)
        {
            moveL *= -1;
            moveR *= -1;
            moveLR *= -1;
            moveRR *= -1;
        }

        leftScoreF = CalculateMove(ref moveL, moveL, Piece, GameBoard);
        rightScoreF = CalculateMove(ref moveR, moveR, Piece, GameBoard);

        if(Piece.CanReverse == true)
        {
            moveLR = moveL * -1;
            moveRR = moveR * -1;

            leftScoreR = CalculateMove(ref moveLR, moveLR, Piece, GameBoard);
            rightScoreR = CalculateMove(ref moveRR, moveRR, Piece, GameBoard);
        }

        //Save off all moves to temp pieces
        GamePiece leftF = new GamePiece();
        leftF.CurLoc = moveL;
        leftF.Player = Piece.Player;
        leftF.CanReverse = Piece.CanReverse;

        GamePiece rightF = new GamePiece();
        rightF.CurLoc = moveR;
        rightF.Player = Piece.Player;
        rightF.CanReverse = Piece.CanReverse;

        GamePiece leftR = new GamePiece();
        leftR.CurLoc = moveLR;
        leftR.Player = Piece.Player;
        leftR.CanReverse = Piece.CanReverse;

        GamePiece rightR = new GamePiece();
        rightR.CurLoc = moveRR;
        rightR.Player = Piece.Player;
        rightR.CanReverse = Piece.CanReverse;

        //Recursivly calculate scores. This ends up being like a minimum spanning tree
        Vector2 dummy = Vector2.zero;
        if(leftScoreF != -1)
            leftScoreF += GetBestMove(ref dummy, leftF, GameBoard, CurDepth + 1, MaxDepth);

        if(rightScoreF != -1)
            rightScoreF += GetBestMove(ref dummy, rightF, GameBoard, CurDepth + 1, MaxDepth);

        if (leftScoreR != -1)
            leftScoreR += GetBestMove(ref dummy, leftR, GameBoard, CurDepth + 1, MaxDepth);

        if (rightScoreR != -1)
            rightScoreR += GetBestMove(ref dummy, rightR, GameBoard, CurDepth + 1, MaxDepth);

        int FBest;
        Vector2 FBestVec;

        int RBest;
        Vector2 RBestVec;

        int Best;
        Vector2 BestVec;

        //Find who did the best
        //foward
        if(leftScoreF > rightScoreF)
        {
            FBestVec = moveL;
            FBest = leftScoreF;
        }
        else
        {
            FBestVec = moveR;
            FBest = rightScoreF;
        }

        //reverse
        if (leftScoreR > rightScoreR)
        {
            RBestVec = moveLR;
            RBest = leftScoreF;
        }
        else
        {
            RBestVec = moveRR;
            RBest = rightScoreR;
        }

        //both
        if (FBest > RBest)
        {
            Best = FBest;
            BestVec = FBestVec; 
        }
        else
        {
            Best = RBest;
            BestVec = RBestVec; 
        }

        dest = BestVec;

        //We we have a -1 and we are not on the first layer then just return a 0
        if (Best == -1 && CurDepth > 0)
            Best = 0;

        return Best;
    }
}
