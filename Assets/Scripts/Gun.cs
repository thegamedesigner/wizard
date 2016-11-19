using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun : MonoBehaviour
{
	public Camera cam;
	public LayerMask mask;
	public LayerMask mouseVsNodesMask;

	bool drawing = false;
	int lineId = -1;
	int lastLineId = -1;
	Vector3 startPos;
	int startNodeId = -1;

	void Start()
	{

	}

	void Update()
	{
		CheckErase();
		CheckUndo();
		CheckDrawLine();

	}

	void CheckDrawLine()
	{
		bool wipe = false;
		if (Input.GetMouseButtonDown(0))
		{
			//Started drawing!
			drawing = true;

			//start drawing lines
			Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000, mask))
			{
				Nodes.Node n = Nodes.FindNearestNode(hit.point);

				//check this is a valid pos
				if (Nodes.CheckValidNode(n))
				{
					lineId = Lines.RegisterNewLine();
					startPos = n.pos;
					startNodeId = n.uId;
					Lines.AddPoint(n, lineId);
				}
				else
				{
					//else, fail out
					drawing = false;
				}
			}

		}
		if (Input.GetMouseButtonUp(0))
		{
			if (drawing)
			{
				//Add last point
				Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 1000, mask))
				{
					Nodes.Node n = Nodes.FindNearestNode(hit.point);
					if (n.uId == startNodeId)
					{
						wipe = true;
					}
					else
					{
						//check this is a valid pos
						if (Nodes.CheckValidNode(n))
						{
							Lines.AddPoint(n, lineId);
						}
						else
						{
							wipe = true;
						}
					}
				}

				if(wipe)
				{
					//kill the line
					Lines.DeleteLine(lineId);
				}
				else
				{
					lastLineId = lineId;
				}
				//Released!
				drawing = false;
				lineId = -1;
			}
		}
		if (Input.GetMouseButton(0))
		{
			if (drawing)
			{
				Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 1000, mask))
				{
					Vector3 point = hit.point;
					Debug.DrawLine(startPos, point, Color.red);
				}
			}
		}
	}

	void CheckErase()
	{

		//Erase
		if (Input.GetKey(KeyCode.E))
		{
			Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000, mask))
			{
				for (int a = 0; a < Lines.lines.Count; a++)
				{
					if (Lines.lines[a].dead) { continue; }
					for (int b = 0; b < Lines.lines[a].points.Count; b++)
					{
						if (Vector3.Distance(Lines.lines[a].points[b].pos, hit.point) < 0.12f)
						{
							Lines.DeleteLine(Lines.lines[a].uId);
						}
					}
				}
			}
		}
	}

	void CheckUndo()
	{
		//Undo
		if (Input.GetKeyDown(KeyCode.Z))
		{
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				if (lastLineId != -1)
				{
					Lines.DeleteLine(lastLineId);
					lastLineId = -1;
				}
			}
		}
	}

}
