using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static class CrashReporter
{
	private const uint MB_OK = 0u;

	private const uint MB_ICONERROR = 16u;

	private const uint MB_TOPMOST = 262144u;

	private const uint MB_SETFOREGROUND = 65536u;

	private static bool hasShownPopup;

	private static readonly object lockObj = new object();

	[DllImport("user32.dll", EntryPoint = "MessageBoxW", SetLastError = true)]
	private static extern int MessageBox(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string text, [MarshalAs(UnmanagedType.LPWStr)] string caption, uint type);

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	public static void Initialize()
	{
		try
		{
			Application.logMessageReceivedThreaded += HandleThreadedLog;
			AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to initialize CrashReporter: " + ex);
		}
	}

	private static void HandleThreadedLog(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Exception || type == LogType.Error)
		{
			ShowCrashPopup("C# Exception (" + type + ")", logString + "\n\nStackTrace:\n" + stackTrace);
		}
	}

	private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		string text = ((e.ExceptionObject != null) ? e.ExceptionObject.ToString() : "Unknown exception");
		ShowCrashPopup("Unhandled Thread Exception", text);
	}

	public static void ShowCrashPopup(string errorType, string details)
	{
		lock (lockObj)
		{
			if (hasShownPopup)
			{
				return;
			}
			hasShownPopup = true;
		}
		try
		{
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			string path = Path.Combine(Application.persistentDataPath, "crash_log.txt");
			string text2 = "========================================\n[Dragon Boy Crash Report - " + text + "]\nLoại lỗi: " + errorType + "\n\nChi tiết:\n" + details + "\n========================================\n";
			File.AppendAllText(path, text2, Encoding.UTF8);
			string text3 = "Game gặp sự cố và bắt buộc phải dừng lại!\n\nVui lòng chụp màn hình thông báo này gửi cho Admin:\n--------------------------------------------------\n" + text2;
			if (text3.Length > 1500)
			{
				text3 = text3.Substring(0, 1500) + "\n...\n(Toàn bộ chi tiết lỗi đã được lưu tại crash_log.txt)";
			}
			MessageBox(IntPtr.Zero, text3, "[Dragon Boy] - LỖI TRÒ CHƠI", 327696u);
		}
		catch (Exception ex)
		{
			Debug.LogError("Error showing crash MessageBox: " + ex);
		}
	}
}
