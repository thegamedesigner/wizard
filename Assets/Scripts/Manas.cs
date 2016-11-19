using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manas : MonoBehaviour
{
	public static List<Mana> manas = new List<Mana>();

	public enum ManaTypes
	{
		None,
		Basic,
		Joy,
		Anger,
		Glory,
		Pride,
		Vengeance,
		IronOre,
		Tears,
		Love,
		Doom,
		Luck,

		End
	}

	public class Mana
	{
		public GameObject go;
		public Nodes.Node node;
		public ManaTypes type;
		public int uId = -1;

	}

	public static void RegisterMana(GameObject go)
	{
		Mana mana = new Mana();
		mana.go = go;
		manas.Add(mana);
	}

	public static void CreateMana(Nodes.Node n, ManaTypes type)
	{
		GameObject go = Instantiate(Defines.self.manaPrefab);
		go.transform.position = n.pos;
		go.transform.localEulerAngles = new Vector3(0, 0, 0);

		Mana m = new Mana();
		m.go = go;
		m.type = type;
		xa.uIds ++;
		m.uId = xa.uIds;
		n.mana = m;
		m.node = n;

		manas.Add(m);
	}

	public static void InitMana()
	{
		manas = new List<Mana>();
	}

	public static void UpdateMana()
	{
		float distBetween2GridPoints = 1 / xa.gridScale;
		float timeBetween2GridPoints = 1;//speed in seconds to go between 2 grid points
		float hops = 15;//Move between grid points in X hops. Can't be higher than the lowest expected frames (Because, if it gets lower, it won't adjust. It doesn't use time.delay)
		float delay = timeBetween2GridPoints / hops;
		float speed = distBetween2GridPoints / hops;

		for (int i = 0; i < manas.Count; i++)
		{
			/*
			//draw a debug line
			Mana m = manas[i];

			Debug.DrawLine(m.go.transform.position,m.nodePos, Color.blue);

			if (m.momentum >= speed)//If it can move at least as far as speed will require of it
			{
				if (Time.timeSinceLevelLoad >= (m.timeSet + delay))
				{
					m.timeSet = Time.timeSinceLevelLoad;

					m.go.transform.SetAngY(m.dir);//Set direction

				}
			}*/
		}
	}
}
