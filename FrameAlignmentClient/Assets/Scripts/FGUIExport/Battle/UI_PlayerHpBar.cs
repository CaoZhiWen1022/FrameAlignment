/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Battle
{
    public partial class UI_PlayerHpBar : GProgressBar
    {
        public Controller m_c_state;
        public const string URL = "ui://0fi67j7ju4on2";

        public static UI_PlayerHpBar CreateInstance()
        {
            return (UI_PlayerHpBar)UIPackage.CreateObject("Battle", "PlayerHpBar");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_c_state = GetControllerAt(0);
        }
    }
}