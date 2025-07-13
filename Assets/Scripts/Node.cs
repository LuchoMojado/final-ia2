using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    Coordinates _coordinates;
    List<Node> _neighbors;
    Grid _grid;

    Renderer _renderer;

    int _cost;
    public int Cost { get => _cost; }

    //string CostText { get => _textMesh.text; set => _textMesh.text = value; }
    //private TextMeshPro _textMesh;

    public Color costColor = Color.green - new Color(0, 0.3f, 0);


    public List<Node> Neighbors
    {
        get
        {
            if (_neighbors == null)
            {
                _neighbors = _grid.GetNeighbors(_coordinates);
            }

            return _neighbors;
        }
    }

    public bool isBlocked;

    public void Initialize(Coordinates coords, Grid grid)
    {
        _coordinates = coords;
        _grid = grid;
        _renderer = GetComponent<Renderer>();
        //_textMesh = GetComponentInChildren<TextMeshPro>();
        //SetCost(1);
    }

    public void SetBlock(bool block)
    {
        isBlocked = block;
        ChangeColor(block ? Color.black : Color.white);
        gameObject.layer = block ? 6 : 0;
    }

    //public void SetCost(int cost)
    //{
    //    _cost = Mathf.Clamp(cost, 1, 99);
    //    CostText = _cost.ToString();
    //    _textMesh.enabled = cost != 1;
    //    if (!isBlocked) ChangeColor(_cost == 1 ? Color.white : costColor);
    //}


    public void ChangeColor(Color color)
    {
        _renderer.material.color = color;
    }
}
