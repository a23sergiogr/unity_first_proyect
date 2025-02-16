using UnityEngine;

public class DashCooldownReductionItem : Item
{
    [SerializeField] private float cooldownReduction = 1f;

    public override void ApplyEffect(PlayerController player)
    {
        player.dashCooldown = player.dashCooldown - cooldownReduction;
    }
}
