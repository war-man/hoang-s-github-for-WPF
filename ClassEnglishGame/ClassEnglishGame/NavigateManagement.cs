using System;
using System.Collections.Generic;
using ClassEnglishGame.GameControl;

namespace ClassEnglishGame
{
    public static class NavigateManagement
    {
        private static Dictionary<string, Action> _navigateActions = new Dictionary<string, Action>();
        private static MainWindow _mainWindow;

        public static void InitNavigate(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public static void NavigateToPage(string keyName)
        {
            if (_navigateActions != null &&
                _navigateActions.ContainsKey(keyName) &&
                _navigateActions[keyName] != null)
            {
                _navigateActions[keyName]();
            }
        }

        public static void AddNavigateAction(string key, Action action)
        {
            if (_navigateActions == null)
                _navigateActions = new Dictionary<string, Action>();

            if (!_navigateActions.ContainsKey(key))
                _navigateActions.Add(key, action);
        }

        public static void RemoveNavigateAction(string key, Action action)
        {
            if (_navigateActions != null && _navigateActions.ContainsKey(key))
                _navigateActions.Remove(key);
        }

        public static void SetDefaultNavigate()
        {
            AddNavigateAction(Constant.GameConstant.Home, () => _mainWindow.SetMainControl(new Welcome()));
            AddNavigateAction(Constant.GameConstant.TalkInMinute, () => _mainWindow.SetMainControl(new TalkInMinute(), Constant.GameConstant.TalkInMinute));
            AddNavigateAction(Constant.GameConstant.Taboo, () => _mainWindow.SetMainControl(new Taboo(), Constant.GameConstant.Taboo));
            AddNavigateAction(Constant.GameConstant.Charades, () => _mainWindow.SetMainControl(new Charades(), Constant.GameConstant.Charades));
            AddNavigateAction(Constant.GameConstant.PictureDash, () => _mainWindow.SetMainControl(new PictureDash(), Constant.GameConstant.PictureDash));
            AddNavigateAction(Constant.GameConstant.WhoAmI, () => _mainWindow.SetMainControl(new WhoAmI(), Constant.GameConstant.WhoAmI));
            AddNavigateAction(Constant.GameConstant.Setting, () => _mainWindow.SetMainControl(new Setting(), Constant.GameConstant.Setting));
        }
    }
}