using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

using UnityEditor;

namespace AdvancedTerrainGrass {

	[CustomEditor (typeof(GrassManager))]
	public class GrassManagerEditor : Editor {


		public int editorSelection = 0;

		private SerializedObject GrassManager;

		private SerializedProperty DebugStats;
		private SerializedProperty FirstTimeSynced;
		private SerializedProperty LayerEditMode;
		private SerializedProperty LayerSelection;
		private SerializedProperty Foldout_Rendersettings;
		private SerializedProperty Foldout_Prototypes;

		private SerializedProperty IngnoreOcclusion;
		private SerializedProperty CameraSelection;
		private SerializedProperty CameraLayer;
		private SerializedProperty CullDistance;
		private SerializedProperty FadeLength;
		private SerializedProperty CacheDistance;
		private SerializedProperty DetailFadeStart;
		private SerializedProperty DetailFadeLength;
		private SerializedProperty ShadowStart;
		private SerializedProperty ShadowFadeLength;
		private SerializedProperty ShadowStartFoliage;
		private SerializedProperty ShadowFadeLengthFoliage;
		private SerializedProperty useBurst;
		private SerializedProperty BurstRadius;
		private SerializedProperty initCompleteCell;
		private SerializedProperty DetailDensity;

		private SerializedProperty SavedTerrainData;
		private SerializedProperty NumberOfBucketsPerCellEnum;

		private SerializedProperty v_mesh;
		private SerializedProperty v_mat;
		private SerializedProperty InstanceRotation;
		private SerializedProperty WriteNormalBuffer;
		private SerializedProperty ShadowCastingMode;
		private SerializedProperty MinSize;
		private SerializedProperty MaxSize;
		private SerializedProperty Noise;
		private SerializedProperty LayerToMergeWith;
		private SerializedProperty DoSoftMerge;

 		private MaterialEditor _materialEditor;
 		private static string baseURL = "https://docs.google.com/document/d/1JrSQVQaPkYLkbF6XJGpzcXnLcLc1G9FGzUhdW1xWoyA/view?pref=2&pli=1#heading=";

		public override void OnInspectorGUI () {
			GrassManager = new SerializedObject(target);
			GetProperties();
			GrassManager script = (GrassManager)target;

//	Styles -------------------

			Color myCol = new Color (1.0f,0.8f,0.0f,1.0f);
			Color myCol01 = new Color(0.30f,0.47f,1.0f,1.0f); // matches highlight blue //new Color(1.0f,0.3f,0.0f,1.0f); // Orange
			if (!EditorGUIUtility.isProSkin) {
				myCol = new Color(0.18f,0.35f,0.85f,1.0f);
				myCol01 = Color.blue;
			}
			GUIStyle mainLabelStyle = new GUIStyle(EditorStyles.label);
			mainLabelStyle.normal.textColor = myCol;

			GUIStyle myMiniBtn = new GUIStyle(EditorStyles.miniButton);
			myMiniBtn.padding = new RectOffset(2, 2, 2, 2);

			// Custom Foldout
			GUIStyle mainFoldoutStyle = new GUIStyle(EditorStyles.foldout);
			mainFoldoutStyle.normal.textColor = myCol;
			mainFoldoutStyle.onNormal.textColor = myCol;
			mainFoldoutStyle.active.textColor = myCol;
			mainFoldoutStyle.onActive.textColor = myCol;
			mainFoldoutStyle.focused.textColor = myCol;
			mainFoldoutStyle.onFocused.textColor = myCol;

			// Help Btn
			GUIStyle myMiniHelpBtn = new GUIStyle(EditorStyles.miniButton);
			myMiniHelpBtn.padding = new RectOffset(0, 0, 2, 2);
			myMiniHelpBtn.normal.background = null;
			myMiniHelpBtn.normal.textColor = myCol01;
			myMiniHelpBtn.onNormal.textColor = myCol01;
			myMiniHelpBtn.active.textColor = myCol01;
			myMiniHelpBtn.onActive.textColor = myCol01;
			myMiniHelpBtn.focused.textColor = myCol01;
			myMiniHelpBtn.onFocused.textColor = myCol01;

			var ter = script.GetComponent<Terrain>();
			var terData = ter.terrainData;
			var bucketSize = terData.size.x / terData.detailWidth;
			var cellSize = (NumberOfBucketsPerCellEnum.intValue * bucketSize);
			var numberOfCells = terData.size.x / cellSize;

// -------------------


			GUILayout.Space(16);
			EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button( "Get Prototypes") ) {
						GetPrototypes();
					}
				if (GUILayout.Button( "Toggle Grid") ) {
					ToggleGrid();
				}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(12);
		//	Render Settings
			EditorGUILayout.BeginHorizontal();
				Foldout_Rendersettings.boolValue = EditorGUILayout.Foldout(Foldout_Rendersettings.boolValue, "Render Settings", mainFoldoutStyle);
				if (GUILayout.Button("Help", myMiniHelpBtn, GUILayout.Width(40))) {
					Application.OpenURL(baseURL + "h.ik42tx1oorzr");
				}
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(4);
			
			if(Foldout_Rendersettings.boolValue) {
				
				EditorGUILayout.PropertyField(CameraSelection, new GUIContent("Cameras"));

				int selectedLayer = CameraLayer.intValue;
				selectedLayer = EditorGUILayout.LayerField("Layer", selectedLayer);
				CameraLayer.intValue = selectedLayer;

				GUILayout.Space(2);
				EditorGUILayout.PropertyField(IngnoreOcclusion, new GUIContent("Ignore Visibility"));
				
				GUILayout.Space(2);
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(CullDistance, new GUIContent("Cull Distance"));
					GUILayout.Space(10);
					GUILayout.Label("Fade Length", GUILayout.Width(72) );
					EditorGUILayout.PropertyField(FadeLength, new GUIContent(""), GUILayout.MaxWidth(50) );
				EditorGUILayout.EndHorizontal();
				
				GUILayout.Space(2);
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(DetailFadeStart, new GUIContent("Detail Fade Start"));
					GUILayout.Space(10);
					GUILayout.Label("Fade Length", GUILayout.Width(72));
					EditorGUILayout.PropertyField(DetailFadeLength, new GUIContent(""), GUILayout.MaxWidth(50)  );
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(2);
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(ShadowStart, new GUIContent("Shadow Fade Start Grass"));
					GUILayout.Space(10);
					GUILayout.Label("Fade Length", GUILayout.Width(72));
					EditorGUILayout.PropertyField(ShadowFadeLength, new GUIContent(""), GUILayout.MaxWidth(50)  );
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(2);
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(ShadowStartFoliage, new GUIContent("Shadow Fade Start Foliage"));
					GUILayout.Space(10);
					GUILayout.Label("Fade Length", GUILayout.Width(72));
					EditorGUILayout.PropertyField(ShadowFadeLengthFoliage, new GUIContent(""), GUILayout.MaxWidth(50)  );
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(CacheDistance, new GUIContent("Cache Distance") );
					EditorGUILayout.BeginVertical( GUILayout.Width(136) );
						GUILayout.Label("");
					EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(2);
				EditorGUILayout.PropertyField(DebugStats, new GUIContent("    Debug Stats"));
				GUILayout.Space(2);

				EditorGUILayout.PropertyField(initCompleteCell, new GUIContent("Init complete Cell"));

				EditorGUILayout.BeginHorizontal();
#if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                EditorGUILayout.PropertyField(useBurst, new GUIContent("    Use Burst Init [beta]"));
#else
                EditorGUILayout.PropertyField(useBurst, new GUIContent("    Use Burst Init"));
#endif
                if (useBurst.boolValue) {
						GUI.enabled = true;
					}
					else {
						GUI.enabled = false;
					}
					GUILayout.Space(10);
					GUILayout.Label("Radius", GUILayout.Width(72));
					EditorGUILayout.PropertyField(BurstRadius, new GUIContent(""), GUILayout.MaxWidth(50));
					GUI.enabled = true;
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.PropertyField(DetailDensity, new GUIContent("Grass Density"));
			
				EditorGUILayout.PropertyField(NumberOfBucketsPerCellEnum, new GUIContent("Buckets per Cell"));
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel(" ");
					EditorGUILayout.BeginVertical();
						GUILayout.Label("Terrain will be divided into " + numberOfCells + "x" + numberOfCells + " Cells." , EditorStyles.miniLabel);
						GUILayout.Space(-5);
						GUILayout.Label("Cell Size: " + cellSize + "x" + cellSize + "m." , EditorStyles.miniLabel);
					EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}

			if(CacheDistance.floatValue <= CullDistance.floatValue) {
				CacheDistance.floatValue = CullDistance.floatValue + cellSize;
			}

		//	Cache TerrainData
			GUILayout.Space(8);
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Saved Terrain Data", mainLabelStyle);
				if (GUILayout.Button("Help", myMiniHelpBtn, GUILayout.Width(40))) {
					Application.OpenURL(baseURL + "h.cgihwefalakr");
				}
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(4);
			EditorGUILayout.PropertyField(SavedTerrainData, new GUIContent("Saved TerrainData"));
			if (GUILayout.Button( "Save TerrainData") ) {
				SaveTerrainData();
			}

		//	Prototypes
			GUILayout.Space(12);
			EditorGUILayout.BeginHorizontal();
				Foldout_Prototypes.boolValue = EditorGUILayout.Foldout(Foldout_Prototypes.boolValue, "Prototypes", mainFoldoutStyle);
				if (GUILayout.Button("Help", myMiniHelpBtn, GUILayout.Width(40))) {
					Application.OpenURL(baseURL + "h.h918om4x0d42");
				}
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(4);

			if(Foldout_Prototypes.boolValue) {
			//	Layer Selection
				int items;
				items = terData.detailPrototypes.Length;
				if (items != v_mesh.arraySize) {
					GetPrototypes();
				}

				if (items > 0) {
					string[] toolbarStrings = new string[] {"Single Prototype", "All Prototypes"};
					LayerEditMode.intValue = GUILayout.Toolbar(LayerEditMode.intValue, toolbarStrings);
					GUILayout.Space(4);

					Texture2D[] previews = new Texture2D[items];
					for(int i = 0; i < items; i++) {
						if (terData.detailPrototypes[i].prototype != null) {
		     				previews[i] = (Texture2D)AssetPreview.GetAssetPreview(terData.detailPrototypes[i].prototype);
		     			}
						else {
							previews[i] = (Texture2D)AssetPreview.GetMiniThumbnail(terData.detailPrototypes[i].prototypeTexture);
						}
					}

					if(LayerEditMode.intValue == 0) {

						int thumbSize = 48;

					//	Show single Layer
						int cols = Mathf.FloorToInt( (EditorGUIUtility.currentViewWidth - thumbSize) / (float)thumbSize );
						int rows = Mathf.CeilToInt ( (float)items / cols);

						LayerSelection.intValue = GUILayout.SelectionGrid(
							LayerSelection.intValue,
							previews,
							cols,
							GUILayout.MaxWidth(cols * thumbSize),
							GUILayout.MaxHeight(rows * thumbSize)
						);

						var currentSelection = LayerSelection.intValue;

					//	We might have deleted an element...
						if (currentSelection > (items - 1) ) {
							LayerSelection.intValue = items - 1;
							currentSelection = items - 1;
						}

						GUILayout.Space(8);
                        GUILayout.Label("Layer " + (currentSelection + 1), mainLabelStyle);
                        GUILayout.Space(2);
                        EditorGUILayout.PropertyField(v_mesh.GetArrayElementAtIndex(currentSelection), new GUIContent("Mesh"));
						EditorGUILayout.PropertyField(v_mat.GetArrayElementAtIndex(currentSelection), new GUIContent("Material"));
						EditorGUILayout.PropertyField(InstanceRotation.GetArrayElementAtIndex(currentSelection), new GUIContent("Rotation Mode"));

                        //	We may not have any material assigned.
                        var mat = v_mat.GetArrayElementAtIndex(currentSelection).objectReferenceValue as Material;
                        var mayWriteNormals = false;
                        if (mat != null)
                        {
                            if (!mat.shader.name.ToLower().Contains("foliage") && !mat.shader.name.ToLower().Contains("vertexlit"))
                            {
                                mayWriteNormals = true;
                            }
                        }

                        if ( InstanceRotation.GetArrayElementAtIndex(currentSelection).enumValueIndex != 2 || !mayWriteNormals ) {
							GUI.enabled = false;
                        }
						else {
							GUI.enabled = true;
						}
						EditorGUILayout.PropertyField(WriteNormalBuffer.GetArrayElementAtIndex(currentSelection), new GUIContent("    Write Normal Buffer"));

                        if (mayWriteNormals) {
                            if (WriteNormalBuffer.GetArrayElementAtIndex(currentSelection).boolValue && InstanceRotation.GetArrayElementAtIndex(currentSelection).enumValueIndex == 2)
                            {
                                mat.SetFloat("_SampleNormal", 1);
                                mat.EnableKeyword("_NORMAL");
                            }
                            else
                            {
                                mat.SetFloat("_SampleNormal", 0);
                                mat.DisableKeyword("_NORMAL");
                            }
                        }
                        else
                        {
                            WriteNormalBuffer.GetArrayElementAtIndex(currentSelection).boolValue = false;
                        }

						GUI.enabled = true;
						EditorGUILayout.PropertyField(ShadowCastingMode.GetArrayElementAtIndex(currentSelection), new GUIContent("Cast Shadows"));

						GUILayout.Space(4);
						//EditorGUILayout.BeginHorizontal();
							EditorGUILayout.PropertyField(MinSize.GetArrayElementAtIndex(currentSelection), new GUIContent("    Min Size"));
							EditorGUILayout.PropertyField(MaxSize.GetArrayElementAtIndex(currentSelection), new GUIContent("    Max Size"));
							EditorGUILayout.PropertyField(Noise.GetArrayElementAtIndex(currentSelection), new GUIContent("    Noise"));
						//EditorGUILayout.EndHorizontal();
						
						GUILayout.Space(4);
						EditorGUILayout.PropertyField(LayerToMergeWith.GetArrayElementAtIndex(currentSelection), new GUIContent("Layer to merge with"));
						EditorGUILayout.PropertyField(DoSoftMerge.GetArrayElementAtIndex(currentSelection), new GUIContent("    Soft Merge"));
						GUILayout.Space(8);

						// http://answers.unity3d.com/questions/429476/edit-chosen-material-in-the-inspector-for-custom-e.html
						_materialEditor = (MaterialEditor)CreateEditor ( (Material)v_mat.GetArrayElementAtIndex(currentSelection).objectReferenceValue );
						if (_materialEditor != null) {
							_materialEditor.DrawHeader ();
						    _materialEditor.OnInspectorGUI ();
						}
						EditorGUIUtility.labelWidth = 0; // Reset Labelwidth which get broken by the material inspector

						if (LayerToMergeWith.GetArrayElementAtIndex(currentSelection).intValue == 0) {
							DoSoftMerge.GetArrayElementAtIndex(currentSelection).boolValue = false;
						}

					}
					else {
                    //	Show all Layers
                        GUILayout.Space(8);
                        for (int i = 0; i < items; i++) {
							EditorGUILayout.BeginHorizontal();
								EditorGUILayout.BeginVertical();
									GUILayout.Label(previews[i], GUILayout.Width(32), GUILayout.Height(32));
								EditorGUILayout.EndVertical();
								EditorGUILayout.BeginVertical();
									GUILayout.Label("Layer " + (i + 1), mainLabelStyle); 
                                    GUILayout.Space(2);
									EditorGUILayout.PropertyField(v_mesh.GetArrayElementAtIndex(i), new GUIContent("Mesh"));
									EditorGUILayout.PropertyField(v_mat.GetArrayElementAtIndex(i), new GUIContent("Material"));
									EditorGUILayout.PropertyField(InstanceRotation.GetArrayElementAtIndex(i), new GUIContent("Rotation Mode"));


                                    //	We may not have any material assigned.
                                    var mat = v_mat.GetArrayElementAtIndex(i).objectReferenceValue as Material;
                                    var mayWriteNormals = false;
                                    if (mat != null)
                                    {
                                        if (!mat.shader.name.ToLower().Contains("foliage") && !mat.shader.name.ToLower().Contains("vertexlit"))
                                        {
                                            mayWriteNormals = true;
                                        }
                                    }

                                    if ( InstanceRotation.GetArrayElementAtIndex(i).enumValueIndex != 2 || !mayWriteNormals) {
										GUI.enabled = false;
									}
									EditorGUILayout.PropertyField(WriteNormalBuffer.GetArrayElementAtIndex(i), new GUIContent("    Write Normal Buffer"));

                                    if (mayWriteNormals)
                                    {
                                        if (WriteNormalBuffer.GetArrayElementAtIndex(i).boolValue && InstanceRotation.GetArrayElementAtIndex(i).enumValueIndex == 2)
                                        {
                                            //GUILayout.Label("Make sure that ", EditorStyles.miniLabel);
                                            mat.SetFloat("_SampleNormal", 1);
                                            mat.EnableKeyword("_NORMAL");
                                        }
                                        else
                                        {
                                            mat.SetFloat("_SampleNormal", 0);
                                            mat.DisableKeyword("_NORMAL");
                                        }
                                    }
                                    else
                                    {
                                        WriteNormalBuffer.GetArrayElementAtIndex(i).boolValue = false;
                                    }
									GUI.enabled = true;

									EditorGUILayout.PropertyField(ShadowCastingMode.GetArrayElementAtIndex(i), new GUIContent("Cast Shadows"));
									
									GUILayout.Space(4);
									//EditorGUILayout.BeginHorizontal();
										EditorGUILayout.PropertyField(MinSize.GetArrayElementAtIndex(i), new GUIContent("    Min Size"));
										EditorGUILayout.PropertyField(MaxSize.GetArrayElementAtIndex(i), new GUIContent("    Max Size"));
										EditorGUILayout.PropertyField(Noise.GetArrayElementAtIndex(i), new GUIContent("    Noise"));
									//EditorGUILayout.EndHorizontal();
									
									GUILayout.Space(4);
									EditorGUILayout.PropertyField(LayerToMergeWith.GetArrayElementAtIndex(i), new GUIContent("Layer to merge with"));
									EditorGUILayout.PropertyField(DoSoftMerge.GetArrayElementAtIndex(i), new GUIContent("    Soft Merge"));
									GUILayout.Space(12);
								EditorGUILayout.EndVertical();
							EditorGUILayout.EndHorizontal();

							if (LayerToMergeWith.GetArrayElementAtIndex(i).intValue == 0) {
								DoSoftMerge.GetArrayElementAtIndex(i).boolValue = false;
							}
						}
					}
				}
			}

			GUILayout.Space(12);		
			GrassManager.ApplyModifiedProperties();
		}


// --------------------------------------------

		private void ToggleGrid() {

			GrassManager script = (GrassManager)target;
			var ter = script.GetComponent<Terrain>();
	        var terData = ter.terrainData;

	        int id = ter.transform.gameObject.GetInstanceID();
	        var goname = "GridProjector_" + id.ToString();
	        var tempPro = GameObject.Find(goname);

		    if( tempPro == null) {

		        var go = new GameObject();
	            go.name = goname;
	            Projector proj = go.AddComponent<Projector>() as Projector;
	            proj.orthographic = true;
	            proj.orthographicSize = terData.size.x * 0.5f;
	            proj.material = new Material(Shader.Find("AdvancedTerrainGrass/GridProjector"));
	            string[] guIDS = AssetDatabase.FindAssets ("Atg_Grid");
	            if (guIDS.Length > 0) {
	            	proj.material.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath(guIDS[0]), typeof(Texture));
	            }
	            else {
	            	Debug.Log("Grid Texture not found.");
	            }

	            var pbucketSize = terData.size.x / terData.detailWidth;
	            var pcellSize = (NumberOfBucketsPerCellEnum.intValue * pbucketSize);
	            var pnumberOfCells = terData.size.x / pcellSize;

	            proj.material.mainTextureScale = new Vector2(pnumberOfCells, pnumberOfCells);

	            proj.farClipPlane = terData.size.y * 2.0f;
	            go.transform.position = new Vector3(0,terData.size.y,0) + ter.GetPosition() + terData.size * 0.5f;
	            go.transform.rotation = Quaternion.Euler(90, 0, 0);
	        }
	        else {
	        	if (tempPro.GetComponent<Projector>().material != null) {
	        		DestroyImmediate(tempPro.GetComponent<Projector>().material);
	        	}
	        	DestroyImmediate(tempPro);
	        }
		}

// --------------------------------------------

		private void GetPrototypes() {
			GrassManager script = (GrassManager)target;
			var ter = script.GetComponent<Terrain>();
	        var terData = ter.terrainData; 

	        int oldNumberOfLayers = v_mesh.arraySize;
	        int NumberOfLayers = terData.detailPrototypes.Length;
	        
	    //	Initial synchronisation with original terrain settings
	        if(!FirstTimeSynced.boolValue) {
	        	FirstTimeSynced.boolValue = true;
	        	DetailDensity.floatValue = ter.detailObjectDensity;
	        	CullDistance.floatValue = ter.detailObjectDistance;
	        	CacheDistance.floatValue = ter.detailObjectDistance * 1.2f;
	        }
	        
	    //	If the arrays grow new entries will be filled with a copy of the last entry so our "if not already assigned" fails
	        v_mesh.arraySize = NumberOfLayers;
	        v_mat.arraySize = NumberOfLayers;
	        InstanceRotation.arraySize = NumberOfLayers;
	        WriteNormalBuffer.arraySize = NumberOfLayers;
	        ShadowCastingMode.arraySize = NumberOfLayers;
	        MinSize.arraySize = NumberOfLayers;
	        MaxSize.arraySize = NumberOfLayers;
	        Noise.arraySize = NumberOfLayers;

	        LayerToMergeWith.arraySize = NumberOfLayers;
	        DoSoftMerge.arraySize = NumberOfLayers;

	        for (int i = 0; i < NumberOfLayers; i ++) {

	        //	Do we deal with a new entry?
	        	bool hasGrown = ( (oldNumberOfLayers < NumberOfLayers) && (i == NumberOfLayers - 1) ) ? true : false;	        
	        	MinSize.GetArrayElementAtIndex(i).floatValue = terData.detailPrototypes[i].minHeight;
	        	MaxSize.GetArrayElementAtIndex(i).floatValue = terData.detailPrototypes[i].maxHeight;
	        	Noise.GetArrayElementAtIndex(i).floatValue = terData.detailPrototypes[i].noiseSpread;
	        	bool materialSet = false;

	        //	As we simply size the array mesh and mat are wrong
	        	if (hasGrown) {
	        		v_mesh.GetArrayElementAtIndex(NumberOfLayers - 1).objectReferenceValue = null;
	        		v_mat.GetArrayElementAtIndex(NumberOfLayers - 1).objectReferenceValue = null;
	        	}
	        	
	        	Texture prototypeTex;

	        //	Here we deal with detail meshes
	        	if (terData.detailPrototypes[i].prototype != null) {
	        		v_mesh.GetArrayElementAtIndex(i).objectReferenceValue = terData.detailPrototypes[i].prototype.GetComponent<MeshFilter>().sharedMesh;
	        		bool vertexLit = false;

	        	//	Handle Detail meshes using the VertexLit Shader
	        		if(terData.detailPrototypes[i].renderMode == DetailRenderMode.VertexLit && v_mat.GetArrayElementAtIndex(i).objectReferenceValue == null ) {
	        			prototypeTex = terData.detailPrototypes[i].prototype.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex");
	        			vertexLit = true;
	        		}
	        	//	Handle Detail Meshes using the Grass Shader
	        		else {
	        			prototypeTex = terData.detailPrototypes[i].prototype.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex");
	        		}

        			if (prototypeTex != null) {
        				var path = Path.GetDirectoryName( AssetDatabase.GetAssetPath(prototypeTex) );
	        			path = path + "/" + prototypeTex.name + ".mat";
	        			Material newMat = AssetDatabase.LoadAssetAtPath (path, typeof (Material)) as Material;
	        			if (newMat == null) {
	        				if(vertexLit){
	        					newMat = new Material(Shader.Find("AdvancedTerrainGrass/Grass VertexLit Shader"));	
	        				}
	        				else {
	        					newMat = new Material(Shader.Find("AdvancedTerrainGrass/Grass Base Shader"));	
	        				}
	        				newMat.SetTexture("_MainTex", prototypeTex );
	        				AssetDatabase.CreateAsset (newMat, path);
	        			}
	        			v_mat.GetArrayElementAtIndex(i).objectReferenceValue = (Material)AssetDatabase.LoadAssetAtPath( path, typeof(Material) );
					}
					else {
						Debug.Log ("Could not create a material as no texture has been assigned.");
					}
					materialSet = true;
	        	}

	        //	Handle simple texture based grass
	        	if (!materialSet && (v_mat.GetArrayElementAtIndex(i).objectReferenceValue == null) ) {
	        		prototypeTex = terData.detailPrototypes[i].prototypeTexture;
	        		if (prototypeTex != null) {
		        		var path = Path.GetDirectoryName(  AssetDatabase.GetAssetPath(prototypeTex)   );
		        		path = path + "/" + prototypeTex.name + ".mat";
		        		Material newMat = AssetDatabase.LoadAssetAtPath (path, typeof (Material)) as Material;
		        		if (newMat == null) {
		        			newMat = new Material(Shader.Find("AdvancedTerrainGrass/Grass Base Shader"));
		        			newMat.SetTexture("_MainTex", prototypeTex );
		        			AssetDatabase.CreateAsset (newMat, path);
		        		}
		        		v_mat.GetArrayElementAtIndex(i).objectReferenceValue = (Material)AssetDatabase.LoadAssetAtPath( path, typeof(Material) );
		        	}
		        }

		    //	Add default Quad for simple Grass Textures
        		if (v_mesh.GetArrayElementAtIndex(i).objectReferenceValue == null) {
        		//	Most common is width * height
        			prototypeTex = terData.detailPrototypes[i].prototypeTexture;
        			string MeshToAssign = "Atg_BaseQuad";
        			if (prototypeTex != null) {
        				if ( (prototypeTex.width / prototypeTex.height) <= 0.5f ) {
        					MeshToAssign = "Atg_BaseRectVertical";
        				}
        				else if ( (prototypeTex.width / prototypeTex.height) >= 2.0f ) {
        					MeshToAssign = "Atg_BaseRectHorizonal";
        				}
        			}
        			string[] guIDS = AssetDatabase.FindAssets (MeshToAssign);
	        		if (guIDS.Length > 0) {
						v_mesh.GetArrayElementAtIndex(i).objectReferenceValue = (Mesh)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath(guIDS[0]), typeof(Mesh));
					}
					else {
						Debug.Log("Could not assign a valid Mesh because '" + MeshToAssign + "' could not be found.");
					}
				}
	        	
		    
		    //	Always update Colors
		    	if (v_mat.GetArrayElementAtIndex(i).objectReferenceValue != null) {
		    		Material Mat = (Material) v_mat.GetArrayElementAtIndex(i).objectReferenceValue as Material;
		    		Mat.SetColor("_HealthyColor", terData.detailPrototypes[i].healthyColor);
		        	Mat.SetColor("_DryColor", terData.detailPrototypes[i].dryColor);
		        //	Handle the case that both values are equal (division by zero)
		        	if (MaxSize.GetArrayElementAtIndex(i).floatValue == MinSize.GetArrayElementAtIndex(i).floatValue ) {
		        		Mat.SetVector("_MinMaxScales", new Vector4 (
		        			MinSize.GetArrayElementAtIndex(i).floatValue,
		        			1.0f, 0, 0)
		        		);
		        	}
		        	else {
			        	Mat.SetVector("_MinMaxScales", new Vector4 (
			        		MinSize.GetArrayElementAtIndex(i).floatValue,
							1.0f / (MaxSize.GetArrayElementAtIndex(i).floatValue - MinSize.GetArrayElementAtIndex(i).floatValue),
							0,0)
			        	);
			        }
		    	}
	        }
		}

//	--------------------------------------------
// 	This function more or less equals InitCells from GrassManager.cs.
//	It merges density maps and configures Cells and CellContents and writes all out to disc.
	
	public void SaveTerrainData() {
	        GrassManager script = (GrassManager)target;
			var ter = script.GetComponent<Terrain>();
	        var terData = ter.terrainData;
	        var TerrainSize = terData.size;
	        
	        Vector2 TerrainDetailSize = Vector2.zero;
            TerrainDetailSize.y = terData.detailHeight;
	        TerrainDetailSize.x = terData.detailWidth;
            TerrainDetailSize.y = terData.detailHeight;

            var TerrainPosition = ter.GetPosition();
            Vector3 OneOverTerrainSize;
            OneOverTerrainSize.x = 1.0f/TerrainSize.x;
            OneOverTerrainSize.y = 1.0f/TerrainSize.y;
            OneOverTerrainSize.z = 1.0f/TerrainSize.z;

	    //  We assume squared Terrains here...
            var BucketSize = TerrainSize.x / TerrainDetailSize.x;
            var NumberOfBucketsPerCell = (int)NumberOfBucketsPerCellEnum.intValue;
            var CellSize = NumberOfBucketsPerCell * BucketSize;
            var NumberOfCells = (int) (TerrainSize.x / CellSize);

        //  Merge Layers
            var NumberOfLayers = terData.detailPrototypes.Length;
            var OrigNumberOfLayers = NumberOfLayers;
            int[][] MergeArray = new int[OrigNumberOfLayers][]; // Actually too big...
            int[][] SoftMergeArray = new int[OrigNumberOfLayers][]; // Actually too big...

            int maxBucketDensity = 0;

        //  Check if we have to merge detail layers
            for (int i = 0; i < OrigNumberOfLayers; i ++) {

                int t_LayerToMergeWith = LayerToMergeWith.GetArrayElementAtIndex(i).intValue;
                int index_LayerToMergeWith = t_LayerToMergeWith - 1;
                
                if( (t_LayerToMergeWith != 0) && (t_LayerToMergeWith != (i+1))  ) {
                    
                //  Check if the Layer we want to merge with does not get merged itself..
                    if ( LayerToMergeWith.GetArrayElementAtIndex(index_LayerToMergeWith).intValue == 0 ) {
                        
                        //Debug.Log("Merge Layer " + (i +1) + " with Layer " + LayerToMergeWith.GetArrayElementAtIndex(i) );

                        if ( MergeArray[ index_LayerToMergeWith ] == null ) {
                            MergeArray[ index_LayerToMergeWith ] = new int[ OrigNumberOfLayers - 1 ]; // Also actually too big
                        }

                        if ( DoSoftMerge.GetArrayElementAtIndex(i).boolValue ) {
                            if( SoftMergeArray[ index_LayerToMergeWith ] == null ) {
                                SoftMergeArray[ index_LayerToMergeWith ] = new int[ OrigNumberOfLayers - 1 ]; // Also actually too big
                            }
                        }

                    //	Find a the first free entry
                        for (int j = 0; j < OrigNumberOfLayers - 1; j++) {
                            if ( MergeArray[ index_LayerToMergeWith ][j] == 0 ) {
                                MergeArray[ index_LayerToMergeWith ][j] = 									i + 1; // as index starts at 1 (0 means: do not merge)
                                break;
                            } 
                        }
                    //  Find a the first free entry Soft Merge
                        if ( DoSoftMerge.GetArrayElementAtIndex(i).boolValue ) {
                            for (int j = 0; j < OrigNumberOfLayers - 1; j++) {
                                if ( SoftMergeArray[ index_LayerToMergeWith ][j] == 0 ) {
                                    SoftMergeArray[ index_LayerToMergeWith ][j] =                           i + 1; // as index starts at 1 (0 means: do not merge)
                                    break;
                                } 
                            }
                        }
                    }
                }
            }


            GrassCell[] tCells = new GrassCell[NumberOfCells * NumberOfCells];
        //  As we do not know the final cout we create cellcontens in a temporary list
            List<GrassCellContent> tempCellContent = new List<GrassCellContent>();
            tempCellContent.Capacity = NumberOfCells * NumberOfCells * NumberOfLayers;

            byte[][] tmapByte = new byte[NumberOfLayers][];

        //  Read and convert [,] into []
            // Here we are working on "pixels" or buckets and lay out the array like x0.y0, x0.y2, x0.y3, ....
            for(int layer = 0; layer < NumberOfLayers; layer++) {
                if (LayerToMergeWith.GetArrayElementAtIndex(layer).intValue == 0 || DoSoftMerge.GetArrayElementAtIndex(layer).boolValue ) {
					tmapByte[layer] = new byte[ (int)(TerrainDetailSize.x * TerrainDetailSize.y) ];
                //  Check merge
                    bool doMerge = false;
                    if ( MergeArray[layer] != null && !DoSoftMerge.GetArrayElementAtIndex(layer).boolValue ) {
                        //Debug.Log("merge into layer " + layer);
                        doMerge = true;
                    }
                    for (int x = 0; x < (int)TerrainDetailSize.x; x++ ){
                        for (int y = 0; y < (int)TerrainDetailSize.y; y ++) {
                            // flipped!!!!!????????? no,not in this case.
                            int[,] temp = terData.GetDetailLayer(x, y, 1, 1, layer );
							tmapByte[layer][ x * (int)TerrainDetailSize.y + y ] = Convert.ToByte( (int)temp[0,0] );                            
                        //  Merge
                            if(doMerge) {
                                for (int m = 0; m < OrigNumberOfLayers - 1; m++ ) {
                                    if (MergeArray[layer][m] != 0) {
                                        temp = terData.GetDetailLayer(x, y, 1, 1, MergeArray[layer][m] 		- 1 ); // as index starts as 1!
										tmapByte[layer][ x * (int)TerrainDetailSize.y + y ] = Convert.ToByte( tmapByte[layer][ x * (int)TerrainDetailSize.y + y ] + (int)temp[0,0] );                                      
                                    }
                                    else {
                                        break;
                                    }
                                }
                            }  //  End of merge
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

                    tCells[CurrrentCellIndex] = new GrassCell();
                    tCells[CurrrentCellIndex].index = CurrrentCellIndex;
                //  Center in World Space – used by cullingspheres
                    Center.x = x * CellSize + 0.5f * CellSize  +  TerrainPosition.x;
                    Center.y = sampledHeight  +  TerrainPosition.y;
                    Center.z = z * CellSize + 0.5f * CellSize  +  TerrainPosition.z;
                    tCells[CurrrentCellIndex].Center = Center;

                    int tempBucketDensity = 0;

                //	Create and setup all CellContens (layers) for the given Cell
                    for(int layer = 0; layer < NumberOfLayers; layer++) {
                    
                    //  Only create cellcontent entry if the layer does not get merged.
                        if (LayerToMergeWith.GetArrayElementAtIndex(layer).intValue == 0 ) {
                        //  Sum up density for all buckets 
                            for(int xp = 0; xp < NumberOfBucketsPerCell; xp++) {
                                for(int yp = 0; yp < NumberOfBucketsPerCell; yp++) {
                                    //	Here we are working on cells and buckets!
                                    tempBucketDensity = (int)(tmapByte[layer][
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
                                            tempBucketDensity = (int)(tmapByte[softMergedLayer][
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
                                    if (NumberOfSoftlyMergedLayers * 16 > maxBucketDensity) {
                                        maxBucketDensity = NumberOfSoftlyMergedLayers * 16 + 16 * 2;
                                    }
                                }
                            }

                        //  Skip CellContent if density = 0                    
                            if(density > 0) {
                            //  Register CellContent to Cell
                                tCells[CurrrentCellIndex].CellContentIndexes.Add(CellContentOffset);
                                tCells[CurrrentCellIndex].CellContentCount += 1;
                            //  Add new CellContent
                                var tempContent = new GrassCellContent();
                                tempContent.index = CellContentOffset;
								tempContent.Layer = layer;
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
                                        if(SoftMergeArray[layer][l] != 0) { //} && DoSoftMerge[  MergeArray[layer][l] - 1 ]  ) {
                                            //Debug.Log("Add softly merged layer to cell content, layer: " + ( SoftMergeArray[layer][l] - 1) + " at index " + CellContentOffset );
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

            GrassCellContent[] tCellContent = tempCellContent.ToArray();
            tempCellContent.Clear();

		//	Finalize and save data
			GrassTerrainDefinitions asset = ScriptableObject.CreateInstance<GrassTerrainDefinitions>();
			
		//	Copy data to Scriptable Object
			asset.DensityMaps = new List<DetailLayerMap>();
			for( int i = 0; i < tmapByte.Length; i++) {
				asset.DensityMaps.Add( new DetailLayerMap() );
				asset.DensityMaps[i].mapByte = tmapByte[i];
			}
			asset.Cells = tCells;
			asset.CellContent = tCellContent;
			asset.maxBucketDensity = maxBucketDensity;

            Debug.Log("MaxBucketDensity: " + maxBucketDensity);

			string terPath = Path.GetDirectoryName( AssetDatabase.GetAssetPath(terData) );
			string terName = Path.GetFileNameWithoutExtension( AssetDatabase.GetAssetPath(terData) );
			AssetDatabase.CreateAsset(asset, terPath + "/" + terName + "_GrassTerrainData.asset");
			AssetDatabase.SaveAssets();
			SavedTerrainData.objectReferenceValue = asset;
        }

// -------

		private void GetProperties() {

			DebugStats = GrassManager.FindProperty("DebugStats");
			FirstTimeSynced = GrassManager.FindProperty("FirstTimeSynced");
			LayerEditMode = GrassManager.FindProperty("LayerEditMode");
			LayerSelection = GrassManager.FindProperty("LayerSelection");
			Foldout_Rendersettings = GrassManager.FindProperty("Foldout_Rendersettings");
			Foldout_Prototypes = GrassManager.FindProperty("Foldout_Prototypes");

			IngnoreOcclusion = GrassManager.FindProperty("IngnoreOcclusion");
			CameraSelection = GrassManager.FindProperty("CameraSelection");
			CameraLayer = GrassManager.FindProperty("CameraLayer");
			CullDistance = GrassManager.FindProperty("CullDistance");
			FadeLength = GrassManager.FindProperty("FadeLength");
			CacheDistance = GrassManager.FindProperty("CacheDistance");
			DetailFadeStart  = GrassManager.FindProperty("DetailFadeStart");
			DetailFadeLength = GrassManager.FindProperty("DetailFadeLength");
			ShadowStart = GrassManager.FindProperty("ShadowStart");
			ShadowFadeLength = GrassManager.FindProperty("ShadowFadeLength");
			ShadowStartFoliage = GrassManager.FindProperty("ShadowStartFoliage");
			ShadowFadeLengthFoliage = GrassManager.FindProperty("ShadowFadeLengthFoliage");
			useBurst = GrassManager.FindProperty("useBurst");
			BurstRadius = GrassManager.FindProperty("BurstRadius");
			initCompleteCell = GrassManager.FindProperty("initCompleteCell");
			DetailDensity = GrassManager.FindProperty("DetailDensity");

			SavedTerrainData = GrassManager.FindProperty("SavedTerrainData");
			NumberOfBucketsPerCellEnum = GrassManager.FindProperty("NumberOfBucketsPerCellEnum");

			v_mesh = GrassManager.FindProperty("v_mesh");
			v_mat = GrassManager.FindProperty("v_mat");
			InstanceRotation = GrassManager.FindProperty("InstanceRotation");
			WriteNormalBuffer = GrassManager.FindProperty("WriteNormalBuffer");
			ShadowCastingMode = GrassManager.FindProperty("ShadowCastingMode");
			MinSize = GrassManager.FindProperty("MinSize");
			MaxSize = GrassManager.FindProperty("MaxSize");
			Noise = GrassManager.FindProperty("Noise");
			LayerToMergeWith = GrassManager.FindProperty("LayerToMergeWith");
			DoSoftMerge = GrassManager.FindProperty("DoSoftMerge");
		}
	}
}