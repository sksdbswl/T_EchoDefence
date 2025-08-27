using System.Collections;
using UnityEngine;

public class GrenadeDef : MonoBehaviour
{
    private bool _triggered;
    
    private void OnTriggerEnter(Collider other)
    {
        var provider = other.GetComponentInParent<IProvider>();
        if (provider == null || provider.PlayerProvider == null) return;
        
        _triggered = true;

        provider.PlayerProvider.playerStat.Grenade++;
        Destroy(gameObject);
        
        _triggered = false;
    }
}