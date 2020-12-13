using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using HananokiEditor;
#endif

namespace HananokiRuntime {
	public class SceneToolBehaviour : MonoBehaviour {
#if UNITY_EDITOR
		[System.Serializable]
		public class Data {
			public Transform target;
			public Vector3 lockAt;
		}
		public List<Data> lightTool = new List<Data>();


		public static Data GetEditorToolBehaviour( Transform trs ) {
			var obj = GetEditorToolBehaviour();

			var find = obj.lightTool.Find( x => x.target == trs );
			if( find != null ) return find;

			var data = new Data { target = trs };
			obj.lightTool.Add( data );
			return data;
		}


		public static SceneToolBehaviour GetEditorToolBehaviour() {
			//Debug.Log( SceneManager.GetActiveScene().name );
			//return null;
			var objs = EditorHelper.GetSceneObjects<SceneToolBehaviour>();
			if( 1 <= objs.Length ) return objs[ 0 ];

			objs = Resources.FindObjectsOfTypeAll( typeof( SceneToolBehaviour ) )
				.Where( a => !AssetDatabase.GetAssetOrScenePath( a ).Contains( ".unity" ) )
				.Select( a => (SceneToolBehaviour) a )
				.ToArray();
			if( 1 <= objs.Length ) return objs[ 0 ];

			var gobj = new GameObject( "SceneToolBehaviour" );
			gobj.tag = "EditorOnly";
			var comp = gobj.AddComponent<SceneToolBehaviour>();
			return comp;
		}
#endif
	}
}
