using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrimino : MonoBehaviour
{
    // component references
    public BoardManager BManager { get; set; }
    public TetriminoManager TManager { get; set; }
    public PlayerController PController { get; set; }

    // the amound of time before a tetrimino will take an action
    public float TetriminoSpeed { get; set; }

    // ghost piece
    public GameObject GhostPiece { get; set; }

    // movement marker
    private bool hasMoved = true;

    // the tetrimino shape
    [SerializeField] private int shape;
    public int Shape { get { return shape; } }

    // the sprite associated with the tetrimino
    [SerializeField] private Sprite tetriminoSprite;
    public Sprite TetriminoSprite { get { return tetriminoSprite; } }

    // current rotation value
    private int currentRotation = 0;
    // wall kick array for 6/7 tetriminos
    private Vector2[,] wallkickArray = new Vector2[,] { 
        { new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, -3), new Vector2(-1, 0)},
        { new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 3), new Vector2(1, 0),},
        { new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 3), new Vector2(1, 0),},
        { new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, -3), new Vector2(-1, 0),},
        { new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, -3), new Vector2(1, 0),},
        { new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 3), new Vector2(-1, 0),},
        { new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 3), new Vector2(-1, 0),},
        { new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, -3), new Vector2(1, 0),} };
    // wall kick array for I tetriminos
    private Vector2[,] wallkickArrayI = new Vector2[,] {
        { new Vector2(-2, 0), new Vector2(3, 0), new Vector2(-3, -1), new Vector2(3, 3)},
        { new Vector2(2, 0), new Vector2(-3, 0), new Vector2(3, 1), new Vector2(-3, -3),},
        { new Vector2(-1, 0), new Vector2(3, 0), new Vector2(-3, 2), new Vector2(3, -3),},
        { new Vector2(1, 0), new Vector2(-3, 0), new Vector2(3, -2), new Vector2(-3, 3),},
        { new Vector2(2, 0), new Vector2(-3, 0), new Vector2(3, 1), new Vector2(-3, -3),},
        { new Vector2(-2, 0), new Vector2(3, 0), new Vector2(-3, -1), new Vector2(3, 3),},
        { new Vector2(1, 0), new Vector2(-3, 0), new Vector2(3, -2), new Vector2(-3, 3),},
        { new Vector2(-1, 0), new Vector2(3, 0), new Vector2(-3, 2), new Vector2(3, -3),} };


    /// <summary>
    /// coroutine to lock the tetrimino in place after a period of time
    /// </summary>
    /// <returns></returns>
    private IEnumerator LockRoutine()
    {
        yield return new WaitForSeconds(0.35f);

        if (!hasMoved)
        {
            LockTetrimino();
        }
        else
        {
            StartCoroutine(DropRoutine());
        }

    }

    /// <summary>
    /// coroutine to constantly drop the tetrimino
    /// </summary>
    /// <returns></returns>
    private IEnumerator DropRoutine()
    {
        yield return new WaitForSeconds(TetriminoSpeed);
        if (Move(Vector3.down))
        {
            // if movement is successful
            hasMoved = true;
            StartCoroutine(DropRoutine());
        }
        else
        {
            // if movement is unsuccessful
            hasMoved = false;
            StartCoroutine(LockRoutine());
        }
    }

    /// <summary>
    /// Lock the tetrimino into place, preventing any further actions.
    /// </summary>
    private void LockTetrimino()
    {
        StopAllCoroutines();
        // remove tetrimino from player reference
        PController.CurrentTetrimino = null;
        // remove ghost piece.
        Destroy(GhostPiece);
        // mark final positions of tetrimino pieces
        BManager.MarkPosition(gameObject, true);
        // clear tetriminos if full lines exist
        BManager.ClearTetriminos(gameObject);
        // spawn new tetrimino
        TManager.SpawnTetrimino(null);
        // allow hold action
        PController.CanHold = true;
    }

    /// <summary>
    /// Drop the tetrimino as low as possible, locking instantly when the movement is complete.
    /// </summary>
    public void HardDrop(Vector3 direction)
    {
        // while movement is still possible
        bool canMove = true;
        while (canMove)
        {
            canMove = Move(direction);
        }

        // when movement is no longer possible
        LockTetrimino();
    }

    /// <summary>
    /// Move the tetrimino to a new position
    /// </summary>
    /// <param name="direction">The direction to move the tetrimino</param>
    /// <returns>True is movement is successful</returns>
    public bool Move(Vector3 direction)
    {
        // move tetrimino
        transform.position += direction;

        // Check if the position is clear
        if (!BManager.ValidatePosition(gameObject))
        {
            // revert movement
            transform.position -= direction;
            return false;
        }

        GhostPieceIndicator();

        hasMoved = true;
        return true;
    }

    /// <summary>
    /// USING SRS ROTATION SYSTEM || https://harddrop.com/wiki/SRS
    /// rotate the tetrimino left or right
    /// </summary>
    /// <param name="direction">-90 = left, 90 = right</param>
    public void Rotate(Vector3 rotation)
    {
        // rotate tetrimino
        transform.eulerAngles += rotation;

        // Check if the new position is clear
        bool isClear = BManager.ValidatePosition(gameObject);

        // try wall kicks if not clear
        if (!isClear)
        {
            isClear = WallKick(rotation);
        }

        // if rotation was successful
        if (isClear)
        {
            // adjust rotation marker
            if (rotation.z == -90)
            {
                currentRotation++;
                if (currentRotation > 3)
                {
                    currentRotation = 0;
                }
            }
            else
            {
                currentRotation--;
                if (currentRotation < 0)
                {
                    currentRotation = 3;
                }
            }

            GhostPieceIndicator();

            hasMoved = true;
        }
        // if rotation was unsuccessful
        else
        {
            // revert rotation
            transform.eulerAngles -= rotation;
        }
    }

    /// <summary>
    /// Kicking tetriminos off of walls.
    /// </summary>
    /// <returns>True if the kick is successful</returns>
    private bool WallKick(Vector3 rotation)
    {
        int wallkickIndex = -1;
        if (currentRotation == 0)
        {
            if (rotation.z == -90)
            {
                wallkickIndex = 0;
            }
            else
            {
                wallkickIndex = 7;
            }
        }
        else if (currentRotation == 1)
        {
            if (rotation.z == -90)
            {
                wallkickIndex = 2;
            }
            else
            {
                wallkickIndex = 1;
            }
        }
        else if (currentRotation == 2)
        {
            if (rotation.z == -90)
            {
                wallkickIndex = 4;
            }
            else
            {
                wallkickIndex = 3;
            }
        }
        else if (currentRotation == 3)
        {
            if (rotation.z == -90)
            {
                wallkickIndex = 6;
            }
            else
            {
                wallkickIndex = 5;
            }
        }

        Vector2 startPos = transform.position;
        // selecting the proper array for the tetrimino
        Vector2[,] usedArray = wallkickArray;
        if (shape == 0)
        {
            usedArray = wallkickArrayI;
        }

        for (int i = 0; i < 4; i++)
        {
            // Based on current rotation, move to new position
            transform.position += (Vector3)usedArray[wallkickIndex, i];

            // complete movement if position is clear
            if (BManager.ValidatePosition(gameObject))
            {
                return true;
            }
        }
        // reset position
        transform.position = startPos;
        return false;
    }

    /// <summary>
    /// Holding a tetrimino, storing it for later use.
    /// </summary>
    public void Hold()
    {
        if (PController.CanHold)
        {
            // prevent more than one hold action in a row
            PController.CanHold = false;
            // remove current tetrimino from player reference
            PController.CurrentTetrimino = null;
            // spawn new piece
            TManager.SpawnTetrimino(PController.HeldTetrimino);
            // add piece as currently held piece
            PController.HeldTetrimino = this;
            // update held piece display
            TManager.DisplayHeldTetrimino();
            // remove current tetrimino from the board
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Moving the ghost piece to the bottom of the play area
    /// </summary>
    private void GhostPieceIndicator()
    {
        // resetting basic position
        GhostPiece.transform.position = transform.position;
        while (true)
        {
            GhostPiece.transform.position -= Vector3.up;
            if (!BManager.ValidatePosition(GhostPiece))
            {
                GhostPiece.transform.position += Vector3.up;
                break;
            }
        }
    }

    public void Setup()
    {
        StartCoroutine(DropRoutine());
        GhostPieceIndicator();
    }
}