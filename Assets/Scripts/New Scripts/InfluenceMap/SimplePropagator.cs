using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPropagator
{
	Vector2I GridPosition { get; }
	float Value { get; }
}

public class SimplePropagator : MonoBehaviour, IPropagator
{
	[SerializeField]
	float value = 0f;
	public float Value { get { return value; } }

	Vector3 bottomLeft;
	Vector3 topRight;

	public Vector2I GridPosition
	{
		get
		{
			return InfluenceMapControl.map.GetGridPosition(transform.position);
		}
	}

	void Start()
	{
		InfluenceMapControl.map.RegisterPropagator(this);
		InfluenceMapControl.map.GetMovementLimits(out bottomLeft, out topRight);
	}

	void OnDestroy()
	{
		InfluenceMapControl.map.RemovePropagator(this);
	}
}