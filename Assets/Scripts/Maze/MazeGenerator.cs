using System.Collections.Generic;
using UnityEngine;
using Procedural;

public class MazeGenerator : MonoBehaviour
{
    public int rows = 13;
    public int cols = 10;

    public GameObject cellPrefab;

    public MazeCell[,] mMazeCells;

    public Maze maze { get; private set; }

    public bool MazeGenerationCompleted
    {
        get;
        private set;
    } = false;

    //The stack for backpropagation
    Stack < Cell > _stack = new Stack<Cell>();


    // Start is called before the first frame update
    void Start()
    {
        int START_X = -cols / 2;
        int START_Y = -rows / 2;

        maze = new Maze(rows, cols);
        mMazeCells = new MazeCell[cols, rows];

        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject obj = Instantiate(cellPrefab);
                obj.transform.parent = transform;
                Cell cell = maze.GetCell(i, j);
                cell.onSetDirFlag = OnCellSetDirFlag;
                obj.transform.position = new Vector3(START_X + cell.x, START_Y + cell.y, 1f);

                mMazeCells[i, j] = obj.GetComponent<MazeCell>();
                mMazeCells[i, j].Cell = cell;
            }
        }

        CreateNewMaze();
        transform.position = new Vector3(0.5f, -2f, transform.position.z);
    }

    private void CreateNewMaze()
    {
        // Push the first cell into the stack
        _stack.Push(maze.GetCell(0, 0));

        //Generate the maze 
        GenerateMaze();

    }

    public void HightlightCell(int i, int j, bool flag)
    {
        mMazeCells[i, j].SetHighlight(flag);
    }

    public void RemoveAllHightlights()
    {
        for (int i = 0; i < cols; i++)
            for (int j = 0; j < rows; j++)
                mMazeCells[i, j].SetHighlight(false);
    }


    private void GenerateMaze()
    {
        bool flag = false;
        while (flag == false)
        {
            flag = GenerateStep();
        }
        MazeGenerationCompleted = true;
    }

    private void OnCellSetDirFlag(int x, int y, Directions dir, bool flag)
    {
        mMazeCells[x, y].SetActive(dir, flag);
    }

    bool GenerateStep()
    {
        if (_stack.Count == 0) return true;

        Cell c = _stack.Peek();
        var neighbours = maze.GetNeighboursNotVisited(c.x, c.y);

        if (neighbours.Count != 0)
        {
            var index = 0;
            if (neighbours.Count > 1)
                index = UnityEngine.Random.Range(0, neighbours.Count);

            var item = neighbours[index];
            Cell neighbour = item.Item2;
            neighbour.isVisted = true;
            maze.RemoveCellWall(c.x, c.y, item.Item1);

            mMazeCells[c.x, c.y].SetHighlight(true);

            _stack.Push(neighbour);
        }
        else
        {
            _stack.Pop();
            mMazeCells[c.x, c.y].SetHighlight(false);
        }

        return false;
    }

}
