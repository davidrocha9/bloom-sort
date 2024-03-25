using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClick;
    [SerializeField]
    private float mouseDragPhysicsSpeed = 10;
    [SerializeField]
    private float mouseDragSpeed = .1f;

    private Camera mainCamera;
    private Vector3 velocity = Vector3.zero;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private void Awake() {
        mainCamera = Camera.main;
    }

    private void OnEnable() {
        mouseClick.performed += MousePressed;
    }

    private void onDisable() {
        mouseClick.performed -= MousePressed;
        mouseClick.Disable();
    }

    private void MousePressed(InputAction.CallbackContext context) {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider != null && (hit.collider.gameObject.CompareTag("Piece"))) {
                StartCoroutine(DragUpdate(hit.collider.gameObject));
            }
        }

        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);
        if (hit2D.collider != null && hit2D.collider.gameObject.CompareTag("Piece")) {
            StartCoroutine(DragUpdate(hit2D.collider.gameObject));
        }
    }

    private IEnumerator DragUpdate(GameObject clickedObject) {
        float initialDistance = Vector3.Distance(clickedObject.transform.position, mainCamera.transform.position);
        clickedObject.TryGetComponent<Rigidbody>(out var rb);
        while (mouseClick.ReadValue<float>() != 0) {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (rb != null) {
                Vector3 direction = ray.GetPoint(initialDistance) - clickedObject.transform.position;
                rb.velocity = direction * mouseDragPhysicsSpeed;
                yield return waitForFixedUpdate;
            }
            else {
                clickedObject.transform.position = Vector3.SmoothDamp(clickedObject.transform.position, ray.GetPoint(initialDistance), ref velocity, mouseDragSpeed);
                yield return null;
            }
        }
    }
}