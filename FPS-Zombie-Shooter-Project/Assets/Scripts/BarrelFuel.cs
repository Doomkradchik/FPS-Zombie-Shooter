using UnityEngine;

public class BarrelFuel : MonoBehaviour
{
    [SerializeField]
    private float _radius; 

    [SerializeField]
    private float _forceMagnitude; // power of force

    [SerializeField]
    private GameObject _explosionEffect; // particle

    private StressReceiver _shakeFX;
    private float _damage = 100f;
    private readonly float _stress = 0.8f;

    private void Start()
    {
        _shakeFX = FindObjectOfType<StressReceiver>();
    }

    [ContextMenu("Explode")]
    public void Explode()
    {
        var colliders = Physics.OverlapSphere(transform.position, _radius); // get ALL colliders which are in radius space
        foreach(var collider in colliders)
        {
            var direction = collider.transform.position - transform.position;
            direction.Normalize();
            var zombie = collider.GetComponentInParent<Zombie>();
            if (zombie == null) // the collider is not zombie
                continue;

            zombie.TryHit(_damage, collider, direction * _forceMagnitude);
        }

        _shakeFX.InduceStress(_stress);
        Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
