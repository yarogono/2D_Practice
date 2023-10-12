using ServerCore;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

class PacketHandler
{
    public static void S_PlayerInfoHandler(PacketSession session, IPacket packet)
    {
        
    }


    public static void S_PlayerLoginHandler(PacketSession session, IPacket packet)
    {
        S_PlayerLogin playerLoginPacket = packet as S_PlayerLogin;
        ServerSession serverSession = session as ServerSession;

        int loginOk = playerLoginPacket.loginOk;

        if (loginOk != 1)
            return;

        Debug.Log("�α����� �Ϸ�Ǿ����ϴ�.");
        UIManager.Instance.ClosePopupUI();
        SceneManagerEx.Instance.LoadScene(Define.Scene.Game);
    }

    public static void S_SavePlayerHandler(PacketSession session, IPacket packet)
    {
        S_SavePlayer savePlayerPacket = packet as S_SavePlayer;
        ServerSession serverSession = session as ServerSession;

        int saveOk = savePlayerPacket.saveOk;

        if (saveOk != 1)
            return;

        Debug.Log("ȸ�������� �Ϸ�Ǿ����ϴ�.");
        UIManager.Instance.ClosePopupUI();
    }
}
