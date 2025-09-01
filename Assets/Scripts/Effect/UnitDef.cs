using System;
using System.Xml;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class UnitDef : MonoBehaviour
{
    public int unitValue;

    [SerializeField]private GameObject Increment;
    [SerializeField]private GameObject Decrement;
    
    private void Awake()
    {
        unitValue = Random.Range(-15, 3);
        UnitDefChanged();
    }

    private void Update()
    {
        UnitDefChanged();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Player>(out var player)) return;
        
        Debug.Log($"{unitValue} 증가합니다.");
        
        GameManager.Instance.Units.ApplyDelta(unitValue); 
        Destroy(gameObject);
    }

    private void UnitDefChanged()
    {
        if (unitValue < 0)
        {
            Increment.SetActive(false);
            Decrement.SetActive(true);
        }
        else
        {
            Increment.SetActive(true);
            Decrement.SetActive(false);
        }
    }
}
