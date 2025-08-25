public class BoostShoot : Bonus
{
    public override void ActivateBonus(
        Player player,
        Friend friend,
        float multiplier,
        float duration
    )
    {
        base.ActivateBonus(player, friend, multiplier, duration);
        friend.ActivateBoostShoot(multiplier, duration);
    }
}
