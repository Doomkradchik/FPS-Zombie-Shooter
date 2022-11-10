using UnityEngine;

public class BarrelFuel : MonoBehaviour
{
    [SerializeField]
    private float _radius;

    [SerializeField]
    private float _forceMagnitude;

    [SerializeField]
    private GameObject _explosionEffect;


    private float _damage = 100f;

    [ContextMenu("Explode")]
    public void Explode()
    {
        var colliders = Physics.OverlapSphere(transform.position, _radius);
        foreach(var collider in colliders)
        {
            var direction = collider.transform.position - transform.position;
            direction.Normalize();
            var zombie = collider.GetComponentInParent<Zombie>();
            if (zombie == null)
                continue;

            zombie.TryHit(_damage, collider, direction * _forceMagnitude);
        }

        Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
