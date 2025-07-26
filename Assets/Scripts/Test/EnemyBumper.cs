using UnityEngine;

// Ennemi mobile classique au sol ; se retourne aux collisions et peut transmettre une force de saut au joueur.
public class EnemyOnG : Enemy
{
    // Valeur spécifique pour ce type d’ennemi
    public override void Start()
    {
        base.Start();
        giveJumpForce = 14f;
    }
}
