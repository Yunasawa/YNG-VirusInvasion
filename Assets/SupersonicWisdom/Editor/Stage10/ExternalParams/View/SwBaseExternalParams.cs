#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal abstract class SwBaseExternalParams
    {
        #region --- Constants ---

        protected const string PARAMETER_KEY_LABEL_TEXT = PARAMETER_LABEL_TEXT + " Key";

        private const string PARAMETER_LABEL_TEXT = "Parameter";
        private const string NAME_LABEL_TEXT = PARAMETER_LABEL_TEXT + " Name";
        private const string PARAMETER_TYPE_LABEL_TEXT = PARAMETER_LABEL_TEXT + " Type";
        private const string DUPLICATE_KEY_ERROR_LABEL = "Duplicate Key";
        private const string DUPLICATE_NAME_ERROR_LABEL = "Duplicate Name";
        private const string EMPTY_KEY_ERROR_LABEL = "Empty Key";
        private const string EMPTY_NAME_ERROR_LABEL = "Empty Name";
        private const string NON_SNAKE_CASE_ERROR_LABEL = "Name format should be snake_case";
        private const string NON_ALPHA_NUMERIC_UNDERSCORE_ERROR_LABEL = "Key format should be alphanumeric and underscore";
        private const string INITIAL_VALUE_LABEL = "Initial Value";

        #endregion


        #region --- Members ---

        protected readonly SwExternalParamsData _data;
        private ESwExternalParamsParamError[] _errors;

        #endregion


        #region --- Properties ---

        private SwExternalParamsType Type
        {
            get { return _data.Type; }
            set { _data.Type = value; }
        }

        internal string Key
        {
            get { return _data.key; }
            set { _data.key = value; }
        }

        private object Value
        {
            get { return _data.InitialValue.Value; }
            set
            {
                _data.InitialValue.Value = value;
            }
        }

        internal string Name
        {
            get { return _data.name; }
            set { _data.name = value; }
        }
        
        internal bool IsEnabled
        {
            get { return _data.IsEnabled; }
        }

        #endregion


        #region --- Construction ---

        protected SwBaseExternalParams(SwExternalParamsData data)
        {
            _data = data;
        }

        #endregion


        #region --- Public Methods ---

        public virtual ESwExternalParamsParamError Validate()
        {
            if (Key.SwIsNullOrEmpty())
            {
                return ESwExternalParamsParamError.EmptyKey;
            }
            
            if (!Key.SwIsAlphaNumericUnderscores())
            {
                return ESwExternalParamsParamError.NonAlphanumericUnderscore;
            }

            if (Name.SwIsNullOrEmpty())
            {
                return ESwExternalParamsParamError.EmptyName;
            }

            if (!Name.SwIsSnakeCase())
            {
                return ESwExternalParamsParamError.NonSnakeCase;
            }

            return ESwExternalParamsParamError.None;
        }

        public void SetErrors(params ESwExternalParamsParamError[] errors)
        {
            _errors = errors;
        }

        public void ClearErrors()
        {
            _errors = null;
        }

        #endregion


        #region --- Private Methods ---

        public virtual void OnGui(bool canChangeKeyName, bool canChangeType)
        {
            DrawKey(canChangeKeyName);
            DrawType(canChangeType);
            DrawInitialValue();
            DrawName();
            DrawErrors();
        }

        protected abstract void DrawKey(bool canChangeKeyName);

        private void DrawName()
        {
            using (new SwGUIVerticalScope())
            {
                Name = EditorGUILayout.TextField(NAME_LABEL_TEXT, Name);
            }
        }

        private void DrawInitialValue()
        {
            using (new SwGUIVerticalScope())
            {
                Value = DrawInitialValueField(Type, Value);
            }
        }

        private void DrawType(bool canChangeType)
        {
            using (new SwGUIEnabledScope(canChangeType))
            {
                using (new SwGUIVerticalScope())
                {
                    var previousType = Type;
                    var newType = (SwExternalParamsType)EditorGUILayout.EnumPopup(PARAMETER_TYPE_LABEL_TEXT, previousType);

                    if (newType != previousType)
                    {
                        Type = newType;
                    }
                } 
            }
        }

        private object DrawInitialValueField(SwExternalParamsType type, object value)
        {
            try
            {
                switch (type)
                {
                    case SwExternalParamsType.String:
                        return EditorGUILayout.TextField(INITIAL_VALUE_LABEL, value as string);
                    case SwExternalParamsType.Bool:
                        return EditorGUILayout.Toggle(INITIAL_VALUE_LABEL, value != null && (bool)value);
                    case SwExternalParamsType.Int:
                        return EditorGUILayout.IntField(INITIAL_VALUE_LABEL, value != null ? (int)value : 0);
                    case SwExternalParamsType.Float:
                        return EditorGUILayout.FloatField(INITIAL_VALUE_LABEL, value != null ? (float)value : 0.0f);
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogException(e, EWisdomLogType.General, e.Message);
                return null;
            }
        }

        private void DrawErrors()
        {
            if (_errors.SwIsNullOrEmpty()) return;

            GUILayout.Space(5);

            using (new SwGUIVerticalScope())
            {
                GUILayout.Label("Errors:", EditorStyles.boldLabel);
            }

            foreach (var error in _errors)
            {
                using (new SwGUIVerticalScope())
                {
                    GUILayout.Label($"*  {Parse(error)}", EditorStyles.boldLabel);
                }
            }
        }

        private string Parse(ESwExternalParamsParamError error)
        {
            switch (error)
            {
                case ESwExternalParamsParamError.DuplicateKey:
                    return DUPLICATE_KEY_ERROR_LABEL;
                case ESwExternalParamsParamError.DuplicateName:
                    return DUPLICATE_NAME_ERROR_LABEL;
                case ESwExternalParamsParamError.EmptyKey:
                    return EMPTY_KEY_ERROR_LABEL;
                case ESwExternalParamsParamError.EmptyName:
                    return EMPTY_NAME_ERROR_LABEL;
                case ESwExternalParamsParamError.NonSnakeCase:
                    return NON_SNAKE_CASE_ERROR_LABEL;
                case ESwExternalParamsParamError.NonAlphanumericUnderscore:
                    return NON_ALPHA_NUMERIC_UNDERSCORE_ERROR_LABEL;
                case ESwExternalParamsParamError.None:
                default:
                    return string.Empty;
            }
        }

        #endregion
    }
}
#endif