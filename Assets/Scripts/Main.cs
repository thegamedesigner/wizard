using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{

	void Awake()
	{
		Defines.self = gameObject.GetComponent<Defines>();
		GameObject go = new GameObject();
		go.name = "emptyGO";
		xa.emptyGO = go;
		go = new GameObject();
		go.name = "emptyGO2";
		xa.emptyGO2 = go;

		Lines.InitLines();
		Runes.InitRunes();
		Nodes.InitNodes();
		Manas.InitMana();
	}

	void Start()
	{

	}

	void Update()
	{
		Lines.UpdateLines();
		Manas.UpdateMana();
		Runes.UpdateRunes();

		Nodes.DisplayNodeDebug();


		if (Input.GetKeyDown(KeyCode.T))
		{
			if (Time.timeScale == 1) { Time.timeScale = 0.5f; }
			else if (Time.timeScale == 0.5f) { Time.timeScale = 0.1f; }
			else if (Time.timeScale == 0.1f) { Time.timeScale = 2f; }
			else if (Time.timeScale == 2) { Time.timeScale = 5f; }
			else if (Time.timeScale == 5) { Time.timeScale = 1f; }
			Debug.Log("TimeScale: " + Time.timeScale);
		}

		Lines.ClearTheDead();

		Lines.PrintInfo();
	}
}
