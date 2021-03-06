using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HananokiEditor.Extensions;
using HananokiEditor.SharedModule;
using UnityEditor;
using UnityEngine;
using E = HananokiEditor.SceneViewTools.SettingsEditor;
using SS = HananokiEditor.SharedModule.S;

namespace HananokiEditor.SceneViewTools {
	public class SettingsDrawer_Main {

		[HananokiSettingsRegister]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				//mode = 1,
				displayName = Package.nameNicify,
				version = Package.version,
				gui = DrawGUI,
			};
		}



		public static void DrawGUI() {
			E.Load();

			ScopeChange.Begin();

			E.i.Enable = HEditorGUILayout.ToggleLeft( SS._Enable, E.i.Enable );
			EditorGUI.indentLevel++;
			GUILayout.Space( 8f );
			using( new EditorGUI.DisabledGroupScope( !E.i.Enable ) ) {
				E.i.multiSceneExec = HEditorGUILayout.ToggleLeft( S._Executeevenwithmultiplesceneviews, E.i.multiSceneExec );

				E.i.enableTimeScaleSlider = HEditorGUILayout.ToggleLeft( S._TimeScaleSlider, E.i.enableTimeScaleSlider );
				E.i.syncScene2Game = HEditorGUILayout.ToggleLeft( S._Syncscenecameratogamecamera, E.i.syncScene2Game );
				E.i.toggleOrthographic = HEditorGUILayout.ToggleLeft( S._ToggleOrthographic, E.i.toggleOrthographic );

				E.i.uiBkColor = EditorGUILayout.ColorField( SS._BackColor, E.i.uiBkColor );
				E.i.textColor = EditorGUILayout.ColorField( SS._TextColor, E.i.textColor );

				GUILayout.Space( 8f );
				EditorGUILayout.LabelField( $"* {SS._Experimental}", EditorStyles.boldLabel );
				E.i.tools = HEditorGUILayout.ToggleLeft( "Tool (UNITY_2019_1_OR_NEWER)", E.i.tools );
				E.i.drawPivotBox = HEditorGUILayout.ToggleLeft( "Draw Pivot Box", E.i.drawPivotBox );
				EditorGUI.indentLevel++;
				ScopeDisable.Begin(!E.i.drawPivotBox );
				E.i.drawPivotLabel = HEditorGUILayout.ToggleLeft( "Label", E.i.drawPivotLabel );
				ScopeDisable.End();
				EditorGUI.indentLevel--;

				E.i.raycastPivot = HEditorGUILayout.ToggleLeft( "RayCast Pivot (Alt)", E.i.raycastPivot );
				E.i.resetPivotSize = HEditorGUILayout.ToggleLeft( "Reset Pivot Size (G)", E.i.resetPivotSize );
				E.i.wsadMove = HEditorGUILayout.ToggleLeft( "Pivot Move (W,A,S,D,Q,E)", E.i.wsadMove );
				E.i.disableSelection = HEditorGUILayout.ToggleLeft( "Disable Selection (Space)", E.i.disableSelection );
				
			}
			EditorGUI.indentLevel--;
			//}

			if( ScopeChange.End() ) {
				E.Save();
				SceneViewUtils.Repaint();
			}
#if TEST_OBJECTSTAT
			if( GUILayout.Button( "ObjectStat" ) ) {
				ObjectStat.Enable();
			}
#endif

		}


	}
}
