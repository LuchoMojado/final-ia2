using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] int _width = 1, _height = 1;
    [SerializeField] float offset = 0.1f;
    Node[,] _grid;
    [SerializeField] Node _nodePrefab;
    //GameManager gm;

    private void Start()
    {
        GenerateGrid();
        //gm = GameManager.instance;
    }

    public void PaintNodesWhite(Node start, Node end)
    {
        foreach (var item in _grid)
        {
            if (item == start || item == end) continue;
            if (item.isBlocked) continue;
            item.ChangeColor(item.Cost > 1 ? item.costColor : Color.white);
        }
    }

    private void GenerateGrid()
    {
        _grid = new Node[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                var node = Instantiate(_nodePrefab);
                _grid[x, z] = node;
                node.transform.position = new Vector3(x + x * offset, 0, z + z * offset);
                node.transform.SetParent(transform);

                node.Initialize(new Coordinates(x, z), this);
            }
        }

    }
    
    public List<Node> GetNeighbors(Coordinates coordinates)
    {
        var neighbors = new List<Node>();
        if (coordinates.z + 1 < _height) neighbors.Add(_grid[coordinates.x, coordinates.z + 1]);
        if (coordinates.x + 1 < _width) neighbors.Add(_grid[coordinates.x + 1, coordinates.z]);
        if (coordinates.z - 1 >= 0) neighbors.Add(_grid[coordinates.x, coordinates.z - 1]);
        if (coordinates.x - 1 >= 0) neighbors.Add(_grid[coordinates.x - 1, coordinates.z]);


        return neighbors;
    }

    public Coordinates GetObjectCoordinates(GameObject current)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_grid[x, y] == current)
                {
                    return new Coordinates(x, y);
                }
            }
        }
        return default;
    }
}
