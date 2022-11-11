using UnityEngine;
using UnityEngine.UI;

public class Game_Controller : MonoBehaviour
{
    [Header("Граница уровня по высоте")]
    public GameObject borders_level;
    [Header("Префабы")]
    public GameObject prefab_player;
    public GameObject prefab_obstacle;
    [Header("Позиции появления объектов")]
    public Transform spawn_player;
    public Transform spawn_obstacles;
    [Header("Ограничение FPS")]
    public int target_frame_rate;
    [Header("Интерфейс")]
    public GameObject panel_start;
    public GameObject panel_gameover;
    [Space(2)]
    public Button b_ld_easy;
    public Button b_ld_middle;
    public Button b_ld_hard;
    [Space(5)]
    public GameObject b_start;
    [Space(5)]
    public Text text_go_l_time;
    public Text text_go_l_number;
    [Header("Спрайты")]
    public Sprite sprite_b_idle;
    public Sprite sprite_b_enter;
    public Sprite sprite_b_select;
    [Header("Флаг активности уровня")]
    public Enum_ActiveLevel flag_start_level; // флаг активности уровня
    [Header("Объект игрока")]
    public GameObject player;

    private int current_number; // количество текущих попыток
    private int all_number; // количество всех попыток
    private float time_duration_game; // продолжительность попытки
    private bool flag_up_jump_velocity; // флаг для увеличения вертикальной скорости
    private float difficulty_player_speed_easy;
    private float difficulty_player_speed_middle;
    private float difficulty_player_speed_hard;
    private float player_horizontal_speed;
    private int player_back_tile_x;
    private int player_length_path;

    private static Game_Controller _instance;
    public static Game_Controller Instance
    {
        get { return _instance; }
    }

    public void Awake()
    {
        _instance = this;

        target_frame_rate = 60;
        flag_start_level = Enum_ActiveLevel.no_active;
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target_frame_rate;
        Cursor.visible = true;

        difficulty_player_speed_easy = 0.04f;
        difficulty_player_speed_middle = 0.08f;
        difficulty_player_speed_hard = 0.12f;


        player_horizontal_speed = difficulty_player_speed_easy;
        time_duration_game = 0f;
        current_number = 0;

        if (PlayerPrefs.HasKey("all_number"))
            all_number = PlayerPrefs.GetInt("all_number");
        else
            all_number = 0;

        borders_level.SetActive(false);
        OpenClose_Interface_StartGame(true);
    }

	void Update()
	{
        if (flag_start_level == Enum_ActiveLevel.active)
        {
            spawn_obstacles.position = new Vector3(player.transform.position.x + 10f, 0f, 0f);
            Camera.main.transform.position = new Vector3(player.transform.position.x, transform.position.y, -10f);

            if (player_back_tile_x < (int)player.transform.position.x)
            {
                player_back_tile_x = (int)player.transform.position.x;
                player_length_path += 1;
            }

            if (player_length_path == 8)
            {
                Vector3 spawn_pos = new Vector3(spawn_obstacles.position.x, Random.Range(-3f, 3f), 0f);
                Instantiate(prefab_obstacle, spawn_pos, Quaternion.identity);

                player_length_path = 0;
            }

            time_duration_game += Time.deltaTime;

            if (!flag_up_jump_velocity)
            {
                flag_up_jump_velocity = true;
                InvokeRepeating("IncreaseVerticalSpeed", 15f, 15f);
            }
        }
        else if(flag_start_level == Enum_ActiveLevel.no_active && Input.GetKeyDown(KeyCode.UpArrow))
        {
            UI_StartLevel();
        }
    }

    // увеличиваем силу подъема шарика
    private void IncreaseVerticalSpeed()
    {
        if (player != null)
        {
            player.GetComponent<Player_Controller>().player_vertical_speed += 0.5f;
        }
    }

    // действия после проигрыша игрока
    public void GameOver()
    {
        current_number += 1;
        all_number += 1;

        flag_start_level = Enum_ActiveLevel.game_over;
        flag_up_jump_velocity = false;

        text_go_l_time.text = "Продолжительность попытки:\n" + ((int)time_duration_game).ToString() + " сек.";
        text_go_l_number.text = "Текущих попыток: " + current_number.ToString() + "\n\n" + "Всего попыток: " + all_number;

        time_duration_game = 0f;

        CancelInvoke();
        SaveProgress();

        Cursor.visible = true;
        panel_gameover.SetActive(true);
    }

    // удаляем неиспользуемые объекты
    private void DestroyGameObjects()
    {
        if (player != null)
            Destroy(player.gameObject);

        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
        foreach (var go in obstacles)
        {
            if (go != null)
            {
                Destroy(go.gameObject);
            }
        }
    }

    // сохраняем прогресс общего количества попыток
    private void SaveProgress()
    {
        PlayerPrefs.SetInt("all_number", all_number);
    }

    // управление стартовым окном
    private void OpenClose_Interface_StartGame(bool flag_interface)
    {
        Cursor.visible = flag_interface;
        panel_start.SetActive(flag_interface);

        panel_gameover.SetActive(false);
    }

    // сбрасываем визуал кнопок на "по умолчанию"
    public void DefaultLevelDifficultyButtons()
    {
        b_ld_easy.GetComponent<UI_Button_Interactable>().Idle_LevelDifficultyButton();
        b_ld_middle.GetComponent<UI_Button_Interactable>().Idle_LevelDifficultyButton();
        b_ld_hard.GetComponent<UI_Button_Interactable>().Idle_LevelDifficultyButton();
    }

    // метод выбора сложности
    public void UI_SelectLevelDifficulty(int level_difficulty)
    {
        current_number = 0;

        DefaultLevelDifficultyButtons();

        switch ((Enum_LevelDifficulty)level_difficulty)
        {
            case Enum_LevelDifficulty.easy_speed:
                {
                    player_horizontal_speed = difficulty_player_speed_easy;

                    b_ld_easy.GetComponent<UI_Button_Interactable>().Select_LevelDifficultyButton();
                }
                break;
            case Enum_LevelDifficulty.middle_speed:
                {
                    player_horizontal_speed = difficulty_player_speed_middle;

                    b_ld_middle.GetComponent<UI_Button_Interactable>().Select_LevelDifficultyButton();
                }
                break;
            case Enum_LevelDifficulty.hard_speed:
                {
                    player_horizontal_speed = difficulty_player_speed_hard;

                    b_ld_hard.GetComponent<UI_Button_Interactable>().Select_LevelDifficultyButton();
                }
                break;
        }
    }

    // метод подготовки интерфеса, игровых объектов и старт уровня
    public void UI_StartLevel()
    {
        DestroyGameObjects();
        OpenClose_Interface_StartGame(false);

        flag_start_level = Enum_ActiveLevel.active;
        borders_level.SetActive(true);

        player_back_tile_x = 0;
        player = Instantiate(prefab_player, spawn_player);
        player.GetComponent<Player_Controller>().player_horizontal_speed = player_horizontal_speed;
    }

    public void UI_EditLevelDifficulty()
    {
        current_number = 0;
        flag_start_level = Enum_ActiveLevel.no_active;

        borders_level.SetActive(false);

        DestroyGameObjects();
        OpenClose_Interface_StartGame(true);
    }
}
