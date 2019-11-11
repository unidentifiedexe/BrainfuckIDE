using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonHelper.Attributes
{
    /// <summary>
    /// デシリアライズ時のコレクションの型を定義します。
    /// </summary>
    public class DefaultCollectionTypeAttribute : Attribute
    {
        private readonly Type _defaultType;
        public DefaultCollectionTypeAttribute(Type defaultType)
        {
            if (defaultType == null) return;
            if (!defaultType.IsAbstract && defaultType.IsClass)
                _defaultType = defaultType;
        }

        public bool CanCast(Type targetType)
        {
            return
                _defaultType != null &&
                targetType.IsAssignableFrom(_defaultType);
        }

        public Type TryUseType(Type targetType)
        {
            if (targetType is null)
                throw new ArgumentNullException(nameof(targetType));

            if (!targetType.IsInterface && !targetType.IsAbstract)
                return targetType;
            if (!CanCast(targetType)) return targetType;

            return _defaultType;
        }
    }
}
