using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Tools;
using ExcelConfig;

namespace Assets.Scripts.UI.Windows
{
	partial class UIAchievement
	{
		public class ItemGridTableModel : TableItemModel<ItemGridTableTemplate>
		{
			public ItemGridTableModel ()
			{
			}

			public override void InitModel ()
			{
				//todo
				this.Template.Bt_itemName.OnMouseClick((s,e)=>{
					if(Config ==null) return;
					UI.UIControllor.Singleton.ShowInfo(Config.Description,3f);
				});
			}

			public void SetDrag (bool canDrag)
			{
				var d = this.Template.Bt_itemName.GetComponent<UIDragScrollView> ();
				d.enabled = canDrag;
			}

			private AchievementConfig _config;

			public AchievementConfig Config {
				set {
					_config = value;
					Template.Bt_itemName.Text (value.Name);
					var data = DataManagers.AchievementManager.Singleton [Config.ID];
					if (data != null)
						Template.s_completed.ActiveSelfObject (data.IsCompleted);
					else
						Template.s_completed.ActiveSelfObject (false);
				}
				get{ return _config; }
			}
		}

		public override void InitModel ()
		{
			base.InitModel ();
			bt_return.OnMouseClick ((s, e) => {
				HideWindow ();
			});
			//Write Code here
		}

		public override void OnShow ()
		{
			base.OnShow ();
			OnUpdateUIData ();

		}

		public override void OnUpdateUIData ()
		{
			base.OnUpdateUIData ();
			var achieves = ExcelConfig.ExcelToJSONConfigManager.Current.GetConfigs<ExcelConfig.AchievementConfig> ();

			this.lb_point.text = string.Format (LanguageManager.Singleton ["UIAchievement_Label"], 
				DataManagers.GamePlayerManager.Singleton.AchievementPoint);

			ItemGridTableManager.Count = achieves.Length;
			int index = 0;
			foreach (var i in ItemGridTableManager) {
				i.Model.Config = achieves [index]; 
				i.Model.SetDrag (ItemGridTableManager.Count >= 15);
				index++;
			}
		}

		public override void OnHide ()
		{
			base.OnHide ();
		}
	}
}