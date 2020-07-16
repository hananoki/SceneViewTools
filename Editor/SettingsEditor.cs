
using Hananoki.Extensions;
using Hananoki.SharedModule;
using UnityEditor;
using UnityEngine;

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

		public static void Open() {
			var w = GetWindow<SettingsEditorWindow>();
			w.SetTitle( new GUIContent( Package.name, EditorIcon.settings ) );
			w.headerMame = Package.name;
			w.headerVersion = Package.version;
			w.gui = DrawGUI;
		}


		public static void DrawGUI() {
			E.Load();

			EditorGUI.BeginChangeCheck();

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
			//}

			if( EditorGUI.EndChangeCheck() ) {
				E.Save();
				SceneViewUtils.Repaint();
			}
#if TEST_OBJECTSTAT
			if( GUILayout.Button( "ObjectStat" ) ) {
				ObjectStat.Enable();
			}
#endif
			if( GUILayout.Button( "remove" ) ) {
				SceneViewUtils.RemoveGUI( SceneViewTools.OnSceneGUI );
			}
			if( GUILayout.Button( "remove" ) ) {
				SceneViewUtils.AddGUI( SceneViewTools.OnSceneGUI );
			}
		}


#if !ENABLE_HANANOKI_SETTINGS
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
			using( new LayoutScope() ) DrawGUI();
		}
#endif
	}



#if ENABLE_HANANOKI_SETTINGS
	[SettingsClass]
	public class SettingsEvent {
		[SettingsMethod]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				//mode = 1,
				displayName = Package.name,
				version = Package.version,
				gui = SettingsEditorWindow.DrawGUI,
			};
		}
	}
#endif
}
