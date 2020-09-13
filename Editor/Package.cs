
using UnityEditor;

namespace Hananoki.SceneViewTools {
  public static class Package {
    public const string name = "SceneViewTools";
    public const string editorPrefName = "Hananoki.SceneViewTools";
    public const string version = "1.0.7";
  }
  
#if UNITY_EDITOR
  [EditorLocalizeClass]
  public class LocalizeEvent {
    [EditorLocalizeMethod]
    public static void Changed() {
      foreach( var filename in DirectoryUtils.GetFiles( AssetDatabase.GUIDToAssetPath( "bee5713c1b219a941859080a666e8150" ), "*.csv" ) ) {
        if( filename.Contains( EditorLocalize.GetLocalizeName() ) ) {
          EditorLocalize.Load( Package.name, AssetDatabase.AssetPathToGUID( filename ), "a010ba5120e7d424daa609a6d016100e" );
        }
      }
    }
  }
#endif
}
