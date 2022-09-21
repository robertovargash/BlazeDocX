using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BlazeDocX.Models
{
    public abstract record BaseModel : INotifyPropertyChanged
    {
        Dictionary<string, object?> _properties = new();

        public State State { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected T? Get<T>([CallerMemberName] string propName = default!)
        {
            if (_properties.TryGetValue(propName, out object? found) && found is T value)
            {
                return value;
            }
            return default;
        }

        protected void Set<T>(T value, [CallerMemberName] string propName = default!)
        {
            if (State == State.Deleted) return;
            var oldValue = Get<T>(propName);
            if (!(oldValue?.Equals(value) ?? false))
            {
                _properties[propName] = value;
                if (State < State.Added)
                    State = State == State.Detached ? State.Added : State.Modified;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }

        public T Update<T>(T newData, bool raiseEvents = false) where T : BaseModel
        {
            var baseModelType = typeof(BaseModel);
            var type = typeof(T);
            foreach (var prop in type.GetProperties())
            {
                var newValue = prop.GetValue(newData);
                if (_properties.TryGetValue(prop.Name, out object? found))
                {
                    if (baseModelType == prop.PropertyType && newValue is T { } _new && found is T { } foundModel)
                    {
                        foundModel.Update(_new, raiseEvents);
                        continue;
                    }
                }
                if (newValue != found)
                {
                    prop.SetValue(this, newValue);
                }
            }
            return (this as T)!;
        }

    }

    public static class ModelExtensions
    {

        static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        };

        public static bool IsLike<T, T1>(T objA, T1 objB) where T : class where T1 : class
        {
            if (objA?.Equals(objB) == true) return true;
            if (objA is null && objB is null) return true;

            var typeA = objA?.GetType() ?? typeof(T);
            var typeB = objB?.GetType() ?? typeof(T1);

            var _typeAProperties = typeB.GetProperties();
            var _properties = from propA in typeA.GetProperties()
                              let propB = _typeAProperties.FirstOrDefault(propOfNew => propOfNew.Name.Equals(propA.Name, StringComparison.InvariantCultureIgnoreCase))
                              where propB != null
                              select (propA, propB);

            return _properties.Any();
        }

        public static T? Update<T, TIn>(this T? dataA, TIn? dataB)
            where T : class
            where TIn : class
        {
            if (dataA == null) return null;
            if (dataB == null) return dataA;

            var typeA = dataA?.GetType() ?? typeof(T);
            var typeB = dataB?.GetType() ?? typeof(TIn);

            var _typeAProperties = typeB.GetProperties();
            var _properties = typeA.GetProperties()
                .Select(prop => (prop, _typeAProperties.FirstOrDefault(propOfNew => propOfNew.Name.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase))))
                .Where(kv => kv.prop.SetMethod == null || kv.Item2 != null)
                .ToList()!;

            foreach (var (typeAProp, typeBProp) in _properties)
            {
                if (typeBProp == null) continue;
                var aValue = typeAProp.GetValue(dataA);
                var bValue = typeBProp?.GetValue(dataB);

                Type aPropType = typeAProp.PropertyType;
                Type bPropType = typeBProp!.PropertyType;

                if (IsCollectionType(aPropType) && IsCollectionType(bPropType) && bValue != null && aValue != null)
                {
                    UpdateEnumerable(aValue, bValue, typeAProp.PropertyType, typeBProp?.PropertyType);
                }
                else if (IsClass(aPropType) && IsClass(bPropType) && aValue != null && bValue != null)
                {
                    aValue.Update(GetValue(bValue, aPropType, aPropType, updateMethod.MakeGenericMethod(new[] { aPropType, bPropType })));
                }
                else if (bValue != null && bValue != aValue && typeAProp.SetMethod != null)
                {
                    typeAProp.SetValue(dataA, bValue);
                }
            }
            return dataA;
        }

        static bool IsClass(Type type)
        {
            return type is { IsPrimitive: false, IsClass: true, IsValueType: false, IsEnum: false } && type != typeof(string) && type != typeof(object);
        }

        static MethodInfo updateMethod = typeof(ModelExtensions).GetMethod("Update")!;

        private static void UpdateEnumerable(dynamic collectionA, dynamic? collectionB, Type typeA, Type? typeB)
        {
            if (typeB == null) return;
            var collAEnumerator = collectionA.GetEnumerator() as IEnumerator;
            var collBEnumerator = collectionB?.GetEnumerator() as IEnumerator;
            var collectionTypeA = typeA.IsSZArray ? typeA.GetElementType() : typeA.GetGenericArguments().First();
            var collectionTypeB = typeB.IsSZArray ? typeB.GetElementType() : typeB.GetGenericArguments().First();
            int i = 0;

            MethodInfo updateItem = updateMethod.MakeGenericMethod(collectionTypeA!, collectionTypeB!)!;

            Action<dynamic, int> update = collectionA is IList list
                ? (dynamic bValue, int i) => list[i] = GetValue(bValue, typeA, typeB, updateItem)
                : collectionA is object[] array
                    ? (dynamic bValue, int i) => array.SetValue(GetValue(bValue, typeA, typeB, updateItem), i)
                    : throw new Exception("Unhandlable collection");

            int countA = Enumerable.Count(collectionA);
            int countB = Enumerable.Count(collectionB);
            var maxCount = Math.Max(countA, countB);

            if (collectionA is object[] _array)
                Array.Resize(ref _array, countB);

            do
            {
                object aValue;

                if (collAEnumerator?.MoveNext() == true)
                    aValue = collAEnumerator.Current;
                else
                    return;

                if (collBEnumerator?.MoveNext() == true)
                {
                    object bValue = collBEnumerator.Current;

                    if (aValue == bValue) return;

                    if (aValue == null)
                        update(bValue, i);
                    else
                        updateItem?.Invoke(null, new[] { aValue, bValue });
                }

                i++;

            } while (true);

        }

        private static object? GetValue(object obj, Type typeA, Type? typeB, MethodInfo updateItem)
        {
            if (!typeA.IsAssignableFrom(typeB))
                return updateItem?.Invoke(null, new[] { Activator.CreateInstance(typeA), obj });
            return obj;

        }

        private static bool IsCollectionType(Type dataType)
        {
            return dataType.IsArray == true ||
                dataType.IsGenericType &&
                    (dataType.GetInterface("IList") != null ||
                     dataType.GetInterface("ICollection") != null);
        }
    }


    public enum State
    {
        Detached,
        Unchanged,
        Added,
        Modified,
        Deleted
    }

}
