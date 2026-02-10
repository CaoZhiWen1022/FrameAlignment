/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_RegisterPopup : GComponent
    {
        public UI_Com_Input m_account;
        public UI_Com_Input m_password;
        public UI_Com_Input m_username;
        public GButton m_register;
        public const string URL = "ui://hhu0yjwqpuiz1";

        public static UI_RegisterPopup CreateInstance()
        {
            return (UI_RegisterPopup)UIPackage.CreateObject("Login", "RegisterPopup");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_account = (UI_Com_Input)GetChildAt(2);
            m_password = (UI_Com_Input)GetChildAt(3);
            m_username = (UI_Com_Input)GetChildAt(4);
            m_register = (GButton)GetChildAt(5);
        }
    }
}