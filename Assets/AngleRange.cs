using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngleRange : MonoBehaviour
{
    public Canvas canvas;
    public Image image;
    public float degreesCCWFromDownOfCentre;
    public float degreesBetweenExtremes;

    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, degreesCCWFromDownOfCentre + degreesBetweenExtremes/2);
        image.fillAmount = degreesBetweenExtremes/360f;
    }
}
