using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Runes : MonoBehaviour
{
	public static List<Rune> runes;

	public enum RuneTypes
	{
		None,
		RuneCrowprint,
		RuneGodsCompass,
		RuneOfHarvest,
		RuneOfMomentum,
		End
	}

	public class Rune
	{
		public RuneTypes type = RuneTypes.None;
		public List<Lines.Line> lines;//Lines that are in this rune
		public bool active = false;
		public bool setColorBefore = false;
		public Vector3[] positionsOfInterest;
		public float timeSet = 0;
		public float delay = 0;
		public Manas.ManaTypes manaType;
		public bool dead = false;

		public void HandleRuneAction()
		{
			switch (type)
			{
				case RuneTypes.RuneOfHarvest:
					if (Time.timeSinceLevelLoad >= (timeSet + 5))
					{
						Debug.Log("Spawned! Time: " + Time.timeSinceLevelLoad + ", TimeSet: " + timeSet + ", delay: 5");
						timeSet = Time.timeSinceLevelLoad;

						/*
						nodeIndex = Nodes.GetIndexForNodePos(positionsOfInterest[2]);
						if (nodeIndex != -1)
						{
							if (Nodes.nodes[nodeIndex].mana == null)//If this node is clear, create mana on it.
							{
								Manas.CreateMana(positionsOfInterest[2], 90, 3, manaType);
							}
						}*/

					}
					break;
				case RuneTypes.RuneOfMomentum:
					//Is there mana on me?
					/*
					nodeIndex = Nodes.GetIndexForNodePos(positionsOfInterest[0]);
					if (nodeIndex != -1)//If this node exists (it probably does, it has lines on it)
					{
						if (Nodes.nodes[nodeIndex].mana != null)
						{
							Nodes.nodes[nodeIndex].mana.momentum = 2;
						}
					}
					nodeIndex = Nodes.GetIndexForNodePos(positionsOfInterest[1]);
					if (nodeIndex != -1)//If this node exists (it probably does, it has lines on it)
					{
						if (Nodes.nodes[nodeIndex].mana != null)
						{
							Nodes.nodes[nodeIndex].mana.momentum = 2;
						}
					}*/
					break;
			}
		}

		public void InitRune()
		{
			for (int i = 0; i < lines.Count; i++)
			{
				lines[i].rune = this;
			}
		}

		public void SetRuneColor(Color c1)
		{
			setColorBefore = true;
			for (int i = 0; i < lines.Count; i++)
			{
				Lines.SetLineColor(lines[i].uId, c1);
			}
		}
	}

	public static void InitRunes()
	{
		runes = new List<Rune>();
	}

	public static void LookForRunes()
	{
		for (int n = 0; n < Nodes.nodes.Count; n++)
		{
			//Is it a double Node?
			if (Nodes.nodes[n].lines.Count >= 2)
			{
				//Alright, then look for runes that include double nodes:
				//LookForRuneOfMomentum(Nodes.nodes[n]);
			}

			//Is it a Tri Node?
			if (Nodes.nodes[n].lines.Count >= 3)
			{
				//Alright, then look for runes that include trinodes:

				//LookForRuneOfHarvest(Nodes.nodes[n]);
			}

			//Is it a Quad Node?
			if (Nodes.nodes[n].lines.Count >= 4)
			{
				//Alright, then look for runes that include quadnodes:

				//Crowprint
				//LookForCrowprint(Nodes.nodes[n]);

				//LookForGodsCompass(Nodes.nodes[n]);
			}
		}
	}
	/*
	public static void LookForRuneOfMomentum(Nodes.Node node)
	{
		Vector3 np = node.pos;
		if (Nodes.CheckConnection(node, new Vector3(np.x - (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))) &&
			Nodes.CheckConnection(n, new Vector3(np.x + (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))) &&
			Nodes.CheckConnection(new Vector3(np.x - (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale)), new Vector3(np.x + (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))))
		{
			//Found one
			Debug.Log("FOUND RUNE: Rune of Momentum");

			Rune rune = new Rune();
			rune.lines = new List<Lines.Line>();
			rune.type = RuneTypes.RuneOfMomentum;
			rune.lines.Add(Nodes.GetConnection(np, new Vector3(np.x - (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))));
			rune.lines.Add(Nodes.GetConnection(np, new Vector3(np.x + (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))));
			rune.lines.Add(Nodes.GetConnection(new Vector3(np.x - (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale)), new Vector3(np.x + (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))));


			rune.positionsOfInterest = new Vector3[2];
			rune.positionsOfInterest[0] = new Vector3(np.x, np.y, np.z);
			rune.positionsOfInterest[1] = new Vector3(np.x, np.y, np.z + (1 / xa.gridScale));
			rune.InitRune();
			CheckRuneRequirements(rune);
			runes.Add(rune);
		}
	}

	public static void LookForRuneOfHarvest(Nodes.Node node)
	{
		Vector3 np = Nodes.nodes[n].pos;
		if (Nodes.CheckConnection(n, new Vector3(np.x - (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))) &&
			Nodes.CheckConnection(n, new Vector3(np.x + (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))) &&
			Nodes.CheckConnection(new Vector3(np.x - (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale)), new Vector3(np.x, np.y, np.z + (2 / xa.gridScale))) &&
			Nodes.CheckConnection(new Vector3(np.x + (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale)), new Vector3(np.x, np.y, np.z + (2 / xa.gridScale))) &&
			Nodes.CheckConnection(n, new Vector3(np.x, np.y, np.z - (1 / xa.gridScale))))
		{
			//Found one
			Debug.Log("FOUND RUNE: Rune of Harvest");

			Rune rune = new Rune();
			rune.lines = new List<Lines.Line>();
			rune.type = RuneTypes.RuneOfHarvest;
			rune.lines.Add(Nodes.GetConnection(np, new Vector3(np.x - (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))));
			rune.lines.Add(Nodes.GetConnection(np, new Vector3(np.x + (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))));
			rune.lines.Add(Nodes.GetConnection(np, new Vector3(np.x, np.y, np.z - (1 / xa.gridScale))));
			rune.lines.Add(Nodes.GetConnection(new Vector3(np.x - (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale)), new Vector3(np.x, np.y, np.z + (2 / xa.gridScale))));
			rune.lines.Add(Nodes.GetConnection(new Vector3(np.x + (1 / xa.gridScale), np.y, np.z + (1 / xa.gridScale)), new Vector3(np.x, np.y, np.z + (2 / xa.gridScale))));

			rune.positionsOfInterest = new Vector3[3];
			rune.positionsOfInterest[0] = new Vector3(np.x, np.y, np.z + (1 / xa.gridScale));//Mana fountain pos
			rune.positionsOfInterest[1] = new Vector3(np.x, np.y, np.z);//Mana creation point
			rune.positionsOfInterest[2] = new Vector3(np.x, np.y, np.z - (1 / xa.gridScale));//Mana fountain pos
			rune.delay = 2;
			rune.InitRune();
			CheckRuneRequirements(rune);
			runes.Add(rune);

		}
	}

	public static void LookForCrowprint(Nodes.Node node)
	{
		//Does one of the lines attach to exactly 2 X to the left?
		Vector3 np = Nodes.nodes[n].pos;
		if (Nodes.CheckConnection(n, new Vector3(np.x - (2 / xa.gridScale), np.y, np.z)) &&
			Nodes.CheckConnection(n, new Vector3(np.x + (2 / xa.gridScale), np.y, np.z)) &&
			Nodes.CheckConnection(n, new Vector3(np.x, np.y, np.z + (2 / xa.gridScale))) &&
			Nodes.CheckConnection(n, new Vector3(np.x + (2 / xa.gridScale), np.y, np.z - (2 / xa.gridScale))) &&
			Nodes.CheckConnection(new Vector3(np.x - (1 / xa.gridScale), np.y, np.z - (1 / xa.gridScale)), new Vector3(np.x, np.y, np.z - (2 / xa.gridScale)))
			)
		{
			//Found one
			Debug.Log("FOUND RUNE: Crowprint");

			Rune rune = new Rune();
			rune.lines = new List<Lines.Line>();
			rune.type = RuneTypes.RuneCrowprint;
			rune.lines.Add(Nodes.GetConnection(np, new Vector3(np.x - (2 / xa.gridScale), np.y, np.z)));
			rune.lines.Add(Nodes.GetConnection(np, new Vector3(np.x + (2 / xa.gridScale), np.y, np.z)));
			rune.lines.Add(Nodes.GetConnection(np, new Vector3(np.x, np.y, np.z + (2 / xa.gridScale))));
			rune.lines.Add(Nodes.GetConnection(np, new Vector3(np.x + (2 / xa.gridScale), np.y, np.z - (2 / xa.gridScale))));
			rune.lines.Add(Nodes.GetConnection(new Vector3(np.x - (1 / xa.gridScale), np.y, np.z - (1 / xa.gridScale)), new Vector3(np.x, np.y, np.z - (2 / xa.gridScale))));
			CheckRuneRequirements(rune);
			runes.Add(rune);

		}
	}

	public static void LookForGodsCompass(Nodes.Node node)
	{
		//Does one of the lines attach to exactly 2 X to the left?
		Vector3 np = Nodes.nodes[n].pos;
		if (Nodes.CheckConnection(n, new Vector3(np.x - (3 / xa.gridScale), np.y, np.z)) &&
			Nodes.CheckConnection(n, new Vector3(np.x + (3 / xa.gridScale), np.y, np.z)) &&
			Nodes.CheckConnection(n, new Vector3(np.x, np.y, np.z - (3 / xa.gridScale))) &&
			Nodes.CheckConnection(n, new Vector3(np.x, np.y, np.z + (3 / xa.gridScale))) &&

			Nodes.CheckConnection(new Vector3(np.x - (3 / xa.gridScale), np.y, np.z), new Vector3(np.x - (2 / xa.gridScale), np.y, np.z - (1 / xa.gridScale))) &&
			Nodes.CheckConnection(new Vector3(np.x - (3 / xa.gridScale), np.y, np.z), new Vector3(np.x - (2 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))) &&

			Nodes.CheckConnection(new Vector3(np.x + (3 / xa.gridScale), np.y, np.z), new Vector3(np.x + (2 / xa.gridScale), np.y, np.z - (1 / xa.gridScale))) &&
			Nodes.CheckConnection(new Vector3(np.x + (3 / xa.gridScale), np.y, np.z), new Vector3(np.x + (2 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))) &&

			Nodes.CheckConnection(new Vector3(np.x, np.y, np.z - (3 / xa.gridScale)), new Vector3(np.x - (1 / xa.gridScale), np.y, np.z - (2 / xa.gridScale))) &&
			Nodes.CheckConnection(new Vector3(np.x, np.y, np.z - (3 / xa.gridScale)), new Vector3(np.x + (1 / xa.gridScale), np.y, np.z - (2 / xa.gridScale))) &&

			Nodes.CheckConnection(new Vector3(np.x, np.y, np.z + (3 / xa.gridScale)), new Vector3(np.x - (1 / xa.gridScale), np.y, np.z + (2 / xa.gridScale))) &&
			Nodes.CheckConnection(new Vector3(np.x, np.y, np.z + (3 / xa.gridScale)), new Vector3(np.x + (1 / xa.gridScale), np.y, np.z + (2 / xa.gridScale)))
			)
		{
			//Found one
			Debug.Log("FOUND RUNE: God's Compass");

			Rune rune = new Rune();
			rune.lines = new List<Lines.Line>();
			rune.type = RuneTypes.RuneGodsCompass;

			rune.lines.Add(Nodes.GetConnection(np, new Vector3(np.x - (3 / xa.gridScale), np.y, np.z)));
			rune.lines.Add(Nodes.GetConnection(np, new Vector3(np.x + (3 / xa.gridScale), np.y, np.z)));
			rune.lines.Add(Nodes.GetConnection(np, new Vector3(np.x, np.y, np.z - (3 / xa.gridScale))));
			rune.lines.Add(Nodes.GetConnection(np, new Vector3(np.x, np.y, np.z + (3 / xa.gridScale))));

			rune.lines.Add(Nodes.GetConnection(new Vector3(np.x - (3 / xa.gridScale), np.y, np.z), new Vector3(np.x - (2 / xa.gridScale), np.y, np.z - (1 / xa.gridScale))));
			rune.lines.Add(Nodes.GetConnection(new Vector3(np.x - (3 / xa.gridScale), np.y, np.z), new Vector3(np.x - (2 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))));

			rune.lines.Add(Nodes.GetConnection(new Vector3(np.x + (3 / xa.gridScale), np.y, np.z), new Vector3(np.x + (2 / xa.gridScale), np.y, np.z - (1 / xa.gridScale))));
			rune.lines.Add(Nodes.GetConnection(new Vector3(np.x + (3 / xa.gridScale), np.y, np.z), new Vector3(np.x + (2 / xa.gridScale), np.y, np.z + (1 / xa.gridScale))));

			rune.lines.Add(Nodes.GetConnection(new Vector3(np.x, np.y, np.z - (3 / xa.gridScale)), new Vector3(np.x - (1 / xa.gridScale), np.y, np.z - (2 / xa.gridScale))));
			rune.lines.Add(Nodes.GetConnection(new Vector3(np.x, np.y, np.z - (3 / xa.gridScale)), new Vector3(np.x + (1 / xa.gridScale), np.y, np.z - (2 / xa.gridScale))));

			rune.lines.Add(Nodes.GetConnection(new Vector3(np.x, np.y, np.z + (3 / xa.gridScale)), new Vector3(np.x - (1 / xa.gridScale), np.y, np.z + (2 / xa.gridScale))));
			rune.lines.Add(Nodes.GetConnection(new Vector3(np.x, np.y, np.z + (3 / xa.gridScale)), new Vector3(np.x + (1 / xa.gridScale), np.y, np.z + (2 / xa.gridScale))));
			CheckRuneRequirements(rune);
			runes.Add(rune);

		}
	}*/

	public static void UpdateRunes()
	{
		//Check the requirements of all runes, then perform their actions (if requirements are met)
		for (int i = 0; i < runes.Count; i++)
		{
			CheckRuneRequirements(runes[i]);

			if (runes[i].active)
			{
				//Do the thing that the rune does
				runes[i].HandleRuneAction();
			}
		}
	}

	public static void CheckRuneRequirements(Rune r)
	{
		bool oldActive = r.active;
		switch (r.type)
		{
			case RuneTypes.RuneCrowprint:
				r.active = true;
				break;
			case RuneTypes.RuneGodsCompass:
				r.active = true;
				break;
			case RuneTypes.RuneOfHarvest:
				//Requires that there is a mana fountain inside it
				/*
				int index = Nodes.GetIndexForNodePos(r.positionsOfInterest[0]);
				if (index != -1 && Nodes.nodes[index].specialType == Nodes.SpecialNodes.ManaFountain)
				{
					r.active = true;
				}
				else
				{
					r.active = false;
				}*/
					r.active = false;
				break;
			case RuneTypes.RuneOfMomentum:
				r.active = true;
				break;
		}

		if (r.active != oldActive || !r.setColorBefore)
		{
			if (r.active)
			{
				r.SetRuneColor(Defines.self.runeColor);
			}
			else
			{
				r.SetRuneColor(Defines.self.unactiveRuneColor);
			}
		}
	}

}
