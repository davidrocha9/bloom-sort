using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckAnimationQueueEventArgs : EventArgs
{
}

public class Chunk : MonoBehaviour
{
    public Image image;
    private Transform border;
    public Animator animator;

    void OnValidate()
    {
        border = transform.Find("Border");
        animator = GetComponent<Animator>();
    }

    private int GenerateRandomColorIndex()
    {
        return UnityEngine.Random.Range(0, Constants.NUMBER_OF_COLORS);
    }

    public void GenerateRandomChunk()
    {
        int randomColorIndex = GenerateRandomColorIndex();

        Color _color = Constants.AVAILABLE_COLORS[randomColorIndex];

        //animator.SetBool("Add", true);
        animator.SetBool("Idle", true);

        SetColor(_color);
    }

    public void SetColor(Color color)
    {
        image.color = color;
        animator.SetBool("Idle", color != Color.white);
    }

    public bool IsFree()
    {
        return image.color == Color.white;
    }

    public Color GetColor()
    {
        return image.color;
    }

    public void Add(Color color)
    {
        SetColor(color);
        animator.SetBool("Idle", true);
        transform.parent.SendMessage("ChunkAdded", SendMessageOptions.DontRequireReceiver);
    }

    public void Remove()
    {
        //animator.SetBool("Remove", true);
        SetColor(Color.white);
        animator.SetBool("Idle", false);
        //animator.SetBool("Add", false);
    }

    public void OnChunkAdded()
    {
        //animator.SetBool("Add", false);
        //animator.SetBool("Idle", true);
    }

    public void OnChunkRemoved()
    {
        //animator.SetBool("Remove", false);

        SetColor(Color.white);
    }
}