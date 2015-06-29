using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using PentagoAgEngine;
using System.Diagnostics;
using System.Collections.Generic;

namespace WindowsPhonePentago
{
    public class AppSettings
    {
        // Our isolated storage settings
        IsolatedStorageSettings isolatedStore;

        // The isolated storage key names of our settings
        const string PlayerColorKeyName = "PlayerColor";
        const string AiStrengthKeyName = "AiStrength";
        const string ColorThemeKeyName = "ColorTheme";
        public string GameStyleKeyName = "GameStyle";

        // The default value of our settings
        const PieceColor PlayerColorSettingDefault = PieceColor.White;
        const int AiStrengthDefault = 0;
        const ColorTheme ColorThemeDefault = ColorTheme.Natural;
        const GameStyle GameStyleDefault = GameStyle.VsAi;

        public AppSettings()
        {
            try
            {
                // Get the settings for this application.
                isolatedStore = IsolatedStorageSettings.ApplicationSettings;

            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception while using IsolatedStorageSettings: " + e.ToString());
            }
        }

        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            try
            {
                // if new value is different, set the new value.
                if (isolatedStore[Key] != value)
                {
                    isolatedStore[Key] = value;
                    valueChanged = true;
                }
            }
            catch (KeyNotFoundException)
            {
                isolatedStore.Add(Key, value);
                valueChanged = true;
            }
            catch (ArgumentException)
            {
                isolatedStore.Add(Key, value);
                valueChanged = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception while using IsolatedStorageSettings: " + e.ToString());
            }

            return valueChanged;
        }


        public valueType GetValueOrDefault<valueType>(string Key, valueType defaultValue)
        {
            valueType value;

            try
            {
                value = (valueType)isolatedStore[Key];
            }
            catch (KeyNotFoundException)
            {
                value = defaultValue;
            }
            catch (ArgumentException)
            {
                value = defaultValue;
            }

            return value;
        }

        public void Save()
        {
            isolatedStore.Save();
        }

        public PieceColor PlayerColorSetting
        {
            get
            {
                return GetValueOrDefault<PieceColor>(PlayerColorKeyName, PlayerColorSettingDefault);
            }
            set
            {
                AddOrUpdateValue(PlayerColorKeyName, value);
                Save();
            }
        }

        public int AiStrengthSetting
        {
            get
            {
                return GetValueOrDefault<int>(AiStrengthKeyName, AiStrengthDefault);
            }
            set
            {
                AddOrUpdateValue(AiStrengthKeyName, value);
                Save();
            }
        }

        public ColorTheme ColorThemeSetting
        {
            get
            {
                return GetValueOrDefault<ColorTheme>(ColorThemeKeyName, ColorThemeDefault);
            }
            set
            {
                AddOrUpdateValue(ColorThemeKeyName, value);
                Save();
            }
        }

        public GameStyle GameStyleSetting
        {
            get
            {
                return GetValueOrDefault<GameStyle>(GameStyleKeyName, GameStyleDefault);
            }
            set
            {
                AddOrUpdateValue(GameStyleKeyName, value);
                Save();
            }
        }
    }
}
