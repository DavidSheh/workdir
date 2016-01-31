using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Tools;
using ExcelConfig;

namespace Assets.Scripts.UI.Windows
{
    partial class UIMapList
    {
        public class MapGridTableModel : TableItemModel<MapGridTableTemplate>
        {
            public MapGridTableModel(){}
            public override void InitModel()
            {
                //todo
				this.Item.Root.OnMouseClick((s,e)=>{
					if(OnClick==null) return;
					OnClick(this);
				});
            }

			public MapConfig Config{ private set; get; }
			public Action<MapGridTableModel> OnClick;

			private int _mapID =0;
			public int MapID{ 
				set
				{
					_mapID = value;
					var mapConfig = ExcelConfig.ExcelToJSONConfigManager.Current.GetConfigByID<ExcelConfig.MapConfig> (value);
					Template.lb_name.text = mapConfig.Name;
					Config = mapConfig;
				} 
				get{return _mapID;}
			}
		}

        public override void InitModel()
        {
            base.InitModel();
            //Write Code here
        }
        public override void OnShow()
        {
            base.OnShow();
        }
        public override void OnHide()
        {
            base.OnHide();
        }

		public override void OnUpdateUIData ()
		{
			base.OnUpdateUIData ();
			var maps = DataManagers.PlayerMapManager.Singleton.GetOpenedMaps ();

			MapGridTableManager.Count = maps.Count;
			int index = 0;
			foreach (var i in MapGridTableManager) {
				i.Model.MapID = maps [index];
				i.Model.OnClick = OnClickItem;
				index++;
			}
		}

		private void OnClickItem(MapGridTableModel model)
		{
			this.HideWindow ();
			App.GameAppliaction.Singleton.GoToExplore (model.Config.ID);

		}
    }
}