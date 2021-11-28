using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Vector2I
{
    public int x;
    public int y;
    public float d;

    public Vector2I(int nx, int ny)
    {
        x = nx;
        y = ny;
        d = 1;
    }

    public Vector2I(int nx, int ny, float nd)
    {
        x = nx;
        y = ny;
        d = nd;
    }
}

public class InfluenceMap : GridData
{
    List<IPropagator> propagators = new List<IPropagator>();

    float[,] influences;
    float[,] influencesBuffer;
    public float Decay { get; set; }
    public float Momentum { get; set; }
    public int Width { get { return influences.GetLength(0); } }
    public int Height { get { return influences.GetLength(1); } }

    public float CoordX { get; }
    public float CoordY { get; }
    public float GetValue(int x, int y) { return influences[x, y]; }

    public InfluenceMap(int size, float decay, float momentum, float x, float y)
    {
        influences = new float[size, size];
        influencesBuffer = new float[size, size];
        Decay = decay;
        Momentum = momentum;
        CoordX = x;
        CoordY = y;
    }

    public InfluenceMap(int width, int height, float decay, float momentum, float x, float y)
    {
        influences = new float[width, height];
        influencesBuffer = new float[width, height];
        Decay = decay;
        Momentum = momentum;
        CoordX = x;
        CoordY = y;
    }

    public void SetInfluence(int x, int y, float value)
    {
        if (x < Width && y < Height)
        {
            influences[x, y] = value;
            influencesBuffer[x, y] = value;
        }
    }

    public void SetInfluence(Vector2I pos, float value)
    {
        if (pos.x < Width && pos.y < Height)
        {
            influences[pos.x, pos.y] = value;
            influencesBuffer[pos.x, pos.y] = value;
        }
    }

    public void RegisterPropagator(IPropagator p)
    {
        propagators.Add(p);
    }

    public void RemovePropagator(IPropagator p)
    {
        propagators.Remove(p);
    }

    public void Propagate()
    {
        updatePropagators();
        updatePropagation();
        updateInfluenceBuffer();
    }

    void updatePropagators()
    {
        foreach (IPropagator p in propagators)
        {
            SetInfluence(p.GridPosition, p.Value);
        }
    }

    void updatePropagation()
    {
        for (int x = 0; x < influences.GetLength(0); ++x)
        {
            for (int y = 0; y < influences.GetLength(1); ++y)
            {
                float maxInf = 0.0f;
                float minInf = 0.0f;
                Vector2I[] neighbors = GetNeighbors(x, y);
                foreach (Vector2I n in neighbors)
                {
                    float inf = influencesBuffer[n.x, n.y] * Mathf.Exp(-Decay * n.d);
                    maxInf = Mathf.Max(inf, maxInf);
                    minInf = Mathf.Min(inf, minInf);
                }

                if (Mathf.Abs(minInf) > maxInf) influences[x, y] = Mathf.Lerp(influencesBuffer[x, y], minInf, Momentum);
                else influences[x, y] = Mathf.Lerp(influencesBuffer[x, y], maxInf, Momentum);
            }
        }
    }

    void updateInfluenceBuffer()
    {
        for (int x = 0; x < influences.GetLength(0); ++x)
        {
            for (int y = 0; y < influences.GetLength(1); ++y)
            {
                influencesBuffer[x, y] = influences[x, y];
            }
        }
    }

    Vector2I[] GetNeighbors(int x, int y)
    {
        List<Vector2I> retVal = new List<Vector2I>();

        // as long as not in left edge
        if (x > 0) { retVal.Add(new Vector2I(x - 1, y)); }

        // as long as not in right edge
        if (x < influences.GetLength(0) - 1) { retVal.Add(new Vector2I(x + 1, y)); }

        // as long as not in bottom edge
        if (y > 0) { retVal.Add(new Vector2I(x, y - 1)); }

        // as long as not in upper edge
        if (y < influences.GetLength(1) - 1) { retVal.Add(new Vector2I(x, y + 1)); }


        // DIAGONALS

        // as long as not in bottom-left
        if (x > 0 && y > 0) { retVal.Add(new Vector2I(x - 1, y - 1, 1.142f)); }

        // as long as not in upper-right
        if (x < influences.GetLength(0) - 1 && y < influences.GetLength(1) - 1) { retVal.Add(new Vector2I(x + 1, y + 1, 1.142f)); }

        // as long as not in upper-left
        if (x > 0 && y < influences.GetLength(1) - 1) { retVal.Add(new Vector2I(x - 1, y + 1, 1.142f)); }

        // as long as not in bottom-right
        if (x < influences.GetLength(0) - 1 && y > 0) { retVal.Add(new Vector2I(x + 1, y - 1, 1.142f)); }

        return retVal.ToArray();
    }

    public float GetInfluenceFromPosition(Vector3 pos)
    {
        return influences[(int)Mathf.Floor(pos.x), (int)Mathf.Floor(pos.y)];
    }

    public List<Vector3> GetPositionsWithAIInfluence()
    {
        List<Vector3> positions = new List<Vector3>();
        for (int x = 0; x < influences.GetLength(0); ++x)
        {
            for (int y = 0; y < influences.GetLength(1); ++y)
            {
                if (influences[x, y] < 0) positions.Add(new Vector3(x - CoordX, y - CoordY, 0));
            }
        }
        return positions;
    }

    public Vector3 GetPositionWithMoreInfluence()
    {
        float maxInfluence = 3f;
        Vector3 posWithMaxInfluence = new Vector3(-99, -99, -99);
        for (int x = 0; x < influences.GetLength(0); x++)
        {
            for (int y = 0; y < influences.GetLength(1); y++)
            {
                if (influences[x, y] >= maxInfluence && PathfindingWithoutThreads.grid.NodeFromWorldPoint(new Vector3(x - CoordX, y - CoordY, 0)).walkable)
                {
                    maxInfluence = influences[x, y];
                    posWithMaxInfluence = new Vector3(x - CoordX, y - CoordY, 0);
                }
            }
        }
        return posWithMaxInfluence;
    }

    public Vector3 GetPositionWithLessInfluence()
    {
        float minInfluence = -3f;
        Vector3 posWithMinInfluence = new Vector3(-99, -99, -99);
        for (int x = 0; x < influences.GetLength(0); x++)
        {
            for (int y = 0; y < influences.GetLength(1); y++)
            {
                if (influences[x, y] <= minInfluence && PathfindingWithoutThreads.grid.NodeFromWorldPoint(new Vector3(x - CoordX, y - CoordY, 0)).walkable)
                {
                    minInfluence = influences[x, y];
                    posWithMinInfluence = new Vector3(x - CoordX, y - CoordY, 0);
                }
            }
        }
        return posWithMinInfluence;
    }
}