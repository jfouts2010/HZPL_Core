using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public sealed class WeaponProfileData
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public string WeaponName { get; set; } = string.Empty;
    public WeaponAttackRole AttackRole { get; set; } = WeaponAttackRole.Atg;
    public float MinRangeKm { get; set; }
    public float MaxRangeKm { get; set; }
    public float Accuracy { get; set; } = 1f;
    public Dictionary<WeaponTargetKind, float> DamageByTargetKind { get; set; } =
        new Dictionary<WeaponTargetKind, float>();
    public int QuantityPerRelease { get; set; } = 1;

    public float GetDamageAgainst(WeaponTargetKind targetKind)
    {
        return DamageByTargetKind.TryGetValue(targetKind, out var damage) ? damage : 0f;
    }
}
