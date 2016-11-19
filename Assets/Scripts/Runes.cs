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

	public enum NodeTags //Tags to put on a Rune's nodes, so it can tell which node is which inside itself.
	{
		None,
		CenterNode,
		ShouldBe_ManaFountain,
		SpawnPointNode,
		End
	}

	public class Rune
	{
		public RuneTypes type = RuneTypes.None;
		public List<Lines.Line> lines;//Lines that are in this rune
		public List<Nodes.Node> nodes;//Nodes that are in this rune
		public bool active = false;
		public bool setColorBefore = false;
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
						timeSet = Time.timeSinceLevelLoad;
						/*
						Debug.Log("Spawned! Time: " + Time.timeSinceLevelLoad + ", TimeSet: " + timeSet + ", delay: 5");
				

						
						Nodes.Node n = Nodes.FindNearestNode(positionsOfInterest[0]);
						if (n != null)
						{
							if (n.mana == null)//If this node is clear, create mana on it.
							{
								Manas.CreateMana(n, manaType);
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
			//Is it a Quad Node?
			if (!Nodes.nodes[n].usedByRune && Nodes.nodes[n].lines.Count >= 4)
			{
				//Alright, then look for runes that include quadnodes:

				//Crowprint
				LookForCrowprint(Nodes.nodes[n]);

				LookForRuneOfHarvest(Nodes.nodes[n]);
			}
		}
	}

	public static void LookForCrowprint(Nodes.Node node)
	{
		//Debug.Log("Looking for Crowprint, Node uId: " + node.uId);
		List<Nodes.Connection> connections = new List<Nodes.Connection>();

		connections.Add(Nodes.Con(0, 0, 2, 0, 0, 0));
		connections.Add(Nodes.Con(-2, 0, 0, 0, 0, 0));
		connections.Add(Nodes.Con(2, 0, 1, 0, 0, 0));
		connections.Add(Nodes.Con(2, 0, -2, 0, 0, 0));
		connections.Add(Nodes.Con(-1, 0, -1, 0, 0, -2));
		connections.Add(Nodes.Con(0, 0, -2, -1, 0, -1));

		List<Nodes.Connection> result = Nodes.CheckRuneMulti(connections, node);

		if (result != null && result.Count > 0)
		{
			//Debug.Log("FOUND RUNE: Crowprint");

			Rune rune = new Rune();
			rune.lines = new List<Lines.Line>();
			rune.type = RuneTypes.RuneCrowprint;

			for (int i = 0; i < result.Count; i++)
			{
				rune.lines.Add(result[i].line);
			}

			//go through all lines in this rune
			for (int i = 0; i < rune.lines.Count; i++)
			{
				rune.lines[i].rune = rune;

				//go through all nodes used by this line
				for (int a = 0; a < rune.lines[i].points.Count; a++)
				{
					rune.lines[i].points[a].node.rune = rune;
					rune.lines[i].points[a].node.usedByRune = true;
				}
			}

			CheckRuneRequirements(rune);
			runes.Add(rune);
		}
	}

	public static void LookForRuneOfHarvest(Nodes.Node node)
	{
		//Debug.Log("Looking for Rune of Harvest, Node uId: " + node.uId);
		List<Nodes.Connection> connections = new List<Nodes.Connection>();

		connections.Add(Nodes.Tag(0, 0, 0, NodeTags.CenterNode));
		connections.Add(Nodes.Tag(0, 0, 1, NodeTags.ShouldBe_ManaFountain));
		connections.Add(Nodes.Tag(0, 0, 2, NodeTags.SpawnPointNode));

		connections.Add(Nodes.Con(-1, 0, 1, 0, 0, 0));
		connections.Add(Nodes.Con(1, 0, 1, 0, 0, 0));
		connections.Add(Nodes.Con(0, 0, -1, 0, 0, 0));
		connections.Add(Nodes.Con(-1, 0, 1, 0, 0, 2));
		connections.Add(Nodes.Con(1, 0, 1, 0, 0, 2));
		connections.Add(Nodes.Con(0, 0, 2, 0, 0, 0));

		List<Nodes.Connection> result = Nodes.CheckRuneMulti(connections, node);

		if (result != null && result.Count > 0)
		{
			//Debug.Log("FOUND RUNE: Rune of Harvest");

			Rune rune = new Rune();
			rune.lines = new List<Lines.Line>();
			rune.nodes = new List<Nodes.Node>();
			rune.type = RuneTypes.RuneOfHarvest;

			for (int i = 0; i < result.Count; i++)//add all the lines to rune.lines
			{
				if (result[i].tag != NodeTags.None) { continue; }//Skip it, it's a tagged node entry
				rune.lines.Add(result[i].line);
			}
			/*
			//Print result
			string s = "Result printout: \n";
			for (int i = 0; i < result.Count; i++)//Add all the nodes to rune.nodes
			{
				s += "\n" + i;
				s += ", x: " + result[i].x;
				s += ", y: " + result[i].y;
				s += ", z: " + result[i].z;
				s += ", x2: " + result[i].x2;
				s += ", y2: " + result[i].y2;
				s += ", z2: " + result[i].z2;
				s += ", n1: " + (result[i].n1 != null);
				s += ", n2: " + (result[i].n2 != null);
				s += ", line: " + (result[i].line != null);
				s += ", tag: " + result[i].tag;
			}
			Debug.Log(s);
			*/
			for (int i = 0; i < result.Count; i++)//Add all the nodes to rune.nodes
			{
				bool doubles = false;
				if (result[i].n1 != null)
				{
					for (int a = 0; a < rune.nodes.Count; a++)
					{
						if (rune.nodes[a].uId == result[i].n1.uId) { doubles = true; }
					}
					if (!doubles) { rune.nodes.Add(result[i].n1); }
				}
				if (result[i].n2 != null)
				{
					doubles = false;
					for (int a = 0; a < rune.nodes.Count; a++)
					{
						if (rune.nodes[a].uId == result[i].n2.uId) { doubles = true; }
					}
					if (!doubles) { rune.nodes.Add(result[i].n2); }
				}
			}

			//go through all lines in this rune
			for (int i = 0; i < rune.lines.Count; i++)
			{
				rune.lines[i].rune = rune;

				//go through all nodes used by this line
				for (int a = 0; a < rune.lines[i].points.Count; a++)
				{
					rune.lines[i].points[a].node.rune = rune;
					rune.lines[i].points[a].node.usedByRune = true;
				}
			}
			rune.delay = 2;
			CheckRuneRequirements(rune);
			runes.Add(rune);
		}

		/*
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
		*/
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

				//Find a node in this rune tagged as ShouldBe_ManaFountain
				r.active = false;
				for (int i = 0; i < r.nodes.Count; i++)
				{
					if (r.nodes[i].tag == NodeTags.ShouldBe_ManaFountain && r.nodes[i].specialType == Nodes.SpecialNodes.ManaFountain)
					{
						//Debug.Log("HERE");
						r.active = true;
						break;
					}
				}
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
