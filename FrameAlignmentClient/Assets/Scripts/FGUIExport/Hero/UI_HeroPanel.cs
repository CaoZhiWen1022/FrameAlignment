/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Hero
{
    public partial class UI_HeroPanel : GComponent
    {
        public Controller m_state;
        public GButton m_close;
        public GList m_heroList;
        public GGraph m_heroRoot;
        public GButton m_useBtn;
        public const string URL = "ui://5vfukh8qthaz0";

        public static UI_HeroPanel CreateInstance()
        {
            return (UI_HeroPanel)UIPackage.CreateObject("Hero", "HeroPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_state = GetControllerAt(0);
            m_close = (GButton)GetChildAt(1);
            m_heroList = (GList)GetChildAt(2);
            m_heroRoot = (GGraph)GetChildAt(3);
            m_useBtn = (GButton)GetChildAt(4);
        }
    }
}