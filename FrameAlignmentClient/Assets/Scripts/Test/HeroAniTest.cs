using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeroAniTest : MonoBehaviour
{
    public Button idle;
    public Button run;
    public Button atk;

    public int testId = 1001;
    HeroCfg heroCfg;

    public BattlePlayerAniController ani;

    private void Start()
    {
        idle.onClick.AddListener(() =>
        {
            ani.SetAni(BattlePlayerAniName.idle);

        });
        run.onClick.AddListener(() =>
        {
            ani.SetAni(BattlePlayerAniName.run);

        });
        atk.onClick.AddListener(() =>
        {
            Atk();

        });

        //¥¥Ω®≤‚ ‘”¢–€
        heroCfg = ConfigMgr.heroCfg.GetHeroById(testId);
        Object prefab = Resources.Load("HeroModel/" + heroCfg.prefabName + "/" + heroCfg.prefabName);
        GameObject player = new GameObject();
        player.transform.position = Vector3.zero;
        player.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        GameObject hero = Instantiate(prefab) as GameObject;
        hero.transform.SetParent(player.transform);
        ani = hero.AddComponent<BattlePlayerAniController>();
    }

    public void Atk()
    {
        ani.SetAni(BattlePlayerAniName.atk);
    }

    public void ShootBullet()
    {
        //yield return new WaitForSeconds(0.5f);
        //Object bullet = Resources.Load("Effect/" + heroCfg.commonAtkPrefab);
        //obj

    }
}
