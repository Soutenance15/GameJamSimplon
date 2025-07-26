using UnityEngine;

// Ennemi non ciblable, hérite de Enemy, ignore les dégâts ou réagit différemment.
public class EnemyNotTargetable : Enemy
{
    public override void Start()
    {
        base.Start();
        giveJumpForce = 4f;
        type = "notTargetable";
    }
}
