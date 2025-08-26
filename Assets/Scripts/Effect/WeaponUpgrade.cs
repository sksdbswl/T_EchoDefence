using UnityEngine;

public class WeaponUpgrade : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Player>(out var player)) return;
        player.playerStat.Level++;
        
        Destroy(gameObject);
    }
}