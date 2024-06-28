using UnityEngine;
using UnityEngine.UI;

public class Chunk : MonoBehaviour
{
    public Image image;
    public Animator animator;

    void OnValidate()
    {
        animator = GetComponent<Animator>();
    }

    public void Reset()
    {
        animator.SetBool(AnimationConstants.ADD, false);
        animator.SetBool(AnimationConstants.IDLE, false);
        animator.SetBool(AnimationConstants.REMOVE, false);
    }

    private int GenerateRandomColorIndex()
    {
        return UnityEngine.Random.Range(0, Constants.NUMBER_OF_COLORS);
    }

    public void GenerateRandomChunk()
    {
        int randomColorIndex = GenerateRandomColorIndex();

        Color _color = Constants.AVAILABLE_COLORS[randomColorIndex];

        AddInstantly(_color);
    }

    public void SetColor(Color color)
    {
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

    // Add and Remove are used for pieces that should be animated (board pieces)
    public void Add(Color color)
    {
        SetColor(color);
        if (color != Color.white) animator.SetBool(AnimationConstants.ADD, true);
    }

    public void Remove()
    {
        animator.SetBool(AnimationConstants.REMOVE, true);
    }

    // AddInstantly and RemoveInstantly are used for pieces that should not be animated (pieces placed by the player)
    public void AddInstantly(Color color)
    {
        SetColor(color);
        animator.SetBool(AnimationConstants.IDLE, color != Color.white);
    }

    public void RemoveInstantly()
    {
        SetColor(Color.white);
        animator.SetBool(AnimationConstants.IDLE, false);
    }

    public void SetIdle(bool isIdle)
    {
        animator.SetBool(AnimationConstants.IDLE, isIdle);
    }

    public void OnChunkAdded()
    {
        animator.SetBool(AnimationConstants.ADD, false);
        animator.SetBool(AnimationConstants.IDLE, true);

        transform.parent.SendMessage(EventConstants.CHUNK_ADDED, SendMessageOptions.DontRequireReceiver);
    }

    public void OnChunkRemoved()
    {
        animator.SetBool(AnimationConstants.IDLE, false);
        animator.SetBool(AnimationConstants.ADD, false);
        animator.SetBool(AnimationConstants.REMOVE, false);

        SetColor(Color.white);
    }
}