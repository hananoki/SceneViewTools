//#define TEST_FOV
//#define TEST_TILE

using HananokiEditor.Extensions;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using HananokiRuntime;
using HananokiRuntime.Extensions;

#if UNITY_2019_1_OR_NEWER
using UnityEditor.EditorTools;
#endif
using UnityEditor.Callbacks;

#if ENABLE_TILEMAP
using UnityEngine.Tilemaps;
#endif

using E = HananokiEditor.SceneViewTools.SettingsEditor;
using UnityScene = UnityEngine.SceneManagement.Scene;
using UnityObject = UnityEngine.Object;

#if UNITY_2020_2_OR_NEWER
using ToolManager = UnityEditor.EditorTools.ToolManager;
#elif UNITY_2019_1_OR_NEWER
using ToolManager = UnityEditor.EditorTools.EditorTools;
#endif

namespace HananokiEditor.SceneViewTools {
	abstract class SceneViewComponentTool {
		public abstract void OnGUI();
		public abstract void OnSceneView( IEnumerable<Component> conponents );
	}

	[InitializeOnLoad]
	internal partial class SceneViewTools {

		internal static Hashtable m_shortCuts;
		internal static Hashtable m_componetTool;


#if TEST_FOV
		static float FOV = -1;
#endif

#if ENABLE_TILEMAP
		static bool s_tileOn;
#endif


		static SceneViewTools() {
			E.Load();
			SceneViewUtils.AddGUI( OnSceneGUI );

			EditorSceneManager.sceneOpened += OnSceneOpened;

			Selection.selectionChanged += OnSelectionChanged;
			OnSelectionChanged();
#if UNITY_2019_1_OR_NEWER
			ToolManager.activeToolChanged += OnActiveToolChanged;
#endif
			m_shortCuts = new Hashtable();
			m_componetTool = new Hashtable();

			foreach( var minfo in AssemblieUtils.GetAllMethodsWithAttribute<Hananoki_SceneView_ComponentButton>() ) {
				foreach( var cus in minfo.GetCustomAttributes( true ) ) {
					if( typeof( Hananoki_SceneView_ComponentButton ) != cus.GetType() ) continue;
					var atb = (Hananoki_SceneView_ComponentButton) cus;

					m_shortCuts.Add( atb.type, (Action) Delegate.CreateDelegate( typeof( Action ), null, minfo ) );
					break;
				}
			}
			foreach( var type in AssemblieUtils.GetAllTypesWithAttribute<Hananoki_SceneView_ComponentTool>() ) {
				foreach( var cAtb in type.GetCustomAttributes( true ) ) {
					if( typeof( Hananoki_SceneView_ComponentTool ) != cAtb.GetType() ) continue;
					var atb = (Hananoki_SceneView_ComponentTool) cAtb;

					m_componetTool.Add( atb.type, (SceneViewComponentTool) Activator.CreateInstance( type ) );
					break;
				}
			}

		}





		static void OnSceneOpened( UnityScene scene, OpenSceneMode mode ) {
			s_cameraSelect = 0;
		}



		[DidReloadScripts]
		static void OnDidReloadScripts() {
		}



		static void OnSelectionChanged() {

#if ENABLE_TILEMAP
			s_tileOn = false;
#endif
			if( !E.i.tools ) return;


			if( Selection.activeGameObject == null ) return;
#if ENABLE_TILEMAP
			if( Selection.activeGameObject.GetComponent<Tilemap>() ) {
				s_tileOn = true;
			}
			if( Selection.activeGameObject.GetComponent<Grid>() ) {
				s_tileOn = true;
			}
			if( Selection.activeGameObject.GetComponent<TilemapPallet>() ) {
				var comp = Selection.activeGameObject.GetComponent<TilemapPallet>();
				var window = EditorWindowUtils.Find( UnityTypes.UnityEditor_Tilemaps_GridPaintPaletteWindow );

				var t = UnityTypes.UnityEditor_Tilemaps_GridPalettes;
				var aa = t.GetProperty<List<GameObject>>( "palettes" );
				var index = aa.FindIndex( x => x.name == comp.palletSettings.name );
				if( 0 <= index ) {
					window.MethodInvoke( "SelectPalette", new object[] { index, comp.gameObject } );
				}
			}
#endif

		}



		static void OnActiveToolChanged() {
#if UNITY_2019_1_OR_NEWER
			if( "NoneTool" == ToolManager.activeToolType.Name ) return;
#endif

		}


#if TEST_FOV
		public static void OnScene( SceneView sceneView ) {
			if( sceneView != null || sceneView.camera != null ) {
				if( FOV < 0.0f ) {
					FOV = sceneView.camera.fieldOfView;
				}
				sceneView.camera.fieldOfView = FOV;
			}
		}
#endif



		public static void OnSceneGUI( SceneView sceneView ) {
			if( !E.i.Enable ) return;

			if( !E.i.multiSceneExec ) {
				if( SceneViewUtils.sceneViewNum != 1 ) return;
			}

			Styles.Init();

			Styles.toggle.normal.textColor = E.i.textColor;
			Styles.rightButton.normal.textColor = E.i.textColor;

			using( new GUISkinScope( EditorSkin.Inspector ) )
			using( new HandlesGUIScope() ) {
				DrawLeftBottom( sceneView );

				DrawRightBottom( sceneView );

				DrawToolPanel( sceneView );
			}

			if( E.i.tools ) {
				SelectionHierarchy.current?.ComponetToolSceneView();
			}

#if TEST_FOV
				rcPop.y -= 24;
				FOV = EditorGUI.Slider( rcPop, FOV, 0.01f, 180.00f );
#endif
		}



		internal static void DrawToolPanel( SceneView sceneView ) {
			if( !E.i.tools ) return;

			var cont = EditorHelper.TempContent( EditorIcon.viewtoolorbit );
			//var size = EditorStyles.miniButton.CalcSize( cont, GUILayout.Width(32) );
			var x = ( Screen.width * 0.5f ) - ( 128 * 0.5f );
			var y = Screen.height - ( 24 * 2 ) - ( 8 * 2 );
			Rect r = new Rect( x, y, 128 + 4, 20 );

			ScopeArea.Begin( r );
			ScopeHorizontal.Begin();
			GUILayout.FlexibleSpace();
			SelectionHierarchy.current?.ComponetToolGUI();
			GUILayout.FlexibleSpace();
			ScopeHorizontal.End();
			ScopeArea.End();


			ScopeArea.Begin( new Rect( Screen.width - 200, 120, 200, Screen.height - 100 ) );
			{
#if ENABLE_TILEMAP
				if( s_tileOn ) {
					ShowWindowButton( UnityTypes.UnityEditor_Tilemaps_GridPaintPaletteWindow, "Tile Pallete", EditorIcon.icons_processed_unityengine_tilemaps_tilemap_icon_asset );
				}
#endif
				SelectionHierarchy.current?.SideButtonGUI();
			}
			ScopeArea.End();
		}



		internal static bool ShowSideButton( string text, Texture2D image ) {
			bool result = false;
			ScopeHorizontal.Begin();
			GUILayout.FlexibleSpace();
			var contt = EditorHelper.TempContent( text, image );
			var rr = GUILayoutUtility.GetRect( contt, Styles.rightButton, GUILayout.Width( text.CalcSizeFromLabel().x + 16 + 4 ) );
			EditorGUI.DrawRect( rr, E.i.uiBkColor );
			if( GUI.Button( rr, contt, Styles.rightButton ) ) {
				result = true;
			}
			ScopeHorizontal.End();
			return result;
		}

		internal static void ShowWindowButton( Type editorWindowType, string text, Texture2D image ) {
			if( editorWindowType == null ) return;
			var _window = EditorWindowUtils.Find( editorWindowType );
			if( _window != null ) return;

			if( ShowSideButton( text, image ) ) {
				var window = HEditorWindow.ShowWindow( editorWindowType );
				window.titleContent = new GUIContent( text, window.titleContent.image );
			}
		}


		internal static void DrawRightBottom( SceneView sceneView ) {
			if( !E.i.enableTimeScaleSlider ) return;


			Rect rcPop = new Rect( 108, 8, 160, 16 );
			rcPop.x = Screen.width - 180 - 10;
			rcPop.width = 180 - 8;
			rcPop.y = Screen.height - ( 24 * 2 ) - ( 8 * 2 );

			var rc = rcPop.AddW( 4 );
			EditorGUI.DrawRect( rc, E.i.uiBkColor );

			//int controlID = GUIUtility.GetControlID( "HEditorSliderKnob".GetHashCode(), FocusType.Passive, rcPop );
			//Time.timeScale = GUI.Slider( rcPop, Time.timeScale, 0.0f, 0.00f, 1.00f, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb, true, controlID );
			//EditorGUI.DrawRect( rcPop, new Color(0,0,1,0.50f));

			Time.timeScale = EditorGUI.Slider( rcPop, Time.timeScale, 0.00f, 1.00f );
		}


		static int s_cameraSelect = 0;
		static bool s_gameviewMatch = false;
		internal static void DrawLeftBottom( SceneView sceneView ) {
			Rect rcPop = new Rect( 8, 0, 0, 24 );
			Rect rcRad = new Rect( 12, 0, 160 - 4, 16 );
			Rect rcToggle = new Rect( rcPop.max.x, 0, 16, 16 );

			var cameras = UnityObject.FindObjectsOfType<Camera>();
			rcPop.y = Screen.height - ( 24 * 2 ) - ( 8 * 2 );

			if( E.i.syncScene2Game ) {

				string[] displayNames = new string[] { "None", "" };
				ArrayUtility.AddRange( ref displayNames, cameras.Select( c => c.name ).ToArray() );

				rcPop.width = 160;

				//GUI.skin = EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector );
				s_cameraSelect = EditorGUI.Popup( rcPop, s_cameraSelect, displayNames, Styles.popup );

				// Match GameView

				int index = s_cameraSelect - 2;

				if( index >= 0 && index < cameras.Length ) {
					var camera = cameras[ index ];
					if( s_gameviewMatch ) {
						var a = camera.transform.rotation * Vector3.forward;
						a *= 25.0f;
						sceneView.pivot = camera.transform.position + a;
						sceneView.rotation = camera.transform.rotation;
						sceneView.size = 25.0f;

						s_gameviewMatch = false;
					}
					else {
						camera.transform.position = sceneView.camera.transform.position;
						camera.transform.rotation = sceneView.camera.transform.rotation;
					}
				}
				else {
					rcRad.y = rcPop.y - 16;
					if( UnitySymbol.Has( "UNITY_2019_3_OR_NEWER" ) ) {
						rcRad.y -= 2;
					}
					var cont = EditorHelper.TempContent( S._MatchGameView );
					var sz = EditorStyles.toggle.CalcSize( cont );
					rcRad.width = sz.x;
					var rc = rcRad.AddW( 2 );
					EditorGUI.DrawRect( rc, E.i.uiBkColor );
					s_gameviewMatch = GUI.Toggle( rcRad, s_gameviewMatch, S._MatchGameView, Styles.toggle );
				}
			}


			// orthographic
			if( E.i.toggleOrthographic ) {
				rcToggle.y = rcPop.y + 1;
				rcToggle.x = rcPop.x + rcPop.width;
				if( UnitySymbol.Has( "UNITY_2019_3_OR_NEWER" ) ) {
					rcToggle.x += 2;
				}
				var cont = EditorHelper.TempContent( "Iso" );
				var sz = EditorStyles.toggle.CalcSize( cont );
				rcToggle.width = sz.x;

				var rc = rcToggle.AddW( 2 );
				EditorGUI.DrawRect( rc, E.i.uiBkColor );
				sceneView.orthographic = GUI.Toggle( rcToggle, sceneView.orthographic, cont, Styles.toggle );
			}
		}
	}
}