using UnityEngine;

public class StressReceiver : MonoBehaviour 
{
    private float _trauma;
    private Vector3 _lastPosition;
    private Vector3 _lastRotation;
    public float TraumaExponent = 1;
    public Vector3 MaximumAngularShake = Vector3.one * 5;
    public Vector3 MaximumTranslationShake = Vector3.one * .75f;
    public float _stress;

    private void Update()
    {
        float shake = Mathf.Pow(_trauma, TraumaExponent);

        if(shake > 0)
        {
            var previousRotation = _lastRotation;
            var previousPosition = _lastPosition;
        
            _lastPosition = new Vector3(
                MaximumTranslationShake.x * (Mathf.PerlinNoise(0, Time.time * 25) * 2 - 1),
                MaximumTranslationShake.y * (Mathf.PerlinNoise(1, Time.time * 25) * 2 - 1),
                MaximumTranslationShake.z * (Mathf.PerlinNoise(2, Time.time * 25) * 2 - 1)
            ) * shake;

            _lastRotation = new Vector3(
                MaximumAngularShake.x * (Mathf.PerlinNoise(3, Time.time * 25) * 2 - 1),
                MaximumAngularShake.y * (Mathf.PerlinNoise(4, Time.time * 25) * 2 - 1),
                MaximumAngularShake.z * (Mathf.PerlinNoise(5, Time.time * 25) * 2 - 1)
            ) * shake;

            transform.localPosition += _lastPosition - previousPosition;
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + _lastRotation - previousRotation);
            _trauma = Mathf.Clamp01(_trauma - Time.deltaTime);
        }
        else
        {
            if (_lastPosition == Vector3.zero && _lastRotation == Vector3.zero) return;
            transform.localPosition -= _lastPosition;
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles - _lastRotation);
            _lastPosition = Vector3.zero;
            _lastRotation = Vector3.zero;
        }
    }

    
    public void InduceStress(float stress)
    {
        _trauma = Mathf.Clamp01(_trauma + stress);
    }

    public void InduceStress()
    {
        _trauma = Mathf.Clamp01(_trauma + _stress);
    }
}