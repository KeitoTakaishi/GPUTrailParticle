using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomMeshTrail
{
    public class GPUPolygonTrail : MonoBehaviour
    {
        #region Trail-Info
        [SerializeField] int trailNum;//dispatchの次元数と一致する
        [SerializeField] int trailPointNum;//何ポイントでトレイルを構成するか
        [SerializeField] int circleResolution; //断面の解像度
        [SerializeField] float radius;//断面の半径
        [SerializeField] float trailLength;
        List<Vector3> positionList;
        List<Vector3> normalList;
        List<Vector2> uvList;
        List<int> indexList;
        #endregion

        #region ComputeShaderParameters
        [SerializeField] ComputeShader cs;
        ComputeBuffer positionBuffer;
        ComputeBuffer velocityBuffer;
        int kernel;
        uint threadX, threadY, threadZ;
        #endregion

        #region Instancing-Params
        ComputeBuffer argsBuffer;
        private uint[] args = new uint[5];
        private int instancingCount;
        Mesh srcMesh;
        [SerializeField] Material instancingMat;
        #endregion

        void Start()
        {
            if(srcMesh == null)
            {
                CreateMesh();
            }
            InitInstancingParameter();
            CreateBuffers();
            InitBuffers();
            kernel = cs.FindKernel("Update");
        }

        void Update()
        {
            cs.SetFloat("time", Time.realtimeSinceStartup);
            cs.SetBuffer(kernel, "positionBuffer", positionBuffer);
            cs.SetBuffer(kernel, "velocityBuffer", velocityBuffer);
            cs.Dispatch(kernel, trailNum, 1, 1);


            instancingMat.SetFloat("trailNum", trailNum);
            instancingMat.SetBuffer("positionBuffer", positionBuffer);
            instancingMat.SetInt("trailPointNum", trailPointNum);
            instancingMat.SetInt("circleResolution", circleResolution);

            Graphics.DrawMeshInstancedIndirect(srcMesh, 0, instancingMat,
            new Bounds(Vector3.zero, Vector3.one * 32.0f), argsBuffer);
        }

        void CreateMesh()
        {
            srcMesh = new Mesh();
            positionList = new List<Vector3>();
            normalList = new List<Vector3>();
            uvList = new List<Vector2>();
            indexList = new List<int>();

            float theta = 2.0f * Mathf.PI / circleResolution;
            float delta = trailLength / trailPointNum;
            float deltaUV = 1.0f / (circleResolution - 1);
            float trainMidPoint = 0.5f * (trailPointNum - 1);
            for(int i = 0; i < trailPointNum; i++)
            {
                float distance = Mathf.Abs(trainMidPoint - i);
                distance = trainMidPoint - distance;
                radius = -5.0f * (Mathf.Exp(-0.1f * distance) - 1.05f);

                //radius = Random.Range(radius * 0.5f, radius);
                for(int j = 0; j < circleResolution; j++)
                {

                    var p = new Vector3(radius * Mathf.Cos(theta * j), radius * Mathf.Sin(theta * j), i * delta);
                    positionList.Add(p);
                    normalList.Add(p);
                    uvList.Add(new Vector2(1.0f / trailPointNum * i, deltaUV * j));
                    if(i > 0)
                    {
                        int i0 = j + circleResolution * i;
                        int i1 = (j + 1) % circleResolution + circleResolution * i;
                        int i2 = j + circleResolution * (i - 1);
                        int i3 = (j + 1) % circleResolution + circleResolution * (i - 1);

                        
                        indexList.Add(i0);
                        indexList.Add(i2);
                        indexList.Add(i1);

                        indexList.Add(i1);
                        indexList.Add(i2);
                        indexList.Add(i3);

                    }
                }
            }

            srcMesh.SetVertices(positionList);
            srcMesh.normals = normalList.ToArray();
            srcMesh.uv = uvList.ToArray();
            srcMesh.SetIndices(indexList.ToArray(), MeshTopology.Triangles, 0);
            //srcMesh.SetIndices(indexList.ToArray(), MeshTopology.LineStrip, 0);
        }
        //--------------------------------------------------------------------
        private void InitInstancingParameter()
        {
            instancingCount = trailNum;
            argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

            args[0] = srcMesh.GetIndexCount(0);
            args[1] = (uint)instancingCount;
            args[2] = srcMesh.GetIndexStart(0);
            args[3] = srcMesh.GetBaseVertex(0);
            args[4] = 0;
            argsBuffer.SetData(args);
        }
        //ComputeBuffer
        //--------------------------------------------------------------------
        void CreateBuffers()
        {
            positionBuffer = new ComputeBuffer(trailNum * trailPointNum, sizeof(float) * 3);
            velocityBuffer = new ComputeBuffer(trailNum * trailPointNum, sizeof(float) * 3);
        }
        //--------------------------------------------------------------------
        void InitBuffers()
        {
            kernel = cs.FindKernel("Init");
            cs.SetBuffer(kernel, "positionBuffer", positionBuffer);
            cs.SetBuffer(kernel, "velocityBuffer", velocityBuffer);
            cs.Dispatch(kernel, trailNum, 1, 1);

        }
        //--------------------------------------------------------------------
        private void OnDisable()
        {
            positionBuffer.Release();
            velocityBuffer.Release();
        }
    }
}
