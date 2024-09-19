using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;
using TMPro;

namespace YuoTools.UI
{

	public partial class View_RoomItemComponent : UIComponent 
	{

		private Image mainImage;

		public Image MainImage
		{
			get
			{
				if (mainImage == null)
					mainImage = rectTransform.GetComponent<Image>();
				return mainImage;
			}
		}

		private HorizontalLayoutGroup mainHorizontalLayoutGroup;

		public HorizontalLayoutGroup MainHorizontalLayoutGroup
		{
			get
			{
				if (mainHorizontalLayoutGroup == null)
					mainHorizontalLayoutGroup = rectTransform.GetComponent<HorizontalLayoutGroup>();
				return mainHorizontalLayoutGroup;
			}
		}

		private TextMeshProUGUI mTextMeshProUGUI_Name;

		public TextMeshProUGUI TextMeshProUGUI_Name
		{
			get
			{
				if (mTextMeshProUGUI_Name == null)
					mTextMeshProUGUI_Name = rectTransform.Find("C_Name").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_Name;
			}
		}


		private Toggle mToggle_UsePassword;

		public Toggle Toggle_UsePassword
		{
			get
			{
				if (mToggle_UsePassword == null)
					mToggle_UsePassword = rectTransform.Find("C_UsePassword").GetComponent<Toggle>();
				return mToggle_UsePassword;
			}
		}


		private TextMeshProUGUI mTextMeshProUGUI_Players;

		public TextMeshProUGUI TextMeshProUGUI_Players
		{
			get
			{
				if (mTextMeshProUGUI_Players == null)
					mTextMeshProUGUI_Players = rectTransform.Find("C_Players").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_Players;
			}
		}


		private TextMeshProUGUI mTextMeshProUGUI_Info;

		public TextMeshProUGUI TextMeshProUGUI_Info
		{
			get
			{
				if (mTextMeshProUGUI_Info == null)
					mTextMeshProUGUI_Info = rectTransform.Find("C_Info").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_Info;
			}
		}


		private Button mButton_Join;

		public Button Button_Join
		{
			get
			{
				if (mButton_Join == null)
					mButton_Join = rectTransform.Find("C_Join").GetComponent<Button>();
				return mButton_Join;
			}
		}



		[FoldoutGroup("ALL")]
		public List<Image> all_Image = new();

		[FoldoutGroup("ALL")]
		public List<HorizontalLayoutGroup> all_HorizontalLayoutGroup = new();

		[FoldoutGroup("ALL")]
		public List<TextMeshProUGUI> all_TextMeshProUGUI = new();

		[FoldoutGroup("ALL")]
		public List<Toggle> all_Toggle = new();

		[FoldoutGroup("ALL")]
		public List<Button> all_Button = new();

		public void FindAll()
		{
				
			all_Image.Add(MainImage);;
				
			all_HorizontalLayoutGroup.Add(MainHorizontalLayoutGroup);;
				
			all_TextMeshProUGUI.Add(TextMeshProUGUI_Name);
			all_TextMeshProUGUI.Add(TextMeshProUGUI_Players);
			all_TextMeshProUGUI.Add(TextMeshProUGUI_Info);;
				
			all_Toggle.Add(Toggle_UsePassword);;
				
			all_Button.Add(Button_Join);;

		}
	}}
