using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // references
    public Tetrimino CurrentTetrimino { get; set; }

    public Tetrimino HeldTetrimino { get; set; }
    public bool CanHold { get; set; } = true;
    public bool IsGameOver { get; set; } = false; 

    private bool canAct = true;

    private Dictionary<KeyCode, bool> delayGroup = new Dictionary<KeyCode, bool>();

    /// <summary>
    /// small delay between each movement when the button is held.
    /// </summary>
    /// <returns></returns>
    private IEnumerator InputDelay(KeyCode delayedKey)
    {
        delayGroup.Add(delayedKey, true);
        yield return new WaitForSeconds(0.085f);
        delayGroup.Remove(delayedKey);
    }

    /// <summary>
    /// Movement on the X axis.
    /// </summary>
    private void MoveTetrimino()
    {
        if (!delayGroup.ContainsKey(KeyCode.A) && Input.GetKey(KeyCode.A))
        {
            CurrentTetrimino.Move(Vector3.left);
            StartCoroutine(InputDelay(KeyCode.A));
        }
        else if (!delayGroup.ContainsKey(KeyCode.D) && Input.GetKey(KeyCode.D))
        {
            CurrentTetrimino.Move(Vector3.right);
            StartCoroutine(InputDelay(KeyCode.D));
        }
    }

    /// <summary>
    /// Rotation of the tetrimino.
    /// </summary>
    private void RotateTetrimino()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            CurrentTetrimino.Rotate(new Vector3(0, 0, -90f));
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            CurrentTetrimino.Rotate(new Vector3(0, 0, 90f));
        }
    }

    /// <summary>
    /// Movement on the Y axis.
    /// </summary>
    private void DropTetrimino()
    {
        if (!delayGroup.ContainsKey(KeyCode.S) && Input.GetKey(KeyCode.S))
        {
            CurrentTetrimino.Move(Vector3.down);
            StartCoroutine(InputDelay(KeyCode.S));
        }
    }

    /// <summary>
    /// Instant movement on the Y axis.
    /// </summary>
    private void HardDropTetrimino()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            canAct = false;
            CurrentTetrimino.HardDrop(Vector3.down);
            canAct = true;
        }
    }

    private void HoldTetrimino()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            CurrentTetrimino.Hold();
        }
    }

    private void RestartGame()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    private void Update()
    {
        if (!IsGameOver)
        {
            // movements that can be held.
            if (canAct)
            {
                MoveTetrimino();
                DropTetrimino();
                RotateTetrimino();
                HardDropTetrimino();
                HoldTetrimino();
            }
        }
        if (IsGameOver)
        {
            RestartGame();
        }

    }
}
