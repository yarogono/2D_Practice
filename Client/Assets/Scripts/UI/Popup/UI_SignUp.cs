using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SignUp : UI_Popup
{

    enum Buttons
    {
        ExitButton,
        SignUpButton,
    }

    enum InputFields
    {
        IDinput,
        PasswordInput,
        PasswordInputTwo,
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputFields));

        GetButton((int)Buttons.ExitButton).gameObject.AddUIEvent(OnExitButtonClicked);
        GetButton((int)Buttons.SignUpButton).gameObject.AddUIEvent(OnSignUpButtonClicked);
    }

    public void OnSignUpButtonClicked(PointerEventData data)
    {
        string id = GetTMPInputField((int)InputFields.IDinput).text;
        string password = GetTMPInputField((int)InputFields.PasswordInput).text;
        string passwordTwo = GetTMPInputField((int)InputFields.PasswordInputTwo).text;

        if (id == null || password == null || passwordTwo == null)
            return;

        if (password.Equals(passwordTwo) == false)
        {
            // ToDo : ��й�ȣ ����ġ �˾� â ����
            Debug.Log("��й�ȣ�� �ٽ� �Է����ּ���.");
            return;
        }

        C_SavePlayer savePlayerPacket = new C_SavePlayer() { username = id, password = password };
        NetworkManager.Instance.Send(savePlayerPacket.Write());
        Debug.Log("���̵� �����Ǿ����ϴ�.");

        ClosePopupUI();
    }


    public void OnExitButtonClicked(PointerEventData data)
    {
        ClosePopupUI();
    }
}
