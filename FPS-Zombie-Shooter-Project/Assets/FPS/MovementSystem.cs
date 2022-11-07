using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    [SerializeField]
    private Transform _head;
    [SerializeField]
    private float _unitsPerSecond;
    [SerializeField]
    private float _sensitivityX;
    [SerializeField]
    private float _sensitivityY;

    private float _currentRightAxis = 0f;

    private void Start()
    {
        _currentRightAxis = transform.localRotation.eulerAngles.y;
    }

    public void LookAt(Vector2 mouseOffset)
    {
        transform.Rotate(Vector3.up * mouseOffset.x * _sensitivityX * Time.fixedDeltaTime);
        _currentRightAxis -= mouseOffset.y * _sensitivityY * Time.fixedDeltaTime;
        _currentRightAxis = Mathf.Clamp(_currentRightAxis, -90, 90);
        _head.transform.localRotation = Quaternion.Euler(Vector3.right * _currentRightAxis);
    }

    public void Move(Vector2 direction)
    {
        var plane = new Vector3(direction.x, 0 , direction.y);
        transform.position += transform.forward* _unitsPerSecond * direction.y * Time.fixedDeltaTime;
        transform.position += transform.right * _unitsPerSecond * direction.x * Time.fixedDeltaTime;
    }
}
