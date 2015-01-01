using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace KaiHelper.Activator
{
    class AutoPot
    {
        private readonly Menu _menu;
        public AutoPot(Menu menu)
        {
            _menu = menu.AddSubMenu(new Menu("Potion Manager", "PotionManager"));
            _menu.AddItem(new MenuItem("HPTrigger", "HP Trigger Percent").SetValue(new Slider(30)));
            _menu.AddItem(new MenuItem("HealthPotion", "Health Potion").SetValue(true));
            _menu.AddItem(new MenuItem("MPTrigger", "MP Trigger Percent").SetValue(new Slider(30)));
            _menu.AddItem(new MenuItem("ManaPotion", "Mana Potion").SetValue(true));
            var autoarrangeMenu = _menu.AddItem(new MenuItem("AutoArrange", "Auto Arrange").DontSave().SetValue(false));
            autoarrangeMenu.ValueChanged += AutoRangeValueChanged;
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        private void AutoRangeValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (!e.GetNewValue<bool>())
            {
                return;
            }
            _menu.Item("HPTrigger").SetValue(new Slider(FomularPercent((int)ObjectManager.Player.MaxHealth, 150), 1, 99));
            Console.WriteLine(ObjectManager.Player.MaxMana);
            if (ObjectManager.Player.MaxMana <= 0)
            {
                _menu.Item("HealthPotion").SetValue(false);
            }else
            _menu.Item("MPTrigger").SetValue(new Slider(FomularPercent((int)ObjectManager.Player.MaxMana, 100), 1, 99));
        }

        private int FomularPercent(int max,int cur)
        {
            return (int) (100 - ((cur*1.0)/ max) * 100);
        }
        void Game_OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead ||
                ObjectManager.Player.InFountain() ||
                ObjectManager.Player.HasBuff("Recall"))
            {
                return;
            }
            var hasItemCrystalFlask = Items.HasItem(2041);
            var buffItemCrystalFlask=false;
            if (_menu.Item("HealthPotion").GetValue<bool>())
            {
                var hasItemMiniRegenPotion = Items.HasItem(2010);
                var hasHealthPotion = Items.HasItem(2003);
                if (ObjectManager.Player.HealthPercentage() <= _menu.Item("HPTrigger").GetValue<Slider>().Value)
                {
                    if (hasItemCrystalFlask)
                    {
                        if (ObjectManager.Player.ManaPercentage() <= _menu.Item("MPTrigger").GetValue<Slider>().Value||!hasHealthPotion && !hasItemMiniRegenPotion)
                        {
                            UseItem(2041, "ItemCrystalFlask");
                            buffItemCrystalFlask = true;
                        }
                        else if (hasHealthPotion)
                        {
                            UseItem(2003, "Health Potion");
                        }
                        else
                        {
                            UseItem(2010, "ItemMiniRegenPotion");
                        }
                    }
                    else if (hasHealthPotion)
                    {
                        UseItem(2003, "Health Potion");
                    }
                    else if (hasItemMiniRegenPotion)
                    {
                        UseItem(2010, "ItemMiniRegenPotion");
                    }
                }
            }
            if (buffItemCrystalFlask)
                return;
            if (!_menu.Item("ManaPotion").GetValue<bool>())
            {
                return;
            }
            if (!(ObjectManager.Player.ManaPercentage() <= _menu.Item("MPTrigger").GetValue<Slider>().Value))
            {
                return;
            }
            var hasManaPotion = Items.HasItem(2004);
            if (hasManaPotion)
            {
                UseItem(2004, "Mana Potion");
            }
            else if (hasItemCrystalFlask)
            {
                UseItem(2041, "ItemCrystalFlask");
            }
        }

        private static void UseItem(int id,string displayName)
        {
            if(!ObjectManager.Player.HasBuff(displayName))
                Items.UseItem(id);
        }
    }
}
