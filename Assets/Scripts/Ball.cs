using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private GameManager gameManager;
    private Racket racket;
    private Transform racketTransform;
    private Transform ballTransform;


    [SerializeField]
    private float ballSpeed;
    public float BallSpeed
    {
        get { return ballSpeed; }
        set { ballSpeed = value; }
    }

    [SerializeField]
    private Vector3 ballRotationInV3;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        racket = GameObject.FindGameObjectWithTag("Racket").GetComponent<Racket>();
        racketTransform = racket.GetComponent<Transform>();

        ballTransform = GetComponent<Transform>();
    }

    private void OnEnable()
    {
        ballRotationInV3 =  ballTransform.rotation * Vector3.up;                                 // Так преобразовываем вращение в вектор!

        ballSpeed = gameManager.baseBallSpeed + gameManager.level * 2 - 1;
    }


    private void FixedUpdate()
    {
        ballTransform.position += ballRotationInV3.normalized * ballSpeed * Time.deltaTime;

        if (ballTransform.position.y < 0.5f) if (!RacketCollide()) return;     

        WallsCollide();

        BricksCheck();
    }

    private void WallsCollide()         // Столкновение со стенами
    {       
        if (ballTransform.position.x < 0f)
        {
            ballTransform.position.Set(0f, ballTransform.position.y, ballTransform.position.z);
            SideCollide();
        }

        if (ballTransform.position.x > 18f)
        {
            ballTransform.position.Set(18f, ballTransform.position.y, ballTransform.position.z);
            SideCollide();
        }

        if (ballTransform.position.y > 24f)
        {
            ballTransform.position.Set(ballTransform.position.x, 24f, ballTransform.position.z);
            FrontCollide();
        }
    }

    [SerializeField]
    private float angle;
    [SerializeField]
    private float eccentricity;
    private bool RacketCollide()        // Столкновение с ракеткой
    {
        float diff = ballTransform.position.x - racketTransform.position.x;
        eccentricity = Mathf.Min(diff * 2 / racket.RacketWidth, 1f);
        if (Mathf.Abs(diff) <= racket.racketWidth / 2)
        {
            ballTransform.position.Set(ballTransform.position.x, 0.5f, ballTransform.position.z);

            // Эксцентриситет учитываем только с ближней к мячику стороны
            angle = ballTransform.rotation.eulerAngles.z;
            if (((angle >= 180) && (eccentricity < 0)) ||
                ((angle >= 0) && (angle < 180)  && (eccentricity > 0)))
            {
                //Debug.Log("ECCS works");
                FrontCollide(eccentricity);
            }
            else
            {
                FrontCollide();
            }

            return true;
        }
        else
        {
            gameManager.quantityOfBalls--;
            gameManager.CheckForGameOver();
            Destroy(this.gameObject);

            return false;
        }
    }


    private void BricksCheck()
    {
        // ряд 3
        if ((ballTransform.position.y > 21f) && (ballTransform.position.y < 22f))
        {
            int startBrickNumber = 18;
            CheckRaw(startBrickNumber);           
        }
        // ряд 2
        if ((ballTransform.position.y > 22f) && (ballTransform.position.y < 23f))
        {
            int startBrickNumber = 9;
            CheckRaw(startBrickNumber);
        }
        // ряд 1
        if ((ballTransform.position.y > 23f) && (ballTransform.position.y < 24f))
        {
            int startBrickNumber = 0;
            CheckRaw(startBrickNumber);
        }
    }

    private void CheckRaw(int _startBrickNumber)
    {
        switch (ballTransform.position.x)
        {
            case float x when (x >= 0) && (x < 2):
                CheckBrick(_startBrickNumber);
                break;
            case float x when (x >= 2) && (x < 4):
                CheckBrick(_startBrickNumber + 1);
                break;
            case float x when (x >= 4) && (x < 6):
                CheckBrick(_startBrickNumber + 2);
                break;
            case float x when (x >= 6) && (x < 8):
                CheckBrick(_startBrickNumber + 3);
                break;
            case float x when (x >= 8) && (x < 10):
                CheckBrick(_startBrickNumber + 4);
                break;
            case float x when (x >= 10) && (x < 12):
                CheckBrick(_startBrickNumber + 5);
                break;
            case float x when (x >= 12) && (x < 14):
                CheckBrick(_startBrickNumber + 6);
                break;
            case float x when (x >= 14) && (x < 16):
                CheckBrick(_startBrickNumber + 7);
                break;
            case float x when (x >= 16) && (x < 18):
                CheckBrick(_startBrickNumber + 8);
                break;
        }
    }

    private void CheckBrick(int number)
    {
        if (gameManager.bricks[number] != null)
        {
            if (Mathf.Abs(ballTransform.position.x - gameManager.bricks[number].x)/2 <= Mathf.Abs(ballTransform.position.y - gameManager.bricks[number].y))
            {
                FrontCollide();
            }
            else
            { 
                SideCollide();
            }

            if (gameManager.bricks[number].TakeDamage())
            {
                //Debug.Log("Brick destroyed!");
                gameManager.BonusCreate(number);
                gameManager.bricks[number] = null;
                gameManager.CheckForLevelEnd();
            }
        }
    }

    private void FrontCollide(float racketEcc=0f)   // racketEcc - эксцентриситет: чем дальше от центра ударится мяч, тем больше угол отскока (дает до 45 градусов)
    {
        ballTransform.rotation = Quaternion.Euler(0f, 0f, 180f - ballTransform.rotation.eulerAngles.z - racketEcc * 45f);
        ballRotationInV3 = ballTransform.rotation * Vector3.up;                            
    }
    private void SideCollide()
    {
        ballTransform.rotation = Quaternion.Euler(0f, 0f, -ballTransform.rotation.eulerAngles.z);
        ballRotationInV3 = ballTransform.rotation * Vector3.up;                        
    }
}
