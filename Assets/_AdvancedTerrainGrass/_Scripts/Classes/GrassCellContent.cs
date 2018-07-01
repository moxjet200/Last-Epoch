using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace AdvancedTerrainGrass {

	[Serializable] 
	public class GrassCellContent  {

		public int index;
		public int Layer;

		public int[] SoftlyMergedLayers;

		public int state = 0; // 0 -> not initialized; 1 -> queued; 2 -> readyToInitialize; 3 -> initialized

		public Mesh v_mesh;
		public Material v_mat;
		public int GrassMatrixBufferPID;
		//public int GrassNormalBufferPID;

		public ShadowCastingMode ShadowCastingMode = ShadowCastingMode.Off;

		public int Instances;
		public Vector3 Center;	// center in world space
		public Vector3 Pivot; 	// lower left corner in local terrain space
		
		public Matrix4x4[] v_matrices;
		//public List<Matrix4x4> matrices_list = new List<Matrix4x4>();
		
		//public Vector3[] v_normals;
		//public List<Vector3> normals_list = new List<Vector3>();

		public int PatchOffsetX;
		public int PatchOffsetZ;


		private ComputeBuffer matrixBuffer;
		//private ComputeBuffer normalBuffer;
		public ComputeBuffer argsBuffer;
		public uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

		private Bounds bounds = new Bounds();

#if !UNITY_5_6_0 && !UNITY_5_6_1 && !UNITY_5_6_2
        private MaterialPropertyBlock block;
#endif

		public void RelaseBuffers() {
			if(matrixBuffer != null) {
				matrixBuffer.Release();
			}
			//if(normalBuffer != null) {
			//	normalBuffer.Release();
			//}
			if(argsBuffer != null) {
				argsBuffer.Release();
			}
		}

		public void ReleaseCellContent() {
			state = 0;
			v_matrices = null;
			RelaseBuffers();
		}

		public void InitCellContent_Delegated() {
			matrixBuffer = new ComputeBuffer(v_matrices.Length, 64);
			matrixBuffer.SetData(v_matrices);
               
#if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                v_mat.SetBuffer(GrassMatrixBufferPID, matrixBuffer);
#else
                block = new MaterialPropertyBlock();
                block.SetBuffer(GrassMatrixBufferPID, matrixBuffer);
#endif

/*            if (v_normals != null) {
			//	Double safe...
				if (v_normals.Length > 0) {
					normalBuffer = new ComputeBuffer(v_normals.Length, 12);
					normalBuffer.SetData(v_normals);
#if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                    v_mat.SetBuffer(GrassNormalBufferPID, normalBuffer);
#else
                    block.SetBuffer(GrassNormalBufferPID, normalBuffer);
#endif
                }
			} */

			argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
			uint numIndices = (v_mesh != null) ? (uint)v_mesh.GetIndexCount(0) : 0;
        	args[0] = numIndices;
        	args[1] = (uint)v_matrices.Length;
        	argsBuffer.SetData(args);

        	bounds.center = Center;
        	var Extent = (Center.x - Pivot.x) * 2.0f;
        	bounds.extents = new Vector3(Extent, Extent, Extent);
        //	Now we are ready to go.
        	state = 3;
	    }


		public void DrawCellContent_Delegated(Camera CameraInWichGrassWillBeDrawn, int CameraLayer) {
			Graphics.DrawMeshInstancedIndirect(
				v_mesh,
				0, 
				v_mat,
				bounds,
				argsBuffer,
				0,
#if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                null,
#else
                block,
#endif
                ShadowCastingMode,
				true,
				CameraLayer,
				CameraInWichGrassWillBeDrawn
			);
		}
	}
}
