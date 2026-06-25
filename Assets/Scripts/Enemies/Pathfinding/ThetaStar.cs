using System;
using System.Collections.Generic;

public static class ThetaStar
{
    public static List<Node> Run(
        Node initialNode,
        Func<Node, bool> isSatisfied,
        Func<Node, List<Node>> getConnections,
        Func<Node, Node, float> getCosts,
        Func<Node, float> heuristic,
        Func<Node, Node, bool> hasLineOfSight,
        int watchDog = 1000)
    {
        List<Node> emptyPath = new List<Node>();

        if (initialNode == null)
        {
            return emptyPath;
        }

        PriorityQueue<Node> pending = new PriorityQueue<Node>();
        HashSet<Node> visited = new HashSet<Node>();
        Dictionary<Node, Node> parents = new Dictionary<Node, Node>();
        Dictionary<Node, float> costs = new Dictionary<Node, float>();

        costs[initialNode] = 0f;
        parents[initialNode] = initialNode;

        pending.Enqueue(initialNode, 0f);

        int counter = 0;

        while (!pending.IsEmpty)
        {
            counter++;

            if (counter > watchDog)
            {
                break;
            }

            Node node = pending.Dequeue();

            if (visited.Contains(node))
            {
                continue;
            }

            visited.Add(node);

            if (isSatisfied(node))
            {
                return BuildPath(node, parents);
            }

            List<Node> children = getConnections(node);

            if (children == null)
            {
                continue;
            }

            for (int i = 0; i < children.Count; i++)
            {
                Node child = children[i];

                if (child == null || visited.Contains(child))
                {
                    continue;
                }

                Node parent = parents[node];

                float currentCost;
                Node newParent;

                if (parent != node && hasLineOfSight(parent, child))
                {
                    currentCost = costs[parent] + getCosts(parent, child);
                    newParent = parent;
                }
                else
                {
                    currentCost = costs[node] + getCosts(node, child);
                    newParent = node;
                }

                if (costs.ContainsKey(child) && currentCost >= costs[child])
                {
                    continue;
                }

                costs[child] = currentCost;
                parents[child] = newParent;

                float priority = currentCost + heuristic(child);
                pending.Enqueue(child, priority);
            }
        }

        return emptyPath;
    }

    private static List<Node> BuildPath(Node endNode, Dictionary<Node, Node> parents)
    {
        List<Node> path = new List<Node>();
        Node current = endNode;

        path.Add(current);

        while (parents.ContainsKey(current) && parents[current] != current)
        {
            current = parents[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }
}