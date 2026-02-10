/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_LoginPanel : GComponent
    {
        public UI_Com_Input m_account;
        public UI_Com_Input m_possword;
        public GButton m_loginBtn;
        public GButton m_registerBtn;
        public const string URL = "ui://hhu0yjwqwnn80";

        public static UI_LoginPanel CreateInstance()
        {
            return (UI_LoginPanel)UIPackage.CreateObject("Login", "LoginPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_account = (UI_Com_Input)GetChildAt(2);
            m_possword = (UI_Com_Input)GetChildAt(3);
            m_loginBtn = (GButton)GetChildAt(4);
            m_registerBtn = (GButton)GetChildAt(5);
        }
    }
}