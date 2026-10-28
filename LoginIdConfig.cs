using System;
using UnityEngine;

public static class LoginIdConfig
{
	private const string LOGIN_ID_RESOURCE_PATH = "login_codes_generated";

	private static readonly object configLock = new object();

	private static bool loaded;

	private static bool valid;

	private static string loginCode;

	public static string getLoginCode()
	{
		lock (configLock)
		{
			if (!loaded)
			{
				loaded = true;
				TextAsset textAsset = Resources.Load<TextAsset>(LOGIN_ID_RESOURCE_PATH);
				if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
				{
					Debug.LogError("[LOGIN] Missing or empty Resources/login_codes_generated.txt.");
					valid = false;
					loginCode = null;
				}
				else
				{
					string text = textAsset.text.Trim();
					if (IsValidLoginId(text))
					{
						valid = true;
						loginCode = text;
					}
					else
					{
						valid = false;
						loginCode = null;
						LogInvalidLoginIdConfig();
					}
				}
			}
			return valid ? loginCode : null;
		}
	}

	private static bool IsValidLoginId(string value)
	{
		if (string.IsNullOrEmpty(value) || value.Length != 32)
		{
			return false;
		}
		for (int i = 0; i < value.Length; i++)
		{
			char c = value[i];
			if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
			{
				return false;
			}
		}
		return true;
	}

	private static string LogInvalidLoginIdConfig()
	{
		Debug.LogError("[LOGIN] Invalid Resources/login_codes_generated.txt. Expected one entry with a 32-character hexadecimal value.");
		return null;
	}
}
