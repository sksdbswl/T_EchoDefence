using System;
using UnityEngine;

/// <summary>
/// 플레이어의 핵심 스탯 데이터
/// - 직렬화되어 Inspector에서 기본값을 설정 가능
/// - 값 변경 시 OnChanged 이벤트 발생
/// </summary>
[Serializable]
public class PlayerStat
{
    // ---- 직렬화 필드 (기본값 Inspector 노출) ----
    [SerializeField] private int   health      = 100;
    [SerializeField] private int   level       = 1;
    //[SerializeField] private int   weaponLevel = 1;   // 1~10 권장
    [SerializeField] private float speed       = 3f;
    [SerializeField] private int   unitCnt     = 1;   // 0 이상 권장
    [SerializeField] private int   baseDamage  = 100; // 기본 공격력
    [SerializeField] private int   grenade     = 1;

    // ---- 상수 정의 (클램프 기준) ----
    private const int   MinLevel       = 1;
    private const int   MaxLevel       = 12;
    // private const int   MinWeaponLevel = 1;
    // private const int   MaxWeaponLevel = 10;
    private const float MinSpeed       = 0.1f;
    private const float MaxSpeed       = 3f;

    // ---- 변경 알림 ----
    public event Action OnChanged;
    private bool _suppressNotify = false;

    // ---- 공통 알림 유틸 ----
    private void RaiseChanged()
    {
        if (!_suppressNotify) OnChanged?.Invoke();
    }
    public void BeginBatch() => _suppressNotify = true;
    public void EndBatch()   { _suppressNotify = false; RaiseChanged(); }

    // ---- 프로퍼티 ----
    public int Health
    {
        get => health;
        set => SetValue(ref health, Mathf.Max(0, value));
    }

    public int Level
    {
        get => level;
        set => SetValue(ref level, Mathf.Clamp(value, MinLevel, MaxLevel));
    }

    // public int WeaponLevel
    // {
    //     get => weaponLevel;
    //     set => SetValue(ref weaponLevel, Mathf.Clamp(value, MinWeaponLevel, MaxWeaponLevel));
    // }

    public float Speed
    {
        get => speed;
        set => SetValue(ref speed, Mathf.Clamp(value, MinSpeed, MaxSpeed));
    }

    public int UnitCnt
    {
        get => unitCnt;
        set => SetValue(ref unitCnt, Mathf.Max(0, value));
    }

    public int BaseDamage
    {
        get => baseDamage;
        set => SetValue(ref baseDamage, Mathf.Max(0, value));
    }

    /// <summary>
    /// 실제 최종 데미지 (무기 레벨에 따라 배율 적용)
    /// </summary>
    public int Damage => baseDamage * level;

    public int Grenade
    {
        get => grenade;
        set => SetValue(ref grenade, Mathf.Max(0, value));
    }

    // ---- 공통 Setter 유틸 ----
    private void SetValue<T>(ref T field, T newValue) where T : IEquatable<T>
    {
        if (field.Equals(newValue)) return;
        field = newValue;
        RaiseChanged();
    }

    // ---- 유틸 메서드 ----
    public void AddHealth(int delta)   => Health    = Mathf.Max(0, Health + delta);
    public void AddWeaponLevel(int d)  => level = Mathf.Clamp(level + d, MinLevel, MaxLevel);
    public void AddUnit(int delta)     => UnitCnt   = Mathf.Max(0, UnitCnt + delta);
    public void AddGrenade(int delta)  => Grenade   = Mathf.Max(0, Grenade + delta);
}
