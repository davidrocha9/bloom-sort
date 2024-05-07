using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPieceEventArgs : EventArgs
{
    public float XCoord { get; }
    public float YCoord { get; }
    public List<Color> chunks { get; }
    public int index { get; }

    public DropPieceEventArgs(float _XCoord, float _YCoord, List<Color> _chunks, int _index)
    {
        XCoord = _XCoord;
        YCoord = _YCoord;
        chunks = _chunks;
        index = _index;
    }
}

public class Piece : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public event EventHandler<DropPieceEventArgs> PieceDroppedEvent;
    public Chunk[] chunks;
    private RectTransform rectTransform;
    private Vector3 startPosition;
    private int index = -1;
    public Animator animator;
    private int amountOfPiecesToBeAdded = 0;

    void OnValidate()
    {
        rectTransform = GetComponent<RectTransform>();
        animator = GetComponent<Animator>();
        UpdateChunks();
    }

    void UpdateChunks()
    {
        chunks = GetComponentsInChildren<Chunk>();
    }

    public void Init(Vector3 position)
    {
        GenerateRandomPiece();
        rectTransform.anchoredPosition = position;
    }

    public void SetIndex(int _index)
    {
        index = _index;
    }

    private void GenerateRandomPiece()
    {
        int numberOfChunks = UnityEngine.Random.Range(1, Constants.MAX_NUMBER_OF_CHUNKS);

        startPosition = rectTransform.anchoredPosition;

        for (int x = 0; x < numberOfChunks; x++)
        {
            chunks[x].GenerateRandomChunk();
        }
    }

    public void SetChunks(List<Color> _chunks)
    {
        for (int x = 0; x < _chunks.Count; x++)
        {
            chunks[x].AddInstantly(_chunks[x]);
        }

        for (int x = _chunks.Count; x < 6; x++)
        {
            chunks[x].AddInstantly(Color.white);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta * 3 * Constants.CANVAS_LOCAL_SCALE;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float XCoord = rectTransform.anchoredPosition.x;
        float YCoord = rectTransform.anchoredPosition.y;

        rectTransform.anchoredPosition = startPosition;

        List<Color> colors = GetChunksColors();

        PieceDroppedEvent?.Invoke(this, new DropPieceEventArgs(XCoord, YCoord, colors, index));
    }

    public List<Chunk> GetChunks()
    {
        return chunks.ToList();
    }

    public List<Color> GetChunksColors()
    {
        List<Color> colors = chunks.Select(chunk => chunk.image.color).ToList();
        colors.RemoveAll(color => color == Color.white);

        return colors;
    }

    public void AddChunk(Color color, int amount)
    {
        amountOfPiecesToBeAdded = amount;
        List<Chunk> targetChunks = chunks.Where(chunk => chunk.GetColor() == Color.white).ToList();

        for (int j = 0; j < amount; j++)
        {
            targetChunks[j].Add(color);
        }
    }

    public void RemoveChunk(Color color, int amount)
    {
        List<Chunk> targetChunks = chunks.Where(chunk => chunk.GetColor() == color).ToList();

        for (int j = 0; j < amount; j++)
        {
            targetChunks[j].Remove();
        }
    }

    public void ChunkAdded()
    {
        amountOfPiecesToBeAdded--;
        if (amountOfPiecesToBeAdded == 0)
        {
            transform.parent.parent.parent.SendMessage(EventConstants.ANIMATION_ENDED, SendMessageOptions.DontRequireReceiver);
        }
    }

    public bool CheckIfCanBeCleared()
    {
        List<Chunk> coloredChunks = chunks.Where(chunk => chunk.GetColor() != Color.white).ToList();

        return coloredChunks.All(chunk => chunk.GetColor() == chunks[0].GetColor()) && coloredChunks.Count == Constants.MAX_NUMBER_OF_CHUNKS;
    }

    public void RemoveInstantly()
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].RemoveInstantly();
        }
    }

    public Dictionary<Color, int> GetColorDictionary()
    {
        Dictionary<Color, int> colorsDict = new Dictionary<Color, int>();

        foreach (Chunk chunk in chunks)
        {
            Color color = chunk.GetColor();

            if (colorsDict.ContainsKey(color))
            {
                colorsDict[color]++;
            }
            else
            {
                colorsDict[color] = 1;
            }
        }

        return colorsDict;
    }

    public void Reset()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.Reset();
        }
    }
}
