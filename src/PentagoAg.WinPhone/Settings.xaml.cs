using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace PentagoAg.WinPhone
{
    public partial class Settings : PhoneApplicationPage
    {
        private AppSettings settings = new AppSettings();
        public Settings()
        {
            InitializeComponent();

            gameStyleAiRadioButton.IsChecked = settings.GameStyleSetting == GameStyle.VsAi;
            gameStyleHumanRadioButton.IsChecked = settings.GameStyleSetting == GameStyle.VsHuman;

            playerBlackRadioButton.IsChecked = settings.PlayerColorSetting == PentagoAgEngine.PieceColor.Black;
            playerBlackRadioButton.IsEnabled = settings.GameStyleSetting == GameStyle.VsAi;
            playerWhiteRadioButton.IsChecked = settings.PlayerColorSetting == PentagoAgEngine.PieceColor.White;
            playerWhiteRadioButton.IsEnabled = settings.GameStyleSetting == GameStyle.VsAi;

            aiStrengthEasyRadioButton.IsChecked = settings.AiStrengthSetting == 0;
            aiStrengthEasyRadioButton.IsEnabled = settings.GameStyleSetting == GameStyle.VsAi;
            aiStrengthMediumRadioButton.IsChecked = settings.AiStrengthSetting == 1;
            aiStrengthMediumRadioButton.IsEnabled = settings.GameStyleSetting == GameStyle.VsAi;
            aiStrengthHardRadioButton.IsChecked = settings.AiStrengthSetting == 2;
            aiStrengthHardRadioButton.IsEnabled = settings.GameStyleSetting == GameStyle.VsAi;

            colorThemeNaturalRadioButton.IsChecked = settings.ColorThemeSetting == ColorTheme.Natural;
            colorThemeRedRadioButton.IsChecked = settings.ColorThemeSetting == ColorTheme.Red;

            gameStyleAiRadioButton.Click += gameStyleAiRadioButton_Click;
            gameStyleHumanRadioButton.Click += gameStyleHumanRadioButton_Click;
            playerBlackRadioButton.Click += playerBlackRadioButton_Click;
            playerWhiteRadioButton.Click += playerWhiteRadioButton_Click;
            aiStrengthEasyRadioButton.Click +=aiStrengthEasyRadioButton_Click;
            aiStrengthMediumRadioButton.Click += aiStrengthMediumRadioButton_Click;
            aiStrengthHardRadioButton.Click += aiStrengthHardRadioButton_Click;
            colorThemeNaturalRadioButton.Click += colorThemeNaturalRadioButton_Click;
            colorThemeRedRadioButton.Click += colorThemeRedRadioButton_Click;
        }

        void gameStyleHumanRadioButton_Click(object sender, RoutedEventArgs e)
        {
            settings.GameStyleSetting = GameStyle.VsHuman;

            aiStrengthEasyRadioButton.IsEnabled = false;
            aiStrengthMediumRadioButton.IsEnabled = false;
            aiStrengthHardRadioButton.IsEnabled = false;

            playerWhiteRadioButton.IsEnabled = false;
            playerBlackRadioButton.IsEnabled = false;
        }

        void gameStyleAiRadioButton_Click(object sender, RoutedEventArgs e)
        {
            settings.GameStyleSetting = GameStyle.VsAi;

            aiStrengthEasyRadioButton.IsEnabled = true;
            aiStrengthMediumRadioButton.IsEnabled = true;
            aiStrengthHardRadioButton.IsEnabled = true;

            playerWhiteRadioButton.IsEnabled = true;
            playerBlackRadioButton.IsEnabled = true;
        }

        void colorThemeRedRadioButton_Click(object sender, RoutedEventArgs e)
        {
            settings.ColorThemeSetting = ColorTheme.Red;
        }
        void colorThemeNaturalRadioButton_Click(object sender, RoutedEventArgs e)
        {
            settings.ColorThemeSetting = ColorTheme.Natural;
        }

        void aiStrengthHardRadioButton_Click(object sender, RoutedEventArgs e)
        {
            settings.AiStrengthSetting = 2;
        }

        void aiStrengthMediumRadioButton_Click(object sender, RoutedEventArgs e)
        {
            settings.AiStrengthSetting = 1;
        }

        void aiStrengthEasyRadioButton_Click(object sender, RoutedEventArgs e)
        {
            settings.AiStrengthSetting = 0;
        }

        void playerWhiteRadioButton_Click(object sender, RoutedEventArgs e)
        {
            settings.PlayerColorSetting = PentagoAgEngine.PieceColor.White;
        }

        void playerBlackRadioButton_Click(object sender, RoutedEventArgs e)
        {
            settings.PlayerColorSetting = PentagoAgEngine.PieceColor.Black;
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}