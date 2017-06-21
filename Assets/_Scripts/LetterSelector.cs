using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterSelector : MonoBehaviour
{
	public Text textBox;
	public Text upArrow;
	public Text downArrow;
	public GameObject lowercaseLetters;
	public GameObject uppercaseLetters;
	public AudioSource letterSelected;
	public AudioSource space;
	public AudioSource delete;
	public AudioSource caseSwap;

	private string letterCase = "upper";
	private float touchDownX = -1.0f;
	private string enteredText;
	private int clickDownSection;
	private float m_TimeStamp;
	private bool cursor = false;
	private string cursorChar = "";

	void Start () {
		upArrow.text = "\u25b2";
		downArrow.text = "\u25bc";
	}

	void Update ()
	{
		// Space/delete logic
		if (GvrController.TouchDown) {
			touchDownX = GvrController.TouchPos.x;
		}

		if (GvrController.TouchUp && touchDownX != -1.0f) {
			float touchUpX = GvrController.TouchPos.x;

			// Add a space
			if (touchUpX - touchDownX > 0.5f) {
				enteredText += " ";
				space.Play ();
			}

			// Delete last char
			if (textBox.text.Length > 0 && touchDownX - touchUpX > 0.5f) {
				enteredText = enteredText.Substring(0, enteredText.Length - 1);
				delete.Play ();
			}
		}

		// Letter selection logic
		if (GvrController.ClickButtonDown) {
			touchDownX = -1.0f;
			Vector2 touchPos = GvrController.TouchPos;
			clickDownSection = GetSection (touchPos.x, touchPos.y);
		}

		if (GvrController.ClickButtonUp) {
			Vector2 touchPos = GvrController.TouchPos;
			int clickUpSection = GetSection (touchPos.x, touchPos.y);
			enteredText += GetLetter (clickDownSection, clickUpSection);
			letterSelected.Play ();
		}

		// Cursor logic
		if (Time.time - m_TimeStamp >= 0.5) {
			m_TimeStamp = Time.time;

			if (cursor == false) {
				cursor = true;
				cursorChar += "_";
			} else {
				cursor = false;

				if (cursorChar.Length != 0) {
					cursorChar = cursorChar.Substring(0, cursorChar.Length - 1);
				}
			}
		}

		textBox.text = enteredText + cursorChar;
	}

	int GetSection (float x, float y)
	{
		int xSection, ySection;

		if (x <= 0.3f) {
			xSection = 0;
		} else if (x <= 0.7f) {
			xSection = 1;
		} else {
			xSection = 2;
		}

		if (y <= 0.3f) {
			ySection = 0;
		} else if (y <= 0.7f) {
			ySection = 1;
		} else {
			ySection = 2;
		}

		return xSection + ySection * 3;
	}

	string GetLetter (int clickDownSection, int clickUpSection)
	{
		string l;

		switch (clickDownSection) {
		case 0:
			l = "a";

			if (clickUpSection == 4 || clickUpSection == 8) {
				l = "v";
			}
			break;
		case 1:
			l = "n";

			if (clickUpSection == 4 || clickUpSection == 7) {
				l = "l";
			}

			if (clickUpSection == 3) {
				l = ".";
			}

			if (clickUpSection == 5) {
				l = ",";
			}
			break;
		case 2:
			l = "i";

			if (clickUpSection == 4 || clickUpSection == 6) {
				l = "x";
			}
			break;
		case 3:
			l = "h";

			if (clickUpSection == 4 || clickUpSection == 5) {
				l = "k";
			}

			if (clickUpSection == 1) {
				l = "'";
			}
			break;
		case 4:
			switch (clickUpSection) {
			case 0:
				l = "q";
				break;
			case 1:
				l = "u";
				break;
			case 2:
				l = "p";
				break;
			case 3:
				l = "c";
				break;
			case 4:
				l = "o";
				break;
			case 5:
				l = "b";
				break;
			case 6:
				l = "g";
				break;
			case 7:
				l = "d";
				break;
			case 8:
				l = "j";
				break;
			default:
				throw new System.Exception ("Unknown clickUpSection: " + clickUpSection);
			}
				
			break;
		case 5:
			l = "r";

			if (clickUpSection == 4 || clickUpSection == 3) {
				l = "m";
			}

			if (clickUpSection == 1) {
				l = "!";
			}

			if (clickUpSection == 7) {
				l = "?";
			}

			if (letterCase == "lower" && clickUpSection == 2) {
				l = "";
				uppercaseLetters.SetActive (true);
				lowercaseLetters.SetActive (false);
				downArrow.gameObject.SetActive (true);
				upArrow.gameObject.SetActive (false);
				letterCase = "upper";
				caseSwap.Play ();
			}

			if (letterCase == "upper" && clickUpSection == 8) {
				l = "";
				lowercaseLetters.SetActive (true);
				uppercaseLetters.SetActive (false);
				upArrow.gameObject.SetActive (true);
				downArrow.gameObject.SetActive (false);
				letterCase = "lower";
				caseSwap.Play ();
			}
			break;
		case 6:
			l = "t";

			if (clickUpSection == 4 || clickUpSection == 2) {
				l = "y";
			}
			break;
		case 7:
			l = "e";

			if (clickUpSection == 4 || clickUpSection == 1) {
				l = "w";
			}

			if (clickUpSection == 8) {
				l = "z";
			}
			break;
		case 8:
			l = "s";

			if (clickUpSection == 4 || clickUpSection == 0) {
				l = "f";
			}
			break;
		default:
			throw new System.Exception ("Unknown clickDownSection: " + clickDownSection);
		}

		if (char.IsLetter (l, 0) && letterCase == "upper") {
			l = l.ToUpper ();
		}

		return l;
	}
}
