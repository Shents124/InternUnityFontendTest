using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Procedural;
using GameAI.PathFinding;

public class MazePathFinder : MonoBehaviour
{
    public Transform destination;
    public NPC npc;
    public MazeGenerator mazeGenerator;

    private Cell startPos;
    private Cell goalPos;
    LineRenderer pathViz;
    AStartPathFinder<Vector2Int> pathFinder = new AStartPathFinder<Vector2Int>();

    public Color COLOR_WALKABLE = new Color(49 / 255.0f, 77 / 255.0f, 121 / 255.0f, 1.0f);
    public Color COLOR_CURRENT_NODE = new Color(0.5f, 0.4f, 0.1f, 1.0f);
    public Color COLOR_ADD_TO_OPEN_LIST = new Color(0.2f, 0.7f, 0.5f, 1.0f);
    public Color COLOR_ADD_TO_CLOSED_LIST = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    public Color COLOR_ACTUAL_PATH = new Color(0.5f, 0.5f, 0.8f, 1.0f);

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForMazeGeneration());
    }

    private IEnumerator WaitForMazeGeneration()
    {
        while (mazeGenerator.MazeGenerationCompleted == false)
            yield return null;

        SetDestination();
        npc.transform.position = mazeGenerator.mMazeCells[0, mazeGenerator.rows - 1].transform.position;

        LineRenderer lr = npc.gameObject.AddComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.startColor = Color.magenta;
        lr.endColor = Color.magenta;

        pathViz = lr;
        // set the pathfinder cost functions.
        pathFinder.HeristicCost = GetManhattanCost;
        pathFinder.NodeTraversalCost = GetEuclideanCost;

        // to show pathfinding progress.
        pathFinder.onChangeCurrentNode = OnChangeCurrentNode;
        pathFinder.onAddToClosedList = OnAddToClosedList;
        pathFinder.onAddToOpenList = OnAddToOpenList;

    }

    public void OnChangeCurrentNode(PathFinder<Vector2Int>.PathFiderNode node)
    {
        // int x = node.Location.Value.x;
        // int y = node.Location.Value.y;
        // MazeCell mazeCell = mazeGenerator.mMazeCells[x, y];
        // mazeCell.SetHighColor(COLOR_CURRENT_NODE);
        // mazeCell.SetHighlight(true);
    }

    public void OnAddToOpenList(PathFinder<Vector2Int>.PathFiderNode node)
    {
        // int x = node.Location.Value.x;
        // int y = node.Location.Value.y;
        // MazeCell mazeCell = mazeGenerator.mMazeCells[x, y];
        // mazeCell.SetHighColor(COLOR_ADD_TO_OPEN_LIST);
        // mazeCell.SetHighlight(true);
    }

    public void OnAddToClosedList(PathFinder<Vector2Int>.PathFiderNode node)
    {
        // int x = node.Location.Value.x;
        // int y = node.Location.Value.y;
        // MazeCell mazeCell = mazeGenerator.mMazeCells[x, y];
        // mazeCell.SetHighColor(COLOR_ADD_TO_CLOSED_LIST);
        // mazeCell.SetHighlight(true);
    }

    private float GetManhattanCost(Vector2Int a, Vector2Int b) => GetCostBetweenTwoCells(a, b);

    public float GetEuclideanCost(Vector2Int a, Vector2Int b) => GetCostBetweenTwoCells(a, b);

    private float GetCostBetweenTwoCells(Vector2Int a, Vector2Int b)
    {
        return (mazeGenerator.mMazeCells[a.x, a.y].transform.position -
         mazeGenerator.mMazeCells[b.x, b.y].transform.position).magnitude;
    }

    private void SetDestination()
    {
        int x = UnityEngine.Random.Range(0, mazeGenerator.cols);
        int y = UnityEngine.Random.Range(0, mazeGenerator.rows);

        while (x == 0 && y == mazeGenerator.rows - 1)
        {
            x = UnityEngine.Random.Range(0, mazeGenerator.cols);
            y = UnityEngine.Random.Range(0, mazeGenerator.rows);
        }

        MazeCell mazeCell = mazeGenerator.mMazeCells[x, y];

        destination.position = mazeCell.transform.position;
        startPos = mazeGenerator.mMazeCells[0, mazeGenerator.rows - 1].Cell;
        goalPos = mazeCell.Cell;
    }

    public void MoveNPC()
    {
        npc.Init();
    }

    public void FindPath()
    {
        pathFinder.Initialize(startPos, goalPos);
        StartCoroutine(FindPathStep());
        pathViz.positionCount = 0;
        for (int i = 0; i < mazeGenerator.maze.NumCols; ++i)
        {
            for (int j = 0; j < mazeGenerator.maze.NumRows; ++j)
            {
                mazeGenerator.mMazeCells[i, j].SetHighColor(COLOR_WALKABLE);
                mazeGenerator.mMazeCells[i, j].SetHighlight(false);
            }
        }
    }

    private IEnumerator FindPathStep()
    {
      //  float x = Time.time;
        while (pathFinder.Status == PathFinderStatus.RUNNING)
        {
            pathFinder.Step();     
            yield return null;  
        }

        if (pathFinder.Status == PathFinderStatus.SUCCESS)
        {
            OnPathFound();
            //Debug.Log(Time.time - x);
        }
            
        else if (pathFinder.Status == PathFinderStatus.FAILURE)
            Debug.Log("Cannot find path to destination");  
    }
    private void OnPathFound()
    {
        PathFinder<Vector2Int>.PathFiderNode node = null;
        node = pathFinder.CurrentNode;

        List<Vector3> reverse_postitions = new List<Vector3>();

        while (node != null)
        {
            Vector3 pos = mazeGenerator.mMazeCells[node.Location.Value.x, node.Location.Value.y].transform.position;
            reverse_postitions.Add(pos);
            node = node.Parent;
        }

        LineRenderer lr = pathViz;
        lr.positionCount = reverse_postitions.Count;

        for (int i = reverse_postitions.Count - 1; i >= 0; i--)
        {
            Vector3 p = reverse_postitions[i];
            npc.AddWayPoint(new Vector2(p.x, p.y));

            lr.SetPosition(i, new Vector3(p.x, p.y, -2.0f));
        }
        startPos = goalPos;
    }
}
