using UnityEngine;

public class Obstacle_Controller : MonoBehaviour
{
    void Update()
    {
        if (Game_Controller.Instance.flag_start_level == Enum_ActiveLevel.active)
        {
            if (Game_Controller.Instance.player.transform.position.x - transform.position.x > 30f)
            {
                Destroy(gameObject);
            }
        }
    }
}
