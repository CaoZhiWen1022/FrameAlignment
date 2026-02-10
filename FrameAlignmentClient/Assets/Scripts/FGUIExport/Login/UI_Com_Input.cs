/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_Com_Input : GLabel
    {
        public GTextInput m_input;
        public const string URL = "ui://hhu0yjwqpuiz2";

        public static UI_Com_Input CreateInstance()
        {
            return (UI_Com_Input)UIPackage.CreateObject("Login", "Com_Input");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_input = (GTextInput)GetChildAt(2);
        }
    }
}