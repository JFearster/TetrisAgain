using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Managing tetrimino spawning and generation.
/// </summary>
public class TetriminoManager : MonoBehaviour
{
    // references
    [SerializeField] private PlayerController player;
    [SerializeField] private BoardManager board;
    [SerializeField] private ScoreManager sManager;

    // TODO: don't do this here, move it to a UI manager along with some of the score manager data.
    [SerializeField] GameObject gameoverSprite;

    // Array of tetriminos that can spawn.
    [SerializeField] private Tetrimino[] tetriminos;

    [SerializeField] private GameObject[] ghostTetriminos;

    // list of tetriminos in the first 6 bag slots.
    [SerializeField] private SpriteRenderer[] tetriminoSpawnList;
    // reference to the held icon
    [SerializeField] private SpriteRenderer heldTetrimino;

    // List of tetriminos contained in the bag using 7 bag generation.
    private List<Tetrimino> tetriminoBag = new List<Tetrimino>();

    /// <summary>
    /// Spawn a new tetrimino at the top of the board
    /// </summary>
    /// <param name="instance">The tetrimino to spawn, null if the spawn is from the generated list.</param>
    public void SpawnTetrimino(Tetrimino instance)
    {
        if (instance == null)
        {
            // spawn tetrimino
            instance = Instantiate(tetriminoBag[0]);
        }
        else
        {
            instance.gameObject.SetActive(true);
        }

        instance.transform.position = new Vector2(4, 18);
        instance.transform.rotation = Quaternion.identity;

        // adjust spawn location of O and I tetriminos
        if (instance.Shape == 3)
        {
            instance.transform.position += new Vector3(0.5f, 0.5f);
        }
        else if (instance.Shape == 0)
        {
            instance.transform.position += new Vector3(0.5f, -0.5f);
        }

        // set references to tetrimino
        instance.BManager = board;
        instance.TManager = this;
        instance.PController = player;
        // tetrimino speed
        instance.TetriminoSpeed = sManager.GameSpeed;
        // ghost piece setup
        if (instance.GhostPiece == null)
        {
            instance.GhostPiece = Instantiate(ghostTetriminos[instance.Shape]);
            instance.GhostPiece.transform.parent = instance.transform;
            instance.GhostPiece.transform.position = instance.transform.position;
        }

        // validate tetrimino spawn position
        bool isLegalSpawn = board.ValidatePosition(instance.gameObject);
        
        if (isLegalSpawn)
        {
            // set player reference to new tetrimino
            player.CurrentTetrimino = instance;
            // start tetrimino movement
            instance.Setup();
            // remove spawned tetrimino from the tetrimino list
            tetriminoBag.RemoveAt(0);
            // update spawn list
            DisplaySpawnList();
            // replenish bag
            if (tetriminoBag.Count <= 7)
            {
                SevenBagGeneration();
            }
        }
        else
        {
            // game over logic
            player.IsGameOver = true;
            gameoverSprite.SetActive(true);
        }
    }

    /// <summary>
    /// Generating 7 tetriminos, shuffling their positions, and adding them to the bag.
    /// </summary>
    private void SevenBagGeneration()
    {
        var newBag = new List<Tetrimino>();
        // Generate
        for (int i = 0; i < 7; i++)
        {
            newBag.Add(tetriminos[i]);
        }
        // Shuffle
        HelperTools.HTools.Shuffle(newBag);
        // Add
        tetriminoBag.AddRange(newBag);
    }

    /// <summary>
    /// Updates the spawn list with new tetriminos and displays them.
    /// </summary>
    private void DisplaySpawnList()
    {
        for (int i = 0; i < tetriminoSpawnList.Length; i++)
        {
            tetriminoSpawnList[i].sprite = tetriminoBag[i].TetriminoSprite;
        }
    }

    /// <summary>
    /// Displays the current held tetrimino.
    /// </summary>
    public void DisplayHeldTetrimino()
    {
        heldTetrimino.sprite = player.HeldTetrimino.TetriminoSprite;
    }

    private void Start()
    {
        // Generate 14 tetriminos
        SevenBagGeneration();
        SevenBagGeneration();
        // Spawn first tetrimino
        SpawnTetrimino(null);
    }
}
