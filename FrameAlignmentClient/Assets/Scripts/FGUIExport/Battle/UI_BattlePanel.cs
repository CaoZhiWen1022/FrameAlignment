/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Battle
{
    public partial class UI_BattlePanel : GComponent
    {
        public UI_PlayerHpBar m_thisHpBar;
        public UI_PlayerHpBar m_enemyHpBar;
        public GComponent m_harmValueLayer;
        public Transition m_t0;
        public const string URL = "ui://0fi67j7ju4on1";

        public static UI_BattlePanel CreateInstance()
        {
            return (UI_BattlePanel)UIPackage.CreateObject("Battle", "BattlePanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_thisHpBar = (UI_PlayerHpBar)GetChildAt(0);
            m_enemyHpBar = (UI_PlayerHpBar)GetChildAt(1);
            m_harmValueLayer = (GComponent)GetChildAt(2);
            m_t0 = GetTransitionAt(0);
        }
    }
}