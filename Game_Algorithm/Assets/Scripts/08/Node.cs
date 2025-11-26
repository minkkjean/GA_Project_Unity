using UnityEngine;

public enum TileType { Wall, Ground, Forest, Mud }

public class Node
{
    public int x, y;
    public TileType type;
    public bool isWall;
    public int cost;

    public Node parent;
    public int gCost;

    public Node(int _x, int _y, TileType _type)
    {
        x = _x;
        y = _y;
        type = _type;
        parent = null;
        gCost = int.MaxValue;

        switch (type)
        {
            case TileType.Wall:
                isWall = true;
                cost = 999;
                break;
            case TileType.Ground:
                isWall = false;
                cost = 1;
                break;
            case TileType.Forest:
                isWall = false;
                cost = 3;
                break;
            case TileType.Mud:
                isWall = false;
                cost = 5;
                break;
        }
    }
}