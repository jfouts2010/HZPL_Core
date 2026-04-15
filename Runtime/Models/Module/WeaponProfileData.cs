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

    public void EnsureInitialized()
    {
        if (ID == Guid.Empty)
            ID = Guid.NewGuid();

        WeaponName = WeaponName?.Trim() ?? string.Empty;
        MinRangeKm = Mathf.Max(0f, MinRangeKm);
        MaxRangeKm = Mathf.Max(MinRangeKm, MaxRangeKm);
        Accuracy = Mathf.Clamp01(Accuracy);
        QuantityPerRelease = Mathf.Max(1, QuantityPerRelease);
        DamageByTargetKind ??= new Dictionary<WeaponTargetKind, float>();
        DamageByTargetKind = DamageByTargetKind
            .Where(pair => pair.Key != WeaponTargetKind.Unknown)
            .ToDictionary(pair => pair.Key, pair => Mathf.Clamp(pair.Value, 0f, 10f));
    }

    public float GetDamageAgainst(WeaponTargetKind targetKind)
    {
        EnsureInitialized();
        return DamageByTargetKind.TryGetValue(targetKind, out var damage) ? damage : 0f;
    }
}
