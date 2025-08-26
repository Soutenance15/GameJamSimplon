using UnityEngine;

public class CoinCycle : MonoBehaviour
{
    public Transform coinTop;
    public Transform coinBottom;
    public float amplitudeX = 1.25f; // Déplacement latéral
    public float amplitudeY = 1.25f; // Déplacement vertical
    public float dureeMouvement = 2.0f;
    public float dureePause = 1.5f;

    private Vector3 basePos1;
    private Vector3 basePos2;
    private float timer = 0.0f;
    private int etape = 0; // 0=aller, 1=pause1, 2=retour, 3=pause2

    void Start()
    {
        basePos1 = coinTop.localPosition;
        basePos2 = coinBottom.localPosition;
    }

    void Update()
    {
        timer += Time.deltaTime;
        switch (etape)
        {
            case 0:
                // Mouvement aller (Coin1: droite+haut, Coin2: gauche+bas)
                float t1 = Mathf.Clamp01(timer / dureeMouvement);
                coinTop.localPosition = Vector3.Lerp(
                    basePos1,
                    basePos1 + new Vector3(amplitudeX, amplitudeY, 0f),
                    t1
                );
                coinBottom.localPosition = Vector3.Lerp(
                    basePos2,
                    basePos2 + new Vector3(-amplitudeX, -amplitudeY, 0f),
                    t1
                );
                if (timer >= dureeMouvement) { timer = 0; etape = 1; }
                break;
            case 1:
                // Pause à l’extrémité
                coinTop.localPosition = basePos1 + new Vector3(amplitudeX, amplitudeY, 0f);
                coinBottom.localPosition = basePos2 + new Vector3(-amplitudeX, -amplitudeY, 0f);
                if (timer >= dureePause) { timer = 0; etape = 2; }
                break;
            case 2:
                // Mouvement retour
                float t2 = Mathf.Clamp01(timer / dureeMouvement);
                coinTop.localPosition = Vector3.Lerp(
                    basePos1 + new Vector3(amplitudeX, amplitudeY, 0f),
                    basePos1,
                    t2
                );
                coinBottom.localPosition = Vector3.Lerp(
                    basePos2 + new Vector3(-amplitudeX, -amplitudeY, 0f),
                    basePos2,
                    t2
                );
                if (timer >= dureeMouvement) { timer = 0; etape = 3; }
                break;
            case 3:
                // Pause à la position de base
                coinTop.localPosition = basePos1;
                coinBottom.localPosition = basePos2;
                if (timer >= dureePause) { timer = 0; etape = 0; }
                break;
        }
    }
}
