//#define TEST_FOV
//#define TEST_TILE

using Hananoki.Extensions;

using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_2019_1_OR_NEWER
using UnityEditor.EditorTools;
#endif
using UnityEditor.Callbacks;

#if ENABLE_TILEMAP
using UnityEngine.Tilemaps;
#endif

using E = Hananoki.SceneViewTools.SettingsEditor;
using UnityScene = UnityEngine.SceneManagement.Scene;
using UnityObject = UnityEngine.Object;

namespace Hananoki.SceneViewTools {
	[InitializeOnLoad]
	internal partial class SceneViewTools {

		internal class Styles {
			public GUIStyle popup;
			public GUIStyle toggle;
			public GUIStyle rightButton;
			const int WIDTH = 16;

			public Styles() {
				toggle = new GUIStyle( EditorStyles.toggle );
				if( UnitySymbol.UNITY_2019_3_OR_NEWER ) {
					popup = new GUIStyle( "MiniPopup" );
				}
				else {
					popup = new GUIStyle( "Popup" );
				}

				rightButton = new GUIStyle( HEditorStyles.iconButton );
				rightButton.imagePosition = ImagePosition.ImageLeft;
				rightButton.fixedWidth = 0;
				rightButton.stretchWidth = true;
			}
		}


		internal static Styles s_styles;

		static Light[] s_lights = new Light[ 0 ];
		static Animator[] s_animator = new Animator[ 0 ];
		static bool s_lightTool;
		static bool s_lookAtTool;

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
			EditorTools.activeToolChanged += OnActiveToolChanged;
#endif

		}

		static void OnSceneOpened( UnityScene scene, OpenSceneMode mode ) {
			s_lights = new Light[ 0 ];
			s_lightTool = false;
			s_lookAtTool = false;
			s_cameraSelect = 0;
		}

		[DidReloadScripts]
		static void OnDidReloadScripts() {
		}

		static void OnSelectionChanged() {
			s_tileOn = false;
			if( !E.i.tools ) return;

			s_lights = Selection.gameObjects.Select( x => x.GetComponent<Light>() ).Where( x => x != null ).ToArray();
			s_animator = Selection.gameObjects.Select( x => x.GetComponent<Animator>() ).Where( x => x != null ).ToArray();
			//s_lightTool = false;

			if( Selection.activeGameObject == null ) return;
			if( Selection.activeGameObject.GetComponent<Tilemap>() ) {
				s_tileOn = true;
			}
			if( Selection.activeGameObject.GetComponent<Grid>() ) {
				s_tileOn = true;
			}
		}

		static void OnActiveToolChanged() {
#if UNITY_2019_1_OR_NEWER
			if( "NoneTool" == EditorTools.activeToolType.Name ) return;
#endif

			s_lightTool = false;
			s_lookAtTool = false;
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

		public static void OnToolLightItensity( Light[] targets ) {
			Tools.current = Tool.None;
			var sceneCamera = SceneView.currentDrawingSceneView.camera;

			using( new HandlesGUIScope() ) {
				foreach( Light p in targets ) {
					var pos = sceneCamera.WorldToScreenPoint( p.transform.position );
					var r = new Rect( pos.x, Screen.height - pos.y - 30, 80, 20 );

					EditorGUI.BeginChangeCheck();
					var _f = EditorGUI.FloatField( r, p.intensity );
					r.y += 20;

					EditorGUI.DrawRect( r.AlignCenterH( 12 ), new Color( 0.5849056f, 0.5849056f, 0.5849056f, 0.4666667f ) );

					//int controlID = GUIUtility.GetControlID( "HEditorSliderKnob".GetHashCode(), FocusType.Passive, r );
					//_f = GUI.Slider( r, _f, 0.0f, 0.00f, 5.00f, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb, true, controlID );
					_f = GUI.HorizontalSlider( r, _f, 0.0f, 5.00f );
					if( EditorGUI.EndChangeCheck() ) {
						EditorHelper.Dirty( p, () => {
							p.intensity = _f;
						} );
					}
				}
			}
		}


		public static void OnToolLightLookAt( Light[] targets ) {
			Tools.current = Tool.None;

			void _DirtyPosition( Transform p, Vector3 _pos, Vector3 _lookat ) {
				EditorHelper.Dirty( p, () => {
					var n = _lookat - _pos;
					p.position = _pos;
					p.rotation = Quaternion.LookRotation( n.normalized );
				} );
			}

			foreach( var l in targets ) {
				var p = l.transform;
				var data = SceneToolBehaviour.GetEditorToolBehaviour( p );
				var pos = p.position;
				var size = HandleUtility.GetHandleSize( pos );

				Handles.CapFunction RectangleHandleCap2D = ( id, _pos, rot, _size, eventType ) => {
					Handles.RectangleHandleCap( id, _pos, Quaternion.Euler( 0.0f, p.localEulerAngles.y, 0.0f ) * Quaternion.Euler( 90.0f, 0.0f, 0.0f ), 0.25f * size, eventType );
				};
				Handles.CapFunction RectangleHandleCapY = ( id, _pos, rot, _size, eventType ) => {
					Handles.RectangleHandleCap( id, _pos, Quaternion.Euler( 0.0f, p.localEulerAngles.y, 0.0f ) /** qt.Euler( 90.0f, 0.0f, 0.0f )*/, 0.25f * size, eventType );
				};
				Handles.CapFunction CircleHandleCap = ( id, _pos, rot, _size, eventType ) => {
					Handles.CircleHandleCap( id, _pos, Quaternion.Euler( 90.0f, 0.0f, 0.0f ), 0.25f * size, eventType );
				};


				EditorGUI.BeginChangeCheck();
				Handles.color = Handles.zAxisColor;
				pos = Handles.Slider( pos, p.forward );

				if( EditorGUI.EndChangeCheck() ) {
					_DirtyPosition( p, pos, data.lockAt );
				}

				EditorGUI.BeginChangeCheck();
				Handles.color = Handles.yAxisColor;
				var pos2 = Handles.FreeMoveHandle( pos, Quaternion.identity, 1, Vector3.zero, RectangleHandleCapY );
				if( EditorGUI.EndChangeCheck() ) {
					pos.y = pos2.y;
					_DirtyPosition( p, pos, data.lockAt );
				}

				EditorGUI.BeginChangeCheck();
				Handles.color = Handles.zAxisColor;
				pos2 = Handles.FreeMoveHandle( pos, Quaternion.identity, 1, Vector3.zero, RectangleHandleCap2D );
				if( EditorGUI.EndChangeCheck() ) {
					pos.x = pos2.x;
					pos.z = pos2.z;
					_DirtyPosition( p, pos, data.lockAt );
				}


				EditorGUI.BeginChangeCheck();
				var vv = Handles.FreeMoveHandle( data.lockAt, Quaternion.identity, 1, Vector3.zero, CircleHandleCap );
				if( EditorGUI.EndChangeCheck() ) {
					EditorHelper.Dirty( p, () => {
						data.lockAt.x = vv.x;
						data.lockAt.z = vv.z;
						var n = data.lockAt - p.transform.position;
						p.transform.position = pos;
						p.transform.rotation = Quaternion.LookRotation( n.normalized );
					} );
				}

			}
		}


		public static void OnSceneGUI( SceneView sceneView ) {
			if( !E.i.Enable ) return;

			if( !E.i.multiSceneExec ) {
				if( SceneViewUtils.sceneViewNum != 1 ) return;
			}

			if( s_styles == null ) s_styles = new Styles();

			s_styles.toggle.normal.textColor = E.i.textColor;
			s_styles.rightButton.normal.textColor = E.i.textColor;

			using( new GUISkinScope( EditorSkin.Inspector ) )
			using( new HandlesGUIScope() ) {
				DrawLeftBottom( sceneView );

				DrawRightBottom( sceneView );

				DrawToolPanel( sceneView );
			}

			if( E.i.tools ) {
				if( s_lightTool ) {
					foreach( var p in s_lights ) {
						OnToolLightItensity( s_lights.ToArray() );
					}
				}
				if( s_lookAtTool ) {
					foreach( var p in s_lights ) {
						OnToolLightLookAt( s_lights.ToArray() );
					}
				}
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
			var x = ( Screen.width * 0.5f ) - ( 64 * 0.5f );
			var y = Screen.height - ( 24 * 2 ) - ( 8 * 2 );
			Rect r = new Rect( x, y, 32 * 2 + 4, 20 );
			GUILayout.BeginArea( r );
			GUILayout.BeginHorizontal();
			if( 0 < s_lights.Length ) {
				EditorGUI.BeginChangeCheck();
				s_lightTool = GUILayout.Toggle( s_lightTool, EditorIcon.icons_processed_directionallight_icon_asset, EditorStyles.miniButton, GUILayout.Width( 32 ) );
				if( EditorGUI.EndChangeCheck() ) {
					Tools.current = Tool.None;
					s_lookAtTool = false;
				}
				EditorGUI.BeginChangeCheck();
				s_lookAtTool = GUILayout.Toggle( s_lookAtTool, EditorIcon.viewtoolorbit, EditorStyles.miniButton, GUILayout.Width( 32 ) );
				if( EditorGUI.EndChangeCheck() ) {
					Tools.current = Tool.None;
					s_lightTool = false;
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();


			HGUIScope.Area( new Rect( Screen.width - 200, 120, 200, Screen.height - 100 ), _area );
			void _area() {
				if( s_tileOn ) {
					ShowWindowButton( UnityTypes.GridPaintPaletteWindow, "Tilemap", EditorIcon.icons_processed_unityengine_tilemaps_tilemap_icon_asset );
				}
				if( 0 < s_lights.Length ) {
					ShowWindowButton( UnityTypes.LightingWindow, "Lighting", EditorIcon.lighting );
					ShowWindowButton( UnityTypes.LightingExplorerWindow, "Light Explorer", EditorIcon.lighting );
				}
				if( 0 < s_animator.Length ) {
					ShowWindowButton( UnityTypes.AnimatorControllerTool, "Animator", EditorIcon.unityeditor_graphs_animatorcontrollertool );
					ShowWindowButton( UnityTypes.AnimationWindow, "Animation", EditorIcon.unityeditor_animationwindow );
				}
				
			}
		}


		internal static void ShowWindowButton( Type type, string text, Texture2D image ) {
			if( type == null ) return;
			var _window = HEditorWindow.Find( type );
			if( _window != null ) return;

			HGUIScope.Horizontal( _ );
			void _() {
				GUILayout.FlexibleSpace();
				var contt = EditorHelper.TempContent( text, image );
				var aa = EditorStyles.label.CalcSize( text.content() );
				var rr = GUILayoutUtility.GetRect( contt, s_styles.rightButton, GUILayout.Width( aa.x + 16+4 ) );
				EditorGUI.DrawRect( rr, E.i.uiBkColor );
				if( GUI.Button( rr, contt, s_styles.rightButton ) ) {
					HEditorWindow.ShowWindow( type );
				}
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
					if( UnitySymbol.Has( "UNITY_2019_3_OR_NEWER" ) ) {
						rcRad.y -= 2;
					}
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
				if( UnitySymbol.Has( "UNITY_2019_3_OR_NEWER" ) ) {
					rcToggle.x += 2;
				}
				var cont = EditorHelper.TempContent( "Iso" );
				var sz = EditorStyles.toggle.CalcSize( cont );
				rcToggle.width = sz.x;

				var rc = rcToggle.AddW( 2 );
				EditorGUI.DrawRect( rc, E.i.uiBkColor );
				sceneView.orthographic = GUI.Toggle( rcToggle, sceneView.orthographic, cont, s_styles.toggle );
			}
		}
	}
}