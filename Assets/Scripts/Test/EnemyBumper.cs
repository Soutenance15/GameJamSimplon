using UnityEngine;

// Ennemi mobile, peut transmettre une force supérieur de saut au joueur.
public class EnemyBumper : Enemy
{
    public override void Start()
    {
        base.Start();
        giveJumpForce = 14f;
        giveKnockBackForce = 6.5f;
        maxHealth = 110f;
    }
}
