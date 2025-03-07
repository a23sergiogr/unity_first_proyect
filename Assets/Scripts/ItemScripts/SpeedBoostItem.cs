using UnityEngine;

public class SpeedBoostItem : Item
{
    [SerializeField] private float speedBoostAmount = 1f;

    public override void ApplyEffect(PlayerController player)
    {
        player.speed = player.speed + speedBoostAmount;
    }
}
