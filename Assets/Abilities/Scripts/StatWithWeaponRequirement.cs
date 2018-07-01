using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatWithWeaponRequirement
{
    public TaggedStatsHolder.TaggableStat stat = null;
    public WeaponRequirementType weaponRequirement = WeaponRequirementType.NoRequirement;

    public StatWithWeaponRequirement(TaggedStatsHolder.TaggableStat _stat, WeaponRequirementType _weaponRequirement)
    {
        stat = _stat;
        weaponRequirement = _weaponRequirement;
    }
}
