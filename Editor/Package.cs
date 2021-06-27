
using UnityEditor;

namespace HananokiEditor.SceneViewTools {
  public static class Package {
    public const string reverseDomainName = "com.hananoki.scene-view-tools";
    public const string name = "SceneViewTools";
    public const string nameNicify = "Scene View Tools";
    public const string editorPrefName = "Hananoki.SceneViewTools";
    public const string version = "1.4.0";
		[HananokiEditorMDViewerRegister]
		public static string MDViewerRegister() {
			return "57fed90f1dde0a349bb31a201545a424";
		}
  }
}
