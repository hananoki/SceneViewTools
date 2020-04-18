
//#define TEST_OBJECTSTAT

using UnityEditor;
using UnityEngine;
using Hananoki.Extensions;

using E = Hananoki.SceneViewTools.SettingsEditor;
using SS = Hananoki.SharedModule.S;

namespace Hananoki.SceneViewTools {

	[System.Serializable]
	public class SettingsEditor {
		public delegate void PreferencesGUICallback();

		public bool Enable = true;

		public bool multiSceneExec = true;

		public bool enableTimeScaleSlider = true;
		public bool toggleOrthographic = false;
		public bool syncScene2Game = true;
		public Color uiBkColor = new Color( 0, 0, 0, 0.10f );
		public Color textColor = Color.white;


		public static E i;

		public PreferencesGUICallback s_preferencesGUICallback;


		public static void Load() {
			if( i != null ) return;
			i = EditorPrefJson<E>.Get( Package.editorPrefName );
		}

		public static void Save() {
			EditorPrefJson<E>.Set( Package.editorPrefName, i );
		}
	}



	public class SettingsEditorWindow : HSettingsEditorWindow {

		static Vector2 scrollPos;

		public static void Open() {
			var window = GetWindow<SettingsEditorWindow>();
			window.SetTitle( new GUIContent( Package.name, Icon.Get( "SettingsIcon" ) ) );
		}

		void OnEnable() {
			drawGUI = DrawGUI;
			E.Load();
		}


		static void DrawGUI() {
			EditorGUI.BeginChangeCheck();

			using( new PreferenceLayoutScope( ref scrollPos ) ) {
				E.i.Enable = HEditorGUILayout.ToggleLeft( SS._Enable, E.i.Enable );
				EditorGUI.indentLevel++;
				GUILayout.Space( 8f );

				E.i.multiSceneExec = HEditorGUILayout.ToggleLeft( S._Executeevenwithmultiplesceneviews, E.i.multiSceneExec );

				E.i.enableTimeScaleSlider = HEditorGUILayout.ToggleLeft( S._TimeScaleSlider, E.i.enableTimeScaleSlider );
				E.i.syncScene2Game = HEditorGUILayout.ToggleLeft( S._Syncscenecameratogamecamera, E.i.syncScene2Game );
				E.i.toggleOrthographic = HEditorGUILayout.ToggleLeft( S._ToggleOrthographic, E.i.toggleOrthographic );

				E.i.uiBkColor = EditorGUILayout.ColorField( SS._BackColor, E.i.uiBkColor );
				E.i.textColor = EditorGUILayout.ColorField( SS._TextColor, E.i.textColor );
				EditorGUI.indentLevel--;
			}

			if( EditorGUI.EndChangeCheck() ) {
				E.Save();
				SceneViewUtils.Repaint();
			}
#if TEST_OBJECTSTAT
			if( GUILayout.Button( "ObjectStat" ) ) {
				ObjectStat.Enable();
			}
#endif
		}



#if UNITY_2018_3_OR_NEWER && !ENABLE_LEGACY_PREFERENCE

		[SettingsProvider]
		public static SettingsProvider PreferenceView() {
			var provider = new SettingsProvider( $"Preferences/Hananoki/{Package.name}", SettingsScope.User ) {
				label = $"{Package.name}",
				guiHandler = PreferencesGUI,
				titleBarGuiHandler = () => GUILayout.Label( $"{Package.version}", EditorStyles.miniLabel ),
			};
			return provider;
		}

		public static void PreferencesGUI( string searchText ) {
#else
		[PreferenceItem( Package.name )]
		public static void PreferencesGUI() {
#endif
			E.Load();
			DrawGUI();
		}
	}
}
