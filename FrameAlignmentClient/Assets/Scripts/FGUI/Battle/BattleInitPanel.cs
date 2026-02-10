using Battle;


public class BattleInitPanel : FGUIFrame.UIPanel
{
    UI_BattleInitPanel ui { get { return m_ui as UI_BattleInitPanel; } }

    public override void Opened()
    {
        base.Opened();

        //发送准备完成
        BattleSocket.BattleReadyReq();
        GameEventMgr.Instance.OnWithParam<int>(GameEventType._战斗倒计时_参_, Downcount);
        GameEventMgr.Instance.On(GameEventType._战斗开始_, StartBattle);
    }

    private void Downcount(int time)
    {
        ui.m_txt.text = time.ToString();
    }

    private void StartBattle()
    {
        CloseThis();
    }

    public override void Closeed()
    {
        base.Closeed();
        GameEventMgr.Instance.OffWithParam<int>(GameEventType._战斗倒计时_参_, Downcount);
        GameEventMgr.Instance.Off(GameEventType._战斗开始_, StartBattle);


    }
}
