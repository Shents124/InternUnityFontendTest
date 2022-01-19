using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAI.PathFinding;
using System;

namespace Procedural
{

    // The directions type for maze propagation.

    public enum Directions
    {
        UP, RIGHT, DOWN, LEFT, NONE
    }

    // A cell in the maze
    public class Cell : Node<Vector2Int>
    {
        public int x { get { return Value.x; } }
        public int y { get { return Value.y; } }

        public bool isVisted = false;

        // Stores the direction that this specific cell connects with its neighbour cell.
        public bool[] flags = { true, true, true, true };

        // A delegate that is called when we set a direction flag to the cell
        public delegate void DelegateSetDirFlag(int x, int y, Directions dir, bool flag);
        public DelegateSetDirFlag onSetDirFlag;

        // Set direction flag for the cell
        // A direction flag shows which direction the cell opens up to
        public void SetDirFlag(Directions dir, bool flag)
        {
            flags[(int)dir] = flag;
            onSetDirFlag?.Invoke(x, y, dir, flag);
        }
        public Cell(int x, int y, Maze maze) : base(new Vector2Int(x, y))
        {
            this.maze = maze;
        }
        private Maze maze;

        public override List<Node<Vector2Int>> GetNeighbours()
        {
            List<Node<Vector2Int>> neighbours = new List<Node<Vector2Int>>();
            foreach (Directions dir in Enum.GetValues(typeof(Directions)))
            {
                int x = Value.x;
                int y = Value.y;

                switch (dir)
                {
                    case Directions.UP:
                        if (y < maze.NumRows - 1)
                        {
                            ++y;
                            if (flags[(int)dir] == false)
                                neighbours.Add(maze.GetCell(x, y));
                        }
                        break;

                    case Directions.RIGHT:
                        if (x < maze.NumRows - 1)
                        {
                            ++x;
                            if (flags[(int)dir] == false)
                                neighbours.Add(maze.GetCell(x, y));
                        }
                        break;

                    case Directions.DOWN:
                        if (y > 0)
                        {
                            --y;
                            if (flags[(int)dir] == false)
                                neighbours.Add(maze.GetCell(x, y));
                        }
                        break;

                    case Directions.LEFT:
                        if (x > 0)
                        {
                            --x;
                            if (flags[(int)dir] == false)
                                neighbours.Add(maze.GetCell(x, y));
                        }
                        break;

                    default: break;
                }       
            }
            return neighbours;
        }
    }
}

