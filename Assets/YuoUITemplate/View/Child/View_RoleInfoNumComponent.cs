using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;
using TMPro;

namespace YuoTools.UI
{

	public partial class View_RoleInfoNumComponent : UIComponent 
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

		private TextMeshProUGUI mTextMeshProUGUI_Num;

		public TextMeshProUGUI TextMeshProUGUI_Num
		{
			get
			{
				if (mTextMeshProUGUI_Num == null)
					mTextMeshProUGUI_Num = rectTransform.Find("C_Num").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_Num;
			}
		}



		[FoldoutGroup("ALL")]

		public List<Image> all_Image = new();

		[FoldoutGroup("ALL")]

		public List<TextMeshProUGUI> all_TextMeshProUGUI = new();

		public void FindAll()
		{
				
			all_Image.Add(MainImage);;
				
			all_TextMeshProUGUI.Add(TextMeshProUGUI_Num);;


		}
	}}
