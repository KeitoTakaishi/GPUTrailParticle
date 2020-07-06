using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JitterMotion : MonoBehaviour
{
    public float positionFrequency = 0.2f;
    public float rotationFrequency = 0.2f;

    public float positionAmount = 1.0f;
    public float rotationAmount = 30.0f;

    public Vector3 positionComponents = Vector3.one;
    public Vector3 rotationComponents = new Vector3(1, 1, 0);

    public int positionOctave = 3;
    public int rotationOctave = 3;

    float timePosition;
    float timeRotation;

    Vector2[] noiseVectors;

    Vector3 initialPosition;
    Quaternion initialRotation;

    void Awake()
    {
        timePosition = Random.value * 10;
        timeRotation = Random.value * 10;

        noiseVectors = new Vector2[6];

        for(var i = 0; i < 6; i++)
        {
            var theta = Random.value * Mathf.PI * 2;
            noiseVectors[i].Set(Mathf.Cos(theta), Mathf.Sin(theta));
        }

        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        timePosition += Time.deltaTime * positionFrequency;
        timeRotation += Time.deltaTime * rotationFrequency;

        if(positionAmount != 0.0f)
        {
            var p = new Vector3(
                Fbm(noiseVectors[0] * timePosition, positionOctave),
                Fbm(noiseVectors[1] * timePosition, positionOctave),
                Fbm(noiseVectors[2] * timePosition, positionOctave)
            );
            p = Vector3.Scale(p, positionComponents) * positionAmount * 2;
            transform.localPosition = initialPosition + p;
        }

        if(rotationAmount != 0.0f)
        {
            var r = new Vector3(
                Fbm(noiseVectors[3] * timeRotation, rotationOctave),
                Fbm(noiseVectors[4] * timeRotation, rotationOctave),
                Fbm(noiseVectors[5] * timeRotation, rotationOctave)
            );
            r = Vector3.Scale(r, rotationComponents) * rotationAmount * 2;
            transform.localRotation = Quaternion.Euler(r) * initialRotation;
        }
    }

    static float Fbm(Vector2 coord, int octave)
    {
        var f = 0.0f;
        var w = 1.0f;
        for(var i = 0; i < octave; i++)
        {
            f += w * (Mathf.PerlinNoise(coord.x, coord.y) - 0.5f);
            coord *= 2;
            w *= 0.5f;
        }
        return f;
    }
}
