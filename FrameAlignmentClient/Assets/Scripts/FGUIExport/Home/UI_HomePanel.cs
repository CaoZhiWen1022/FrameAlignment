/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Home
{
    public partial class UI_HomePanel : GComponent
    {
        public GTextField m_title;
        public UI_HeroItem m_heroRoot;
        public GButton m_battle;
        public const string URL = "ui://hmul0fbzcax80";

        public static UI_HomePanel CreateInstance()
        {
            return (UI_HomePanel)UIPackage.CreateObject("Home", "HomePanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_title = (GTextField)GetChildAt(1);
            m_heroRoot = (UI_HeroItem)GetChildAt(2);
            m_battle = (GButton)GetChildAt(3);
        }
    }
}