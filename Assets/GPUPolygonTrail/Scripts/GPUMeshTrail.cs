using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
TrailNum -> disptch数
TrailPoint（関節点がPositionBufferの座標に相当する）
*/
namespace GPUMeshTrail
{
    public class GPUMeshTrail : MonoBehaviour
    {
        #region trail info
        [SerializeField] int trailNum;//dispatchの次元数と一致する
        [SerializeField] int trailPointNum;//何ポイントでトレイルを構成するか(中心の軸を形成している点)
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
        ComputeBuffer initPositionBuffer;
        ComputeBuffer lifeBuffer;
        int kernel;
        uint threadX, threadY, threadZ;
        #endregion

        #region instancing params
        ComputeBuffer argsBuffer;
        private uint[] args = new uint[5];
        private int instancingCount;
        [SerializeField] Mesh srcMesh;
        [SerializeField] Material instancingMat;
        #endregion

        [SerializeField]
        Texture2D[] texes;
        [SerializeField]
        Sprite[] sprites;
        Texture2D tex;
        [SerializeField]
        Image img;
        [SerializeField]
        Text title;
        [SerializeField]
        Text description;
        int index = 0;

        

        void Start()
        {
            if(srcMesh == null)
            {
                CreateMesh();
            }
            InitInstancingParameter();
            CreateBuffers();
            InitBuffers();
            kernel = cs.FindKernel("update");
            tex = texes[0];
           
        }

        void Update()
        {
            //if(OscData.kick == 1)
            //{
            //    pulse = 1;
            //}

            cs.SetFloat("time", Time.realtimeSinceStartup);
            //cs.SetInt("BLOCK_SIZE", trailPointNum);
            //cs.SetInt("pulse", pulse);
            //cs.SetFloat("blend", blend);
            cs.SetBuffer(kernel, "positionBuffer", positionBuffer);
            cs.SetBuffer(kernel, "initPositionBuffer", initPositionBuffer);
            //cs.SetBuffer(kernel, "velocityBuffer", velocityBuffer);
            cs.SetBuffer(kernel, "lifeBuffer", lifeBuffer);
            //cs.SetVector("target", target.transform.position);
            cs.Dispatch(kernel, trailNum, 1, 1);
            

            instancingMat.SetFloat("trailNum", trailNum);
            //instancingMat.SetInt("pulse", pulse); 
            instancingMat.SetFloat("_TrailNumsq", Mathf.Sqrt(trailNum));
            instancingMat.SetBuffer("positionBuffer", positionBuffer);
            instancingMat.SetBuffer("lifeBuffer", lifeBuffer);
            instancingMat.SetInt("BLOCK_SIZE", trailPointNum);
            instancingMat.SetInt("circleResolution", circleResolution);


            
            if(Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log((int)Random.Range(0, texes.Length));
                index = (int)Random.Range(0, texes.Length);
                tex = texes[index ];
                //img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                img.sprite = sprites[index];

            }

            
            instancingMat.SetTexture("_Tex0", tex);
            

            //UI-text
            if(index == 0)
            {
                title.text = "Wheat Field with Cypresses(1889)";
                description.text =
                    "Cypresses gained ground in Van Gogh’s work \n" +
                    "by late June 1889 when he resolved to \n" +
                    "devote one of his first series in Saint-Rémy to \n" +
                    " the towering trees. Distinctive for their rich impasto, \n" +
                    " his exuberant on-the-spot studies include \n" +
                    "the Met’s close-up vertical view of cypresses (49.30) \n" +
                    " and this majestic horizontal composition, \n" +
                    " which he illustrated in reed-pen drawings sent to \n" +
                    " his brother on July 2. \n" +
                    "Van Gogh regarded the present work as one \n" +
                    "of his “best” summer landscapes and \n" +
                    "was prompted that September to make two studio renditions: \n" +
                    " one on the same scale (National Gallery, London) \n" +
                    " and the other a smaller replica, intended \n" +
                    "as a gift for his mother and sister (private collection).";


            } else if(index == 1)
            {
                title.text = "Christ Asleep during the Tempest(1853)";
            } else if(index == 2)
            {
                title.text = "Irises(1890)";
                description.text =
                    "Delacroix painted at least six versions \n" +
                    "of this New Testament lesson in faith: \n" +
                    " when awakened by his terrified disciples, \n" +
                    "Christ scolded them for their lack of trust in Providence. \n" +
                    " In the earlier works, the seascape is more prominent; \n" +
                    " in the later ones, as here, \n" +
                    " Christ’s bark occupies a more significant place. \n" +
                    " After Vincent van Gogh saw this version in Paris in 1886, \n" +
                    " he wrote, Christ’s boat—I’m talking about \n" +
                    "the blue and green sketch with touches \n" +
                    "of purple and red and a little lemon yellow for the halo, \n" +
                    " the aureole—speaks a symbolic language through color itself. \n";

            } else if(index == 3)
            {
                title.text = "Roses(1890)";
                description.text =
                    "On the eve of his departure from the asylum in \n" +
                    "Saint-Rémy in May 1890, \n" +
                    "Van Gogh painted an exceptional group of four still lifes \n" +
                    ", to which both the Museum's Roses and \n" +
                    "Irises (58.187) belong. \n" +
                    "These bouquets and their counterparts—an upright \n" +
                    " composition of irises (Van Gogh Museum,\n" +
                    "Amsterdam) and a horizontal composition of roses \n" +
                    "(National Gallery of Art, Washington, D.C.)\n" +
                    "—were conceived as a series or ensemble, \n" +
                    " on a par with the earlier Sunflower decoration \n" +
                    " he made in Arles.\n" +
                    " Traces of pink along the tabletop and \n" +
                    "rose petals in the present painting, \n" +
                    "which have faded over time,\n" +
                    " offer a faint reminder of the formerly more vivid \n" +
                    "canvas of pink roses against \n" +
                    "a yellow - green background in a green vase.\n";
            }



            Graphics.DrawMeshInstancedIndirect(srcMesh, 0, instancingMat,
            new Bounds(Vector3.zero, Vector3.one * 32.0f), argsBuffer);

            //pulse = 0;
        }

        void CreateMesh()
        {
            srcMesh = new Mesh();
            positionList = new List<Vector3>();
            normalList = new List<Vector3>();
            uvList = new List<Vector2>();
            indexList = new List<int>();

            float theta = 2.0f * Mathf.PI / circleResolution;
            float delta = trailLength / trailPointNum;//関節間の距離
            float deltaUV = 1.0f / (circleResolution - 1);
            for(int i = 0; i < trailPointNum; i++)
            {
                //radius = Random.Range(radius * 0.5f, radius);
                //radius = Random.Range(radius * 0.5f, radius);
                for(int j = 0; j < circleResolution; j++)
                {


                    var p = new Vector3(radius * Mathf.Cos(theta * j - Mathf.PI/4.0f),
                                        radius * Mathf.Sin(theta * j - Mathf.PI / 4.0f),
                                        i * delta);
                    positionList.Add(p);
                    normalList.Add(p);
                    uvList.Add(new Vector2(1.0f / trailPointNum * i, deltaUV * j));
                    //一周打ち終わってから
                    if(i > 0)
                    {
                        int i0 = j + circleResolution * i;
                        //int next = i0 + (j + 1) % circleResolution;
                        int i1 = (j + 1) % circleResolution + circleResolution * i;
                        int i2 = j + circleResolution * (i - 1);
                        int i3 = (j + 1) % circleResolution + circleResolution * (i - 1);

                        /*
                        indexList.Add(i0);
                        indexList.Add(i1);
                        indexList.Add(i2);

                        indexList.Add(i1);
                        indexList.Add(i3);
                        indexList.Add(i2);
                        */


                        if(i == 1 && j == 0)
                        {
                            indexList.Add(0);
                            indexList.Add(3);
                            indexList.Add(1);

                            indexList.Add(1);
                            indexList.Add(3);
                            indexList.Add(2);
                        }else if((i == trailPointNum - 1) && j == 0)
                        {
                            
                            int end = trailPointNum * circleResolution -1;
                            indexList.Add(end-3);
                            indexList.Add(end - 2);
                            indexList.Add(end);
                            

                           
                            indexList.Add(end - 1);
                            indexList.Add(end);
                            indexList.Add(end - 2);
                            


                        }
                        indexList.Add(i0);
                        indexList.Add(i2);
                        indexList.Add(i1);

                        indexList.Add(i1);
                        indexList.Add(i2);
                        indexList.Add(i3);

                    }
                }
            }

            //srcMesh.vertices = positionList.ToArray();
            srcMesh.SetVertices(positionList);
            srcMesh.normals = normalList.ToArray();
            srcMesh.uv = uvList.ToArray();
            //srcMesh.RecalculateNormals();
            //srcMesh.RecalculateBounds();
            //srcMesh.triangles = indexList.ToArray();
            srcMesh.SetIndices(indexList.ToArray(), MeshTopology.Triangles, 0);
            //srcMesh.SetIndices(indexList.ToArray(), MeshTopology.Lines, 0);
        }

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
            initPositionBuffer = new ComputeBuffer(trailNum * trailPointNum, sizeof(float) * 3);
            //velocityBuffer = new ComputeBuffer(trailNum * trailPointNum, sizeof(float) * 3);
            lifeBuffer = new ComputeBuffer(trailNum, sizeof(float) * 2);
            Vector2[] lifeArr = new Vector2[trailNum];
            for(int i = 0; i < lifeArr.Length; i++)
            {
                var _  = radius * 2.0f;
                lifeArr[i] = new Vector2(_, _);
            }
            lifeBuffer.SetData(lifeArr);
        }
        //--------------------------------------------------------------------
        void InitBuffers()
        {
            kernel = cs.FindKernel("init");
            cs.SetBuffer(kernel, "positionBuffer", positionBuffer);
            cs.SetBuffer(kernel, "initPositionBuffer", initPositionBuffer);
            cs.SetFloat("radius", radius);
            cs.Dispatch(kernel, trailNum, 1, 1);

        }
        private void OnDisable()
        {
            positionBuffer.Release();
            initPositionBuffer.Release();
            //velocityBuffer.Release();
            lifeBuffer.Release();
        }
    }
}