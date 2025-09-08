using UnityEngine;

public class WeaponUpgrade : MonoBehaviour
{
    private bool _triggered;

    private void OnTriggerEnter(Collider other)
    {
        var provider = other.GetComponentInParent<IProvider>();
        if (provider == null || provider.PlayerProvider == null) return;
        
        _triggered = true;

        provider.PlayerProvider.playerStat.Level++;
        
        Destroy(gameObject);
        
        _triggered = false;
    }
}