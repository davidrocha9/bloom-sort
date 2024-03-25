using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chunk : MonoBehaviour
{
    public Image image;

    void OnValidate()
    {
        image = GetComponent<Image>();
    }
    
    private int GenerateRandomColorIndex()
    {
        return UnityEngine.Random.Range(0, Constants.AVAILABLE_COLORS.Count);
    }

    public void GenerateRandomChunk()
    {
        Color _color;

        int randomColorIndex = GenerateRandomColorIndex();

        ColorUtility.TryParseHtmlString(Constants.AVAILABLE_COLORS[randomColorIndex], out _color);

        SetColor(_color);

        gameObject.SetActive(true);
    }
    
    public void SetColor(Color color)
    {
        bool shouldBeVisible = color != Color.white;
        
        gameObject.SetActive(shouldBeVisible);
        image.color = color;
    }

    public void Clear()
    {
        image.color = Color.white;
        gameObject.SetActive(false);
    }

    public bool IsFree()
    {
        return image.color == Color.white;
    }

    public Color GetColor()
    {
        return image.color;
    }
}