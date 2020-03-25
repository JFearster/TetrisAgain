using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private ScoreManager sManager;
    private KeyValuePair<GameObject, bool>[,] board = new KeyValuePair<GameObject, bool>[10, 20];

    /// <summary>
    /// Generating an empty game board
    /// </summary>
    private void GenerateBoard()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                board[j, i] = new KeyValuePair<GameObject, bool>(null, false);
            }
        }
    }

    /// <summary>
    /// Clear a row of tetriminos
    /// </summary>
    public void ClearTetriminos(GameObject tetrimino)
    {
        int linesCleared = 0;
        List<TetriminoPiece> tetriminoPieces = new List<TetriminoPiece>();

        // add all pieces to a list
        for (int i = 0; i < 4; i++)
        {
            tetriminoPieces.Add(tetrimino.transform.GetChild(i).GetComponent<TetriminoPiece>());
        }


        // iterate through each piece
        for (int i = 0; i < tetriminoPieces.Count; i++)
        {
            bool isRowFull = true;
            // check for empty space
            for (int j = 0; j < 10; j++)
            {
                if (board[j, (int)tetriminoPieces[i].BoardPosition.y].Value == false)
                {
                    isRowFull = false;
                    break;
                }
            }

            if (isRowFull)
            {
                // select y position
                int yPos = (int)tetriminoPieces[i].BoardPosition.y;
                // clear row
                for (int k = 0; k < 10; k++)
                {
                    // clear piece in row
                    GameObject piece = board[k, yPos].Key;
                    board[k, yPos] = new KeyValuePair<GameObject, bool>(null, false);
                    Destroy(piece);
                }
                // track cleared rows, report to score manager
                linesCleared++;

                // drop pieces above
                DropPieces(yPos);
            }
        }

        if (linesCleared != 0)
        {
            //TODO: score points
            sManager.ScorePoints(linesCleared);
        }
    }


    /// <summary>
    /// Drop rows of tetriminos after clearing.
    /// </summary>
    /// <param name="yPos">The position tetriminos were cleared from</param>
    private void DropPieces(int yPos)
    {
        for (int i = yPos + 1; i < board.GetLength(1); i++)
        {
            for (int j = 0; j < board.GetLength(0); j++)
            {
                if (board[j, i].Value)
                {
                    // set new positions
                    TetriminoPiece key = board[j, i].Key.GetComponent<TetriminoPiece>();
                    key.BoardPosition = new Vector2(key.BoardPosition.x, key.BoardPosition.y - 1);
                    key.transform.position = new Vector2(key.transform.position.x, key.transform.position.y - 1);
                    board[j, i - 1] = board[j, i];
                    // remove old position
                    board[j, i] = new KeyValuePair<GameObject, bool>(null, false);
                }
            }
        }
    }

    /// <summary>
    /// Mark the occupancy of spaces on the game board.
    /// </summary>
    /// <param name="tetrimino">The requested tetrimino</param>
    /// <param name="isOccupied">Whether to mark the space occupied or unoccupied</param>
    public void MarkPosition(GameObject tetrimino, bool isOccupied)
    {
        // mark the position of each piece of the tetrimino as occupied on the game board
        for (int i = 0; i < 4; i++)
        {
            GameObject child = tetrimino.transform.GetChild(i).gameObject;
            Vector2 currentPiecePos = child.transform.position;
            Vector2 tetriminoPosition = new Vector2(Mathf.Round(currentPiecePos.x * 100f) / 100f, Mathf.Round(currentPiecePos.y * 100) / 100);
            child.GetComponent<TetriminoPiece>().BoardPosition = tetriminoPosition;
            board[(int)tetriminoPosition.x, (int)tetriminoPosition.y] = new KeyValuePair<GameObject, bool>(child, isOccupied);
        }
    }

    /// <summary>
    /// Ensuring that a movement is valid.
    /// </summary>
    /// <param name="tetrimino">The tetrimino to validate</param>
    /// <returns>True if the movement is valid</returns>
    public bool ValidatePosition(GameObject tetrimino)
    {
        // check new positions
        for (int i = 0; i < 4; i++)
        {
            if (!CheckPosition(tetrimino.transform.GetChild(i).position))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Check if a requested position is unoccupied
    /// </summary>
    /// <param name="newPosition">The position to check</param>
    /// <returns>True if the requested position is clear</returns>
    public bool CheckPosition(Vector3 newPosition)
    {
        // round the position
        newPosition = new Vector3(Mathf.Round(newPosition.x * 100f) / 100f, Mathf.Round(newPosition.y * 100f) / 100f);
        // check boundaries of the position
        if (newPosition.x > -1 && newPosition.x < 10 && newPosition.y > -1 && newPosition.y < 20)
        {
            // check the position for another object
            if (board[(int)newPosition.x, (int)newPosition.y].Value == false)
            {
                return true;
            }
        }
        return false;
    }

    private void Awake()
    {
        GenerateBoard();
    }
}
