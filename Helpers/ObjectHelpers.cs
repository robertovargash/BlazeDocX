using Blazored.LocalStorage;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazeDocX.Helpers
{
    public static class ObjectHelpers
    {

        static readonly Dictionary<string, object> _localCache = new();

        static JsonSerializerOptions jsonOptions = new()
        {
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyProperties = true,
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        //static internal ISyncLocalStorageService CacheService = default!;

        public static string ToJson<T>(this T? obj, JsonSerializerOptions? json = null)
        {
            return JsonSerializer.Serialize(obj, json ?? jsonOptions);
        }

        public static T? FromJson<T>(this string? strJson, JsonSerializerOptions? json = null)
        {
            if (strJson is null) return default;
            return JsonSerializer.Deserialize<T>(strJson, json ?? jsonOptions);
        }

        public static string AsBase64Json<T>(this T? obj)
        {
            return JsonSerializer.Serialize(obj, jsonOptions);
        }

        public static T? FromBase64Json<T>(this string? strJson)
        {
            if (strJson is null) return default;
            return JsonSerializer.Deserialize<T>(strJson, jsonOptions);
        }

        public static string ToBase64(this string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        public static string FromBase64(this string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }

        public static void SetItemBase64<T>(this ISyncLocalStorageService localStorageService, string key, T value)
        {
            string
                base64Key = key.ToBase64(),
                base64JsonValue = value.ToJson().ToBase64();

            localStorageService.SetItemAsString(base64Key, base64JsonValue);
        }

        public static T? GetItemBase64<T>(this ISyncLocalStorageService localStorageService, string key)
        {
            var base64Key = key.ToBase64();
            var base64Result = localStorageService.GetItemAsString(base64Key)?.FromBase64();

            if (base64Result?.Trim() is { Length: > 0 } base64Json)
                return base64Json.FromJson<T>();

            return default;
        }

        //public static T? ReadCache<T>(string key)
        //{
        //    if (_localCache.TryGetValue(key, out var value) && value is T result)
        //        return result;
        //    else if (CacheService.GetItem<T>(key) is { } storedValue)
        //    {
        //        _localCache[key] = storedValue;
        //        return storedValue;
        //    }
        //    return default;
        //}

        //public static T? WriteCache<T>(string key, T? value)
        //{
        //    CacheService.SetItem(key, value);
        //    return value;
        //}

        //public static string? ReadCacheAsString(string key)
        //{
        //    if (_localCache.TryGetValue(key, out var value) && value is string result)
        //        return result;
        //    else if (CacheService.GetItemAsString(key) is { } storedValue)
        //    {
        //        _localCache[key] = storedValue;
        //        return storedValue;
        //    }

        //    return default;
        //}

        //public static string? WriteStringToCache(string key, string? value)
        //{
        //    CacheService.SetItemAsString(key, value);
        //    return value;
        //}


    }
}
