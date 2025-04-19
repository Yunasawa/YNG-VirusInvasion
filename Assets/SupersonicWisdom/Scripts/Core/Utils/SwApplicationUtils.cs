#if DEVELOPMENT_BUILD
using UnityEngine;
#endif

namespace SupersonicWisdomSDK
{
    public static class SwApplicationUtils
    {
        public static void QuitApplicationOnDevelopment()
        {
#if DEVELOPMENT_BUILD
                Application.Quit(1);
#endif
        }
    }
}