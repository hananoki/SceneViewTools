using HananokiEditor.Extensions;
using HananokiRuntime.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HananokiEditor.SceneViewTools {

	[Hananoki_SceneView_ComponentTool( typeof( SphereCollider ) )]
	class SphereColliderTool : SceneViewComponentTool {

		bool m_toggle���a;

		public override void OnGUI() {

			ScopeChange.Begin();
			GUILayout.Toggle( m_toggle���a, EditorIcon.scaletool, EditorStyles.miniButton, GUILayout.Width( 32 ) );
			if( ScopeChange.End() ) {
				Tools.current = Tool.None;
				m_toggle���a.Invert();
			}
		}


		public override void OnSceneView( IEnumerable<Component> conponents ) {
			if( m_toggle���a ) {
				var pp = conponents.Cast<SphereCollider>().ToArray();
				if( m_toggle���a ) _center( pp );
			}
		}


		public static void _center( SphereCollider[] targets ) {
			Tools.current = Tool.None;
			var sceneCamera = SceneView.currentDrawingSceneView.camera;

			using( new HandlesGUIScope() ) {
				foreach( var p in targets ) {
					var pos = sceneCamera.WorldToScreenPoint( p.transform.position );
					var r = new Rect( pos.x, Screen.height - pos.y - 30, 160, 20 );

					ScopeChange.Begin();
					var _f = EditorGUI.FloatField( r, p.radius );
					r.y += 20;

					EditorGUI.DrawRect( r.AlignCenterH( 12 ), new Color( 0.5849056f, 0.5849056f, 0.5849056f, 0.4666667f ) );

					_f = GUI.HorizontalSlider( r, _f, 0.0f, 5.00f );
					if( ScopeChange.End() ) {
						EditorHelper.Dirty( p, () => {
							p.radius = _f;
						} );
					}
				}
			}
		}
	}
}

