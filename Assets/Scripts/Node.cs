using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Coordinates coordinates;
    List<Node> _neighbors;
    Grid _grid;

    Renderer _renderer;

    [HideInInspector] public Arrow arrow;

    Color _baseColor;

    public Transform characterPos, othersPos;

    public List<Node> Neighbors
    {
        get
        {
            if (_neighbors == null)
            {
                _neighbors = _grid.GetNeighbors(coordinates);
            }

            return _neighbors;
        }
    }

    public bool isBlocked, isTaken;

    public void Initialize(Coordinates coords, Grid grid)
    {
        coordinates = coords;
        _grid = grid;
        _renderer = GetComponent<Renderer>();
        _baseColor = _renderer.material.color;
    }

    public void SetBlock(bool block)
    {
        isBlocked = block;
        ChangeColor(block ? Color.black : _baseColor);
    }

    public void ChangeColor(Color color)
    {
        _renderer.material.color = color;
    }
}
