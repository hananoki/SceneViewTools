using HananokiRuntime.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HananokiEditor.SceneViewTools {

	[Hananoki_SceneView_ComponentTool( typeof( BoxCollider ) )]
	class BoxColliderTool : SceneViewComponentTool {
		bool toggleCenter;
		bool toggleSize;

		public override void OnGUI() {
			ScopeChange.Begin();
			GUILayout.Toggle( toggleCenter, EditorIcon.transformtool, EditorStyles.miniButton, GUILayout.Width( 32 ) );
			if( ScopeChange.End() ) {
				Tools.current = Tool.None;
				toggleCenter.Invert();
				toggleSize = false;
			}

			ScopeChange.Begin();
			GUILayout.Toggle( toggleSize, EditorIcon.scaletool, EditorStyles.miniButton, GUILayout.Width( 32 ) );
			if( ScopeChange.End() ) {
				Tools.current = Tool.None;
				toggleSize.Invert();
				toggleCenter = false;
			}
		}



		public override void OnSceneView( IEnumerable<Component> conponents ) {
			if( toggleCenter || toggleSize ) {
				var pp = conponents.Cast<BoxCollider>().ToArray();
				if( toggleCenter ) _center( pp );
				if( toggleSize ) _draw( pp );
			}
		}


		public static void _center( BoxCollider[] targets ) {
			Tools.current = Tool.None;
			var sceneCamera = SceneView.currentDrawingSceneView.camera;
			foreach( BoxCollider p in targets ) {
				ScopeChange.Begin();
				var pos = p.transform.TransformPoint( p.center );
				pos = Handles.PositionHandle( pos, p.transform.rotation );
				if( ScopeChange.End() ) {
					EditorHelper.Dirty( p, () => {
						p.center = p.transform.InverseTransformPoint( pos );
					} );
				}
			}

			using( new HandlesGUIScope() ) {
				foreach( BoxCollider p in targets ) {
					var pos = sceneCamera.WorldToScreenPoint( p.transform.TransformPoint( p.center ) );
					var r = new Rect( pos.x, Screen.height - pos.y - 30, 240, 20 );
					ScopeChange.Begin();
					var _f = EditorGUI.Vector3Field( r, "", p.center );
					if( ScopeChange.End() ) {
						EditorHelper.Dirty( p, () => {
							p.center = p.transform.InverseTransformPoint( _f );
						} );
					}
				}
			}
		}


		public static void _draw( BoxCollider[] targets ) {
			Tools.current = Tool.None;
			var sceneCamera = SceneView.currentDrawingSceneView.camera;
			foreach( BoxCollider p in targets ) {
				ScopeChange.Begin();
				var _pos = p.transform.TransformPoint( p.center );
				var _scl = Handles.ScaleHandle( p.size, _pos, p.transform.rotation, HandleUtility.GetHandleSize( _pos ) );
				if( ScopeChange.End() ) {
					EditorHelper.Dirty( p, () => {
						p.size = _scl;
					} );
				}
			}

			using( new HandlesGUIScope() ) {
				foreach( BoxCollider p in targets ) {
					var pos = sceneCamera.WorldToScreenPoint( p.transform.TransformPoint( p.center ) );
					var r = new Rect( pos.x, Screen.height - pos.y - 30, 240, 20 );
					ScopeChange.Begin();
					var _f = EditorGUI.Vector3Field( r, "", p.size );
					if( ScopeChange.End() ) {
						EditorHelper.Dirty( p, () => {
							p.size = _f;
						} );
					}
				}
			}
		}
	}
}
