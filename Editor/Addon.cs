using HananokiEditor.Extensions;
using HananokiRuntime;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityReflection;

namespace HananokiEditor.SceneViewTools {
	class Addon {
		[Hananoki_SceneView_ComponentButton( typeof( Light ) )]
		static void _Light() {
			SceneViewTools.ShowWindowButton( UnityTypes.UnityEditor_LightingWindow, "Lighting", EditorIcon.lighting );
			SceneViewTools.ShowWindowButton( UnityTypes.UnityEditor_LightingExplorerWindow, "Light Explorer", EditorIcon.lighting );
			if( Lightmapping.isRunning ) {
				if( SceneViewTools.ShowSideButton( "Cancel", EditorIcon.lighting ) ) {
					Lightmapping.Cancel();
				}
			}
			else {
				if( SceneViewTools.ShowSideButton( "Generate Lighting", EditorIcon.lighting ) ) {
					Lightmapping.ClearLightingDataAsset();
					Lightmapping.Clear();
					Lightmapping.BakeAsync();
				}
			}
		}

		[Hananoki_SceneView_ComponentButton( typeof( ReflectionProbe ) )]
		static void _ReflectionProbe() {
			if( SceneViewTools.ShowSideButton( "Bake", EditorIcon.icons_processed_unityengine_reflectionprobe_icon_asset ) ) {
				foreach( var p in SelectionHierarchy.current.GetComponents<ReflectionProbe>() ) {
					UnityEditorLightmapping.BakeReflectionProbeSnapshot( p );
				}
			}
			if( SceneViewTools.ShowSideButton( "Bake All Reflection Probes", EditorIcon.icons_processed_unityengine_reflectionprobe_icon_asset ) ) {
				UnityEditorLightmapping.BakeAllReflectionProbesSnapshots();
			}
		}


		[Hananoki_SceneView_ComponentButton( typeof( Animator ) )]
		static void _Animator() {
			SceneViewTools.ShowWindowButton( UnityTypes.UnityEditor_Graphs_AnimatorControllerTool, "Animator", EditorIcon.unityeditor_graphs_animatorcontrollertool );
			SceneViewTools.ShowWindowButton( UnityTypes.UnityEditor_AnimationWindow, "Animation", EditorIcon.unityeditor_animationwindow );
		}

		const string FONT_ASSET_CREATOR = "Window/TextMeshPro/Font Asset Creator";
		static Texture2D tico;
		static void _TMPro_TMP_Text() {
			//if( EditorHelper.HasMenuItem( FONT_ASSET_CREATOR ) ) {
			if( tico == null ) {
				tico = UnityTypes.TMPro_TextMeshProUGUI.GetIcon();
			}
			if( SceneViewTools.ShowSideButton( FONT_ASSET_CREATOR.FileNameWithoutExtension(), tico ) ) {
				EditorApplication.ExecuteMenuItem( FONT_ASSET_CREATOR );
			}
			//}
		}
		[Hananoki_SceneView_ComponentButton( "TMPro.TextMeshProUGUI" )]
		static void _TMPro_TextMeshProUGUI() => _TMPro_TMP_Text();
		[Hananoki_SceneView_ComponentButton( "TMPro.TextMeshPro" )]
		static void _TMPro_TextMeshPr() => _TMPro_TMP_Text();
	}



	public static class Extensions {

		public static void SideButtonGUI( this SelectionData selectionData ) {
			foreach( var p in selectionData.componentTypes ) {
				var action = (Action) SceneViewTools.m_shortCuts[ p ];
				action?.Invoke();
			}
		}


		public static void ComponetToolGUI( this SelectionData selectionData ) {
			foreach( var t in selectionData.componentTypes.Distinct() ) {
				var tool = (SceneViewComponentTool) SceneViewTools.m_componetTool[ t ];
				tool?.OnGUI();
			}
		}


		public static void ComponetToolSceneView( this SelectionData selectionData ) {
			foreach( var t in selectionData.componentTypes ) {
				var tool = (SceneViewComponentTool) SceneViewTools.m_componetTool[ t ];
				if( tool == null ) continue;
				tool?.OnSceneView( selectionData.components.Where( x => !Helper.IsNull( x ) ).Where( x => x.GetType() == t ) );
			}
		}
	}
}
