using UnityEngine;
using Steamworks;

[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
    private static SteamManager s_instance;

    public static bool Initialized => s_instance != null && s_instance.m_bInitialized;

    private bool m_bInitialized;
    private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

    private static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
    {
        Debug.LogWarning(pchDebugText);
    }

    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
        DontDestroyOnLoad(gameObject);

        if (!Packsize.Test())
        {
            Debug.LogError("[Steamworks.NET] Packsize Test failed.");
            return;
        }

        if (!DllCheck.Test())
        {
            Debug.LogError("[Steamworks.NET] DllCheck Test failed.");
            return;
        }

        try
        {
            if (SteamAPI.RestartAppIfNecessary((AppId_t)5080810))
            {
                Application.Quit();
                return;
            }
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("[Steamworks.NET] Could not load steam_api.dll/so/dylib: " + e);
            return;
        }

        m_bInitialized = SteamAPI.Init();
        if (!m_bInitialized)
        {
            Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed.");
            return;
        }
    }

    private void OnEnable()
    {
        if (s_instance == null) s_instance = this;

        if (!m_bInitialized) return;

        if (m_SteamAPIWarningMessageHook == null)
        {
            m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
            SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
        }
    }

    private void OnDestroy()
    {
        if (s_instance != this) return;

        s_instance = null;

        if (!m_bInitialized) return;

        SteamAPI.Shutdown();
    }

    private void Update()
    {
        if (!m_bInitialized) return;

        SteamAPI.RunCallbacks();
    }
}