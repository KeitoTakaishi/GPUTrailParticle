using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    #region private data
    [SerializeField] bool isMidiIn;
    Camera camera;
    [SerializeField] public Transform target;
    [SerializeField] int moveTimeLength = 60; //何秒かけて目的地まで行くか
    [SerializeField] float radius;
    int curTime = 0;
    Vector3 position;
    Vector3 startPosition;
    Vector3 nextPosition;
    Vector3 dir;
    bool isMoving = false;
    float t, v = 0;
    [SerializeField] EasingType type;
    [SerializeField] bool isUp = false;

    //[SerializeField] JitterMotion jitterMotion;
    //[SerializeField] ConstantMotion constantMotion;

    [SerializeField] bool isAuto = true;
    #endregion

   
    public enum EasingType
    {
        easeInQuad,
        easeOutQuad,
        easeInOutQuad,
        easeInCubic,
        easeOutCubic,
        easeInOutCubic,
        easeInExpo,
        easeOutExpo,
        easeInOutExpo
    }


    void Awake()
    {
        camera = this.GetComponent<Camera>();
    }

    void Start()
    {
        Cursor.visible = false;
        startPosition = this.transform.position;
        camera.transform.position = dir * v + startPosition;
        camera.transform.LookAt(target, Vector3.up);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            isAuto = !isAuto;
        }

        if(isMidiIn)
        {
            //Debug.Log(MidiReciever.knobs[5]);
            //radius = MidiReciever.knobs[1] * 60 + 2.0f;
            //moveTimeLength = (int)(MidiReciever.knobs[2] * 30) + 10;
        }

        if(isMoving == false)
        {
            //constantMotion.enabled = true;
            //jitterMotion.enabled = true;
            //if(MidiReciever.notes[0] ||Input.GetKeyDown(KeyCode.A) || (target.position - this.transform.position).magnitude > 0.0f)

            if(!isAuto)
            {
                if(Input.GetKeyDown(KeyCode.A))
                {
                    //constantMotion.enabled = false;
                    //jitterMotion.enabled = false;
                    isMoving = true;
                    t = v = 0;
                    curTime = 0;
                    startPosition = camera.transform.position;

                    if(Random.Range(0.0f, 1.0f) < 0.95f)
                    {
                        nextPosition = SpereRandomNextPos(radius);
                    } else
                    {
                        nextPosition = ForwardNextPos();
                    }


                    dir = nextPosition - startPosition;

                }
            } else
            {
                /*
                if(OscData.kick == 1.0)
                {
                    //constantMotion.enabled = false;
                    //jitterMotion.enabled = false;
                    isMoving = true;
                    t = v = 0;
                    curTime = 0;
                    startPosition = camera.transform.position;

                    if(Random.Range(0.0f, 1.0f) < 0.95f)
                    {
                        nextPosition = SpereRandomNextPos(radius);
                    } else
                    {
                        nextPosition = ForwardNextPos();
                    }


                    dir = nextPosition - startPosition;

                }
                */
            }

            
            var time = Time.realtimeSinceStartup;
            var vel = new Vector3(Mathf.PerlinNoise(this.transform.position.x, time), 
                Mathf.PerlinNoise(this.transform.position.y, time)*0.04f,
                Mathf.PerlinNoise(this.transform.position.z, time))*0.04f;
            this.transform.position += vel;
            this.transform.LookAt(target);
            
        }

        
        if(isMoving)
        {
            t = normalizeTime(curTime, moveTimeLength);

            if(type == EasingType.easeInQuad)
            {
                v = Easing.easeInQuad(t);
            }else if(type == EasingType.easeOutQuad)
            {
                v = Easing.easeOutQuad(t);
            }else if(type == EasingType.easeInOutQuad)
            {
                v = Easing.easeInOutQuad(t);
            }else if(type == EasingType.easeInCubic)
            {
                v = Easing.easeInCubic(t);
            } else if(type == EasingType.easeOutCubic)
            {
                v = Easing.easeOutCubic(t);
            } else if(type == EasingType.easeInOutCubic)
            {
                v = Easing.easeInOutCubic(t);
            } else if(type == EasingType.easeInExpo)
            {
                v = Easing.easeInExpo(t);
            } else if(type == EasingType.easeOutExpo)
            {
                v = Easing.easeOutExpo(t);
            } else if(type == EasingType.easeInOutExpo)
            {
                v = Easing.easeInOutExpo(t);
            }



            curTime++;
            if(t > 1.0) isMoving = false;
            camera.transform.position = dir * v + startPosition;
            camera.transform.LookAt(target, Vector3.up);
        }

        
    }


    
    float normalizeTime(float cur, float length)
    {
        float t = cur / length;
        return t;
    }

    Vector3 SpereRandomNextPos(float radius)
    {
        
        var nextPos = Random.insideUnitSphere;
        
        if(isUp)
        {
            while(nextPos.y <= 0.0f)
            {
                nextPos = Random.insideUnitSphere;
            }   
        }
        nextPos = nextPos * radius + target.transform.position;
        return nextPos;
    }

    Vector3 ForwardNextPos()
    {
        var ratio = Random.Range(0.0f, 0.5f);
        var nextPos = (target.transform.position - this.transform.position) * ratio + startPosition;
        return nextPos;
    }

}
