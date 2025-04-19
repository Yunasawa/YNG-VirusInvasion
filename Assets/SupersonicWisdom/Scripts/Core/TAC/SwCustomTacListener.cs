using System;
using System.Collections;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwCustomTacListener : ISwTacSystemListener
    {
        #region --- Members ---

        private readonly Dictionary<string, Func<IEnumerator>> _callbacks = new();

        #endregion

        
        #region --- Properties ---

        public bool IsUiActionType
        {
            get { return false; }
        }

        public ESwActionType ActionType
        {
            get { return ESwActionType.Custom; }
        }

        public bool DidPerformAction { get; set; }

        #endregion

        
        #region --- Public Methods ---

        public IEnumerator TryPerformAction(Dictionary<string, object> metaData)
        {
            DidPerformAction = false;

            if (metaData == null)
            {
                yield break;
            }

            var id = metaData.SwSafelyGet(SwTacSystem.METADATA_ACTION_ID_KEY_NAME, string.Empty) as string;
            
            if (id.SwIsNullOrEmpty())
            {
                yield break;
            }

            var callback = _callbacks.SwSafelyGet(id, null);
            
            if (callback != null)
            {
                yield return callback.Invoke();
                DidPerformAction = true;
            }
        }

        public void AddOrUpdateAction(SwActionData action, Func<IEnumerator> callback)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _callbacks[action.Id] = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        #endregion
    }
}