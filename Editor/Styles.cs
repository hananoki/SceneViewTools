using UnityEditor;
using UnityEngine;

namespace HananokiEditor.SceneViewTools {

	internal class Styles {
		public static GUIStyle popup => s_styles.Popup;
		public static GUIStyle toggle => s_styles.Toggle;
		public static GUIStyle rightButton => s_styles.RightButton;

		public GUIStyle Popup;
		public GUIStyle Toggle;
		public GUIStyle RightButton;
		const int WIDTH = 16;

		public Styles() {
			Toggle = new GUIStyle( EditorStyles.toggle );
			if( UnitySymbol.UNITY_2019_3_OR_NEWER ) {
				Popup = new GUIStyle( "MiniPopup" );
			}
			else {
				Popup = new GUIStyle( "Popup" );
			}

			RightButton = new GUIStyle( HEditorStyles.iconButton );
			RightButton.imagePosition = ImagePosition.ImageLeft;
			RightButton.fixedWidth = 0;
			RightButton.stretchWidth = true;
		}

		static Styles s_styles;

		public static void Init() {
			if( s_styles == null ) {
				s_styles = new Styles();
			}
		}
	}
}
