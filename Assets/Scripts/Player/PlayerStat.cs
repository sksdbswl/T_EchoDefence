using System;
using UnityEngine;

[Serializable]
public class PlayerStat
{
    // ---- 직렬화 필드(인스펙터 노출) + 기본값 ----
    [SerializeField] private int   health      = 100;
    [SerializeField] private int   level       = 1;
    [SerializeField] private int   weaponLevel = 1;   // 1~10 권장
    [SerializeField] private float speed       = 1f;
    [SerializeField] private int   unitCnt     = 1;   // 0 이상 권장
    [SerializeField] private int   damage      = 100;

    // ---- 변경 알림 ----
    public event Action OnChanged;
    private bool _suppressNotify = false;

    // ---- 공통 알림 헬퍼 ----
    private void RaiseChanged()
    {
        if (!_suppressNotify) OnChanged?.Invoke();
    }
    public void BeginBatch() => _suppressNotify = true;
    public void EndBatch()   { _suppressNotify = false; RaiseChanged(); }

    // ---- 프로퍼티 (값이 바뀔 때만 알림) ----
    public int Health
    {
        get => health;
        set { if (health == value) return; health = Mathf.Max(0, value); RaiseChanged(); }
    }

    public int Level
    {
        get => level;
        set { if (level == value) return; level = Mathf.Max(1, value); RaiseChanged(); }
    }

    public int WeaponLevel
    {
        get => weaponLevel;
        set
        {
            int v = Mathf.Clamp(value, 1, 10);     // 필요 시 상한 조정
            if (weaponLevel == v) return;
            weaponLevel = v;
            RaiseChanged();
        }
    }

    public float Speed
    {
        get => speed;
        set
        {
            float v = Mathf.Clamp(value, 0.1f, 50f);
            if (Mathf.Approximately(speed, v)) return;
            speed = v;
            RaiseChanged();
        }
    }

    public int UnitCnt
    {
        get => unitCnt;
        set
        {
            int v = Mathf.Max(0, value);
            if (unitCnt == v) return;
            unitCnt = v;
            RaiseChanged();
        }
    }

    public int Damage
    {
        get => damage;
        set
        {
            int v = Mathf.Max(0, value);
            if (damage == v) return;
            damage = v;
            RaiseChanged();
        }
    }

    // ---- 유틸 메서드(클램프/증감) ----
    public void AddUnits(int delta, int min = 0, int max = 999)
        => UnitCnt = Mathf.Clamp(UnitCnt + delta, min, max);

    public void AddWeaponLevels(int delta, int min = 1, int max = 10)
        => WeaponLevel = Mathf.Clamp(WeaponLevel + delta, min, max);

    public void SetWeaponLevelClamped(int lv, int min = 1, int max = 10)
        => WeaponLevel = Mathf.Clamp(lv, min, max);

    public void AddDamage(int delta)
        => Damage = Mathf.Max(0, Damage + delta);

    public void SetSpeedClamped(float v, float min = 0.1f, float max = 50f)
        => Speed = Mathf.Clamp(v, min, max);

    // ---- Damage 오버로드(기존 float 호출 대응) ----
    public void SetDamage(int v)   => Damage = v;
    public void SetDamage(float v) => Damage = Mathf.Max(0, Mathf.RoundToInt(v));

    // ✅ 주의: Unity 직렬화 특성상 생성자(initializer)보다
    // 인스펙터 값이 우선됩니다. Awake에서 new PlayerStat()로 덮지 마세요.
    // (Inspector 값을 쓰려면 Player.Awake에서 new 호출 제거)
}
