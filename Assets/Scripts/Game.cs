using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    void Awake()
    {
        Constants.CANVAS_LOCAL_SCALE = transform.localScale.x;   
    }
}
