#if SW_STAGE_STAGE10_OR_ABOVE

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace SupersonicWisdomSDK
{
    internal class SwStage10Container : SwCoreContainer
    {
        #region --- Members ---
        
        protected internal readonly ISwAppUpdateManager AppUpdateManager;
        protected internal readonly SwBlockingApiHandler BlockingApiHandler;
        protected internal readonly SwStage10FacebookAdapter FacebookAdapter;
        protected internal readonly SwStage10GameAnalyticsAdapter GameAnalyticsAdapter;
        protected internal readonly SwStage10AppsFlyerAdapter AppsFlyerAdapter;
        protected internal readonly SwStage10Tracker Stage10Tracker;
        protected internal readonly SwFilesCacheManager FilesCacheManager;
        protected internal readonly SwStage10CvUpdater CvUpdater;
        protected internal readonly SwStage10RevenueCalculator RevenueCalculator;
        protected readonly List<ISwScriptLifecycleListener> MonoLifecycleListeners;
        protected internal readonly ISwTacSystem TacSystem;
        protected internal readonly SwCoreFpsMeasurementManager FpsMeasurementManager;
        protected internal readonly SwStage10ResourcesManager ResourceManager;
        private readonly SwProgressionStatusSender ProgressionStatusSender;
        private readonly SwStage10GameSessionManager _stage10GameSessionManager;
        private readonly SwAliveStatusSender AliveStatusSender;
        private readonly SwUserActiveDay UserActiveDay;

        #endregion


        #region --- Construction ---

        internal SwStage10Container(
            Dictionary<string, object> initParamsDictionary,
            SwStage10MonoBehaviour mono,
            SwFilesCacheManager filesCacheManager,
            ISwAsyncCatchableRunnable stageSpecificCustomInitRunnable,
            SwSettings settings,
            ISwReadyEventListener[] readyEventListeners,
            ISwUserStateListener[] userStateListeners,
            ISwLocalConfigProvider[] configProviders,
            ISwAdapter[] coreAdapters,
            SwStage10NativeAdapter wisdomNativeAdapter,
            SwStage10DeepLinkHandler deepLinkHandler,
            SwStage10DevTools devTools,
            SwCoreUserData coreUserData,
            SwStage10Tracker tracker,
            ISwConfigManager configManager,
            SwBlockingApiHandler blockingApiHandler,
            SwGameStateSystem gameStateSystem,
            SwStage10DataBridge dataBridge,
            SwUiToolkitManager uiToolkitManager,
            ISwAppUpdateManager appUpdateManager,
            SwStage10AppsFlyerAdapter appsFlyerAdapter,
            SwStage10FacebookAdapter facebookAdapter,
            SwStage10GameAnalyticsAdapter gameAnalyticsAdapter,
            SwTimerManager timerManager,
            SwStage10CvUpdater cvUpdater,
            SwStage10RevenueCalculator revenueCalculator,
            SwAliveStatusSender aliveStatusSender,
            SwProgressionStatusSender progressionStatusSender,
            SwStage10GameSessionManager gameSessionManager,
            SwCoreFpsMeasurementManager fpsMeasurementManager,
            SwUserActiveDay userActiveDay,
            SwStage10TacSystem tacSystem,
            SwStage10ResourcesManager resourceManager
            )
            : base(initParamsDictionary,
                mono,
                stageSpecificCustomInitRunnable,
                settings,
                readyEventListeners,
                userStateListeners,
                configProviders,
                coreAdapters,
                wisdomNativeAdapter,
                deepLinkHandler,
                devTools,
                coreUserData,
                tracker,
                configManager,
                timerManager,
                gameStateSystem,
                dataBridge,
                uiToolkitManager)
        {
            FilesCacheManager = filesCacheManager;
            BlockingApiHandler = blockingApiHandler;
            FacebookAdapter = facebookAdapter;
            GameAnalyticsAdapter = gameAnalyticsAdapter;
            AppsFlyerAdapter = appsFlyerAdapter;
            Stage10Tracker = tracker;
            CvUpdater = cvUpdater;
            RevenueCalculator = revenueCalculator;
            FacebookAdapter.OnFacebookInitCompleteEvent += OnFacebookInitComplete;
            AliveStatusSender = aliveStatusSender;
            ProgressionStatusSender = progressionStatusSender;
            _stage10GameSessionManager = gameSessionManager;
            FpsMeasurementManager = fpsMeasurementManager;
            UserActiveDay = userActiveDay;
            AppUpdateManager = appUpdateManager;
			TacSystem = tacSystem;
            ResourceManager = resourceManager;

            MonoLifecycleListeners = new List<ISwScriptLifecycleListener>
            {
                FacebookAdapter,
                TimerManager,
                UserActiveDay,
                _stage10GameSessionManager,
            };
        }


        #endregion


        #region --- Mono Override ---

        public override void OnApplicationPause(bool pauseStatus)
        {
            MonoLifecycleListeners.ForEach(e => e.OnApplicationPause(pauseStatus));
            SwInfra.Logger.Log(EWisdomLogType.Container, $"OnApplicationPause | {nameof(pauseStatus)}: {pauseStatus}");
        }

        public override void OnApplicationQuit()
        {
            MonoLifecycleListeners.ForEach(e => e.OnApplicationQuit());
            SwInfra.Logger.Log(EWisdomLogType.Container);
        }

        #endregion


        #region --- Public Methods ---

        [Preserve]
        public new static ISwContainer GetInstance(Dictionary<string, object> initParamsDictionary)
        {
            var resourcePath = $"{SwStageUtils.CurrentStageName}/{SwConstants.GAME_OBJECT_NAME}{SwStageUtils.CurrentStageName}";
            var mono = SwContainerUtils.InstantiateSupersonicWisdom<SwStage10MonoBehaviour>(resourcePath);
            var tacSystem = new SwStage10TacSystem();
            SwInfra.Initialize(mono, mono, tacSystem);
            
            var filesCacheManager = new SwFilesCacheManager();
            var settings = new SwSettingsFactory<SwSettings>().GetInstance();
            var wisdomNativeApi = SwStage10NativeApiFactory.GetInstance();
            var userActiveDay = new SwUserActiveDay();
            var userData = new SwStage10UserData(settings, wisdomNativeApi, userActiveDay);
            var timerManager = new SwTimerManager(mono);
            var revenueCalculator = new SwStage10RevenueCalculator();
            var nativeAdditionalDataAssistant = new SwNativeAdditionalDataProvider(wisdomNativeApi, userData);
            var testUserState = new SwStage10TestUserState();
            var wisdomNativeAdapter = new SwStage10NativeAdapter(wisdomNativeApi, settings, userData, testUserState, nativeAdditionalDataAssistant);
            var webRequestClient = new SwUnityWebRequestClient();
            var deepLinkHandler = new SwStage10DeepLinkHandler(settings, webRequestClient);
            var devTools = new SwStage10DevTools(filesCacheManager);
            var dataBridge = new SwStage10DataBridge(userData, wisdomNativeApi, settings, timerManager);
            var tracker = new SwStage10Tracker(wisdomNativeAdapter, userData, webRequestClient, timerManager, dataBridge);
            var gameSessionManager = new SwStage10GameSessionManager(mono, new List<ISwGameSessionListener> { userData, tracker });
            var fpsMeasurementManager = new SwCoreFpsMeasurementManager(mono, timerManager, tracker);
            var gameStateSystem = new SwGameStateSystem(userData);
            var appsFlyerEventDispatcher = mono.GetComponent<SwAppsFlyerEventDispatcher>();
            var appsFlyerAdapter = new SwStage10AppsFlyerAdapter(appsFlyerEventDispatcher, userData, settings, tracker, timerManager);
            tacSystem?.InjectDependencies(userData, timerManager, gameStateSystem, settings, userActiveDay, revenueCalculator);

            var facebookAdapter = new SwStage10FacebookAdapter();
            var gameAnalyticsAdapter = new SwStage10GameAnalyticsAdapter();
            var uiToolkitManager = new SwUiToolkitManager(mono, mono.UiToolkitWindows);
            var installSourceHandler = new SwStage10InstallSourceHandler(wisdomNativeAdapter, uiToolkitManager);
            var abInternalHandler = new SwAbInternalHandler(userData, tracker);
            var remoteConfigHandler = new SwStage10WisdomRemoteConfigHandler(settings, userData, tracker, wisdomNativeAdapter, deepLinkHandler);
            var dynamicConfigHandler = new SwDynamicConfigManager(new List<ISwDynamicConfigProvider>() { deepLinkHandler, settings, abInternalHandler, remoteConfigHandler});
            var configManager = new SwStage10ConfigManager(dynamicConfigHandler, tracker, wisdomNativeAdapter, remoteConfigHandler, abInternalHandler);
            var aliveStatusSender = new SwAliveStatusSender(fpsMeasurementManager, tracker, mono, settings.isTimeBased, gameStateSystem);
            var progressionStatusSender = new SwProgressionStatusSender(fpsMeasurementManager, tracker, timerManager, userData);
            var blockingApiHandler = new SwBlockingApiHandler(settings, gameStateSystem, null);
            var appUpdateManager = SwStage10AppUpdateManagerFactory.CreateAppUpdateManager(settings, tracker, wisdomNativeApi, uiToolkitManager);
            var resourceManager = new SwStage10ResourcesManager();
            
            var fetchRemoteConfigStep = new SwStage10FetchRemoteConfig(configManager);
            var cvUpdater = new SwStage10CvUpdater(mono, revenueCalculator, userData, tracker);
            var beforeReadyStep = new SwBeforeReadyTriggerStep();

            var stageSpecificCustomInitRunnable = new SwStage10AsyncFlow(mono.gameObject, 
                new[]
            {
                new SwAsyncFlowStep(fetchRemoteConfigStep, 0),
                new SwAsyncFlowStep(facebookAdapter, 0),
                new SwAsyncFlowStep(gameAnalyticsAdapter, 1),
                new SwAsyncFlowStep(appsFlyerAdapter, 1, true),
                new SwAsyncFlowStep(beforeReadyStep, 2),
            });

            ISwAdapter[] swAdapters = { appsFlyerAdapter, gameAnalyticsAdapter, facebookAdapter };
            // User data should be after config manager
            ISwReadyEventListener[] readyEventListeners = { configManager, appsFlyerAdapter, wisdomNativeAdapter, timerManager, gameSessionManager };
            ISwUserStateListener[] userStateListeners = { };
            ISwLocalConfigProvider[] configProviders = { stageSpecificCustomInitRunnable.TimeoutHandler, appUpdateManager, remoteConfigHandler, appsFlyerAdapter, fpsMeasurementManager, aliveStatusSender, gameSessionManager, tacSystem, installSourceHandler, gameAnalyticsAdapter };
            
            configManager.AddListeners(new List<ISwCoreConfigListener> { remoteConfigHandler, stageSpecificCustomInitRunnable.TimeoutHandler, fpsMeasurementManager, aliveStatusSender, appUpdateManager, tacSystem });
            configManager.AddListeners(new List<ISwStage10ConfigListener> { userData, appsFlyerAdapter, wisdomNativeAdapter, gameSessionManager, installSourceHandler, gameAnalyticsAdapter });
            gameStateSystem.AddGameStateListeners(new List<ISwGameStateSystemListener>() { configManager, aliveStatusSender });
            gameStateSystem.AddGameProgressionListeners(new ISwGameProgressionListener[] { gameAnalyticsAdapter, progressionStatusSender });
            deepLinkHandler.AddListeners(new List<ISwDeepLinkListener>() { devTools, testUserState, userData });
            wisdomNativeAdapter.AddListeners(new List<ISwSessionListener> { userData, timerManager });
            dataBridge.SetRevenueCalculator(revenueCalculator);

            var trackerDataProviders = new List<ISwTrackerDataProvider> { timerManager, userData, userActiveDay, gameSessionManager, installSourceHandler, configManager, resourceManager };
            nativeAdditionalDataAssistant.SetTrackerDataProviders(trackerDataProviders);
            tracker.AddListeners(trackerDataProviders);
            uiToolkitManager.AddListeners(new List<ISwUiToolkitWindowStateListener>{ stageSpecificCustomInitRunnable });
            
            return new SwStage10Container(initParamsDictionary, mono, filesCacheManager, stageSpecificCustomInitRunnable, settings, readyEventListeners, userStateListeners, configProviders, swAdapters, wisdomNativeAdapter, deepLinkHandler, devTools, userData, tracker, configManager, blockingApiHandler, gameStateSystem, dataBridge, uiToolkitManager, appUpdateManager, appsFlyerAdapter, facebookAdapter, gameAnalyticsAdapter, timerManager, cvUpdater, revenueCalculator, aliveStatusSender, progressionStatusSender, gameSessionManager, fpsMeasurementManager, userActiveDay, tacSystem, resourceManager);
        }

        public override ISwInitParams CreateInitParams()
        {
            return new SwStage10InitParams();
        }

        public override void OnAwake()
        {
            base.OnAwake();
            MonoLifecycleListeners.ForEach(e => e.OnAwake());
            SwInfra.Logger.Log(EWisdomLogType.Container);
        }

        public override void OnStart()
        {
            MonoLifecycleListeners.ForEach(e => e.OnStart());
            SwInfra.Logger.Log(EWisdomLogType.Container);
        }

        #endregion


        #region --- Private Methods ---

        protected override IEnumerator BeforeReady()
        {
            yield return base.BeforeReady();
            yield return BlockingApiHandler.PrepareForGameStarted();
        }

        internal SwUserState CopyOfUserState()
        {
            return CoreUserData.ImmutableUserState();
        }
        
        private void OnFacebookInitComplete()
        {
            SwInfra.CoroutineService.StartCoroutine(CvUpdater.TryUpdateFirstCvUpdate());
        }

        #endregion
    }
}
#endif