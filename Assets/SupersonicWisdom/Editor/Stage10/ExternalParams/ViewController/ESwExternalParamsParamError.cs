#if SW_STAGE_STAGE10_OR_ABOVE
namespace SupersonicWisdomSDK.Editor
{
    internal enum ESwExternalParamsParamError
    {
        None,
        DuplicateKey,
        DuplicateName,
        EmptyKey,
        EmptyName,
        NonSnakeCase,
        NonAlphanumericUnderscore,
    }
}
#endif