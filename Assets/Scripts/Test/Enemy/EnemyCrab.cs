using UnityEngine;

public class EnemyCrab : MonoBehaviour
{
    public Transform BatonGauche;
    public Transform BatonDroit;
    public float openAngle = 45f;
    public float closeTime = 1f;
    public float openTime = 1f;
    private bool isOpen = true;

    void Start()
    {
        OuvrePince();
        InvokeRepeating(nameof(TogglePince), openTime, openTime + closeTime);
    }

    void TogglePince()
    {
        if (isOpen)
            FermePince();
        else
            OuvrePince();
    }

    void OuvrePince()
    {
        isOpen = true;
        BatonGauche.localEulerAngles = new Vector3(0, 0, openAngle);
        BatonDroit.localEulerAngles = new Vector3(0, 0, -openAngle);
    }

    void FermePince()
    {
        isOpen = false;
        BatonGauche.localEulerAngles = Vector3.zero;
        BatonDroit.localEulerAngles = Vector3.zero;
    }
}
