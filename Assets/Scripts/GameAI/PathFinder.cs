using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameAI
{
    namespace PathFinding
    {
        // An enumration type to represent the status of the pathfider at any given time
        public enum PathFinderStatus
        {
            NOT_INITIALIZED,
            SUCCESS,
            FAILURE,
            RUNNING
        }

        // The Noce class. 
        // It is an abstract class that provides the base class
        // for any type of vertex that you want to implement in
        // your path finding problem.
        abstract public class Node<T>
        {
            // We store a reference to the T as Value
            public T Value { get; private set; }

            public Node(T value)
            {
                Value = value;
            }

            // Get the neighbours for this node. 
            // This is the most important function that 
            // your concrete vertex class should implement.
            abstract public List<Node<T>> GetNeighbours();
        }

        // The abstract generic PathFinder class that implements the core
        // pathfinding related codes.
        public abstract class PathFinder<T>
        {

            // Create a delegate that defines the signature
            // for calculating the cost between two 
            // Nodes (T which makes a Node)
            public delegate float CostFunction(T a, T b);
            public CostFunction HeristicCost { get; set; }
            public CostFunction NodeTraversalCost { get; set; }
            // The PathFinderNode class.
            // This class equates to a node in a the tree generated
            // by the pathfinder in its search for the most optimal
            // path. Do not confuse this with the Node class on top.
            // This class encapsulates a Node and hold other attributes
            // needed for the search traversal.
            // The pathfinder creates instances of this class at runtime
            // while doing the search.
            public class PathFiderNode
            {
                public PathFiderNode Parent { get; set; }

                // The Node that this PathFinderNode is pointing to.
                public Node<T> Location { get; private set; }

                // The various costs
                public float FCost { get; private set; }
                public float GCost { get; private set; }
                public float HCost { get; private set; }

                public PathFiderNode(Node<T> location, PathFiderNode parent, float gCost, float hCost)
                {
                    Location = location;
                    Parent = parent;
                    HCost = hCost;
                    SetGCost(gCost);

                }

                public void SetGCost(float c)
                {
                    GCost = c;
                    FCost = GCost + HCost;
                }
            }


            #region Properties

            // Add a property that holds the current status of the
            // pathfinder. By default it is set to NOT_INITIALIZED.
            // Also note that we have made the set to private to 
            // ensure that only this class can change and set
            // the status.

            public PathFinderStatus Status
            {
                get;
                private set;
            } = PathFinderStatus.NOT_INITIALIZED;

            // Add the properties for the start and goal nodes
            public Node<T> StartNode { get; private set; }
            public Node<T> GoalNode { get; private set; }

            public PathFiderNode CurrentNode { get; private set; }

            #endregion

            #region Open and Closed lists and associated functions          
            protected List<PathFiderNode> mOpenList = new List<PathFiderNode>();
            protected List<PathFiderNode> mClosedList = new List<PathFiderNode>();
            #endregion

            // A helper method to find the least node from a list
            protected PathFiderNode GetLeastCostNode(List<PathFiderNode> myList)
            {
                int best_index = 0;
                float best_priority = myList[0].FCost;
                for (int i = 0; i < myList.Count; i++)
                {
                    if (best_priority > myList[i].FCost)
                    {
                        best_priority = myList[i].FCost;
                        best_index = i;
                    }
                }

                PathFiderNode n = myList[best_index];
                return n;
            }

            // // A helper method to check if a value of T is in a list.
            // If it is then return the index of the item where the
            // value is. Otherwise return -1.
            protected int IsInList(List<PathFiderNode> myList, T cell)
            {
                for (int i = 0; i < myList.Count; i++)
                {
                    if (EqualityComparer<T>.Default.Equals(myList[i].Location.Value, cell))
                        return i;
                }

                return -1;
            }

            #region Delegates for action callbacks.

            // Some callbacks to handle on changes to the internal values.
            // these callbacks can be used by the game to display visually the
            // changes to the cells and lists.
            public delegate void DelegatePathFinderNode(PathFiderNode node);
            public DelegatePathFinderNode onChangeCurrentNode;
            public DelegatePathFinderNode onAddToOpenList;
            public DelegatePathFinderNode onAddToClosedList;
            public DelegatePathFinderNode onDestinationFound;

            public delegate void DelegateNoArgument();
            public DelegateNoArgument onStarted;
            public DelegateNoArgument onRunning;
            public DelegateNoArgument onFailure;
            public DelegateNoArgument onSuccess;
            #endregion

            // Stage 1. Initialize the serach.
            // Initialize a new search.
            // Note that a search can only be initialized if 
            // the path finder is not already running.
            public bool Initialize(Node<T> start, Node<T> goal)
            {
                if (Status == PathFinderStatus.RUNNING)
                    return false;

                Reset();

                StartNode = start;
                GoalNode = goal;

                float H = HeristicCost(StartNode.Value, GoalNode.Value);

                PathFiderNode root = new PathFiderNode(StartNode, null, 0f, H);

                mOpenList.Add(root);
                CurrentNode = root;

                onChangeCurrentNode?.Invoke(CurrentNode);
                onStarted?.Invoke();

                Status = PathFinderStatus.RUNNING;
                return true;
            }

            // Stage 2: Step until success or failure
            // Take a search step. The user must continue to call this method 
            // until the Status is either SUCCESS or FAILURE.
            public PathFinderStatus Step()
            {
                mClosedList.Add(CurrentNode);
                onAddToClosedList?.Invoke(CurrentNode);
                if (mOpenList.Count == 0)
                {
                    Status = PathFinderStatus.FAILURE;
                    onFailure?.Invoke();
                    return Status;
                }

                CurrentNode = GetLeastCostNode(mOpenList);
                onChangeCurrentNode?.Invoke(CurrentNode);
                mOpenList.Remove(CurrentNode);

                if (EqualityComparer<T>.Default.Equals(CurrentNode.Location.Value, GoalNode.Value))
                {
                    Status = PathFinderStatus.SUCCESS;
                    onDestinationFound?.Invoke(CurrentNode);
                    onSuccess?.Invoke();
                    return Status;
                }

                List<Node<T>> neighbours = CurrentNode.Location.GetNeighbours();
                foreach (Node<T> cell in neighbours)
                {
                    AlgorithmSpecificImplementation(cell);
                }

                Status = PathFinderStatus.RUNNING;
                return Status;
            }

            abstract protected void AlgorithmSpecificImplementation(Node<T> cell);

            private void Reset()
            {
                if (Status == PathFinderStatus.RUNNING)
                    return;

                CurrentNode = null;
                mOpenList.Clear();
                mClosedList.Clear();

                Status = PathFinderStatus.NOT_INITIALIZED;
            }
        }

        #region AStartFinder
        public class AStartPathFinder<T> : PathFinder<T>
        {
            protected override void AlgorithmSpecificImplementation(Node<T> cell)
            {
                // first of all check if the node is already in the closedlist.
                // if so then we do not need to continue search for this node.
                if (IsInList(mClosedList, cell.Value) == -1)
                {
                    // The cell does not exist in the closed list.

                    // Calculate the cost of the node from its parent.
                    // Remember G is the cost from the start till now.
                    // So to get G we will get the G cost of the currentNode
                    // and add the cost from currentNode to this cell.
                    // We can actually implement a function to calculate the cost 
                    // between two adjacent cells. 

                    float G = CurrentNode.GCost + NodeTraversalCost(CurrentNode.Location.Value, cell.Value);
                    float H = HeristicCost(cell.Value, GoalNode.Value);

                    int idOpenList = IsInList(mOpenList, cell.Value);
                    if(idOpenList == -1)
                    {
                        PathFiderNode n = new PathFiderNode(cell,CurrentNode, G, H);
                        mOpenList.Add(n);
                        onAddToClosedList?.Invoke(n);
                    }
                    else
                    {
                        float oldG = mOpenList[idOpenList].GCost;
                        if(G < oldG)
                        {
                            mOpenList[idOpenList].Parent = CurrentNode;
                            mOpenList[idOpenList].SetGCost(G);
                            onAddToClosedList?.Invoke(mOpenList[idOpenList]);
                        }
                    }
                }
            }
        }
        #endregion

    }
}
