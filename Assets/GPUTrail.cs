using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUTrail : MonoBehaviour
{

    #region MeshData
    [SerializeField] int trailNum;
    [SerializeField] int trailLength;
    [SerializeField] float trailWidth;
    List<Vector3> verticesList;
    List<Vector3> normalList;
    List<Vector2> uvList;
    List<int> indexList;
    #endregion


    #region ComputeShader
    [SerializeField] ComputeShader cs;
    const int BLOCK_SIZE = 64;
    ComputeBuffer positionBuffer;
    ComputeBuffer velocityBuffer;
    int kernel;
    int threadGroupSize;
    #endregion


    #region instancingParams
    ComputeBuffer argsBuffer;
    private uint[] args = new uint[5];
    private int instancingCount;
    [SerializeField] Mesh srcMesh;
    [SerializeField] Material instancingMat;
    #endregion


    void Start()
    {
        CreateMesh();
        initInstancingParameter();
        initBuffer();
        kernel = cs.FindKernel("update");
    }

    void Update()
    {

        cs.SetBuffer(kernel, "positionBuffer", positionBuffer);
        cs.SetBuffer(kernel, "velocityBuffer", velocityBuffer);
        cs.SetFloat("dt", Time.deltaTime);
        cs.SetFloat("time", Time.realtimeSinceStartup);
        cs.Dispatch(kernel, trailNum, 1, 1);


        //Graphics.DrawMeshInstancedIndirect(srcMesh, 0, instancingMat,
        //new Bounds(Vector3.zero, Vector3.one * 32.0f), argsBuffer, 0, null, UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly, true, 0);

        instancingMat.SetBuffer("positionBuffer", positionBuffer);
        instancingMat.SetInt("BLOCK_SIZE", BLOCK_SIZE);
        Graphics.DrawMeshInstancedIndirect(srcMesh, 0, instancingMat,
        new Bounds(Vector3.zero, Vector3.one * 32.0f), argsBuffer);
    }


        //--------------------------------------------------------------------------------------------------------
        void CreateMesh()
    {
        srcMesh = new Mesh();
        verticesList = new List<Vector3>();
        normalList = new List<Vector3>();
        uvList = new List<Vector2>();
        indexList = new List<int>();

        for(int i = 0; i < BLOCK_SIZE; i++)
        {
            float delta = trailLength / (float)BLOCK_SIZE;
            var x = (i - BLOCK_SIZE / 2.0f) * delta;
            Vector3 p1 = new Vector3(x, 0.0f, trailWidth/2.0f);
            Vector3 p2 = new Vector3(x, 0.0f,-1.0f * trailWidth/2.0f);

            verticesList.Add(p1);
            verticesList.Add(p2);

            normalList.Add(p1.normalized);
            normalList.Add(p2.normalized);


            uvList.Add(new Vector2( (float)i / (float)(BLOCK_SIZE-1.0), 0.0f));
            uvList.Add(new Vector2( (float)i / (float)(BLOCK_SIZE-1.0), 1.0f));
            if(i > 1 && i % 2 == 0)
            {
                int i0 = i;
                int i1 = i + 1;
                int i2 = i - 1;
                int i3 = i - 2;
                indexList.Add(i0);
                indexList.Add(i1);
                indexList.Add(i2);

                indexList.Add(i0);
                indexList.Add(i2);
                indexList.Add(i3);
            }
        }

        srcMesh.vertices = verticesList.ToArray();
        srcMesh.normals = normalList.ToArray();
        srcMesh.uv = uvList.ToArray();

        //srcMesh.SetTriangles(indexList.ToArray(), 0);
        srcMesh.SetIndices(indexList.ToArray(), MeshTopology.Triangles, 0);
        srcMesh.RecalculateNormals();
        
    }
    //--------------------------------------------------------------------------------------------------------
    private void initInstancingParameter()
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

    //--------------------------------------------------------------------------------------------------------
    private void initBuffer()
    {
        List<Vector3> p = new List<Vector3>();
        List<Vector3> v = new List<Vector3>();

        positionBuffer = new ComputeBuffer(trailNum * BLOCK_SIZE, sizeof(float) * 3);
        velocityBuffer = new ComputeBuffer(trailNum * BLOCK_SIZE, sizeof(float) * 3);


        for(int i = 0; i < trailNum; i++)
        {
            for(int j = 0; j < trailLength; j++)
            {
                p.Add(Random.insideUnitSphere * 0.0f);
                v.Add(Random.insideUnitSphere * 3.0f);
            }
        }
        positionBuffer.SetData(p);
        velocityBuffer.SetData(v);
    }
    //--------------------------------------------------------------------------------------------------------
    private void OnDisable()
    {
        if(argsBuffer != null) argsBuffer.Release();
        if(positionBuffer != null) positionBuffer.Release();
        if(velocityBuffer != null) velocityBuffer.Release();
    }
}
