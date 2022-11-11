using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public float player_horizontal_speed;
    public float player_vertical_speed;

    private Rigidbody2D rb2d;
    
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        player_vertical_speed = 5f;
    }

    void Update()
    {
        Player_Action();
    }

    void Player_Action()
    {
        if (Game_Controller.Instance.flag_start_level == Enum_ActiveLevel.active)
        {
            transform.position = new Vector3(transform.position.x + player_horizontal_speed, transform.position.y, 0f);
            if (transform.position.y < 5f && Input.GetKeyDown(KeyCode.UpArrow))
                rb2d.velocity = new Vector2(0, player_vertical_speed);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        rb2d.velocity = Vector2.zero;
        rb2d.gravityScale = 0f;
        Game_Controller.Instance.GameOver();
    }
}
