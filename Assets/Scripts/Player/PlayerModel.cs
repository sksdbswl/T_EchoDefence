using UnityEngine;

public class PlayerModel:MonoBehaviour
{
    [SerializeField] private GameObject Hat;
    [SerializeField] private GameObject Cloth;
    [SerializeField] private GameObject Sports;
    public GameObject Rifle;
    
    public void SetUpgradeModel()
    {
        Hat.SetActive(true);
        Cloth.SetActive(true);
        Sports.SetActive(false);
    }
    
    public void SetDownModel()
    {
        Hat.SetActive(false);
        Cloth.SetActive(false);
        Sports.SetActive(true);
    }
}