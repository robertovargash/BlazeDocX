using Blazored.LocalStorage;
using MudBlazor;

// ReSharper disable once CheckNamespace
namespace BlazeDocX.Services
{
    public class ThemeManager
    {

        private readonly ISyncLocalStorageService _localStorage;

        public ThemeManager(ISyncLocalStorageService localStorage)
        {
            _localStorage = localStorage;
            IsDark = localStorage.GetItem<bool>("isDark");

        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public MudTheme Theme = new()
        {
            Palette = new()
            {
                Primary = "#ff671b",
                TextPrimary = "#ff671b"
            },
            PaletteDark = new()
            {
                Primary = "#ff671b",
                TextPrimary = "#ff671b",
                Black = "#27272f",
                Background = "#32333d",
                BackgroundGrey = "#27272f",
                Surface = "#373740",
                DrawerBackground = "#27272f",
                DrawerText = "rgba(255,255,255, 0.50)",
                DrawerIcon = "rgba(255,255,255, 0.50)",
                AppbarBackground = "#27272f",
                AppbarText = "rgba(255,255,255, 0.70)",
                TextSecondary = "rgba(255,255,255, 0.50)",
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                Divider = "rgba(255,255,255, 0.12)",
                DividerLight = "rgba(255,255,255, 0.06)",
                TableLines = "rgba(255,255,255, 0.12)",
                LinesDefault = "rgba(255,255,255, 0.12)",
                LinesInputs = "rgba(255,255,255, 0.3)",
                TextDisabled = "rgba(255,255,255, 0.2)",
                Info = "#3299ff",
                Success = "#0bba83",
                Warning = "#ffa800",
                Error = "#f64e62",
                Dark = "#27272f"
            }
        };

        bool _isDarkMode = false;

        public bool IsDark
        {
            get => _isDarkMode;
            set => _localStorage.SetItem("isDark", _isDarkMode = value);
        }
    }
}
