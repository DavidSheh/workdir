using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Tools;
using Assets.Scripts.DataManagers;
using ExcelConfig;
using UnityEngine;

namespace Assets.Scripts.UI.Windows
{
    public class ItemData
    {
        public PlayerSoldier Soldier { set; get; }

        public bool IsSelectd { set; get; }

        public MonsterConfig Monster { get; set; }
    }
    partial class UIGoToExplore
    {
        public class ItemGridTableModel : TableItemModel<ItemGridTableTemplate>
        {
            public ItemGridTableModel() { }
            public override void InitModel()
            {
                //todo
                startTable = new UITableManager<UITableItem>();
                startTable.InitFromGrid(this.Template.StartGrid);
                this.Item.Root.OnMouseClick((s, e) =>
                {
                    if (OnClick == null) return;
                    OnClick(this);
                });
            }
            public Action<ItemGridTableModel> OnClick;
            public ItemData _soldier;

            private void SetHero(ItemData hero)
            {
                IsSelected = hero.IsSelectd;
                var monster = ExcelToJSONConfigManager.Current.GetConfigByID<ExcelConfig.MonsterConfig>(hero.Soldier.SoldierID);
                Monster = monster;
                if (monster == null) return;
                var skillName = string.Empty;
                var skill = ExcelToJSONConfigManager.Current.GetConfigByID<SkillConfig>(monster.SkillID);
                if (skill != null)
                {
                    skillName = skill.Name;
                }

                Template.lb_attack.text = string.Format(LanguageManager.Singleton["UITavern_Attack"], monster.Damage);
                Template.lb_hp.text = string.Format(LanguageManager.Singleton["UITavern_hp"], monster.Hp);
                Template.lb_skill.text = string.Format(LanguageManager.Singleton["UITavern_skill"], skillName);
                Template.lb_name.text = monster.Name;
                DataManagers.PlayerArmyManager.Singleton.SetJob(Template.s_job, monster);
                DataManagers.PlayerArmyManager.Singleton.SetIcon(Template.icon, monster);
                startTable.Count = monster.Star;
            }



            public ItemData PlayerSoldier
            {
                get { return _soldier; }
                set
                {
                    _soldier = value;
                    SetHero(_soldier);
                }
            }

            private UITableManager<UITableItem> startTable;

            public MonsterConfig Monster { get; set; }

            private bool selected = false;

            public bool IsSelected
            {
                set
                {
                    Template.s_lock.ActiveSelfObject(value);
                    selected = value;
                }
                get { return selected; }
            }
        }

        public override void InitModel()
        {
            base.InitModel();
            bt_close.OnMouseClick((s, e) =>
            {
                HideWindow();
            });

            bt_go.OnMouseClick((s, e) =>
            {
                var team = new List<int>();
                foreach (var i in AllHeros)
                {
                    if (i.IsSelectd)
                    {
                        team.Add(i.Soldier.SoldierID);
                    }
                }
                if (team.Count == 0)
                {
                    UITipDrawer.Singleton.DrawNotify(LanguageManager.Singleton["UI_GOEXPLORE_NEED_ARMY"]);
                    return;
                }

                DataManagers.PlayerArmyManager.Singleton.SetTeam(team);

                App.GameAppliaction.Singleton.GoToExplore(DataManagers.GamePlayerManager.Singleton.CurrentMap);
            });

            bt_add.OnMouseClick((s, e) =>
            {
                if (DataManagers.GamePlayerManager.Singleton.FoodCount == DataManagers.GamePlayerManager.Singleton.PackageSize)
                {
                    UITipDrawer.Singleton.DrawNotify(LanguageManager.Singleton["UI_GOEXPLORE_NO_PLACE"]);
                    return;
                }
                var foodEntry = App.GameAppliaction.Singleton.ConstValues.FoodItemID;
                if (DataManagers.PlayerItemManager.Singleton.GetItemCount(foodEntry) < 1)
                {
                    var config = ExcelToJSONConfigManager.Current.GetConfigByID<ItemConfig>(foodEntry);
                    UITipDrawer.Singleton.DrawNotify(LanguageManager.Singleton["UI_GOEXPLORE_NO_FOOD"],config.Name);
                    return;
                }
                if (DataManagers.GamePlayerManager.Singleton.AddFood(1))
                {
                    ShowFood();
                }
            });

            bt_cal.OnMouseClick((s, e) =>
            {
                if (DataManagers.GamePlayerManager.Singleton.SubFood(1))
                {
                    ShowFood();
                }
            });

            to_xian.OnMouseClick((s, e) => { ClickCategory(Proto.HeroJob.Xian, to_xian); });
            to_fo.OnMouseClick((s, e) => { ClickCategory(Proto.HeroJob.Fo,to_fo); });
            to_yao.OnMouseClick((s, e) => { ClickCategory(Proto.HeroJob.Yao,to_yao); });
            to_ming.OnMouseClick((s, e) => { ClickCategory(Proto.HeroJob.Ming,to_ming); });

            //Write Code here
        }

        private void ClickCategory(Proto.HeroJob job,UIToggle cu)
        {
            currentJob = job;

            var list = new List<UIToggle>(){ to_fo,to_ming,to_xian, to_yao};
            foreach(var i in list)
            {
                i.transform.FindChild<UISprite>("on").ActiveSelfObject(false);
            }
            cu.transform.FindChild<UISprite>("on").ActiveSelfObject(true);
            ShowCurrent();
        }

        private Proto.HeroJob currentJob = Proto.HeroJob.Fo;

        private void ShowFood()
        {
            lb_packageSize.text = string.Format(LanguageManager.Singleton["UI_GOEXPLORE_PACKAGE"],
                DataManagers.GamePlayerManager.Singleton.FoodCount, DataManagers.GamePlayerManager.Singleton.PackageSize);
            this.lb_foodvalue.text = string.Format("{0}", DataManagers.GamePlayerManager.Singleton.FoodCount);
        }

        private void ShowArmyCount()
        {
            var selectCount = 0;
            foreach (var i in  AllHeros)
            {
                if (i.IsSelectd)
                    selectCount++;
            }
            lb_herolb.text = string.Format(LanguageManager.Singleton["UI_GOEXPLORE_TEAM_SIZE"],
                selectCount, DataManagers.GamePlayerManager.Singleton.TeamSize);
        }

        public override void OnShow()
        {
            base.OnShow();
            OnUpdateUIData();
            ClickCategory(Proto.HeroJob.Fo, to_fo );
        }


        private List<ItemData> AllHeros;
        public override void OnUpdateUIData()
        {
            base.OnUpdateUIData();
            var foodEntry = App.GameAppliaction.Singleton.ConstValues.FoodItemID;
            var config = ExcelToJSONConfigManager.Current.GetConfigByID<ItemConfig>(foodEntry);
            lb_food_name.text = config.Name;

            AllHeros = DataManagers.PlayerArmyManager.Singleton.GetAllSoldier()
                .Select(t => new ItemData { 
                    Soldier = t, 
                    IsSelectd = DataManagers.PlayerArmyManager.Singleton.IsTeam(t.SoldierID),
                    Monster = ExcelToJSONConfigManager.Current.GetConfigByID<MonsterConfig>(t.SoldierID)
                }).ToList();

        }

        private void ShowCurrent()
        {
            var current = AllHeros.Where(t => t.Monster.Type == (int)currentJob).ToList();
            ItemGridTableManager.Count = current.Count;
            int index = 0;
            foreach (var i in ItemGridTableManager)
            {
                i.Model.PlayerSoldier = current[index];
                i.Model.OnClick = OnClickItem;
                index++;
            }

            ShowFood();
            ShowArmyCount();

        }

        private void OnClickItem(ItemGridTableModel obj)
        {
            if (!obj.PlayerSoldier.Soldier.IsAlive)
            {
                var config = ExcelToJSONConfigManager.Current.GetConfigByID<ItemConfig>(App.GameAppliaction.Singleton.ConstValues.ReliveNeedItem);

                UIMessageBox.ShowMessage(LanguageManager.Singleton["UI_GOEXPLORE_Relive_OK"],
                    string.Format(LanguageManager.Singleton["UI_GOEXPLORE_Relive_Message"], config.Name, obj.Monster.Name),
                    () => {
                        if (DataManagers.PlayerArmyManager.Singleton.Relive(obj.PlayerSoldier.Soldier.SoldierID))
                        {
                            UIManager.Singleton.UpdateUIData();
                        }
                    },
                    null);

                return;
               //复活
            }

            if (obj.IsSelected)
            {
               obj.PlayerSoldier.IsSelectd  = false;
               obj.PlayerSoldier = obj.PlayerSoldier;//
            }
            else
            {

                var selectCount = 0;
                foreach (var i in  AllHeros)
                {
                    if (i.IsSelectd)
                        selectCount++;
                }
                if (selectCount >= DataManagers.GamePlayerManager.Singleton.TeamSize)
                {
                    //已经上限了
                    UITipDrawer.Singleton.DrawNotify(LanguageManager.Singleton["UI_GOEXPLORE_NO_TEAM_PLACE"]);
                    return;
                }

                obj.PlayerSoldier.IsSelectd = true;
                obj.PlayerSoldier = obj.PlayerSoldier;//
            }
            ShowArmyCount();

        }

        public override void OnHide()
        {
            base.OnHide();
        }
    }

   
}