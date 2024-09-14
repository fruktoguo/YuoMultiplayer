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
		public const string NumTipPanel = "NumTipPanel";
	}
	public partial class View_NumTipPanelComponent : UIComponent 
	{

		public static View_NumTipPanelComponent GetView() => UIManagerComponent.Get.GetUIView<View_NumTipPanelComponent>();


		private View_NumTipComponent mChild_NumTip;

		public View_NumTipComponent Child_NumTip
		{
			get
			{
				if (mChild_NumTip == null)
				{
					mChild_NumTip = Entity.AddChild<View_NumTipComponent>();
					mChild_NumTip.Entity.EntityName = "NumTip";
					mChild_NumTip.rectTransform = rectTransform.Find("D_NumTip") as RectTransform;
					mChild_NumTip.RunSystem<IUICreate>();
				}
				return mChild_NumTip;
			}
		}


		[FoldoutGroup("ALL")]

		public List<View_NumTipComponent> all_View_NumTipComponent = new();

		public void FindAll()
		{
				
			all_View_NumTipComponent.Add(Child_NumTip);;


		}
	}}
