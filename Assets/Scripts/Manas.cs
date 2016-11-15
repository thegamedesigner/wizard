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
		public Vector3 nodePos;
		public int nodeIndex = -1;
		public Nodes.Node node;
		public float timeSet;
		public float dir;
		public float momentum;//Number of grid points before stopping.
		public ManaTypes type;

	}

	public static void RegisterMana(GameObject go)
	{
		Mana mana = new Mana();
		mana.go = go;
		manas.Add(mana);
	}

	public static void CreateMana(Vector3 pos, float dir, float momentumInGridPoints, ManaTypes type)
	{
		GameObject go = Instantiate(Defines.self.manaPrefab);
		go.transform.position = pos;
		go.transform.localEulerAngles = new Vector3(0, 0, 0);

		Mana m = new Mana();
		m.go = go;
		m.dir = dir;
		m.type = type;
		m.momentum = momentumInGridPoints / xa.gridScale;//Turns momentum of 3 grid points into whatever that distance actually is in grid points, based on gridScale.

		//Reserve the node that this mana has been created on, for this mana.
		m.go.transform.SetAngY(m.dir);//Set direction
		m.nodePos = Lines.RoundPos(m.go.transform.position);//Careful to not create a mana if the node space isn't free.
		m.go.transform.Translate(-(1 / xa.gridScale), 0, 0);//Move backwards the correct amount

		int nodeIndex = Nodes.GetIndexForNodePos(m.nodePos);
		if (nodeIndex == -1)
		{
			//Then create this node
			nodeIndex = Nodes.CreateNode(m.nodePos);
		}
		m.nodeIndex = nodeIndex;
		//Then reserve this node
		Nodes.nodes[nodeIndex].mana = m;
		m.node = Nodes.nodes[nodeIndex];

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
			//draw a debug line
			Mana m = manas[i];

			Debug.DrawLine(m.go.transform.position,m.nodePos, Color.blue);

			if (m.momentum >= speed)//If it can move at least as far as speed will require of it
			{
				if (Time.timeSinceLevelLoad >= (m.timeSet + delay))
				{
					m.timeSet = Time.timeSinceLevelLoad;

					m.go.transform.SetAngY(m.dir);//Set direction

					//Am I about to switch to the next node?
					//Am I on my node pos? Then check if the next one is free
					Vector3 startPos = m.go.transform.position;
					m.go.transform.Translate(speed, 0, 0);
					Vector3 projectedPos = m.go.transform.position;
					m.go.transform.position = startPos;
					bool dontMove = false;

					if (m.nodePos == m.go.transform.position)
					//if (Vector3.Distance(manas[i].nodePos, projectedPos) > ((1 / Lines.gridScale) * 0.5f))
					{
						//Then this would move across the line and be at the next node.
						//Is it free?
						m.go.transform.position = m.nodePos;//Just to make sure, snap to that pos
						m.go.transform.Translate((1 / xa.gridScale), 0, 0);
						Vector3 nextNodePos = m.go.transform.position;
						m.go.transform.position = startPos;

						int nextNodeIndex = Nodes.GetIndexForNodePos(nextNodePos);
						if (nextNodeIndex == -1)
						{
							nextNodeIndex = Nodes.CreateNode(nextNodePos);
						}
						if (Nodes.nodes[nextNodeIndex].mana == null)
						{
							//Then switch to this node. Don't stop moving.
							Nodes.nodes[m.nodeIndex].mana = null;//Unreserve from my last node
							Nodes.nodes[nextNodeIndex].mana = m;//Reserve with my new node
							m.node = Nodes.nodes[nextNodeIndex];
							m.nodeIndex = nextNodeIndex;
							m.nodePos = Nodes.nodes[nextNodeIndex].pos;
						}
						else
						{
							//Dont move
							dontMove = true;
						}
					}

					if (!dontMove)
					{
						m.momentum -= speed;
						m.go.transform.Translate(speed, 0, 0);
					}
				}
			}
		}





	}
}
