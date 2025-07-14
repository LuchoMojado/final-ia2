using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    int _width, _length;
    [SerializeField] float offset = 0.1f;
    Node[,] _grid;
    [SerializeField] Node _nodePrefab;
    GameManager gm;

    private void Start()
    {
        gm = GameManager.instance;
    }

    public void GenerateGrid(int width, int length)
    {
        _width = width;
        _length = length;

        _grid = new Node[_width, _length];

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _length; z++)
            {
                var node = Instantiate(_nodePrefab);
                _grid[x, z] = node;
                node.transform.position = new Vector3(x + x * offset, 0, z + z * offset);
                node.transform.SetParent(transform);

                node.Initialize(new Coordinates(x, z), this);

                gm.allNodes.Add(node);
            }
        }

    }
    
    public List<Node> GetNeighbors(Coordinates coordinates)
    {
        var neighbors = new List<Node>();
        if (coordinates.z + 1 < _length) neighbors.Add(_grid[coordinates.x, coordinates.z + 1]);
        if (coordinates.x + 1 < _width) neighbors.Add(_grid[coordinates.x + 1, coordinates.z]);
        if (coordinates.z - 1 >= 0) neighbors.Add(_grid[coordinates.x, coordinates.z - 1]);
        if (coordinates.x - 1 >= 0) neighbors.Add(_grid[coordinates.x - 1, coordinates.z]);

        return neighbors;
    }

    public Coordinates GetObjectCoordinates(GameObject current)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _length; y++)
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
