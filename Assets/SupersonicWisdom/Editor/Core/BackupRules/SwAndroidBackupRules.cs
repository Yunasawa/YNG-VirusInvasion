using System.Xml.Linq;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwAndroidBackupRules
    {
        #region --- Constants ---

        private const string FullBackupContent = "fullBackupContent";
        private const string FullBackupContentValue = "@xml/sw_backup_rules";
        private const string FullBackupContentXmlSourcePath = "SupersonicWisdom/Editor/Core/sw_backup_rules.xml";
        private const string Replace = "replace";
        private const string ReplaceValue = "android:fullBackupContent";
        private const string Tools = "tools";

        #endregion


        #region --- Private Methods ---

        internal static bool ApplyAndroidBackupRules ()
        {
            var shouldSaveManifest = false;
            var manifest = SwAndroidManifestUtils.LoadManifest();

            if (manifest == null)
            {
                SwEditorLogger.LogWarning($"{SwAndroidManifestUtils.TARGET_MANIFEST_PATH} is missing.");

                return false;
            }

            var elemManifest = manifest.Element(SwAndroidManifestUtils.MANIFEST_ELEMENT);

            if (elemManifest == null)
            {
                SwEditorLogger.LogWarning($"{SwAndroidManifestUtils.TARGET_MANIFEST_PATH} is not valid, missing <manifest> element.");

                return false;
            }

            var elemApplication = elemManifest.Element(SwAndroidManifestUtils.APPLICATION_ELEMENT);

            if (elemApplication == null)
            {
                SwEditorLogger.LogWarning($"{SwAndroidManifestUtils.TARGET_MANIFEST_PATH} is not valid, missing <application> element");

                return false;
            }

            var toolsAttr = elemManifest.Attribute(SwAndroidManifestUtils.Namespace + Tools);

            if (toolsAttr == null)
            {
                elemManifest.SetAttributeValue(SwAndroidManifestUtils.Namespace + Tools, SwAndroidManifestUtils.TOOLS_PATH);
                shouldSaveManifest = true;
            }

            var replaceAttr = elemApplication.Attribute(SwAndroidManifestUtils.ToolsNamespace + Replace);

            if (replaceAttr == null)
            {
                elemApplication.SetAttributeValue(SwAndroidManifestUtils.ToolsNamespace + Replace, ReplaceValue);
                shouldSaveManifest = true;
            }

            var fullBackupContentAttr = elemApplication.Attribute(SwAndroidManifestUtils.AndroidNamespace + FullBackupContent);

            if (fullBackupContentAttr == null)
            {
                elemApplication.SetAttributeValue(SwAndroidManifestUtils.AndroidNamespace + FullBackupContent, FullBackupContentValue);
                shouldSaveManifest = true;
            }
            else if (!fullBackupContentAttr.Value.Equals(FullBackupContentValue))
            {
                SwEditorLogger.LogWarning($"the <application/> element in {SwAndroidManifestUtils.TARGET_MANIFEST_PATH} already has value for attribute android:fullBackupContent");
                SwEditorLogger.LogWarning($"Copy the the <exclude/> elements from {FullBackupContentXmlSourcePath} to {fullBackupContentAttr.Value}");

                return false;
            }

            if (shouldSaveManifest)
            {
                elemManifest.Save(SwAndroidManifestUtils.GetAndroidManifestPath());
            }

            return true;
        }

        #endregion
    }
}