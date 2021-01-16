using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public enum BonusTypes : int { BallSlowing, BallAcceleration, BallDoubling, RacketWidening }

public class GameManager : MonoBehaviour
{
    //Настройки балланса игры
    public float racketSpeed = 7f;                // максимальная скорость ракетки
    public float baseRacketWidth = 2f;            // базовая ширина ракетки

    public float baseBallSpeed = 7f;              // базовая скорость мячика

    public float brickBonusPart = 0.2f;           // доля кирпичей с бонусами
    public float brickMaxHealth = 4f;             // максимальное здоровье кирпича

    public float bonusSpeed = 7f;                 // скорость падения бонуса


    // глобальные переменные
    public Brick[] bricks;                        // массив кирпичей
    public int level;                             // уровень
    public int quantityOfBalls;                   // количество мячей в игре

    [SerializeField]
    private GameObject ballInstance=null;
    [SerializeField]
    private GameObject doubleBallInstance=null;
    private Racket racket;

    [SerializeField]
    private GameObject oddBrickPrefab;
    [SerializeField]
    private GameObject evenBrickPrefab;
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private GameObject bonusPrefab;

    private void Awake()
    {
        racket = GameObject.FindGameObjectWithTag("Racket").GetComponent<Racket>();
    }

    private void Start()
    {
        Time.timeScale = 1;
        level = 1;
        Debug.Log("Game started!");
        StartLevel();
    }

    private void StartLevel()
    {
        Debug.Log("Level " + level.ToString());
        ballInstance = Instantiate(ballPrefab, new Vector3(9f, 0.75f, 0f), transform.rotation.normalized * Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)));
        quantityOfBalls = 1;
        racket.enabled = true;
        InitialBricksSpawn();
    }
    private void InitialBricksSpawn(int raws = 3, int columns = 9)
    {
        bricks = new Brick[raws*columns];
        for (int height = 0; height < raws; height++)
        {
            for (int width = 0; width < columns; width++)
            {
                bool oddEven = (height + width) % 2 == 0;
                GameObject oddEvenPrefab = oddEven ? evenBrickPrefab : oddBrickPrefab;

                float x = width * 2f + 1f;      // 1 3 5 7 9 11 13 15 17
                float y = 23.5f - height;       // 23.5  22.5  21.5

                GameObject instance = Instantiate(oddEvenPrefab, new Vector3(x, y, 0f), transform.rotation.normalized);

                float randomForHealth = Random.Range(1f, brickMaxHealth);
                float health = 1f;
                bool hasBonus = false;
                switch (level)
                {
                    case 1:
                        health = 1;
                        break;
                    case 2:
                        health = (int)randomForHealth;
                        break;
                    case 3:
                        health = (int)randomForHealth;
                        hasBonus = Random.value < brickBonusPart;
                        break;
                }

                bricks[height*columns + width] = new Brick(health, hasBonus, instance, x, y);
            }
        }
    }

    public void CheckForGameOver()
    {
        if (quantityOfBalls < 1)
        {
            Debug.Log("Game Over! You lose! Press R for restart!");
            EndLevel();
            Time.timeScale = 0;
        }
    }


    [SerializeField]
    private int bricksLeft;
    public void CheckForLevelEnd()
    {
        bricksLeft = (from num in bricks where (num != null) select num).Count();

        if (bricksLeft == 0)
        {
            level++;
            Debug.Log("Level End!");
            if (level > 3)
            {
                Debug.Log("Game Over! You win!");
                Time.timeScale = 0;
            }
            else
            {
                EndLevel();
                StartLevel();
            }
        }
    }

    private void EndLevel()
    {
        if (ballInstance != null) Destroy(ballInstance);
        if (doubleBallInstance != null) Destroy(doubleBallInstance);
        racket.enabled = false;
    }

    public void BonusCreate(int brickNumber)
    {
        if (bricks[brickNumber].hasBonus)
        {
            GameObject bonusInstance;
            bonusInstance = Instantiate(bonusPrefab, new Vector3(bricks[brickNumber].x, bricks[brickNumber].y, 0f), transform.rotation.normalized);
        }
    }

    [SerializeField]
    BonusTypes bonusType;
    public void BonusCatched()
    {
        // не более 2 мячиков в игре
        do bonusType = (BonusTypes)(int)Random.Range(0f, 4f);
        while ((ballInstance != null) && (doubleBallInstance != null) && (bonusType == BonusTypes.BallDoubling));

        switch (bonusType)
        {
            case BonusTypes.BallSlowing:
                BallsSpeedChange(-2f);
                Debug.Log("Balls Speed slowed down!");
                break;
            case BonusTypes.BallAcceleration:
                BallsSpeedChange(2f);
                Debug.Log("Balls Speed growed up!");
                break;
            case BonusTypes.BallDoubling:
                DoubleBall();
                break;
            case BonusTypes.RacketWidening:
                RacketWidthChange();
                break;
        }
    }

    private void BallsSpeedChange(float n)
    {
        if (ballInstance != null)
        {
            Ball ballBall1 = ballInstance.GetComponent<Ball>();
            ballBall1.BallSpeed = ballBall1.BallSpeed + n;           
        }
        if (doubleBallInstance != null)
        {
            Ball ballBall2 = doubleBallInstance.GetComponent<Ball>();
            ballBall2.BallSpeed = ballBall2.BallSpeed + n;
        }
    }

    private void DoubleBall()
    {
        if (ballInstance != null)
        {
            Transform ballTransform = ballInstance.GetComponent<Transform>();
            doubleBallInstance = Instantiate(ballPrefab, ballTransform.position, ballTransform.rotation * Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)));
            quantityOfBalls++;
            Debug.Log("Ball doubled!");
        }

    }

    private void RacketWidthChange()
    {
        // Событие??
        racket.RacketWidth *= 1.5f;
        racket.GetComponent<Transform>().localScale = new Vector3(racket.RacketWidth, 0.5f, 1f);
        Debug.Log("Racket wided!");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
}

