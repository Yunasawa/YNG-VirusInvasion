#if SW_STAGE_STAGE10_OR_ABOVE
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwStage10ExternalParamsTabs
    {
        #region --- Constant --- 
        
        private const int SPACE_BETWEEN_ITEMS = 10;
        private const string LABEL_TEXT = "PARAMETERS";

        #endregion
        

        #region --- Members ---

        private readonly SwResourcesManagementExternalParamsData _model;

        private int _selectedTab;
        private Vector2 _scrollPosition;
        private SwWalletExternalParamsViewController _view;

        #endregion


        #region --- Properties ---

        protected virtual string[] TabNames
        {
            get
            {
                return new[]
                {
                    _view.Name,
                };
            }
        }

        #endregion


        #region --- Construction ---

        public SwStage10ExternalParamsTabs()
        {
            _model = SwResourceManagementEditorUtils.CreateOrLoad();
            CreateView();
            EditorUtility.SetDirty(_model);
        }

        #endregion


        #region --- Public Methods ---

        public void Draw()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            GUILayout.Label(LABEL_TEXT, EditorStyles.boldLabel);
            
            using (new SwGUIVerticalScope())
            {
                using (new SwGUIIgnoreChangeScope()) 
                {
                    _selectedTab = GUILayout.Toolbar(_selectedTab, TabNames, GUILayout.Height(30));
                }
                
                GUILayout.Space(SPACE_BETWEEN_ITEMS);

                if (_selectedTab <= 0)
                {
                    _view.Draw();
                }
                else
                {
                    DrawSelectedTab(_selectedTab);
                }
            }

            GUILayout.EndScrollView();
        }

        #endregion


        #region --- Private Methods ---

        protected virtual void DrawSelectedTab(int selectedTab) { }

        private void CreateView()
        {
            _view = new SwWalletExternalParamsViewController(_model.Wallet);
        }

        #endregion
    }
}
#endif