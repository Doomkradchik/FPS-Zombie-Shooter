using UnityEngine;

public sealed class ClientRaycaster : MonoBehaviour
{
    [SerializeField]
    private RectTransform _aimTransform;
    [SerializeField]
    private Camera _camera;

    private Vector2 AimPosition =>
          new Vector2(Screen.width, Screen.height) * 0.5f + _aimTransform.anchoredPosition;

    private Ray Ray => _camera.ScreenPointToRay(AimPosition);


    public bool TryThrowRay<T>(float distance, out T target) where T : MonoBehaviour
    {
        target = null;
        if (Physics.Raycast(Ray, out RaycastHit hit, distance) == false)
            return false;

        return hit.collider.TryGetComponent(out target);
    }

    public bool TryThrowRay(float distance, out RaycastHit target)
    {
        return Physics.Raycast(Ray, out target, distance);
    }
}
