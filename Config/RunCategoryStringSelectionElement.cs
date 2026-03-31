using System;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace SpeedrunTimer.Config
{
    public class RunCategoryStringSelectionElement : RangeElement
    {
        public string[] Options { get; set; }

        public override int NumberTicks => Options.Length;
        public override float TickIncrement => 1f / (Options.Length - 1);

        protected override float Proportion
        {
            get => GetIndex() / (float)(Options.Length - 1);
            set => SetValue((int)Math.Round(value * (Options.Length - 1)));
        }

        public override void OnBind()
        {
            base.OnBind();

            Options = SpeedrunTimer.AllCategories.Values.Select(c => Language.GetTextValue(c.LocalizationKey)).ToArray();
            TextDisplayFunction = () => MemberInfo.Name + ": " + GetValue();

            if (Label != null)
                TextDisplayFunction = () => Label + ": " + GetValue();
        }

        private void SetValue(int index)
        {
            if (!MemberInfo.CanWrite)
                return;

            MemberInfo.SetValue(Item, Options[index]);
            ConfigManager.SetPendingChanges();
        }

        private string GetValue()
        {
            return (string)MemberInfo.GetValue(Item);
        }

        private int GetIndex()
        {
            return Array.IndexOf(Options, GetValue());
        }
    }
}
