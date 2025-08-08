using UnityEngine;

// Ennemi non targetable
public class EnemyNotTargetable : Enemy
{
    public override void Start()
    {
        base.Start();
        giveKnockBackForce = 8f;
        damageOnHead = 25f;
    }
}
