using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chunk : MonoBehaviour
{
    public Image image;
    private Transform border;

    void OnValidate()
    {
        border = transform.Find("Border");
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
    }
    
    public void SetColor(Color color)
    {
        if (color == Color.white)
        {
            border.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            border.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }
        
        image.color = color;
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