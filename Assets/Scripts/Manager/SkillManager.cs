using System;
using TMPro;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private Player _owner;
    
    [Header("Grenade Settings")]
    [SerializeField] private GameObject GrenadePrefab;
    [SerializeField] private GameObject GrenadeBtn;
    [SerializeField] private TMP_Text GrenadeCount;
    
    public void Init(Player owner)
    {
        _owner = owner;
    }

    private void Update()
    {
        GrenadeCount.text = _owner.playerStat.Grenade.ToString();
    }

    public void UseGrenade()
    {
        if (_owner.playerStat.Grenade < 0) return;
        
        Debug.Log("Use Grenade");
    }
}