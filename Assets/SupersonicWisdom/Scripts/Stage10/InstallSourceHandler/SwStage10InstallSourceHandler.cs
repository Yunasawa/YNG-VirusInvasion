#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwStage10InstallSourceHandler : ISwStage10ConfigListener, ISwLocalConfigProvider, ISwTrackerDataProvider
    {
        #region --- Constants ---

        private const string UNKNOWN = "unknown";
        private const string TRACKER_SOURCE_DOWNLOAD_KEY = "sourceDownload";

        #endregion


        #region --- Members ---

        private readonly SwCoreNativeAdapter _nativeAdapter;
        private readonly SwUiToolkitManager _uiToolkitManager;

        private string _body;
        private string _title;
        private string _installSource = "";
        private bool _isEnabled;

        #endregion


        #region --- Properties ---

        public Tuple<EConfigListenerType, EConfigListenerType> ListenerType
        {
            get { return new Tuple<EConfigListenerType, EConfigListenerType>(EConfigListenerType.Construction, EConfigListenerType.EndOfGame); }
        }

        private SwVisualElementPayload[] Payload
        {
            get
            {
                var payload = new List<SwVisualElementPayload>();

#if UNITY_ANDROID
                if (!_body.SwIsNullOrEmpty())
                {
                    payload.Add(new SwVisualElementPayload
                    {
                        Name = SwUiToolkitWindow.TITLE_VE_NAME,
                        Text = _title,
                    });
                }
                
                if (!_title.SwIsNullOrEmpty())
                {
                    payload.Add(new SwVisualElementPayload
                    {
                        Name = SwUiToolkitWindow.BODY_ELEMENT_NAME,
                        Text = _body,
                    });
                }
                
                payload.Add(new SwVisualElementPayload
                {
                    Name = SwUiToolkitWindow.BUTTON_VE_NAME,
                    ButtonCallback = OnButtonClicked,
                });
#endif

                return payload.ToArray();
            }
        }

        #endregion


        #region --- Construction ---

        public SwStage10InstallSourceHandler(SwCoreNativeAdapter nativeAdapter, SwUiToolkitManager uiToolkitManager)
        {
#if UNITY_ANDROID
            _nativeAdapter = nativeAdapter;
            _uiToolkitManager = uiToolkitManager;
            _nativeAdapter.NativeSDKInitializedEvent += OnNativeInitialized;
#endif
        }

        #endregion


        #region --- Public Methods ---

        public void OnConfigResolved(ISwStage10InternalConfig internalConfig, ISwConfigManagerState state, ISwConfigAccessor swConfigAccessor, ESwConfigSource source)
        {
#if UNITY_ANDROID && !DEVELOPMENT_BUILD
            _isEnabled = swConfigAccessor.GetValue(SwStage10InstallSourceHandlerLocalConfig.INSTALL_SOURCE_POPUP_ENABLED_KEY, SwStage10InstallSourceHandlerLocalConfig.INSTALL_SOURCE_POPUP_ENABLED_VALUE);

            if (_isEnabled)
            {
                _title = swConfigAccessor.GetValue(SwStage10InstallSourceHandlerLocalConfig.INSTALL_SOURCE_POPUP_TITLE_KEY, SwStage10InstallSourceHandlerLocalConfig.INSTALL_SOURCE_POPUP_TITLE_VALUE);
                _body = swConfigAccessor.GetValue(SwStage10InstallSourceHandlerLocalConfig.INSTALL_SOURCE_POPUP_BODY_KEY, SwStage10InstallSourceHandlerLocalConfig.INSTALL_SOURCE_POPUP_BODY_VALUE);

                TryShowPopup();
            }
#endif
        }

        public SwLocalConfig GetLocalConfig()
        {
            return new SwStage10InstallSourceHandlerLocalConfig();
        }

        public (SwJsonDictionary dataDictionary, IEnumerable<string> keysToEncrypt) ConditionallyAddExtraDataToTrackEvent(SwCoreUserData coreUserData, string eventName = "")
        {
            var dataDictionary = new SwJsonDictionary();

#if UNITY_ANDROID
            dataDictionary.SwAddOrReplace(TRACKER_SOURCE_DOWNLOAD_KEY, _installSource);
#endif

            return (dataDictionary, KeysToEncrypt());

            IEnumerable<string> KeysToEncrypt()
            {
                yield break;
            }
        }

        #endregion


        #region --- Private Methods ---

        private void TryShowPopup()
        {
#if UNITY_ANDROID && !DEVELOPMENT_BUILD
            if (IsOfficialInstallSource(_installSource)) return;

            SwInfra.CoroutineService.StartCoroutine(_uiToolkitManager.OpenWindow(ESwUiToolkitType.InstallSourceBlocker, onOpen:OnWindowOpened, payloads: Payload));
#endif
        }

        private void OnWindowOpened()
        {
            SwUtils.Ui.LockUI();
        }

        private bool IsOfficialInstallSource(string installSource)
        {
            var isOfficial = true;
#if UNITY_ANDROID
            isOfficial = installSource is SwConstants.OFFICIAL_INSTALL_SOURCE or UNKNOWN;
#endif
            
            SwInfra.Logger.Log(EWisdomLogType.InstallSourceHandler, $"Is official = {isOfficial}");
            
            return isOfficial;
        }

        #endregion


        #region --- Event Handler ---

        private void OnNativeInitialized()
        {
#if UNITY_ANDROID
            _installSource = _nativeAdapter.GetAppInstallSource();
            SwInfra.Logger.Log(EWisdomLogType.InstallSourceHandler, $"Install source = {_installSource}");
#endif
        }

        private void OnButtonClicked()
        {
            SwInfra.Logger.Log(EWisdomLogType.InstallSourceHandler, "Clicked");
            
            SwUtils.Native.OpenStoreKit();
        }

        #endregion
    }
}

#endif