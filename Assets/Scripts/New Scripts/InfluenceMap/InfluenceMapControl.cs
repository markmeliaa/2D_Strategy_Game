using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMapControl : MonoBehaviour
{
	public static InfluenceMapControl map;

	[SerializeField]
	Transform bottomLeft = null;

	[SerializeField]
	Transform upperRight = null;

	[SerializeField]
	float gridSize = 0f;

	[SerializeField]
	float decay = 0.3f;

	[SerializeField]
	float momentum = 0.8f;

	[SerializeField]
	int updateFrequency = 3;

	public static InfluenceMap influenceMap;

	[SerializeField]
	GridDisplay display = null;

	public float x;
	public float y;

	void createMap()
	{
		int width = (int)(Mathf.Abs(upperRight.position.x - bottomLeft.position.x) / gridSize);
		int height = (int)(Mathf.Abs(upperRight.position.y - bottomLeft.position.y) / gridSize);

		influenceMap = new InfluenceMap(width, height, decay, momentum, x, y);

		display.SetGridData(influenceMap);
		display.CreateMesh(bottomLeft.position, gridSize);
	}

	public void RegisterPropagator(IPropagator p)
	{
		influenceMap.RegisterPropagator(p);
	}

	public void RemovePropagator(IPropagator p)
	{
		influenceMap.RemovePropagator(p);
	}

	public Vector2I GetGridPosition(Vector3 pos)
	{
		int x = (int)((pos.x - bottomLeft.position.x) / gridSize);
		int y = (int)((pos.y - bottomLeft.position.y) / gridSize);

		return new Vector2I(x, y);
	}

	public void GetMovementLimits(out Vector3 bl, out Vector3 tr)
	{
		bl = bottomLeft.position;
		tr = upperRight.position;
	}

	void Awake()
	{
		map = this;
		createMap();
		InvokeRepeating("propagationUpdate", 0.001f, 1.0f / updateFrequency);
	}

	void propagationUpdate()
	{
		influenceMap.Propagate();
	}

	void setInfluence(int x, int y, float value)
	{
		influenceMap.SetInfluence(x, y, value);
	}

	void setInfluence(Vector2I pos, float value)
	{
		influenceMap.SetInfluence(pos, value);
	}

	void Update()
	{
		influenceMap.Decay = decay;
		influenceMap.Momentum = momentum;

		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit mouseHit;
		if (Physics.Raycast(mouseRay, out mouseHit) && Input.GetMouseButton(0) || Input.GetMouseButton(1))
		{
			Vector3 hit = mouseHit.point;
			if (hit.x > bottomLeft.position.x && hit.x < upperRight.position.x && hit.z > bottomLeft.position.y && hit.z < upperRight.position.y)
			{
				Vector2I gridPos = GetGridPosition(hit);

				if (gridPos.x < influenceMap.Width && gridPos.y < influenceMap.Height)
				{
					setInfluence(gridPos, (Input.GetMouseButton(0) ? 1.0f : -1.0f));
				}
			}
		}
	}
}