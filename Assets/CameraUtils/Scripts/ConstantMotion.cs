using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMotion : MonoBehaviour
{
    public enum TransformMode
    {
        Off, XAxis, YAxis, ZAxis, Arbitrary, Random
    };

    // A class for handling each transformation.
    [System.Serializable]
    public class TransformElement
    {
        public TransformMode mode = TransformMode.Off;
        public float velocity = 1;

        // Used only in the arbitrary mode.
        public Vector3 arbitraryVector = Vector3.up;

        // Affects velocity.
        public float randomness = 0;

        // Randomizer states.
        Vector3 randomVector;
        float randomScalar;

        public void Initialize()
        {
            randomVector = Random.onUnitSphere;
            randomScalar = Random.value;
        }

        // Get a vector corresponds to the current transform mode.
        public Vector3 Vector
        {
            get {
                switch(mode)
                {
                    case TransformMode.XAxis: return Vector3.right;
                    case TransformMode.YAxis: return Vector3.up;
                    case TransformMode.ZAxis: return Vector3.forward;
                    case TransformMode.Arbitrary: return arbitraryVector;
                    case TransformMode.Random: return randomVector;
                }
                return Vector3.zero;
            }
        }

        // Get the current delta value.
        public float Delta
        {
            get {
                var scale = (1.0f - randomness * randomScalar);
                return velocity * scale * Time.deltaTime;
            }
        }
    }

    public TransformElement position = new TransformElement();
    public TransformElement rotation = new TransformElement { velocity = 30 };
    public bool useLocalCoordinate = true;

    void Awake()
    {
        position.Initialize();
        rotation.Initialize();
    }

    void Update()
    {
        if(position.mode != TransformMode.Off)
        {
            if(useLocalCoordinate)
                transform.localPosition += position.Vector * position.Delta;
            else
                transform.position += position.Vector * position.Delta;
        }

        if(rotation.mode != TransformMode.Off)
        {
            var delta = Quaternion.AngleAxis(rotation.Delta, rotation.Vector);
            if(useLocalCoordinate)
                transform.localRotation = delta * transform.localRotation;
            else
                transform.rotation = delta * transform.rotation;
        }
    }
}
