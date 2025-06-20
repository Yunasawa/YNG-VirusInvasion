using System.Collections;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    public interface ISwTacSystemListener
    {
        bool DidPerformAction { get; set; }
        IEnumerator TryPerformAction(Dictionary<string, object> metaData);
        ESwActionType ActionType { get; }
        bool IsUiActionType { get; }
    }
}