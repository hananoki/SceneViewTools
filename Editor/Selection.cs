using HananokiEditor.Extensions;
using HananokiRuntime.Extensions;
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

using UnityScene = UnityEngine.SceneManagement.Scene;

namespace HananokiEditor.SceneViewTools {
	[InitializeOnLoad]
	public class SelectionHierarchy {

		static Hashtable s_componets;
		static SelectionData s_current;

		public static SelectionData current => s_current;


		/////////////////////////////////////////
		static SelectionHierarchy() {
			EditorApplication.hierarchyChanged += OnHierarchyChanged;
			Selection.selectionChanged += OnSelectionChanged;
			OnSelectionChanged();
		}


		/////////////////////////////////////////
		static void CreateHashTable() {
			s_componets = new Hashtable( 256 );
			s_current = null;
		}


		/////////////////////////////////////////
		static void OnHierarchyChanged() {
			CreateHashTable();
			OnSelectionChanged();
			SceneViewUtils.Repaint();
		}


		/////////////////////////////////////////
		static void OnSceneOpened( UnityScene scene, OpenSceneMode mode ) {
			CreateHashTable();
		}


		/////////////////////////////////////////
		static void OnSelectionChanged() {
			if( s_componets == null ) {
				CreateHashTable();
			}

			foreach( var go in Selection.gameObjects ) {
				s_current = (SelectionData) s_componets[ go.GetInstanceID() ];

				if( !go.ToAssetPath().IsEmpty() ) continue;

				if( s_current != null ) continue;

				s_current = new SelectionData {
					components = go.GetComponents( typeof( Component ) ).Where( x => x != null ).ToArray(),
				};
				s_current.componentTypes = s_current.components.Select( x => x.GetType() ).ToArray();
				s_componets.Add( go.GetInstanceID(), s_current );
			}
		}
	}



	public class SelectionData {
		public Component[] components;
		public Type[] componentTypes;

		public T[] GetComponents<T>() where T : Component {
			return components.OfType<T>().ToArray();
		}
	}

}
