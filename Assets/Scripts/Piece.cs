using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPieceEventArgs : EventArgs
{
    public float XCoord { get; }
    public float YCoord { get; }
    public Chunk[] chunks { get; }
    public int index { get; }

    public DropPieceEventArgs(float _XCoord, float _YCoord, Chunk[] _chunks, int _index)
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
    private Chunk[] chunks;
    private RectTransform rectTransform;
    private Vector3 startPosition;
    private int index = -1;
    public Animator animator;
    
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

    public void Reset()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Init", false);
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

        animator.SetBool("Init", true);
        animator.SetBool("Idle", false);

        for (int x = 0; x < numberOfChunks; x++)
        {
            chunks[x].GenerateRandomChunk();
        }

        for (int x = numberOfChunks; x < chunks.Length; x++)
        {
            chunks[x].SetColor(Color.white);
        }
    }

    public void SetChunks(Chunk[] _chunks)
    {
        for (int x = 0; x < chunks.Length; x++)
        {
            chunks[x].SetColor(_chunks[x].image.color);
        }

        animator.SetBool("Idle", true);
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

        PieceDroppedEvent?.Invoke(this, new DropPieceEventArgs(XCoord, YCoord, chunks, index));
    }

    public List<Color> GetChunksColors()
    {
        List<Color> colors = chunks.Select(chunk => chunk.image.color).ToList();
        colors.RemoveAll(color => color == Color.white);

        return colors;
    }

    public void AddChunk(Color color)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            if (chunks[i].IsFree())
            {
                chunks[i].SetColor(color);
                break;
            }
        }
    }

    public void RemoveChunk(Color color)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            if (chunks[i].GetColor() == color)
            {
                chunks[i].SetColor(Color.white);
                break;
            }
        }
    }

    public bool CheckIfCanBeCleared(Color color)
    {
        return chunks.All(chunk => chunk.GetColor() == color);
    }

    public void MakeIdle()
    {
        animator.SetBool("Init", false);
        animator.SetBool("Idle", true);
    }
}
