using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTaeget : MonoBehaviour
{
    enum Axis
    {
        XY = 0,
        YZ = 1,
        ZX = 2,
    }
    [SerializeField] Axis axisType;
    
    [SerializeField] float radius;
    [SerializeField] float speed;
    Vector3 startPos;
    void Start()
    {
        axisType = new Axis();
        axisType = Axis.ZX;
        startPos = this.transform.position;
    }

    void Update()
    {
        float t = Time.realtimeSinceStartup;
        if(axisType == Axis.XY) {
            this.transform.position = new Vector3(
            radius * Mathf.Cos(speed * t * Mathf.Deg2Rad),
            radius * Mathf.Sin(speed * t * Mathf.Deg2Rad),
            0.0f);
        }else if(axisType == Axis.YZ)
        {
            this.transform.position = new Vector3(
            0.0f,
            radius * Mathf.Cos(speed * t * Mathf.Deg2Rad),
            radius* Mathf.Sin(speed * t * Mathf.Deg2Rad));
        } else
        {
            this.transform.position = new Vector3(
            radius * Mathf.Cos(speed * t * Mathf.Deg2Rad),
            0.0f,
            radius * Mathf.Sin(speed * t * Mathf.Deg2Rad));
        }
        
    }
}
