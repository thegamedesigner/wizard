using UnityEngine;
using System.Collections;

public class ManaFountainScript : MonoBehaviour
{
	public GameObject manaPrefab;
	public GameObject spawnPoint;
	public float delayInSeconds = 0;

	float timeSet;

	void Start()
	{
		timeSet = Time.timeSinceLevelLoad;

		//register on my node
		Nodes.Node node = Nodes.FindNearestNode(transform.position);
		Nodes.RegisterSpecialNode(node, Nodes.SpecialNodes.ManaFountain);
	}

	void Update()
	{

		if (Time.timeSinceLevelLoad >= (timeSet + delayInSeconds))
		{
			timeSet = Time.timeSinceLevelLoad;
			//GameObject go = (GameObject)Instantiate(manaPrefab,spawnPoint.transform.position,spawnPoint.transform.rotation);
		}
	}
}
