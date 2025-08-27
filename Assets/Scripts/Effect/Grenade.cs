using System.Collections;
using UnityEngine;

public class Grenade:ThrowingWeapon
{
    public int damage = 100;                    // 데미지
    public LayerMask attackableMask;

    protected override IEnumerator Explode()
    {
        yield return new WaitForSeconds(explosiondelay);

        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius, attackableMask);

        // foreach(Collider _collider in _colliders)
        // {
        //     float _distance = Vector3.Distance(transform.position, _collider.transform.position);
        //     float _damagePersent = 1 - (_distance/explosionRadius);
        //     int _calDamage = Mathf.RoundToInt(damage * _damagePersent);
        //     _collider.GetComponent<IDamageAble>().Damaged(_calDamage);
        // }
    }
}