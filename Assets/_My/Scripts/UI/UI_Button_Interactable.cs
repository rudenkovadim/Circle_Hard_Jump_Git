using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button_Interactable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public bool flag_select;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if(!flag_select)
			GetComponent<Image>().sprite = Game_Controller.Instance.sprite_b_enter;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!flag_select)
			GetComponent<Image>().sprite = Game_Controller.Instance.sprite_b_idle;
	}

	public void Select_LevelDifficultyButton()
	{
		flag_select = true;
		GetComponent<Image>().sprite = Game_Controller.Instance.sprite_b_select;
	}

	public void Idle_LevelDifficultyButton()
	{
		flag_select = false;
		GetComponent<Image>().sprite = Game_Controller.Instance.sprite_b_idle;
	}
}
