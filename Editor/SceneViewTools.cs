//#define TEST_FOV
//#define TEST_TILE

using Hananoki.Extensions;

using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

using E = Hananoki.SceneViewTools.SettingsEditor;


namespace Hananoki.SceneViewTools {
	[InitializeOnLoad]
	internal partial class SceneViewTools {

		internal class Styles {
			public GUIStyle popup;
			public GUIStyle toggle;
			const int WIDTH = 16;
			
			public Styles() {
				toggle = new GUIStyle( EditorStyles.toggle );
				
#if UNITY_2019_3_OR_NEWER
				popup = new GUIStyle( "MiniPopup" );
#else
				popup = new GUIStyle( "Popup" );
#endif
				
			}
		}


		internal static Styles s_styles;


		

#if TEST_FOV
		static float FOV = -1;
#endif

#if TEST_TILE
		static string typeName;
#endif


		static SceneViewTools() {
			E.Load();
			SceneViewUtils.AddGUI( OnSceneGUI );

			EditorSceneManager.sceneOpened += ( scene, mode ) => {
				s_cameraSelect = 0;
			};

#if TEST_FOV
			UTJ.UnityEditorExtension.SceneViewFovControl.SceneViewHiddenApi.AddOnPreSceneGUIDelegate( OnScene );
#endif

#if TEST_TILE
			Selection.selectionChanged += () => {
				if( Selection.activeGameObject == null ) return;
				if( Selection.activeGameObject.GetComponent<Tilemap>() ) {
					typeName = "Tilemap";
				}
				else {
					typeName = "";
				}
			};
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

		static void TimeSlider() {
		}


		public static void OnSceneGUI( SceneView sceneView ) {
			if( !E.i.Enable ) return;

			if( !E.i.multiSceneExec ) {
				if( SceneViewUtils.sceneViewNum != 1 ) return;
			}

			if( s_styles == null ) s_styles = new Styles();

			s_styles.toggle.normal.textColor = E.i.textColor;

			using( new GUISkinScope( EditorSkin.Inspector ) )
			using( new HandlesGUIScope() ) {
				DrawLeftBottom( sceneView );

				DrawRightBottom( sceneView );
			}

#if TEST_FOV
				rcPop.y -= 24;
				FOV = EditorGUI.Slider( rcPop, FOV, 0.01f, 180.00f );
#endif

#if TEST_TILE
				if( !string.IsNullOrEmpty( typeName ) ) {
				if( typeName == "Tilemap" ) {
					rcPop.y -= 24;
					rcPop.width = GUI.skin.button.CalcSize( EditorHelper.TempContent( "Tile Palette" ) ).x;
					rcPop.x = Screen.width - rcPop.width - 8;
					if( GUI.Button( rcPop, "Tile Palette" ) ) {
						EditorApplication.ExecuteMenuItem( "Window/2D/Tile Palette" );
					}
				}
			}
#endif
		}


		internal static void DrawRightBottom( SceneView sceneView ) {
			if( E.i.enableTimeScaleSlider ) {
				Rect rcPop = new Rect( 108, 8, 160, 16 );
				rcPop.x = Screen.width - 180 - 10;
				rcPop.width = 180 - 8;
				rcPop.y = Screen.height - ( 24 * 2 ) - ( 8 * 2 );

				var rc = rcPop.AddW( 4 );
				EditorGUI.DrawRect( rc, E.i.uiBkColor );

				//int controlID = GUIUtility.GetControlID( "HEditorSliderKnob".GetHashCode(), FocusType.Passive, rcPop );
				//Time.timeScale = GUI.Slider( rcPop, Time.timeScale, 0.0f, 0.00f, 1.00f, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb, true, controlID );
				//EditorGUI.DrawRect( rcPop, new Color(0,0,1,0.50f));

				Time.timeScale = EditorGUI.Slider( rcPop, Time.timeScale, 0.00f, 1.00f);
			}

		}


		static int s_cameraSelect = 0;
		static bool s_gameviewMatch = false;
		internal static void DrawLeftBottom( SceneView sceneView ) {
			Rect rcPop = new Rect( 8, 0, 0, 24 );
			Rect rcRad = new Rect( 12, 0, 160 - 4, 16 );
			Rect rcToggle = new Rect( rcPop.max.x, 0, 16, 16 );

			var cameras = Object.FindObjectsOfType<Camera>();
			rcPop.y = Screen.height - ( 24 * 2 ) - ( 8 * 2 );

			if( E.i.syncScene2Game ) {

				string[] displayNames = new string[] { "None", "" };
				ArrayUtility.AddRange( ref displayNames, cameras.Select<Camera, string>( c => c.name ).ToArray() );

				rcPop.width = 160;

				//GUI.skin = EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector );
				s_cameraSelect = EditorGUI.Popup( rcPop, s_cameraSelect, displayNames, SceneViewTools.s_styles.popup );

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
#if UNITY_2019_3_OR_NEWER
					rcRad.y -= 2;
#endif
					var cont = EditorHelper.TempContent( S._MatchGameView );
					var sz = EditorStyles.toggle.CalcSize( cont );
					rcRad.width = sz.x;
					var rc = rcRad.AddW( 2 );
					EditorGUI.DrawRect( rc, E.i.uiBkColor );
					s_gameviewMatch = GUI.Toggle( rcRad, s_gameviewMatch, S._MatchGameView, s_styles.toggle );
				}
			}


			// orthographic
			if( E.i.toggleOrthographic ) {
				rcToggle.y = rcPop.y + 1;
				rcToggle.x = rcPop.x + rcPop.width;
#if UNITY_2019_3_OR_NEWER
				rcToggle.x += 2;
#endif
				var cont = EditorHelper.TempContent( "Iso" );
				var sz = EditorStyles.toggle.CalcSize( cont );
				rcToggle.width = sz.x;

				var rc = rcToggle.AddW( 2 );
				EditorGUI.DrawRect( rc, E.i.uiBkColor );
				sceneView.orthographic = GUI.Toggle( rcToggle, sceneView.orthographic, cont , s_styles.toggle );
			}
		}
	}
}