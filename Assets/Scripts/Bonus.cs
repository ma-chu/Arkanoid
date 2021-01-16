using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Bonus : MonoBehaviour
{
    private GameManager gameManager;
    private Racket racket;
    private Transform racketTransform;
    private Transform bonusTransform;


    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        racket = GameObject.FindGameObjectWithTag("Racket").GetComponent<Racket>();
        racketTransform = racket.GetComponent<Transform>();

        bonusTransform = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        bonusTransform.position += Vector3.down * gameManager.bonusSpeed * Time.deltaTime;

        if (bonusTransform.position.y < 0.5f)
        {
            if (RacketCollide())
            {
                gameManager.BonusCatched();
            }
            Destroy(this.gameObject);
        }
    }

    private bool RacketCollide()
    {
        float diff = bonusTransform.position.x - racketTransform.position.x;
        float eccentricity = Mathf.Min(diff * 2 / racket.RacketWidth, 1f);
        if (Mathf.Abs(diff) <= racket.racketWidth / 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
