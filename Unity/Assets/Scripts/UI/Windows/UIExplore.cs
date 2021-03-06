using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Tools;
using Assets.Scripts.GameStates;
using UnityEngine;
using Assets.Scripts.DataManagers;

namespace Assets.Scripts.UI.Windows
{
    partial class UIExplore
    {
		public class BagGridTableModel : TableItemModel<BagGridTableTemplate>
		{

			public BagGridTableModel() { }
			public override void InitModel()
			{
				//todo
			}
			private DataManagers.PlayerGameItem _GameItem;
			public DataManagers.PlayerGameItem GameItem
			{
				get
				{
					return _GameItem;
				}
				set
				{
					_GameItem = value;
					Template.lb_name.text = value.Config.Name;
					Template.lv_num.text = string.Format(string.Format(LanguageManager.Singleton["APP_NUM_FORMAT"], value.Num));
				}
			}

			public void SetDrag(bool canDrag)
			{
				var d = this.Item.Root.GetComponent<UIDragScrollView>();
				if (d == null)
					return;
				d.enabled = canDrag;
			}
		}

        public UIExplore()
        {
            NoCollider = true;
        }
        public override void InitModel()
        {
            base.InitModel();

            bt_close.OnMouseClick((s, e) => 
				{
                var state = App.GameAppliaction.Singleton.Current as ExploreState;
                if (state == null) return;
                UIMessageBox.ShowMessage(LanguageManager.Singleton["UIEXPLORE_EXIT_TITLE"],
                    LanguageManager.Singleton["UIEXPLORE_EXIT_MESSGAE"],
					LanguageManager.Singleton["UIEXPLORE_GOHOME"],
					LanguageManager.Singleton["UIEXPLORE_STILL_HRER"],
                    () => {
                        state.JoinCastle(true);
                    }, null);

            });

			bt_package.OnMouseClick ((s, e) => {
				ShowPackage();
			});

			bt_hide.OnMouseClick ((s, e) => {
				var tween = this.s_bagRoot.GetComponent<TweenPosition> ();
				tween.PlayReverse ();
			});
            //Write Code here
        }
        public override void OnShow()
        {
            base.OnShow();
			OnUpdateUIData ();
        }
        public override void OnHide()
        {
            base.OnHide();
        }

		public override void OnUpdateUIData ()
		{
			base.OnUpdateUIData ();
			var state = App.GameAppliaction.Singleton.Current as ExploreState;
			if (state == null) return;
			this.lb_title.text =state.Config.Name;
			int foodNum = PlayerItemManager.Singleton.GetFoodNum ();// GamePlayerManager.Singleton.FoodCount;
			int packageCur = PlayerItemManager.Singleton.CurrentSize;
			int packageSize = GamePlayerManager.Singleton.PackageSize;
			var totalExplore = state.TotalExploreValue;
			var currentExplore = DataManagers.PlayerMapManager.Singleton.GetMapExploreValue (state.Config.ID);

			lb_explorevalue.text = string.Format (LanguageManager.Singleton ["UI_EXPLORE_EXPLOREVALUE"],
				100f*((float)currentExplore / Mathf.Max (1, totalExplore)));

			lb_food.text = string.Format (LanguageManager.Singleton ["UI_EXPLORE_FOOD"], foodNum);
			lb_package.text = string.Format ("{0}/{1}", packageCur, packageSize);
			var cur = map.GetCurrent();
			lb_vector.text = string.Format (LanguageManager.Singleton ["UI_EXPLORE_VECTOR"], cur.x, cur.y);

			var all =DataManagers.PlayerItemManager.Singleton.PackageToList;
			BagGridTableManager.Count = all.Count;
			int index = 0;
			foreach (var i in BagGridTableManager) {
				i.Model.GameItem = all [index];
				i.Model.SetDrag (all.Count >= 10);
				index ++;
			}
		}
		private GameMap map;
		public void SetMap(GameMap map)
		{
			this.map = map;
		}

		public void ShowPackage()
		{
			var tween = this.s_bagRoot.GetComponent<TweenPosition> ();
			tween.PlayForward ();

		}
    }
}