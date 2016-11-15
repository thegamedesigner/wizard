using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public static Player self;
	public static Vector3 playerPos;
	
	void Awake()
	{
		self = this;
	}

	void Start()
	{
	}

	void Update()
	{
		playerPos = transform.position;
	}
}
