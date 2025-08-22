using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform muzzlePoint;
    public Transform MuzzlePoint => muzzlePoint;
}
