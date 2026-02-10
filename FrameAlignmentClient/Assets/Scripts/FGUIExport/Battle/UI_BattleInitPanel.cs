/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Battle
{
    public partial class UI_BattleInitPanel : GComponent
    {
        public GTextField m_txt;
        public const string URL = "ui://0fi67j7j8z1o0";

        public static UI_BattleInitPanel CreateInstance()
        {
            return (UI_BattleInitPanel)UIPackage.CreateObject("Battle", "BattleInitPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_txt = (GTextField)GetChildAt(1);
        }
    }
}