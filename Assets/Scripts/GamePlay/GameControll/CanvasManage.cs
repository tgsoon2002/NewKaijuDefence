using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasManage : MonoBehaviour
{
	
    public Slider HPSlider;
    public Slider MiniMapSlider;
    public Golem mainEnemy;
    public GameObject Goal;
    private float maxDistance;

    public int GolemHealth
    {
        set{ HPSlider.value = value; }
    }

    // Use this for initialization
    void Start()
    {
        HPSlider.maxValue = mainEnemy.Golem_MaxHealth;
        MiniMapSlider.maxValue = Mathf.Abs(Goal.transform.position.x - mainEnemy.transform.position.x);
    }
	
    // Update is called once per frame
    void Update()
    {
        if (!CombatSceneManager.instance.winOrLose)
        {
            MiniMapSlider.value = Mathf.Abs(Goal.transform.position.x - mainEnemy.transform.position.x);
            HPSlider.value = mainEnemy.Golem_CurrentHealth;
        }
        if (MiniMapSlider.value <= 0.1f)
        {
            CombatSceneManager.instance.GolemReachLeatPoint();
        }
    }

    public void SetupGolemInfo(Golem enemy)
    {
        Debug.Log("Setup value" + enemy.Golem_CurrentHealth);
        mainEnemy = enemy;
        HPSlider.maxValue = enemy.Golem_MaxHealth;
        HPSlider.value = enemy.Golem_CurrentHealth;
    }
}
