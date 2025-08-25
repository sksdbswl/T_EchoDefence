using UnityEngine;

public enum EffectType { Unit, Buff, Skill }
public enum UnitEffectKind { MultiplyX2, MultiplyX3, MultiplyX4, Percent2, Percent3, Percent4 }
public enum BuffEffectKind { BulletSpeedX2, BulletLevelUp }
public enum SkillEffectKind { Meteor }

public struct EffectId {
    public EffectType Type;
    public int Kind;  // 내부적으로는 UnitEffectKind/BuffEffectKind/SkillEffectKind 캐스팅
}
