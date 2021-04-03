#if UNITY_EDITOR_WIN

using System;
using System.Runtime.InteropServices;
using System.Text;


public class Win32API {
	[StructLayout( LayoutKind.Sequential, Pack = 2 )]
	public struct POINT {
		public int x, y;
	}
	public const ushort VK_LBUTTON = 0x01;
	public const ushort VK_RBUTTON = 0x02;
	public const ushort VK_MBUTTON = 0x04;
	public const ushort VK_LMENU = 0xA4; // 左ALT

	public delegate int EnumWindowsCallback( IntPtr hWnd, int lParam );

	[DllImport( "user32.dll" )]
	public static extern int ShowCursor( bool bShow );

	[DllImport( "user32.dll" )]
	public static extern bool SetCursorPos( int x, int y );

	[DllImport( "user32.dll" )]
	public static extern bool GetCursorPos( out POINT point );

	[DllImport( "user32.dll" )]
	public static extern short GetKeyState( int nVirtKey );

	[DllImport( "user32.dll", SetLastError = true )]
	public static extern short GetAsyncKeyState( int vKey );

	[DllImport( "user32.dll" )]
	public static extern int EnumWindows( EnumWindowsCallback lpEnumFunc, int lParam );

	[DllImport( "User32.dll" )]
	public static extern uint GetWindowThreadProcessId( IntPtr hWnd, out uint lpdwProcessId );

	[DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
	public static extern int GetWindowText( IntPtr hWnd, StringBuilder lpString, int nMaxCount );

	[DllImport( "user32.dll" )]
	public extern static IntPtr GetForegroundWindow();

}
#endif
