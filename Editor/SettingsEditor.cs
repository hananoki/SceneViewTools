#define ENABLE_HANANOKI_SETTINGS

using HananokiRuntime.Extensions;
using UnityEngine;
using E = HananokiEditor.SceneViewTools.SettingsEditor;

namespace HananokiEditor.SceneViewTools {

	[System.Serializable]
	public class SettingsEditor {
		public delegate void PreferencesGUICallback();

		public bool Enable = true;

		public int flag;

		#region Constant

		const int MULTI_SCENE_EXEC = ( 1 << 0 );
		const int ENABLE_TIME_SCALE_SLIDER = ( 1 << 1 );
		const int TOGGLE_ORTHOGRAPHIC = ( 1 << 2 );
		const int SYNC_SCENE2GAME = ( 1 << 3 );
		const int TOOLS = ( 1 << 4 );

		const int DRAW_PIVOT_BOX = ( 1 << 5 );
		const int DRAW_PIVOT_LABEL = ( 1 << 6 );
		const int RESET_PIVOT_SIZE = ( 1 << 7 );
		const int RAYCAST_PIVOT = ( 1 << 8 );
		const int DISABLE_SELECTION = ( 1 << 9 );
		const int WSAD_MOVE = ( 1 << 10 );
		const int MOUSE_DRAG = ( 1 << 11 );
		const int CROSS_LINE = ( 1 << 12 );

		#endregion


		#region Property

		public bool multiSceneExec {
			get => flag.Has( MULTI_SCENE_EXEC );
			set => flag.Toggle( MULTI_SCENE_EXEC, value );
		}
		public bool enableTimeScaleSlider {
			get => flag.Has( ENABLE_TIME_SCALE_SLIDER );
			set => flag.Toggle( ENABLE_TIME_SCALE_SLIDER, value );
		}
		public bool toggleOrthographic {
			get => flag.Has( TOGGLE_ORTHOGRAPHIC );
			set => flag.Toggle( TOGGLE_ORTHOGRAPHIC, value );
		}
		public bool syncScene2Game {
			get => flag.Has( SYNC_SCENE2GAME );
			set => flag.Toggle( SYNC_SCENE2GAME, value );
		}
		public bool tools {
			get => flag.Has( TOOLS );
			set => flag.Toggle( TOOLS, value );
		}
		public bool drawPivotBox {
			get => flag.Has( DRAW_PIVOT_BOX );
			set => flag.Toggle( DRAW_PIVOT_BOX, value );
		}
		public bool drawPivotLabel {
			get => flag.Has( DRAW_PIVOT_LABEL );
			set => flag.Toggle( DRAW_PIVOT_LABEL, value );
		}
		public bool resetPivotSize {
			get => flag.Has( RESET_PIVOT_SIZE );
			set => flag.Toggle( RESET_PIVOT_SIZE, value );
		}
		public bool raycastPivot {
			get => flag.Has( RAYCAST_PIVOT );
			set => flag.Toggle( RAYCAST_PIVOT, value );
		}
		public bool disableSelection {
			get => flag.Has( DISABLE_SELECTION );
			set => flag.Toggle( DISABLE_SELECTION, value );
		}
		public bool wsadMove {
			get => flag.Has( WSAD_MOVE );
			set => flag.Toggle( WSAD_MOVE, value );
		}
		public bool mouseDrag {
			get => flag.Has( MOUSE_DRAG );
			set => flag.Toggle( MOUSE_DRAG, value );
		}
		public bool crossLine {
			get => flag.Has( CROSS_LINE );
			set => flag.Toggle( CROSS_LINE, value );
		}

		#endregion


		public Color uiBkColor = new Color( 0, 0, 0, 0.10f );
		public Color textColor = Color.white;


		public static E i;


		/////////////////////////////////////////
		public static void Load() {
			if( i != null ) return;
			i = EditorPrefJson<E>.Get( Package.editorPrefName );
		}


		/////////////////////////////////////////
		public static void Save() {
			EditorPrefJson<E>.Set( Package.editorPrefName, i );
		}
	}
}
