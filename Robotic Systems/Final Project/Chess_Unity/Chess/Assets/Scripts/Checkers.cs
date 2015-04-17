using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkers
{
    //Very dumb checkers AI
    public static void MoveAI(List<ChessPiece> PlayerPieces, List<ChessPiece> AIPieces)
    {
        //Build the current game board
        Dictionary<Vector2, ChessPiece> gameBoard = new Dictionary<Vector2, ChessPiece>();

        foreach (ChessPiece curPiece in PlayerPieces)
            gameBoard.Add(curPiece.CurLocation, curPiece);

        foreach (ChessPiece curPiece in AIPieces)
            gameBoard.Add(curPiece.CurLocation, curPiece);

        //Run through each AI piece and see which can move
        //Seperate jumps into a seperate list

        //If the jump list has moves pull the first one

        //If the jump list is empty then pull the one closest to the other side or the first one

        //do the move
    }
}
