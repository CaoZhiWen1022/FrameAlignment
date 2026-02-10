/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Home
{
    public partial class UI_MatchPanel : GComponent
    {
        public GTextField m_player1;
        public GTextField m_player2;
        public GButton m_cancel;
        public const string URL = "ui://hmul0fbzthaz2";

        public static UI_MatchPanel CreateInstance()
        {
            return (UI_MatchPanel)UIPackage.CreateObject("Home", "MatchPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_player1 = (GTextField)GetChildAt(2);
            m_player2 = (GTextField)GetChildAt(3);
            m_cancel = (GButton)GetChildAt(5);
        }
    }
}