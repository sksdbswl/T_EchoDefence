using UnityEngine;

public class WeaponUpgrade : MonoBehaviour
{
    private bool _triggered;

    private void OnTriggerEnter(Collider other)
    {
        var provider = other.GetComponentInParent<IProvider>();
        if (provider == null || provider.PlayerProvider == null) return;
        
        _triggered = true;

        provider.PlayerProvider.playerStat.WeaponLevel++;
        //provider.PlayerProvider.playerStat.Damage = provider.PlayerProvider.playerStat.Damage *  provider.PlayerProvider.playerStat.WeaponLevel;
        
        Destroy(gameObject);
        
        _triggered = false;
    }
}