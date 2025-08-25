using UnityEngine;

[CreateAssetMenu(menuName="Effect/Unit")]
public class UnitEffectDef : ScriptableObject {
    public bool isMultiply;  // true=xN, false=%N
    public int value;        // 2, 3, 4 ...
}