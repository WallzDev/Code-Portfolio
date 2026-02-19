using UnityEngine;
using System.Collections.Generic;
using GameCreator.Runtime.VisualScripting;
using System;
using System.Linq;

public class BaseEsferaElemental : MonoBehaviour
{
    public int gridWidth = 11;
    public int gridHeight = 6;

    private Vector2Int currentPos;
    public List<Vector2Int> playerPath = new List<Vector2Int>();
    private Vector2Int lastPos = Vector2Int.zero;

    [SerializeField] private List<Vector2Int> correctPath;
    [SerializeField] private Actions correctActions;
    [SerializeField] private Actions incorrectActions;
    [SerializeField] public Transform lineContainer;
    [SerializeField] public GameObject straightLine;
    [SerializeField] public GameObject currentPoint;

    [SerializeField] private Transform gridOrigin;
    [SerializeField] private float cellSize = 1f;
    private bool isOnXZPlane = false;
    [SerializeField] private Transform puzzleRoot;

    private bool IsInGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y <= 0 && pos.x < gridWidth && pos.y > gridHeight;
    }

    public void StartPuzzle()
    {
        ResetLines();
        currentPos = correctPath[0];
        currentPoint.SetActive(true);
        PlaceCurrentPoint(currentPos);
        lastPos = currentPos;
        playerPath.Clear();
        playerPath.Add(currentPos);
    }

    public void ResetPuzzle()
    {
        ResetLines();
        currentPos = correctPath[0];
        PlaceCurrentPoint(currentPos);
        lastPos = currentPos;
        playerPath.Clear();
        playerPath.Add(currentPos);
    }

    private void InputDirection(Vector2Int dir)
    {
        Vector2Int nextPos = currentPos + dir;
        if (!IsInGrid(nextPos)) return;

        lastPos = currentPos;
        playerPath.Add(nextPos);
        currentPos = nextPos;
        PlaceCurrentPoint(currentPos);
        DrawLine(lastPos, currentPos);

        CheckPath();
    }

    private void CheckPath()
    {
        if (playerPath.SequenceEqual(correctPath))
        {
            correctActions.Invoke();
        }
    }

    public void Direction(string direction)
    {
        if (direction == "Up")
        {
            InputDirection(new Vector2Int(0, 1));
        }
        else if (direction == "Down")
        {
            InputDirection(new Vector2Int(0, -1));
        }
        else if (direction == "Left")
        {
            InputDirection(new Vector2Int(-1, 0));
        }
        else if (direction == "Right")
        {
            InputDirection(new Vector2Int(1, 0));
        }
    }

    private void DrawLine(Vector2Int from, Vector2Int to)
    {
        Vector3 fromWorld = GridToWorld(from);
        Vector3 toWorld = GridToWorld(to);

        Vector3 fromLocal = puzzleRoot.InverseTransformPoint(fromWorld);
        Vector3 toLocal = puzzleRoot.InverseTransformPoint(toWorld);
        Vector3 midpoint = (fromLocal + toLocal) / 2f;

        Vector3 direction = toLocal - fromLocal;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);

        GameObject piece = Instantiate(straightLine, lineContainer);
        piece.transform.localPosition = midpoint;
        piece.transform.localRotation = rotation;

        float length = direction.magnitude;
        piece.transform.localScale = new Vector3(0.05f, length + .04f, .07f);
    }

    private void PlaceCurrentPoint(Vector2Int current)
    {
        currentPoint.transform.position = GridToWorld(current);
    }

    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        Vector3 localOffset;

        if (!isOnXZPlane)
            localOffset = new Vector3(-gridPos.x * cellSize, gridPos.y * cellSize, 0f);
        else
            localOffset = new Vector3(-gridPos.x * cellSize, 0f, gridPos.y * cellSize);

        return gridOrigin.TransformPoint(localOffset);
    }


    public void ResetLines()
    {
        foreach (Transform child in lineContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
