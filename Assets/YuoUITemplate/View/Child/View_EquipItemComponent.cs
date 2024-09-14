using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;

namespace YuoTools.UI
{

	public partial class View_EquipItemComponent : UIComponent 
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

		private Image mImage_Icon;

		public Image Image_Icon
		{
			get
			{
				if (mImage_Icon == null)
					mImage_Icon = rectTransform.Find("C_Icon").GetComponent<Image>();
				return mImage_Icon;
			}
		}


		private Image mImage_Frame;

		public Image Image_Frame
		{
			get
			{
				if (mImage_Frame == null)
					mImage_Frame = rectTransform.Find("C_Frame").GetComponent<Image>();
				return mImage_Frame;
			}
		}



		[FoldoutGroup("ALL")]

		public List<Image> all_Image = new();

		public void FindAll()
		{
				
			all_Image.Add(MainImage);
			all_Image.Add(Image_Icon);
			all_Image.Add(Image_Frame);;


		}
	}}
