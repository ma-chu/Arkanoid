using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racket : MonoBehaviour
{
	private GameManager gameManager;
	private Transform racketTransform;

	[SerializeField]
	private float racketSpeed;
	[SerializeField]
	public float racketWidth;
	public float RacketWidth
	{
		get { return racketWidth; }
		set { racketWidth = value; }
	}

	private Vector3 movement;

	private void Awake()
	{
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		racketTransform = GetComponent<Transform>();
	}
	private void OnEnable()
	{
		racketTransform.position = new Vector3(9f, 0.25f, 0f);

		racketWidth = gameManager.baseRacketWidth;
		racketTransform.localScale = new Vector3(racketWidth, 0.5f, 1f);

		racketSpeed = gameManager.racketSpeed;
	}

	void FixedUpdate()
	{
		// Если останется время, реализовать разгон ракетки до макс. скорости!!! По аналогии с выстрелом в танках

		float h = Input.GetAxisRaw("Horizontal");
		movement.Set(h, 0f, 0f);
		movement = movement.normalized * racketSpeed * Time.deltaTime;
		racketTransform.position += movement;

		if (racketTransform.position.x < 0.8f)
        {
			racketTransform.position = new Vector3(0.8f, 0.25f, 0f);
		}
		if (racketTransform.position.x > 17.2f)
		{
			racketTransform.position = new Vector3(17.2f, 0.25f, 0f);
		}
	}
}
