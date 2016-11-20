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
		PassToNode,
		ConveyanceNode1,
		ConveyanceNode2,
		ConveyanceNode3,
		ConveyanceNode4,
		ConveyanceNode5,
		ConveyanceNode6,
		End
	}

	public class NodeAndTag
	{
		public Nodes.Node node = null;
		public NodeTags tag = NodeTags.None;
	}

	public class Rune
	{
		public RuneTypes type = RuneTypes.None;
		public List<Lines.Line> lines = new List<Lines.Line>();//Lines that are in this rune
		public List<NodeAndTag> nodes = new List<NodeAndTag>();//Nodes that are in this rune
		public bool active = false;
		public bool setColorBefore = false;
		public float timeSet = 0;
		public float delay = 0;
		public Manas.ManaTypes manaType;
		public bool dead = false;

		public void SetRuneColor(Color c1)
		{
			setColorBefore = true;
			for (int i = 0; i < lines.Count; i++)
			{
				Lines.SetLineColor(lines[i].uId, c1);
			}
		}

		public Nodes.Node GetNodeForTag(NodeTags tag)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				if (nodes[i].tag == tag) { return nodes[i].node; }
			}
			return null;
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
			if (!Nodes.nodes[n].usedByRune && Nodes.nodes[n].lines.Count >= 3)
			{
				//Alright, then look for runes that include quadnodes:

				//Momentum
				LookForRuneOfMomentum(Nodes.nodes[n]);

				//Harvest
				LookForRuneOfHarvest(Nodes.nodes[n]);
			}

			//Is it a Quad Node?
			if (!Nodes.nodes[n].usedByRune && Nodes.nodes[n].lines.Count >= 4)
			{
				//Alright, then look for runes that include quadnodes:

				//Crowprint
				LookForCrowprint(Nodes.nodes[n]);

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
			rune.type = RuneTypes.RuneCrowprint;
			InitRune(rune, result);
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
		connections.Add(Nodes.Tag(0, 0, -1, NodeTags.SpawnPointNode));
		connections.Add(Nodes.Tag(0, 0, -2, NodeTags.PassToNode));

		connections.Add(Nodes.Con(-1, 0, 1, 0, 0, 0));
		connections.Add(Nodes.Con(1, 0, 1, 0, 0, 0));
		connections.Add(Nodes.Con(0, 0, -1, 0, 0, 0));

		connections.Add(Nodes.Con(-1, 0, 1, 0, 0, 2));
		connections.Add(Nodes.Con(1, 0, 1, 0, 0, 2));

		List<Nodes.Connection> result = Nodes.CheckRuneMulti(connections, node);

		if (result != null && result.Count > 0)
		{
			//Debug.Log("FOUND RUNE: Rune of Harvest");

			Rune rune = new Rune();
			rune.type = RuneTypes.RuneOfHarvest;
			InitRune(rune, result);
			rune.delay = 2;
			CheckRuneRequirements(rune);
			runes.Add(rune);
		}
	}

	public static void LookForRuneOfMomentum(Nodes.Node node)
	{
		//Debug.Log("Looking for Rune of Harvest, Node uId: " + node.uId);
		List<Nodes.Connection> connections = new List<Nodes.Connection>();

		connections.Add(Nodes.Tag(0, 0, 0, NodeTags.CenterNode));
		connections.Add(Nodes.Tag(0, 0, -1, NodeTags.ConveyanceNode1));
		connections.Add(Nodes.Tag(0, 0, 1, NodeTags.ConveyanceNode2));
		connections.Add(Nodes.Tag(0, 0, 2, NodeTags.ConveyanceNode3));
		connections.Add(Nodes.Tag(0, 0, 3, NodeTags.ConveyanceNode4));
		connections.Add(Nodes.Tag(0, 0, 4, NodeTags.ConveyanceNode5));
		connections.Add(Nodes.Tag(0, 0, 5, NodeTags.ConveyanceNode6));

		connections.Add(Nodes.Con(-1, 0, -1, 0, 0, 0));
		connections.Add(Nodes.Con(1, 0, -1, 0, 0, 0));
		connections.Add(Nodes.Con(0, 0, 2, 0, 0, 0));
		connections.Add(Nodes.Con(0, 0, 2, 1, 0, 2));
		connections.Add(Nodes.Con(1, 0, 2, 0, 0, 4));
		connections.Add(Nodes.Con(0, 0, 4, -1, 0, 2));

		List<Nodes.Connection> result = Nodes.CheckRuneMulti(connections, node);

		if (result != null && result.Count > 0)
		{
			//Debug.Log("FOUND RUNE: Rune of Harvest");

			Rune rune = new Rune();
			rune.type = RuneTypes.RuneOfMomentum;
			InitRune(rune, result);
			CheckRuneRequirements(rune);
			runes.Add(rune);
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
				HandleRuneAction(runes[i]);
			}
		}
	}

	public static void InitRune(Rune rune, List<Nodes.Connection> result)
	{
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
			NodeAndTag nodeAndTag;

			if (result[i].n1 != null)
			{
				for (int a = 0; a < rune.nodes.Count; a++)
				{
					if (rune.nodes[a].node.uId == result[i].n1.uId) { doubles = true; }
				}
				if (!doubles)
				{
					nodeAndTag = new NodeAndTag();
					nodeAndTag.node = result[i].n1;
					nodeAndTag.tag = nodeAndTag.node.tag;
					rune.nodes.Add(nodeAndTag);
				}
			}
			if (result[i].n2 != null)
			{
				doubles = false;
				for (int a = 0; a < rune.nodes.Count; a++)
				{
					if (rune.nodes[a].node.uId == result[i].n2.uId) { doubles = true; }
				}
				if (!doubles)
				{
					nodeAndTag = new NodeAndTag();
					nodeAndTag.node = result[i].n2;
					nodeAndTag.tag = nodeAndTag.node.tag;
					rune.nodes.Add(nodeAndTag);
				}
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
					if (r.nodes[i].tag == NodeTags.ShouldBe_ManaFountain && r.nodes[i].node.specialType == Nodes.SpecialNodes.ManaFountain)
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

	public static void HandleRuneAction(Rune rune)
	{
		switch (rune.type)
		{
			case RuneTypes.RuneOfHarvest:
				if (Time.timeSinceLevelLoad >= (rune.timeSet + 5))
				{
					rune.timeSet = Time.timeSinceLevelLoad;

					Nodes.Node n = rune.GetNodeForTag(NodeTags.SpawnPointNode);
					if (n != null)
					{
						if (n.mana == null)//If this node is clear, create mana on it.
						{
							Debug.Log("Spawned! Time: " + Time.timeSinceLevelLoad + ", TimeSet: " + rune.timeSet + ", delay: 5");
							Manas.CreateMana(n, rune.manaType);
						}
					}
				}

				//Push mana
				Nodes.Node n1 = null;
				Nodes.Node n2 = null;
				n1 = rune.GetNodeForTag(NodeTags.SpawnPointNode);
				n2 = rune.GetNodeForTag(NodeTags.PassToNode);
				PushMana(n1, n2);

				break;
			case RuneTypes.RuneOfMomentum:

				//Push mana
				PushMana(rune.GetNodeForTag(NodeTags.ConveyanceNode1), rune.GetNodeForTag(NodeTags.CenterNode));
				PushMana(rune.GetNodeForTag(NodeTags.CenterNode), rune.GetNodeForTag(NodeTags.ConveyanceNode2));
				PushMana(rune.GetNodeForTag(NodeTags.ConveyanceNode2), rune.GetNodeForTag(NodeTags.ConveyanceNode3));
				PushMana(rune.GetNodeForTag(NodeTags.ConveyanceNode3), rune.GetNodeForTag(NodeTags.ConveyanceNode4));
				PushMana(rune.GetNodeForTag(NodeTags.ConveyanceNode4), rune.GetNodeForTag(NodeTags.ConveyanceNode5));
				PushMana(rune.GetNodeForTag(NodeTags.ConveyanceNode5), rune.GetNodeForTag(NodeTags.ConveyanceNode6));

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

	public static void PushMana(Nodes.Node nodeFrom, Nodes.Node nodeTo)
	{
		//If there is a mana on the first node, and no mana on the second node, it will move it.

		if (nodeFrom == null) { return; }
		if (nodeTo == null) { return; }

		if (nodeFrom.mana == null) { return; }
		if (nodeTo.mana != null) { return; }

		if (Time.timeSinceLevelLoad >= (nodeFrom.mana.timeSet + 1))
		{
			Manas.Mana mana = nodeFrom.mana;
			mana.timeSet = Time.timeSinceLevelLoad;
			mana.go.transform.position = nodeTo.pos;
			mana.node = nodeTo;
			nodeFrom.mana = null;
			nodeTo.mana = mana;

		}
	}

}
