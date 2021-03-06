﻿
using System;
using UnityEngine;
using HananokiEditor;
using HananokiRuntime;

using UnityEditor;

#if ENABLE_TILEMAP
using UnityEngine.Tilemaps;


using UnityObject = UnityEngine.Object;

[CustomEditor( typeof( TilemapPallet ), true )]
public class TilemapPalletInspector : Editor {
	TilemapPallet self { get { return target as TilemapPallet; } }

	Type m_gridPalettes;
#if ENABLE_TILEMAP
	TileBase m_tileBase;
	Editor m_tileBaseEditor;
#endif

	void OnEnable() {
		m_gridPalettes = UnityTypes.UnityEditor_Tilemaps_GridPalettes;
	}

	//void OnDisable() {
	//	var tilemap = self.GetComponent<Tilemap>();
	//	if( tilemap ) {
	//		tilemap.color = ColorUtils.Alpha( tilemap.color, 1.0f );
	//	}
	//}

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		var tilemap = self.GetComponent<Tilemap>();
		if( tilemap ) {
			EditorGUILayout.LabelField( "Tilemap", EditorStyles.boldLabel );
			EditorGUI.BeginChangeCheck();
			var a = EditorGUILayout.Slider( "Alpha", tilemap.color.a, 0.00f, 1.00f );
			if( EditorGUI.EndChangeCheck() ) {
				tilemap.color = ColorUtils.RGBA( tilemap.color, a );
			}
		}

#if ENABLE_TILEMAP
		var window = EditorWindowUtils.Find( UnityTypes.UnityEditor_Tilemaps_GridPaintPaletteWindow );
		if( window == null ) return;
		var clipboardView = window.GetProperty<object>( "clipboardView" );
		var _tileBase = clipboardView.GetProperty<TileBase>( "activeTile" );
		if( _tileBase != m_tileBase ) {
			m_tileBase = _tileBase;
			m_tileBaseEditor = Editor.CreateEditor( m_tileBase );
		}
		if( m_tileBaseEditor != null ) {
			ScopeVertical.Begin( EditorStyles.helpBox );
			ScopeHorizontal.Begin();
			EditorGUILayout.LabelField( "Active Tile", EditorStyles.boldLabel );
			GUILayout.FlexibleSpace();
			if( GUILayout.Button( "Delete" ) ) {
				UnityObject.DestroyImmediate( _tileBase, true );
			}
			ScopeHorizontal.End();
			m_tileBaseEditor.OnInspectorGUI();
			ScopeVertical.End();
		}
#endif
	}
}

#endif
