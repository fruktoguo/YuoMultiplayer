using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;

namespace YuoTools.UI
{
	public static partial class ViewType
	{
		public const string HoverMessage = "HoverMessage";
	}

	public partial class View_HoverMessageComponent : UIComponent 
	{

		public static View_HoverMessageComponent GetView() => UIManagerComponent.Get.GetUIView<View_HoverMessageComponent>();


		private CanvasGroup mainCanvasGroup;

		public CanvasGroup MainCanvasGroup
		{
			get
			{
				if (mainCanvasGroup == null)
					mainCanvasGroup = rectTransform.GetComponent<CanvasGroup>();
				return mainCanvasGroup;
			}
		}

		private ScrollRect mScrollRect_MessagePanel;

		public ScrollRect ScrollRect_MessagePanel
		{
			get
			{
				if (mScrollRect_MessagePanel == null)
					mScrollRect_MessagePanel = rectTransform.Find("C_MessagePanel").GetComponent<ScrollRect>();
				return mScrollRect_MessagePanel;
			}
		}


		private View_HoverMessageItemComponent mChild_HoverMessageItem;

		public View_HoverMessageItemComponent Child_HoverMessageItem
		{
			get
			{
				if (mChild_HoverMessageItem == null)
				{
					mChild_HoverMessageItem = Entity.AddChild<View_HoverMessageItemComponent>();
					mChild_HoverMessageItem.Entity.EntityName = "HoverMessageItem";
					mChild_HoverMessageItem.rectTransform = rectTransform.Find("C_MessagePanel/Viewport/Content/D_HoverMessageItem") as RectTransform;
					mChild_HoverMessageItem.RunSystem<IUICreate>();
				}
				return mChild_HoverMessageItem;
			}
		}


		[FoldoutGroup("ALL")]
		public List<CanvasGroup> all_CanvasGroup = new();

		[FoldoutGroup("ALL")]
		public List<ScrollRect> all_ScrollRect = new();

		[FoldoutGroup("ALL")]
		public List<View_HoverMessageItemComponent> all_View_HoverMessageItemComponent = new();

		public void FindAll()
		{
				
			all_CanvasGroup.Add(MainCanvasGroup);;
				
			all_ScrollRect.Add(ScrollRect_MessagePanel);;
				
			all_View_HoverMessageItemComponent.Add(Child_HoverMessageItem);;

		}
	}}
