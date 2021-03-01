using HananokiEditor.Extensions;
using HananokiRuntime;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HananokiEditor.SceneViewTools {

	[Hananoki_SceneView_ComponentTool( typeof( Light ) )]
	class LightTool : SceneViewComponentTool {
		bool s_lightTool;
		bool s_lookAtTool;

		public override void OnGUI() {
			ScopeChange.Begin();
			s_lightTool = GUILayout.Toggle( s_lightTool, EditorIcon.icons_processed_directionallight_icon_asset, EditorStyles.miniButton, GUILayout.Width( 32 ) );
			if( ScopeChange.End() ) {
				Tools.current = Tool.None;
				s_lookAtTool = false;
			}

			ScopeChange.Begin();
			s_lookAtTool = GUILayout.Toggle( s_lookAtTool, EditorIcon.viewtoolorbit, EditorStyles.miniButton, GUILayout.Width( 32 ) );
			if( ScopeChange.End() ) {
				Tools.current = Tool.None;
				s_lightTool = false;
			}
		}

		public override void OnSceneView( IEnumerable<Component> conponents ) {
			if( s_lightTool ) {
				OnToolLightItensity( conponents.Cast<Light>().ToArray() );

			}
			if( s_lookAtTool ) {
				OnToolLightLookAt( conponents.Cast<Light>().ToArray() );
			}
		}



		public static void OnToolLightItensity( Light[] targets ) {
			Tools.current = Tool.None;
			var sceneCamera = SceneView.currentDrawingSceneView.camera;

			using( new HandlesGUIScope() ) {
				foreach( Light p in targets ) {
					var pos = sceneCamera.WorldToScreenPoint( p.transform.position );
					var r = new Rect( pos.x, Screen.height - pos.y - 30, 80, 20 );

					ScopeChange.Begin();
					var _f = EditorGUI.FloatField( r, p.intensity );
					r.y += 20;

					EditorGUI.DrawRect( r.AlignCenterH( 12 ), new Color( 0.5849056f, 0.5849056f, 0.5849056f, 0.4666667f ) );

					//int controlID = GUIUtility.GetControlID( "HEditorSliderKnob".GetHashCode(), FocusType.Passive, r );
					//_f = GUI.Slider( r, _f, 0.0f, 0.00f, 5.00f, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb, true, controlID );
					_f = GUI.HorizontalSlider( r, _f, 0.0f, 5.00f );
					if( ScopeChange.End() ) {
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
	}
}
