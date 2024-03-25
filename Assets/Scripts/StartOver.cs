using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartOver : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}