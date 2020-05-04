using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine ;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;      //Time to wait before starting level, in seconds.
    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    private BoardManager boardScript;

    private Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;     //Boolean to check if we're setting up board, prevent Player from moving during setup.

    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;
    void Awake() 
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }
    //This is called each time a scene is loaded.
    void OnLevelWasLoaded(int index)
    {
        //Add one to our level number.
        level++;
        //Call InitGame to initialize our level.
        InitGame();
    }
    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false; //So that the player can now  move.
    }

    void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay); // start the function after levelStartDelay time.

        enemies.Clear();
        boardScript.SetupScene(level);
    }
    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved";
        levelImage.SetActive(true);
        enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;
        StartCoroutine(MoveEnemies());
    }
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if(enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        playersTurn = true;
        enemiesMoving = false;
    }
}
