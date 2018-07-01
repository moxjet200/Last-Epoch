/*
useful links:
https://forum.unity3d.com/threads/how-to-assign-matrix4x4-to-transform.121966/
http://answers.unity3d.com/questions/402280/how-to-decompose-a-trs-matrix.html
https://github.com/MattRix/UnityDecompiled/blob/master/UnityEngine/UnityEngine/Matrix4x4.cs
http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToMatrix/index.htm
https://forum.unity3d.com/threads/problem-copying-real-time-lighting-data.488474/
copiedRenderer.realtimeLightmapIndex = sourceRenderer.realtimeLightmapIndex;
copiedRenderer.realtimeLightmapScaleOffset = sourceRenderer.realtimeLightmapScaleOffset;
*/

using System;
using System.Threading;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedTerrainGrass {

    public delegate float GrassDelegate(int layer, int index, int cellIndex, bool b_initCompleteCell);

    [System.Serializable]
    public enum GrassCameras {
        AllCameras = 0,
        MainCameraOnly = 1
    }

    [System.Serializable]
    public enum RotationMode {
        AlignedRandomYAxis = 0,
        AlignedRandomYXAxes = 1,
        NotAlignedRandomYAxis = 2
    }

    [System.Serializable]
    public enum BucketsPerCell : int {
        _4x4 = 4,
        _8x8 = 8,
        _16x16 = 16,
        _32x32 = 32,
        _64x64 = 64
    }

    [RequireComponent(typeof(Terrain))]
    //[ExecuteInEditMode]
    public class GrassManager : MonoBehaviour {


        public Camera Cam;
        public bool IngnoreOcclusion = true;
        private Transform CamTransform;


        public bool showGrid = false;
        private GameObject go;
        public Material ProjMat;

        public /*static*/ GrassDelegate del;   // Must not be static in case we have multiple instances
        
        AsyncCallback callback;
        AsyncCallback callback1;


        public /*static*/ System.Random random = new System.Random(1234); // Must not be static in case we have multiple instances / We use a custom seed to make sure that grass will look the same each time we enter playmode

        public Terrain ter;
        public TerrainData terData;

        public GrassTerrainDefinitions SavedTerrainData;

        public bool initCompleteCell = true; // lets you switch between one cellcontent each frame and all contents of a cell per frame
        public bool useBurst = false;
        public float BurstRadius = 50.0f;

        public float DetailDensity = 1;
        private float CurrentDetailDensity;
        
        private Vector3 TerrainPosition;
        private Vector3 TerrainSize;
        private Vector3 OneOverTerrainSize;

        private Vector2 TerrainDetailSize;
        private float SqrTerrainCullingDist;
        private bool ThreadIsRunning = false;

//  Height
        private int TerrainHeightmapWidth;
        private int TerrainHeightmapHeight;
        private float[] TerrainHeights;
        private float OneOverHeightmapWidth;
        //private float OneOverHeightmapHeight;
        private float TerrainSizeOverHeightmap;
        private float OneOverHeightmapWidthRight;
        //private float OneOverHeightmapWidthUp;

// Culling
        public GrassCameras CameraSelection;
        private Camera CameraInWichGrassWillBeDrawn; // default null -> all cameras
        public int CameraLayer = 0;

        private CullingGroup cullingGroup;
        private BoundingSphere[] boundingSpheres;
        private int numResults = 0;
        private int[] resultIndices;
        public float CullDistance = 80.0f;
        public float FadeLength = 20.0f;
        public float CacheDistance = 120.0f;

        public float DetailFadeStart = 20.0f;
        public float DetailFadeLength = 30.0f;

        public float ShadowStart = 10.0f;
        public float ShadowFadeLength = 20.0f;
        public float ShadowStartFoliage = 30.0f;
        public float ShadowFadeLengthFoliage = 20.0f;

// Layers
        [Space(12)]
        private int NumberOfLayers;
        private int OrigNumberOfLayers;
        public Mesh[] v_mesh;
        public Material[] v_mat;
        public RotationMode[] InstanceRotation;
        public bool[] WriteNormalBuffer;
        public ShadowCastingMode[] ShadowCastingMode;
        public float[] MinSize;
        public float[] MaxSize;
        public float[] Noise;
        public int[] LayerToMergeWith;
        public bool[] DoSoftMerge;

        public int[][] SoftlyMergedLayers;
// End Layer 

 
        private byte[][] mapByte;            // Density Maps
        private int GrassMatrixBufferPID;
        private int GrassNormalBufferPID;

        private int TotalCellCount;         // Number of Cells
        private int NumberOfCells;          // Number of Cells per Axis
        public  BucketsPerCell NumberOfBucketsPerCellEnum = BucketsPerCell._16x16; // Number of Patches per Cell per Axis (Detail Resolution)
        private int NumberOfBucketsPerCell;

        private float CellSize = 0.0f;
        private float BucketSize;
        //private float OneOverBucketSize;

// The shared tempArrays as used by the workerthread
        private int maxBucketDensity = 0;
        private Matrix4x4[] tempMatrixArray;
        private Vector3[] tempNormalArray;
//

        private Vector2 samplePosition;
        private Vector2 tempSamplePosition;

        private GrassCell[] Cells;
        private GrassCellContent[] CellContent;
        private List<int> CellsOrCellContentsToInit = new List<int>();

        private Shader sh;

        private int GrassFadePropsPID;
        private Vector4 GrassFadeProps;
        private int GrassShadowFadePropsPID;
        private Vector2 GrassShadowFadeProps;

        //
        //private Vector3 ZeroVec = Vector3.zero;
        //private Matrix4x4 ZeroMatrix = Matrix4x4.identity;
        private Matrix4x4 tempMatrix = Matrix4x4.identity;
        private Vector3 tempPosition;
        private Quaternion tempRotation;
        private Vector3 tempScale;

        //private Vector3 UpVec = Vector3.up;
        private Quaternion ZeroQuat = Quaternion.identity;

    //  Editor variables
        public bool DebugStats = false;

        public bool FirstTimeSynced = false;
        public int LayerEditMode = 0;
        public int LayerSelection = 0;
        public bool Foldout_Rendersettings = true;
        public bool Foldout_Prototypes = true;

//  --------------------------------------------------------------------

        void OnEnable() {
            Init();
        }

    //  Make sure we clean up everything
        void OnDisable() {
            cullingGroup.Dispose();
        //  Release ComputeBuffers
            for (int i = 0; i < CellContent.Length; i ++) {
                CellContent[i].RelaseBuffers();
            }
        }

//  --------------------------------------------------------------------
//  Update function

        void Update () {    
            DrawGrass();
        }

//  --------------------------------------------------------------------
//  Function to change rendering settings at run time
        
        public void RefreshGrassRenderingSettings(
            float t_DetailDensity,
            float t_CullDistance,
            float t_FadeLength,
            float t_CacheDistance,
            float t_DetailFadeStart,
            float t_DetailFadeLength,
            float t_ShadowStart,
            float t_ShadowFadeLength,
            float t_ShadowStartFoliage,
            float t_ShadowFadeLengthFoliage
            )
        {
        
        //  Remove already queued cells
            CellsOrCellContentsToInit.Clear();

        //  Update Culling Group
            float[] distances = new float[] {t_CullDistance, t_CacheDistance};
            cullingGroup.SetBoundingDistances(distances);

        //  Set new DetailDensity
            CurrentDetailDensity = t_DetailDensity;

        //  Update fade distances in shaders
            Shader.SetGlobalVector(GrassFadePropsPID, new Vector4(
                (t_CullDistance - t_FadeLength) * (t_CullDistance - t_FadeLength),
                1.0f / (t_FadeLength * t_FadeLength),
                t_DetailFadeStart * t_DetailFadeStart,
                1.0f / (t_DetailFadeLength * t_DetailFadeLength)
            ));
            Shader.SetGlobalVector(GrassShadowFadePropsPID, new Vector4(
                t_ShadowStart * t_ShadowStart,
                1.0f / (t_ShadowFadeLength * t_ShadowFadeLength),
                t_ShadowStartFoliage * t_ShadowStartFoliage,
                1.0f / (t_ShadowFadeLengthFoliage * t_ShadowFadeLengthFoliage)
            ));

            //  Clear all currently set Cells and/or CellContents
            //  We simply rely on the Update function here, which will generate new Cells and/or CellContents based on the new settings.
            for (int i = 0; i < Cells.Length; i ++) {
                var CurrentCell = Cells[i];
            //  A)
                if (!initCompleteCell) {
                    int NumberOfLayersInCell = CurrentCell.CellContentCount;
                    for (int j = 0; j < NumberOfLayersInCell; j++) {
                        CellContent[ CurrentCell.CellContentIndexes[j] ].ReleaseCellContent();
#if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                        Destroy(CellContent[ CurrentCell.CellContentIndexes[j] ].v_mat);
#endif
                    } 
                }
            //  B)
                else {
                    if (CurrentCell.state == 3) {
                        int NumberOfLayersInCell = CurrentCell.CellContentCount;
                        for (int j = 0; j < NumberOfLayersInCell; j++) {
                            CellContent[ CurrentCell.CellContentIndexes[j] ].ReleaseCellContent();
#if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                            Destroy(CellContent[ CurrentCell.CellContentIndexes[j] ].v_mat);
#endif
                        }
                    }
                    CurrentCell.state = 0;
                }
            }

        //  Set up tempMatrixArray
            var maxInstances = Mathf.CeilToInt(NumberOfBucketsPerCell * NumberOfBucketsPerCell * maxBucketDensity * CurrentDetailDensity);
            tempMatrixArray = new Matrix4x4[maxInstances];
            tempNormalArray = new Vector3[maxInstances];

            if (useBurst) {
                BurstInit();
            }
        }

//  --------------------------------------------------------------------
//  Init Cells, CellContens and Density Maps from Scriptable Object

        public void InitCellsFast() {
            mapByte = new byte[SavedTerrainData.DensityMaps.Count][];
            for (int i = 0; i < SavedTerrainData.DensityMaps.Count; i ++) {
               mapByte[i] =  SavedTerrainData.DensityMaps[i].mapByte;
            }
            Cells = new GrassCell[ SavedTerrainData.Cells.Length ];
            CellContent = new GrassCellContent[ SavedTerrainData.CellContent.Length ];
maxBucketDensity = SavedTerrainData.maxBucketDensity;

            int CellContentIndex = 0;

        //  We really have to cpoy the arrays as otherwise material, mesh or argsbuffer error?!
            int cLength = SavedTerrainData.Cells.Length;
            for(int CurrrentCellIndex = 0; CurrrentCellIndex < cLength; CurrrentCellIndex++) {

                Cells[CurrrentCellIndex] = new GrassCell();
                Cells[CurrrentCellIndex].index = CurrrentCellIndex;

                Cells[CurrrentCellIndex].Center = SavedTerrainData.Cells[CurrrentCellIndex].Center;

                Cells[CurrrentCellIndex].CellContentIndexes = SavedTerrainData.Cells[CurrrentCellIndex].CellContentIndexes;
                Cells[CurrrentCellIndex].CellContentCount = SavedTerrainData.Cells[CurrrentCellIndex].CellContentCount;

                int CellContentsinCell = Cells[CurrrentCellIndex].CellContentCount;
                for (int layer = 0; layer < CellContentsinCell; layer++) {

                    CellContent[CellContentIndex] = new GrassCellContent();

                    var CurrentCellContent = CellContent[CellContentIndex];
                    var CurrentSavedCellContent = SavedTerrainData.CellContent[CellContentIndex];

                    CurrentCellContent.index = CellContentIndex;
                    CurrentCellContent.Layer = CurrentSavedCellContent.Layer;
                    if(CurrentSavedCellContent.SoftlyMergedLayers.Length > 0) {
                        CurrentCellContent.SoftlyMergedLayers = CurrentSavedCellContent.SoftlyMergedLayers;   
                    }
                    else {
                        CurrentCellContent.SoftlyMergedLayers = null;  
                    }
                    CurrentCellContent.GrassMatrixBufferPID = GrassMatrixBufferPID;
                    //CurrentCellContent.GrassNormalBufferPID = GrassNormalBufferPID;
                    CurrentCellContent.Center = CurrentSavedCellContent.Center;
                    CurrentCellContent.Pivot = CurrentSavedCellContent.Pivot;
                    CurrentCellContent.PatchOffsetX = CurrentSavedCellContent.PatchOffsetX;
                    CurrentCellContent.PatchOffsetZ = CurrentSavedCellContent.PatchOffsetZ;
                    CurrentCellContent.Instances = CurrentSavedCellContent.Instances;

                    CellContentIndex += 1;
                }
            }
        }

//  --------------------------------------------------------------------
//  Init Cells, CellContens and Density Maps directly at start

        public void InitCells() {
        //  Merge Layers
            NumberOfLayers = terData.detailPrototypes.Length;
            OrigNumberOfLayers = NumberOfLayers;
            int[][] MergeArray = new int[OrigNumberOfLayers][]; // Actually too big...
            int[][] SoftMergeArray = new int[OrigNumberOfLayers][]; // Actually too big...

        //  Check if we have to merge detail layers
            for (int i = 0; i < OrigNumberOfLayers; i ++) {

                int t_LayerToMergeWith = LayerToMergeWith[i];
                int index_LayerToMergeWith = t_LayerToMergeWith - 1;

                if( (t_LayerToMergeWith != 0) && (t_LayerToMergeWith != (i+1))  ) {
                //  Check if the Layer we want to merge with does not get merged itself..
                    if ( LayerToMergeWith[ index_LayerToMergeWith  ] == 0 ) {
                        if ( MergeArray[ index_LayerToMergeWith ] == null ) {
                            MergeArray[ index_LayerToMergeWith ] = new int[ OrigNumberOfLayers - 1 ]; // Also actually too big
                        }

                        if ( DoSoftMerge[i] ) {
                            if( SoftMergeArray[ index_LayerToMergeWith ] == null ) {
                                SoftMergeArray[ index_LayerToMergeWith ] = new int[ OrigNumberOfLayers - 1 ]; // Also actually too big
                            }
                        }
                    //  Find a the first free entry Merge
                        for (int j = 0; j < OrigNumberOfLayers - 1; j++) {
                            if ( MergeArray[ index_LayerToMergeWith ][j] == 0 ) {
                                MergeArray[ index_LayerToMergeWith ][j] =                                             i + 1; // as index starts at 1 (0 means: do not merge)
                                break;
                            } 
                        }

                    //  Find a the first free entry Soft Merge
                        if ( DoSoftMerge[i] ) {
                            for (int j = 0; j < OrigNumberOfLayers - 1; j++) {
                                if ( SoftMergeArray[ index_LayerToMergeWith ][j] == 0 ) {
                                    SoftMergeArray[ index_LayerToMergeWith ][j] =                                     i + 1; // as index starts at 1 (0 means: do not merge)
                                    break;
                                } 
                            }
                        }
                    }
                }
            }


            Cells = new GrassCell[NumberOfCells * NumberOfCells];
        //  As we do not know the final cout we create cellcontents in a temporary list
            List<GrassCellContent> tempCellContent = new List<GrassCellContent>();
            tempCellContent.Capacity = NumberOfCells * NumberOfCells * NumberOfLayers;
            mapByte = new byte[NumberOfLayers][];

        //  Read and convert [,] into byte[]
            // Here we are working on "pixels" or buckets and ly out the array like x0.y0, x0.y2, x0.y3, ....
            for(int layer = 0; layer < NumberOfLayers; layer++) {
        //  TODO: remapping of layers did not work out. So we do it brute force...
 
            //  Skip mapByte if layer uses hard merge
                if (LayerToMergeWith[layer] == 0 || DoSoftMerge[layer] ) {
                    mapByte[layer] = new byte[ (int)(TerrainDetailSize.x * TerrainDetailSize.y) ];
                //  Check merge. Only merge densities if SoftMerge is disabled.
                    bool doMerge = false;
                    if ( MergeArray[layer] != null && !DoSoftMerge[layer] ) {
                        doMerge = true;
                    }
                    for (int x = 0; x < (int)TerrainDetailSize.x; x++ ){
                        for (int y = 0; y < (int)TerrainDetailSize.y; y ++) {
                            // flipped!!!!!????????? no,not in this case.
                            int[,] temp = terData.GetDetailLayer(x, y, 1, 1, layer );
                            mapByte[layer][ x * (int)TerrainDetailSize.y + y ] = Convert.ToByte( (int)temp[0,0] );
                        //  Merge
                            if(doMerge) {
                                for (int m = 0; m < OrigNumberOfLayers - 1; m++ ) {
                                    if (MergeArray[layer][m] != 0) {
                                        temp = terData.GetDetailLayer(x, y, 1, 1, MergeArray[layer][m]              - 1 ); // as index starts as 1!
                                        mapByte[layer][ x * (int)TerrainDetailSize.y + y ] = Convert.ToByte( mapByte[layer][ x * (int)TerrainDetailSize.y + y ] + (int)temp[0,0] );  
                                    }
                                    else {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            int density = 0;
            int CellContentOffset = 0;
            Vector3 Center;
            int ArrayOffset = 0;

            for(int x = 0; x < NumberOfCells; x++) {
                for(int z = 0; z < NumberOfCells; z++) {
                    var CurrrentCellIndex = x * NumberOfCells + z;
                //  Find Height or Position.y of Cell
                    Vector2 normalizedPos;
                    normalizedPos.x = (x * CellSize + 0.5f * CellSize) * OneOverTerrainSize.x;
                    normalizedPos.y = (z * CellSize + 0.5f * CellSize) * OneOverTerrainSize.z;
                    float sampledHeight = terData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);

                    Cells[CurrrentCellIndex] = new GrassCell();
                    Cells[CurrrentCellIndex].index = CurrrentCellIndex;
                //  Center in World Space – used by cullingspheres
                    Center.x = x * CellSize + 0.5f * CellSize  +  TerrainPosition.x;
Center.y = sampledHeight + TerrainPosition.y;
                    Center.z = z * CellSize + 0.5f * CellSize  +  TerrainPosition.z;
                    Cells[CurrrentCellIndex].Center = Center;

                    int tempBucketDensity;

                //  Create and setup all CellContens (layers) for the given Cell
                    for(int layer = 0; layer < NumberOfLayers; layer++) {
                    
                    //  Only create cellcontent entry if the layer does not get merged.
                        if (LayerToMergeWith[layer] == 0 ) {
                        //  Sum up density for all buckets 
                            for(int xp = 0; xp < NumberOfBucketsPerCell; xp++) {
                                for(int yp = 0; yp < NumberOfBucketsPerCell; yp++) {
                                    //  here we are working on cells and buckets!
                                    tempBucketDensity = (int)(mapByte[layer][
                                       (x * NumberOfBucketsPerCell) * (int)TerrainDetailSize.y + xp * (int)TerrainDetailSize.y 
                                       + z * NumberOfBucketsPerCell + yp 
                                    ]);
                                    if (tempBucketDensity > maxBucketDensity) {
                                        maxBucketDensity = tempBucketDensity;
                                    }
                                    density += tempBucketDensity;
                                }
                            }

                        //  Check for softly merged layers
                            int NumberOfSoftlyMergedLayers = 0;

                            if(SoftMergeArray[layer] != null) {
                                for (int l = 0; l < OrigNumberOfLayers - 1; l++) {
                                    if ( SoftMergeArray[layer][l] == 0) {
                                        break;
                                    }
                                    int softMergedLayer = SoftMergeArray[layer][l] - 1; // as index starts at 1
                                    int softDensity = 0;

                                    //  Sum up density for all buckets 
                                    for(int xp = 0; xp < NumberOfBucketsPerCell; xp++) {
                                        for(int yp = 0; yp < NumberOfBucketsPerCell; yp++) {
                                            //  here we are working on cells and buckets!
                                            tempBucketDensity = (int)(mapByte[softMergedLayer][
                                               (x * NumberOfBucketsPerCell) * (int)TerrainDetailSize.y + xp * (int)TerrainDetailSize.y 
                                               + z * NumberOfBucketsPerCell + yp 
                                            ]);
                                            softDensity += tempBucketDensity;
                                        }
                                    }
                                    if (softDensity > 0) {
                                        NumberOfSoftlyMergedLayers += 1;
                                        density += softDensity;
                                    }
                                }
                                if (NumberOfSoftlyMergedLayers * 16 > maxBucketDensity) {
                                    maxBucketDensity = NumberOfSoftlyMergedLayers * 16 + 16 * 2; // * 2 is safe guard!
                                }
                            }
                            
                        //  Skip Content if density = 0
                            if(density > 0) {
                            //  Register CellContent to Cell
                                Cells[CurrrentCellIndex].CellContentIndexes.Add(CellContentOffset);
                                Cells[CurrrentCellIndex].CellContentCount += 1;
                            //  Add new CellContent
                                var tempContent = new GrassCellContent();
                                tempContent.index = CellContentOffset;
                                tempContent.Layer = layer;
                                tempContent.GrassMatrixBufferPID = GrassMatrixBufferPID;
                                //tempContent.GrassNormalBufferPID = GrassNormalBufferPID;
                            //  Center in World Space – used by drawmeshindirect
                                tempContent.Center = Center;
                            //  Pivot of cell in local terrain space
                                tempContent.Pivot = new Vector3( x * CellSize, sampledHeight, z * CellSize );
                                tempContent.PatchOffsetX = x * NumberOfBucketsPerCell * (int)TerrainDetailSize.y;
                                tempContent.PatchOffsetZ = z * NumberOfBucketsPerCell;
                                tempContent.Instances = density;

                            //  Softly merged Layers
                                if( NumberOfSoftlyMergedLayers > 0 ) {
                                    List<int> tempSoffMergedLayers = new List<int>();
                                    for (int l = 0; l < OrigNumberOfLayers - 1; l++) {
                                        if(SoftMergeArray[layer][l] != 0) {
                                            tempSoffMergedLayers.Add( SoftMergeArray[layer][l] - 1  );
                                        }
                                    }
                                    tempContent.SoftlyMergedLayers = tempSoffMergedLayers.ToArray();
                                }

                            //  Add content to temp content list
                                tempCellContent.Add(tempContent);
                                CellContentOffset += 1;
                            }
                            density = 0;
                        }
                    }
                }
            ArrayOffset += (int)TerrainDetailSize.x;                
            }
            CellContent = tempCellContent.ToArray();
            tempCellContent.Clear();
        }


//  --------------------------------------------------------------------
//  Init Grid, Culling groups and all the rest

        public void Init() {
            // Just for the case that there is no wind script.
            Shader.SetGlobalFloat("_AtgWindGust", 0);    
            Shader.SetGlobalVector("_AtgWindDirSize", new Vector4(1,0,0,0));
            Shader.SetGlobalVector("_AtgWindStrengthMultipliers", new Vector4(0,0,0,0) );

            ter = GetComponent<Terrain>();
            terData = ter.terrainData;

        //  Hide grass from the terrain
            ter.detailObjectDistance = 0;

            CurrentDetailDensity = DetailDensity;
            
            TerrainPosition = ter.GetPosition();
            TerrainSize = terData.size;
            OneOverTerrainSize.x = 1.0f/TerrainSize.x;
            OneOverTerrainSize.y = 1.0f/TerrainSize.y;
            OneOverTerrainSize.z = 1.0f/TerrainSize.z;
            TerrainDetailSize.x = terData.detailWidth;
            TerrainDetailSize.y = terData.detailHeight;

        //  We assume squared Terrains here...
            BucketSize = TerrainSize.x / TerrainDetailSize.x;
            // OneOverBucketSize = 1.0f / BucketSize;
            // Number of buckets or detail map pixels per cell per axis. Must be 2^x! as it has to fit the detail resolution.
            NumberOfBucketsPerCell = (int)NumberOfBucketsPerCellEnum;
            CellSize = NumberOfBucketsPerCell * BucketSize;
            NumberOfCells = (int) (TerrainSize.x / CellSize);
            TotalCellCount = NumberOfCells * NumberOfCells;

            sh = Shader.Find("AdvancedTerrainGrass/Grass Base Shader");

            GrassMatrixBufferPID = Shader.PropertyToID("GrassMatrixBuffer");
            GrassNormalBufferPID = Shader.PropertyToID("GrassNormalBuffer");
            GrassFadePropsPID = Shader.PropertyToID("_AtgGrassFadeProps");
            GrassShadowFadePropsPID = Shader.PropertyToID("_AtgGrassShadowFadeProps");

            var CullingRadius1 = Mathf.Sqrt(CellSize * CellSize * 2.0f);
            CullDistance = CullingRadius1 * Mathf.Round(CullDistance / CullingRadius1);
            
            Shader.SetGlobalVector(GrassFadePropsPID, new Vector4(
                (CullDistance - FadeLength) * (CullDistance - FadeLength),
                1.0f / (FadeLength * FadeLength),
                DetailFadeStart * DetailFadeStart,
                1.0f / (DetailFadeLength * DetailFadeLength)
            ));

            Shader.SetGlobalVector(GrassShadowFadePropsPID, new Vector4(
                ShadowStart * ShadowStart,
                1.0f / (ShadowFadeLength * ShadowFadeLength),
                ShadowStartFoliage * ShadowStartFoliage,
                1.0f / (ShadowFadeLengthFoliage * ShadowFadeLengthFoliage)
            ));

            if(SavedTerrainData != null) {
                InitCellsFast(); 
            //  Just to make sure
                TotalCellCount = Cells.Length; 
            }
            else {
                InitCells();  
            }

        //  Get Heights
            TerrainHeightmapWidth = terData.heightmapWidth;
            TerrainHeightmapHeight = terData.heightmapHeight;
            TerrainHeights = new float[TerrainHeightmapWidth * TerrainHeightmapHeight];
            for(int x = 0; x < TerrainHeightmapWidth; x++) {
                for(int z = 0; z < TerrainHeightmapHeight; z++) {
                    TerrainHeights[ x * TerrainHeightmapWidth + z ] = terData.GetHeight(x, z);
                }
            }
            OneOverHeightmapWidth = 1.0f / TerrainHeightmapWidth;
//            OneOverHeightmapHeight = 1.0f / TerrainHeightmapHeight;
            TerrainSizeOverHeightmap = TerrainSize.x / TerrainHeightmapWidth;
        //  Needed guards for normal sampling
            OneOverHeightmapWidthRight = TerrainSize.x - 2*(TerrainSize.x/ (TerrainHeightmapWidth - 1))     - 1;    
            //OneOverHeightmapWidthUp = TerrainSize.z - 2*(TerrainSize.z / (TerrainHeightmapHeight - 1));

        //  Set up CullingGroup
            cullingGroup = new CullingGroup();
            //cullingGroup.targetCamera = Camera.main;
            boundingSpheres = new BoundingSphere[TotalCellCount];
            resultIndices = new int[TotalCellCount];
            var CullingRadius = Mathf.Sqrt(CellSize * CellSize * 2.0f);
            for(int i = 0; i < TotalCellCount; i++) {
                boundingSpheres[i] = new BoundingSphere( Cells[i].Center, CullingRadius);
            }
            cullingGroup.SetBoundingSpheres(boundingSpheres);
            cullingGroup.SetBoundingSphereCount(TotalCellCount);
            float[] distances = new float[] {CullDistance, CacheDistance};
            
            cullingGroup.SetBoundingDistances(distances);
            cullingGroup.onStateChanged = StateChangedMethod;

            SqrTerrainCullingDist = Mathf.Sqrt(TerrainSize.x * TerrainSize.x + TerrainSize.z * TerrainSize.z) + CullDistance;
            SqrTerrainCullingDist *= SqrTerrainCullingDist;

Debug.Log("Max Bucket Density: " + maxBucketDensity);
        //  Set up tempMatrixArray
            var maxInstances = Mathf.CeilToInt(NumberOfBucketsPerCell * NumberOfBucketsPerCell * maxBucketDensity * CurrentDetailDensity);
            tempMatrixArray = new Matrix4x4[maxInstances];
            tempNormalArray = new Vector3[maxInstances];
Debug.Log("Max Instances per Layer per Cell: " + maxInstances);

        //  Burst init – if checked
            if(useBurst){
                BurstInit();
            }
        }


//  --------------------------------------------------------------------
//  Function to immediately init all cells within the user defined BurstRadius

        public void BurstInit() {
            
            if(Cam == null) {
                Cam = Camera.main;
                if(Cam == null) {
                    return;
                }
            }

            CamTransform = Cam.transform;

            if( (TerrainPosition - CamTransform.position).sqrMagnitude > SqrTerrainCullingDist ) {
                return;
            }

        //  Loop over all Cells
            int t_NumberOfCells = Cells.Length;
            for (int i = 0; i < t_NumberOfCells; i ++) {
                var CurrentCell = Cells[i];
                float distance = Vector3.Distance(CamTransform.position, CurrentCell.Center);
                int Layer;

            //  Find Cells in BurstRadius – Please note: Culling groups are not available here (first frame!). So we do it manually.
                if (distance < BurstRadius) {

                    var NumberOfLayersInCell = CurrentCell.CellContentCount;
                //  A) ---- one layer 
                    if (!initCompleteCell) {

                        for (int j = 0; j < NumberOfLayersInCell; j ++) {
                            var CurrentCellContent = CellContent[ CurrentCell.CellContentIndexes[j] ];
                            Layer = CurrentCellContent.Layer;
#if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                            CurrentCellContent.v_mat = new Material(v_mat[Layer]);
#else
                            CurrentCellContent.v_mat = v_mat[Layer];
#endif
                            CurrentCellContent.v_mesh = v_mesh[Layer];
                            CurrentCellContent.ShadowCastingMode = ShadowCastingMode[Layer];
                            InitCellContent( Layer, CurrentCell.CellContentIndexes[j], 0, false );
                            ThreadIsRunning = false;
                            CurrentCellContent.InitCellContent_Delegated();
                        }
                    }
                //  B) ---- complete cell
                    else {
                        for (int j = 0; j < NumberOfLayersInCell; j ++) {
                            var CurrentCellContent = CellContent[ CurrentCell.CellContentIndexes[j] ];
                            Layer = CurrentCellContent.Layer;
#if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                            CurrentCellContent.v_mat = new Material(v_mat[Layer]);
#else
                            CurrentCellContent.v_mat = v_mat[Layer];
#endif
                            CurrentCellContent.v_mesh = v_mesh[Layer];
                            CurrentCellContent.ShadowCastingMode = ShadowCastingMode[Layer];
                        }
                        InitCellContent( 0, 0, CurrentCell.index, true );
                        ThreadIsRunning = false;
                        for (int j = 0; j < NumberOfLayersInCell; j ++) {
                            var CurrentCellContent = CellContent[ CurrentCell.CellContentIndexes[j] ];
                            CurrentCellContent.InitCellContent_Delegated();
                        }   
                    }
                    CurrentCell.state = 3;
                }
            }
        }

//  --------------------------------------------------------------------
//  Culling group callback methods
    
        private void StateChangedMethod(CullingGroupEvent evt)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Release Cell");
        //  Method to release cells
            if(evt.currentDistance == 2 && evt.previousDistance == 1) {
                var CurrentCell = Cells[evt.index];
                int NumberOfLayersInCell = CurrentCell.CellContentCount;
                for (int j = 0; j < NumberOfLayersInCell; j++) {
                    CellContent[ CurrentCell.CellContentIndexes[j] ].ReleaseCellContent();

#if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                        Destroy(CellContent[ CurrentCell.CellContentIndexes[j] ].v_mat);
#endif
                }
                CurrentCell.state = 0;
            //  What happens to already queued cells here? --> ReleaseCell() sets isQueued = false;
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

//  --------------------------------------------------------------------
//  The real update function which culls and queues grass cells and draws the finished ones

        void DrawGrass() {
                
        //  First check if the terrain is within the culling distance
            Cam = Camera.main;
            if (Cam == null) {
                return;
            }
            else {
                CamTransform = Cam.transform;
                cullingGroup.targetCamera = Cam;
            }

            if( (TerrainPosition - CamTransform.position).sqrMagnitude > SqrTerrainCullingDist ) {
                return;
            }

            if((int)CameraSelection == 0) {
                CameraInWichGrassWillBeDrawn = null;
            }
            else {
                CameraInWichGrassWillBeDrawn = Cam;
            }

        //  Get all visible Cells
            cullingGroup.SetDistanceReferencePoint(CamTransform.position);
            if(IngnoreOcclusion) {
                numResults = cullingGroup.QueryIndices(0, resultIndices, 0);   
            }
            else {
                numResults = cullingGroup.QueryIndices(true, 0, resultIndices, 0);
            }

        //  CullingGroup most likely did not return a valid result... (which happens in the first frame)
            if(numResults == TotalCellCount) {
                return;
            }

            GrassCell CurrentCell;
            int CellState;
            GrassCellContent CurrentCellContent;
            int CellContentState;
            int NumberOfLayersInCell;

        //  -----
        //  A) Always init only one single cellcontent

            if (!initCompleteCell) {
            //  Loop over visible Cells.
                for (int i = 0; i < numResults; i ++) {

                    CurrentCell = Cells[ resultIndices[i] ];
                    CellState = CurrentCell.state;
                    NumberOfLayersInCell = CurrentCell.CellContentCount;

                    for (int j = 0; j < NumberOfLayersInCell; j++) {
                        CurrentCellContent = CellContent[ CurrentCell.CellContentIndexes[j] ];
                        CellContentState = CurrentCellContent.state;
                        switch (CellContentState) {
                        //  Draw the cellcontent if it is ready.
                            case 3 :
                                UnityEngine.Profiling.Profiler.BeginSample("Draw Grass");
                                    CurrentCellContent.DrawCellContent_Delegated( CameraInWichGrassWillBeDrawn, CameraLayer );
                                UnityEngine.Profiling.Profiler.EndSample();
                                break;
                        //  Finalize the initialisation of last updated cellcontent – which has to be done on the main thread. Then draw.
                            case 2 :
                                CurrentCellContent.InitCellContent_Delegated();
                                UnityEngine.Profiling.Profiler.BeginSample("Draw Grass");
                                    CurrentCellContent.DrawCellContent_Delegated( CameraInWichGrassWillBeDrawn, CameraLayer );
                                UnityEngine.Profiling.Profiler.EndSample();
                                break;

                        //  Otherwise we have to add the cell content to the list of cell contents to be initialised.
                            case 0 :
                                if ( CurrentCell.CellContentCount > 0) {
                                    //CurrentCellContent.isQueued = true;
                                    CurrentCellContent.state = 1;
                                    CellsOrCellContentsToInit.Add( CurrentCellContent.index );
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

            //  Initialise the next queued cell content
                var CellsOrCellContentsToInitCount = CellsOrCellContentsToInit.Count;
                if (CellsOrCellContentsToInitCount > 0) {
                    if (!ThreadIsRunning) {
                    //  Check if the cell has already been released 
                        if (CellContent[ CellsOrCellContentsToInit[0] ].state != 1) {
                            CellsOrCellContentsToInit.RemoveAt(0);
                            if(CellsOrCellContentsToInitCount == 1) {
                                return;
                            }
                        }
                        CurrentCellContent = CellContent[CellsOrCellContentsToInit[0]];
                        var Layer = CurrentCellContent.Layer;
                        // As Unity < 5.6.3 does not support MaterialPropertyBlocks properly

#if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                        CurrentCellContent.v_mat = new Material(v_mat[Layer]);
#else
                        CurrentCellContent.v_mat = v_mat[Layer];
#endif
                        CurrentCellContent.v_mesh = v_mesh[Layer];
                        CurrentCellContent.ShadowCastingMode = ShadowCastingMode[Layer];
                       
                    //  Create Matrices asynchronously
                        del = new GrassDelegate(InitCellContent);
                        AsyncCallback callback = new AsyncCallback(InitCellContent_Callback);
                        del.BeginInvoke( Layer, CellsOrCellContentsToInit[0], 0, false,    callback, null);

                    //  Create Matrices on the main thread
                    //  InitCellContent( Layer, CellsOrCellContentsToInit[0], 0, false );
                    //  ThreadIsRunning = false;
                        
                        CellsOrCellContentsToInit.RemoveAt(0);
                    }
                }
            } // A) End

        //  -----
        //  B) Init all cellcontents of a given cell at once

            else {
            //  Loop over visible Cells.
                for (int i = 0; i < numResults; i ++) {

                    CurrentCell = Cells[ resultIndices[i] ];
                    CellState = CurrentCell.state;
                    NumberOfLayersInCell = CurrentCell.CellContentCount;

                    switch(CellState) {
                    //  Draw
                        case 3 :
                            UnityEngine.Profiling.Profiler.BeginSample("Draw Grass");
                            for (int j = 0; j < NumberOfLayersInCell; j++) {
                                CellContent[ CurrentCell.CellContentIndexes[j] ].DrawCellContent_Delegated( CameraInWichGrassWillBeDrawn, CameraLayer );
                            }
                            UnityEngine.Profiling.Profiler.EndSample();
                            break;
                    //  Init and Draw
                        case 2 :
                            UnityEngine.Profiling.Profiler.BeginSample("InitCellContent Delegated");
                        //  Finalize the initialisation of last updated cell – which has to be done on the main thread.
                            for (int j = 0; j < NumberOfLayersInCell; j++) {
                                CellContent[ CurrentCell.CellContentIndexes[j] ].InitCellContent_Delegated();
                            }
                            CurrentCell.state = 3;
                            UnityEngine.Profiling.Profiler.EndSample();
                        //  Draw
                            UnityEngine.Profiling.Profiler.BeginSample("Draw Grass");
                            for (int j = 0; j < NumberOfLayersInCell; j++) {
                                CellContent[ CurrentCell.CellContentIndexes[j] ].DrawCellContent_Delegated( CameraInWichGrassWillBeDrawn, CameraLayer );
                            }
                            UnityEngine.Profiling.Profiler.EndSample();
                            break;
                    //  Queue Cell
                        case 0 :
                            if (CurrentCell.CellContentCount > 0) {
                                CurrentCell.state = 1;
                                CellsOrCellContentsToInit.Add( CurrentCell.index );
                            }
                            break;
                    //  Cell is queued but not finished yet – nothing to do.
                        default:
                            break;
                    }
                }

            //  Initialise the next queued cell
                var CellsOrCellContentsToInitCount = CellsOrCellContentsToInit.Count;
                if (CellsOrCellContentsToInitCount > 0) {
                    if (!ThreadIsRunning) {

                        UnityEngine.Profiling.Profiler.BeginSample("Queue Cell");

                    //  Check if the cell has already been released 
                        if (Cells[ CellsOrCellContentsToInit[0] ].state != 1) {
                            CellsOrCellContentsToInit.RemoveAt(0);
                            if(CellsOrCellContentsToInitCount == 1) {
                                return;
                            }
                        }
                        CurrentCell = Cells[CellsOrCellContentsToInit[0]];
                        NumberOfLayersInCell = CurrentCell.CellContentCount;
                        int Layer = 0;
                        for (int j = 0; j < NumberOfLayersInCell; j++) {
                            CurrentCellContent = CellContent[ CurrentCell.CellContentIndexes[j] ];
                            Layer = CurrentCellContent.Layer;
                            // As Unity < 5.6.3 does not support MaterialPropertyBlocks properly
#if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                            CurrentCellContent.v_mat = new Material(v_mat[Layer]);
#else
                            CurrentCellContent.v_mat = v_mat[Layer];
#endif
                            CurrentCellContent.v_mesh = v_mesh[Layer];
                            CurrentCellContent.ShadowCastingMode = ShadowCastingMode[Layer];
                        }
                    //  Create Matrices asynchronously
                        del = new GrassDelegate(InitCellContent);
                        AsyncCallback callback = new AsyncCallback(InitCellContent_Callback);
                        del.BeginInvoke( Layer, 0, CellsOrCellContentsToInit[0], true, callback, null);

                    //  Create Matrices on the main thread
                    //  InitCellContent( Layer, 0, CellsOrCellContentsToInit[0], true );
                    //  ThreadIsRunning = false;

                        CellsOrCellContentsToInit.RemoveAt(0);
                        UnityEngine.Profiling.Profiler.EndSample();
                    }
                    
                }
            } // B) End
        } // End Update


//  --------------------------------------------------------------------
//  From here on everything has to be thread safe

//  --------------------------------------------------------------------
//  Bilinear sampling from the height array

        public float GetfilteredHeight(float normalizedHeightPos_x, float normalizedHeightPos_y) {
            //  NOTE: Using Floor and Ceil each take 0.8ms - so we go with (int) instead
            int normalizedHeightPosLower_x = (int)(normalizedHeightPos_x);
            int normalizedHeightPosLower_y = (int)(normalizedHeightPos_y);
            int normalizedHeightPosUpper_x = (int)(normalizedHeightPos_x + 1.0f);
            int normalizedHeightPosUpper_y = (int)(normalizedHeightPos_y + 1.0f); 

        //  Get weights
            float Lowerx = (normalizedHeightPos_x - normalizedHeightPosLower_x);
            float Upperx = (normalizedHeightPosUpper_x - normalizedHeightPos_x);
            float Lowery = (normalizedHeightPos_y - normalizedHeightPosLower_y);
            float Uppery = (normalizedHeightPosUpper_y - normalizedHeightPos_y);

        //  Adjust x positions to match our array
            normalizedHeightPosLower_x *= TerrainHeightmapHeight; // "* height" as we sample height in the inner loop
            normalizedHeightPosUpper_x *= TerrainHeightmapHeight;

            // NOTE: We simply use "swapped weights" in order to not have to calculate (1 - factor)
            float HeightSampleLowerRow =  TerrainHeights [ normalizedHeightPosLower_x + normalizedHeightPosLower_y ] * Upperx;
                  HeightSampleLowerRow += TerrainHeights [ normalizedHeightPosUpper_x + normalizedHeightPosLower_y ] * Lowerx;
            float HeightSampleUpperRow =  TerrainHeights [ normalizedHeightPosLower_x + normalizedHeightPosUpper_y ] * Upperx;
                  HeightSampleUpperRow += TerrainHeights [ normalizedHeightPosUpper_x + normalizedHeightPosUpper_y ] * Lowerx;
            return HeightSampleLowerRow * Uppery + HeightSampleUpperRow * Lowery;
        }

//  --------------------------------------------------------------------
//  Our callback methode.
        public void InitCellContent_Callback(IAsyncResult arg)
        {
            float result = del.EndInvoke(arg);
            ThreadIsRunning = false;
        }

//  --------------------------------------------------------------------
//  The function which runs on the worker thread.
        public float InitCellContent(int layer, int index, int cellIndex, bool b_initCompleteCell)
        {
            ThreadIsRunning = true;

            // B) index points to the cell not the content            
            var CurrentCell = Cells[cellIndex]; // A) Just a dummy value here (0) B) index of cell
            var CurrentCellContent = CellContent[index];

            int startIndex = index;
            int loopCounter = index + 1;
            
            // -- B)
            if (b_initCompleteCell) {
                CurrentCell = Cells[ cellIndex ]; // now we get the real value
                startIndex = 0;
                loopCounter = CurrentCell.CellContentCount;
            }

            // Loop in case B) we have to init multiple Cell Contents
            for (int c = startIndex; c < loopCounter; c ++) {

                //  Here we have to overwrite the initial value
                    if (b_initCompleteCell) {
                        index = CurrentCell.CellContentIndexes[c];
                        CurrentCellContent = CellContent[index];
                        layer = CurrentCellContent.Layer;
                    }
                    
                    //CurrentCellContent.matrices_list.Clear();
                    //CurrentCellContent.matrices_list.Capacity = (int) (CellContent[index].Instances        * CurrentDetailDensity);
                    int tempMatrixArrayPointer = 0;

                    samplePosition.x = CurrentCellContent.Pivot.x;
                    samplePosition.y = CurrentCellContent.Pivot.z;
                    tempSamplePosition = samplePosition;

                    var Rotation = (int)InstanceRotation[layer];
                    var DoWriteNormalBuffer = WriteNormalBuffer[layer];

                    var noise = Noise[layer];

                //  In case we do not align the instances to the terrain we have to store the terrain normal
                //    if (Rotation == 2 && DoWriteNormalBuffer ) {
                //        CurrentCellContent.normals_list.Clear();
                //        CurrentCellContent.normals_list.Capacity = (int) (CellContent[index].Instances     * CurrentDetailDensity);
                //    }

                    float tempMinSize = MinSize[layer];
                    float tempMaxSize = MaxSize[layer] - tempMinSize;

                //  Outer loop for softly merged layers
                    int outerLoop = 1;

                    if (CurrentCellContent.SoftlyMergedLayers != null) {
                        outerLoop += CurrentCellContent.SoftlyMergedLayers.Length;
                    }

                    for (int layers = 0; layers < outerLoop; layers ++) {

                    //  This still creates a lot of garbage..
                        random = new System.Random(cellIndex + layer + layers * 9949  ); // + layers as otherwise simple quads might fall upon each other

                    //  Reset
                        tempSamplePosition = samplePosition;
                        
                    //  Tweak layer to point at softly merged Layer
                        if( layers > 0) {
                            layer = CurrentCellContent.SoftlyMergedLayers[ layers - 1 ];
                            noise = Noise[layer];
                            tempMinSize = MinSize[layer];
                            tempMaxSize = MaxSize[layer] - tempMinSize;
                        }

                        for(int x = 0; x < NumberOfBucketsPerCell; x++) {
                            for(int z = 0; z < NumberOfBucketsPerCell; z++) {              
                            //  worldPos --> localPos as all terrain data is relative
                                Vector2 terrainLocalPos;
                                // per component: 21ms vs Vector3: 23ms (10000 times)
                                terrainLocalPos.x = tempSamplePosition.x;
                                terrainLocalPos.y = tempSamplePosition.y;

                                int density = mapByte[layer][ 
                                    CurrentCellContent.PatchOffsetX + CurrentCellContent.PatchOffsetZ
                                    + x * (int)TerrainDetailSize.y
                                    + z
                                ];
                              
                            //  We have to use Ceil here as otherwise we may create empty buffers.
                                density = (int)Mathf.Ceil(density                                         * CurrentDetailDensity);

                            //  When sampling the height map we may not go (x || y < 0). So we use clamped factors:
                                float OneOverHeightmapWidthClamped = (terrainLocalPos.x < TerrainSizeOverHeightmap)? 0.0f : OneOverHeightmapWidth;
                                float OneOverHeightmapWidthClampedRight = (terrainLocalPos.x >= OneOverHeightmapWidthRight )? 0.0f : OneOverHeightmapWidth;    
                            //    float OneOverHeightmapHeightClamped = (terrainLocalPos.y < TerrainSizeOverHeightmap)? 0.0f : OneOverHeightmapHeight;
                                //Debug.Log(terrainLocalPos.y);
                                //Debug.Log("Guard: " + (TerrainSize.z - 2*(TerrainSize.z / (TerrainHeightmapHeight - 1)) ) );
                            //    float OneOverHeightmapWidthClampedUp = (terrainLocalPos.y >= OneOverHeightmapWidthUp )? 0.0f : OneOverHeightmapHeight;

                            //  Now that we have the density for the Bucket we spawn x instances based on density within the given Bucket.

                                for(int i = 0; i < density; i++) {

                                //  Random Offsets
                                // float rand = ( (float)random.Next(0, 10000)) * 0.0001f; //Random01();
                                // A tiny bit faster
                                    float rand = (float)random.NextDouble();
                                    float XOffset = rand * BucketSize; //UnityEngine.Random.value * BucketSize;
                                    //rand = ( (float)random.Next(0, 10000)) * 0.0001f; //Random01();
                                    rand = (float)random.NextDouble();
                                    float ZOffset = rand * BucketSize; //UnityEngine.Random.value * BucketSize;

                                //  localPos --> normalizedPos in 0-1 range to sample height and normal
                                //  0.4ms – most expensive part here
                                //  UnityEngine.Profiling.Profiler.BeginSample("read from terrain");
                                    Vector2 normalizedPos;
                                    normalizedPos.x = (terrainLocalPos.x + XOffset) * OneOverTerrainSize.x;
                                    normalizedPos.y = (terrainLocalPos.y + ZOffset) * OneOverTerrainSize.z;

                                //  UnityEngine.Profiling.Profiler.BeginSample("manually sample height"); // manually: 0.18ms / built in: 0.3ms!!!!!
                                //  tempPosition.y = terData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);   // 0.08ms

                                //  Get bilinear filtered height value, see: https://en.wikipedia.org/wiki/Bilinear_interpolation
                                    float normalizedHeightPos_x = normalizedPos.x * (TerrainHeightmapWidth  - 1); // "- 1" because we are working with indexes here
                                    float normalizedHeightPos_y = normalizedPos.y * (TerrainHeightmapHeight - 1);
                                    
                                //  NOTE: Using Floor and Ceil each take 0.8ms - so we go with (int) instead
                                    int normalizedHeightPosLower_x = (int)(normalizedHeightPos_x);
                                    int normalizedHeightPosLower_y = (int)(normalizedHeightPos_y);
                                    int normalizedHeightPosUpper_x = (int)(normalizedHeightPos_x + 1.0f);
                                    int normalizedHeightPosUpper_y = (int)(normalizedHeightPos_y + 1.0f); 

                                //  Get weights
                                    float Lowerx = (normalizedHeightPos_x - normalizedHeightPosLower_x);
                                    float Upperx = (normalizedHeightPosUpper_x - normalizedHeightPos_x);
                                    float Lowery = (normalizedHeightPos_y - normalizedHeightPosLower_y);
                                    float Uppery = (normalizedHeightPosUpper_y - normalizedHeightPos_y);

                                //  Adjust x positions to match our array
                                    normalizedHeightPosLower_x *= TerrainHeightmapHeight; // "* height" as we sample height in the inner loop
                                    normalizedHeightPosUpper_x *= TerrainHeightmapHeight;

                                    // NOTE: We simply use "swapped weights" in order to not have to calculate (1 - factor)
                                    float HeightSampleLowerRow =  TerrainHeights [ normalizedHeightPosLower_x + normalizedHeightPosLower_y ] * Upperx;
                                          HeightSampleLowerRow += TerrainHeights [ normalizedHeightPosUpper_x + normalizedHeightPosLower_y ] * Lowerx;
                                    float HeightSampleUpperRow =  TerrainHeights [ normalizedHeightPosLower_x + normalizedHeightPosUpper_y ] * Upperx;
                                          HeightSampleUpperRow += TerrainHeights [ normalizedHeightPosUpper_x + normalizedHeightPosUpper_y ] * Lowerx;
                                //  Final bilinear filtered height value
                                    tempPosition.y = HeightSampleLowerRow * Uppery + HeightSampleUpperRow * Lowery;
                            
                                //  UnityEngine.Profiling.Profiler.EndSample();
                                //  UnityEngine.Profiling.Profiler.BeginSample("manually sample normal");

                                    Vector3 normal; // = Vector3.Cross(va, vb);
                                //  NOTE: When looking up the height map we need guards!!!!!!                       
                                    float Left = GetfilteredHeight( 
                                                    (normalizedPos.x - OneOverHeightmapWidthClamped) * (TerrainHeightmapWidth - 1),
                                                    normalizedPos.y * (TerrainHeightmapHeight - 1)
                                                );
                                    float Right = GetfilteredHeight( 
                                                    (normalizedPos.x + OneOverHeightmapWidthClampedRight) * (TerrainHeightmapWidth - 1),
                                                    normalizedPos.y * (TerrainHeightmapHeight - 1)
                                                );
                                    float Up = GetfilteredHeight( 
                                                    normalizedPos.x * (TerrainHeightmapWidth - 2), // 2 to make the last cell 
                                                    //(normalizedPos.y + OneOverHeightmapWidthClampedUp) * (TerrainHeightmapHeight - 1)
                                                    normalizedHeightPosUpper_y
                                                );
                                    float Down = GetfilteredHeight( 
                                                    normalizedPos.x * (TerrainHeightmapWidth - 1),
                                                    //(normalizedPos.y - OneOverHeightmapHeightClamped) * (TerrainHeightmapHeight - 1)
                                                    normalizedHeightPosLower_y
                                                );


                                    // https://stackoverflow.com/questions/33736199/calculating-normals-for-a-height-map
                                    // Vector3 vertical = new Vector3(0.0f, Up - Down, 2.0f );
                                    // Vector3 horizontal = new Vector3(2.0f, Right - Left, 0.0f );
                                    // normal = Vector3.Cross(vertical, horizontal);
                                    // Manually calculated cross product:
                                    normal.x = -2.0f * (Right - Left);
                                //  if aligned to terrain
                                    if(Rotation != 2  && Rotation != 4 ) {
                                        normal.y = 2.0f * 2.0f    *   1.570796f    *   TerrainSizeOverHeightmap; // Magic number? At least it is half PI.
                                    }
                                //  upright oriented
                                    else {
                                        normal.y = 2.0f * 2.0f * TerrainSizeOverHeightmap; 
                                    }
                                    normal.z = (Up - Down) * -2.0f;
                                //  normal.Normalize(); // Takes about 0.77ms for 593 calls! – doing it manually only takes 0.14ms
                                    float lengthSqr = normal.x * normal.x + normal.y * normal.y + normal.z * normal.z;
                                    float length = (float)Math.Sqrt((double)lengthSqr);
                                    float inverseLength = 1.0f / length;
                                    normal.x *= inverseLength;
                                    normal.y *= inverseLength;
                                    normal.z *= inverseLength;

                                //  normal  = terData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);  // 0.5ms!!!!!! super expensive, so we better cache it in an array?
                                //  UnityEngine.Profiling.Profiler.EndSample();
                                //  UnityEngine.Profiling.Profiler.EndSample();

                                //  Set Position in Worldspace
                                    tempPosition.x = tempSamplePosition.x + XOffset   +   TerrainPosition.x;
                                    tempPosition.z = tempSamplePosition.y + ZOffset   +   TerrainPosition.z;

                                //  Set scale
                                    float scale = tempMinSize + Mathf.PerlinNoise(tempPosition.x * noise, tempPosition.z * noise) * tempMaxSize;
                                //  IMPORTANT: We have to use uniform scaling as otherwise we would need a proper WorldToObject matrix or hack UnityShaderUtilities.cginc
                                    tempScale.x = scale;
                                    tempScale.y = scale;
                                    tempScale.z = scale;

                                //  Align grass to terrain normal / fromto rotation
                                //  q = Quaternion.FromToRotation(UpVec, normal);
                                //  https://stackoverflow.com/questions/1171849/finding-quaternion-representing-the-rotation-from-one-vector-to-another
                                    Quaternion q = ZeroQuat;
                                    if(Rotation != 2) {
                                    /*  Vector3 a = Vector3.Cross(UpVec, normal);
                                        q.x = a.x;
                                        q.y = a.y;
                                        q.z = a.z; */
                                    //  As we use the up vector we can simplify it like this:
                                        q.x = normal.z;
                                        q.y = 0;
                                        q.z =  -normal.x;
                                        //q.w = Mathf.Sqrt(( v1.Magnitude ^ 2) * ( v2.Length ^ 2)) + dotproduct(v1, v2);
                                    //  Another simplification here:
                                        // q.w = Mathf.Sqrt( 1.0f + Vector3.Dot(Vector3.up, normal) );
                                        q.w = (float)Math.Sqrt( 1.0f + normal.y);
                                    //  Normalize!
                                        var OneOverqMagnitude = (float)(1.0 / Math.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z));
                                        q.w *= OneOverqMagnitude;
                                        q.x *= OneOverqMagnitude;
                                        q.y *= OneOverqMagnitude;
                                        q.z *= OneOverqMagnitude;
                                    }
                                    
                                //  Now add random rotation around y axis
                                    float HalfAngle = (float)random.Next(0, 180);

                                    float Cos = (float)Math.Cos(HalfAngle);
                                    float Sin = (float)Math.Sin(HalfAngle);
                                /*  q *= rotation;
                                    which is: return new Quaternion(
                                        lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
                                        lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
                                        lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
                                        lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z); */
                                //  As we only rotate around the y axis, we can do better:
                                    var lhs = q;
                                    q.x = lhs.x * Cos - lhs.z * Sin;
                                    q.y = lhs.w * Sin + lhs.y * Cos;
                                    q.z = lhs.z * Cos + lhs.x * Sin;
                                    q.w = lhs.w * Cos - lhs.y * Sin;
                                //  Rotation around x axis (for e.g. small stones)
                                    if(Rotation == 1) {
                                        HalfAngle = (float)random.Next(0, 180);
                                        float SinX = (float)Math.Sin(HalfAngle); // --> rhs.x
                                        float CosX = (float)Math.Cos(HalfAngle); // --> rhs.w

                                        lhs = q;
                                        q.x = lhs.w * SinX + lhs.x * CosX;
                                        q.y = lhs.y * CosX + lhs.z * SinX;
                                        q.z = lhs.z * CosX - lhs.y * SinX;
                                        q.w = lhs.w * CosX - lhs.x * SinX;
                                    }
                                //  q.x = lhs.w * SinX + lhs.x * Cos /*+ lhs.y * rhs.z*/ - lhs.z * Sin;
                                //  q.y = lhs.w * Sin + lhs.y * CosX + lhs.z * SinX /*- lhs.x * rhs.z*/;
                                //  q.z = /*lhs.w * rhs.z +*/ lhs.z * Cos + lhs.x * Sin - lhs.y * SinX;
                                //  q.w = lhs.w * Cos - lhs.x * SinX - lhs.y * Sin /*- lhs.z * rhs.z*/;
                                    
                                //  Set Matrix TRS   
                                    //  UnityEngine.Profiling.Profiler.BeginSample("set");
                                    //  tempMatrix.SetTRS( tempPosition, q, tempScale );
                                    //  this is almost 1/3rd of the time of the whole loop! 0.8ms out of 2.3ms
                                    //  Doing it manually takes 2.5 – 3.6ms (10.000 calls) (0.6 - 0.8 ms in built) ---> 2.6 times faster! (1.5 times faster in built)
                                    //  Matrix needs to be normalized!?

                                //  Set position
                                    tempMatrix.m03 = tempPosition.x;
                                    tempMatrix.m13 = tempPosition.y + TerrainPosition.y; // Add terrain y pos
                                    tempMatrix.m23 = tempPosition.z;

                                //  Set rotation
                                    var DoubleSqrqx = 2.0f * q.x*q.x;
                                    var DoubleSqrqy = 2.0f * q.y*q.y;
                                    var DoubleSqrqz = 2.0f * q.z*q.z;

                                    tempMatrix.m00 = 1.0f - DoubleSqrqy/*2.0f * q.y*q.y*/ - DoubleSqrqz /* 2.0f * q.z*q.z*/;
                                    tempMatrix.m01 = 2.0f * q.x * q.y - 2.0f * q.z * q.w;
                                    tempMatrix.m02 = 2.0f * q.x * q.z + 2.0f * q.y * q.w;
                                    
                                    tempMatrix.m10 = 2.0f * q.x * q.y + 2.0f * q.z * q.w;
                                    tempMatrix.m11 = 1.0f - DoubleSqrqx /* 2.0f * q.x*q.x*/ - DoubleSqrqz /* 2.0f * q.z*q.z*/;
                                    tempMatrix.m12 = 2.0f * q.y * q.z - 2.0f * q.x * q.w;

                                    tempMatrix.m20 = 2.0f * q.x * q.z - 2.0f * q.y * q.w;
                                    tempMatrix.m21 = 2.0f * q.y * q.z + 2.0f * q.x * q.w;
                                    tempMatrix.m22 = 1.0f - DoubleSqrqx/* 2.0f * q.x*q.x*/ - DoubleSqrqy/* 2.0f * q.y*q.y*/;

                                //  Set scale
                                    tempMatrix.m00 *= tempScale.x;
                                    tempMatrix.m01 *= tempScale.y;
                                    tempMatrix.m02 *= tempScale.z;
                                    tempMatrix.m10 *= tempScale.x;
                                    tempMatrix.m11 *= tempScale.y;
                                    tempMatrix.m12 *= tempScale.z;
                                    tempMatrix.m20 *= tempScale.x;
                                    tempMatrix.m21 *= tempScale.y;
                                    tempMatrix.m22 *= tempScale.z;

                                //  UnityEngine.Profiling.Profiler.EndSample();
                                    
                                    if(Rotation == 2 && DoWriteNormalBuffer) {
                                    //  As the terrain normal will be rotated around the y axis (HalfAngle) by the unity_ObjectToWorld matrix we have to rotate it the other way around
                                        var rotatedTerrainNormal = normal;
                                        float MinusHalfAngle = -HalfAngle;
                                        Cos = (float)Math.Cos(MinusHalfAngle);
                                        Sin = (float)Math.Sin(MinusHalfAngle);
                                    //  Build and apply a simplified quaternion
                                        float num2 = Sin * 2.0f;
                                        float num5 = Sin * num2;
                                        float num11 = Cos * num2;
                                        rotatedTerrainNormal.x = (1.0f - (num5)) * normal.x + (num11) * normal.z;
                                        rotatedTerrainNormal.z = (- num11) * normal.x + (1.0f - (num5)) * normal.z;
                                    //  Add terrain normal to list
                                        //CurrentCellContent.normals_list.Add(rotatedTerrainNormal);
                                        //CurrentCellContent.normals_list.Add(new Vector4(normal.x, normal.y, normal.z, 0)); // GL needs vec4???
                                        //tempNormalArray[tempMatrixArrayPointer] = rotatedTerrainNormal;
                                        tempMatrix.m30 = rotatedTerrainNormal.x;
                                        tempMatrix.m31 = rotatedTerrainNormal.y;
                                        tempMatrix.m32 = rotatedTerrainNormal.z;
                                    }
                                    else {
                                    //  These are always zero in case we do not need the terrain normal
                                        tempMatrix.m30 = 0.0f; //*= tempScale.x;
                                        tempMatrix.m31 = 0.0f; //*= tempScale.y;
                                        tempMatrix.m32 = 0.0f; //*= tempScale.z;
                                    }
                                //  tempMatrix.m33 is always 1! So layers gets written to the integer part / scale to the fractional
                                    tempMatrix.m33 = layers + tempScale.x * 0.01f;
                                //  Add final matrix
                                    //CurrentCellContent.matrices_list.Add(tempMatrix);
                                    tempMatrixArray[tempMatrixArrayPointer] = tempMatrix;
                                    tempMatrixArrayPointer++;
                                }
                                tempSamplePosition.y += BucketSize;
                            }
                            tempSamplePosition.y = samplePosition.y;
                            tempSamplePosition.x += BucketSize;
                        }

                    } // end outerLoop for softly merged layers

                //  CurrentCellContent.v_matrices = CurrentCellContent.matrices_list.ToArray();
                //  CurrentCellContent.matrices_list.Clear();
                    CurrentCellContent.v_matrices = new Matrix4x4[tempMatrixArrayPointer];
                    System.Array.Copy(tempMatrixArray, CurrentCellContent.v_matrices, tempMatrixArrayPointer);

                /*   if (Rotation == 2 && DoWriteNormalBuffer) {
                        //CurrentCellContent.v_normals = CurrentCellContent.normals_list.ToArray();
                        //CurrentCellContent.normals_list.Clear();
                        CurrentCellContent.v_normals = new Vector3[tempMatrixArrayPointer];
                        System.Array.Copy(tempNormalArray, CurrentCellContent.v_normals, tempMatrixArrayPointer);
                    } */

                }// end cell content    
                
                if(!b_initCompleteCell) {
                    CurrentCellContent.state = 2;  
                }    
                else {
                    CurrentCell.state = 2;
                }
            return cellIndex;
        }

// ---------------

#if UNITY_EDITOR
            void OnGUI() {
                if(DebugStats) {
                    var Alignement = GUI.skin.box.alignment;
                    GUI.skin.box.alignment = TextAnchor.MiddleLeft;
                    GUI.Box(new Rect(10, 10, 240, 40), "Visible Cells: " + numResults.ToString() + " / Cells to init: " + CellsOrCellContentsToInit.Count.ToString() );
                    GUI.skin.box.alignment = Alignement;
                }
            }
#endif

// ---------------

    }
}
