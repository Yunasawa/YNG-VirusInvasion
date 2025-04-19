#if SW_STAGE_STAGE10_OR_ABOVE && SUPERSONIC_WISDOM_PARTNER
using System;
using System.Collections;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    public static class SwStage10PartnerApiExtension
    {
        #region --- Members ---

        private static SwStage10Container _container;

        #endregion


        #region --- Properties ---

        private static SwStage10Container Container
        {
            get
            {
                if (_container == null || _container.IsDestroyed())
                {
                    _container = (SwStage10Container)SwApi.Container;
                }

                return _container;
            }
        }

        #endregion


        #region --- Public Methods ---

        [PublicAPI]
        public static void AddOrUpdateCustomAction(this SwApi self, SwCustomAction customAction, Func<IEnumerator> callback)
        {
            var actionData = SwCustomActionUtils.ConvertToSwActionData(customAction);
            
            self.RunWithContainerAndReadyOrThrow(() => Container.TacSystem.AddOrUpdateAction(actionData, callback), nameof(AddOrUpdateCustomAction));
        }

        #endregion
    }
}
#endif