using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	public Camera cam;
	public LayerMask mask;

	bool drawing = false;
	int lineId = -1;
	int lastLineId = -1;
	Vector3 lastRoundedPos;
	Vector3 startPos;

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
		if (Input.GetMouseButtonDown(0))
		{
			//Started drawing!
			drawing = true;

			//Add first point
			Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000, mask))
			{
				Vector3 point = Lines.RoundPos(hit.point);

				//check this is a valid pos
				if (Nodes.CheckValidDrawLinePos(point))
				{
					lineId = Lines.RegisterNewLine();
					startPos = point;
					lastRoundedPos = point;
					Lines.AddPoint(point, lineId);
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
					Vector3 point = Lines.RoundPos(hit.point);
					//check this is a valid pos
					if (Nodes.CheckValidDrawLinePos(point))
					{
						lastRoundedPos = point;
						Lines.AddPoint(point, lineId);
						//Debug.DrawLine(startPos, point, Color.red, 100);
						//Debug.DrawLine(ray.origin, hit.point, Color.blue, 100);
						//Debug.DrawLine(startPos, hit.point, Color.blue, 100);
					}
				}

				//Released!
				lastLineId = lineId;
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
					Vector3 point = Lines.RoundPos(hit.point);
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
