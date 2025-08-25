using UnityEngine;

class ObastacleExplosive : MonoBehaviour
{
    public GameObject explosionEffectPrefab;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        // TODO
        // ajoute le player dans Friend pour gérer ce cas en dessous
        // if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Friend"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (null != player)
                Explode(player);
        }
    }

    public void Explode(Player player)
    {
        float explosionRadius = 3.5f;
        float damageDealt = 300f;
        player.TakeDamage(damageDealt);
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                explosionEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            float effectScale = explosionRadius / 3f;
            effect.transform.localScale = new Vector3(effectScale, effectScale, 1f);
            Destroy(effect, 1.5f);
        }

        Destroy(gameObject);
    }
}
