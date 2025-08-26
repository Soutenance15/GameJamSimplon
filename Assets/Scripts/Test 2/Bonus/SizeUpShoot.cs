public class SizeUpShoot : Bonus
{
    public override void ActivateBonus(
        Player player,
        Friend friend,
        float multiplier,
        float duration
    )
    {
        base.ActivateBonus(player, friend, multiplier, duration);
        friend.ActivateProjectileScaleBoost(multiplier, duration);
    }
}
