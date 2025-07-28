using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class LocalizationUtils
{
    public static async Task<string> GetLocalizedText(string tableName, string entryKey)
    {
        var handle = LocalizationSettings.StringDatabase.GetTableAsync(tableName);

        // Await using a TaskCompletionSource wrapper
        var table = await WrapHandle(handle);
        if (table == null)
        {
            Debug.LogWarning($"Localization table '{tableName}' not found.");
            return string.Empty;
        }

        var entry = table.GetEntry(entryKey);
        if (entry == null)
        {
            Debug.LogWarning($"Entry '{entryKey}' not found in table '{tableName}'.");
            return string.Empty;
        }

        return entry.GetLocalizedString();
    }

    private static Task<StringTable> WrapHandle(AsyncOperationHandle<StringTable> handle)
    {
        var tcs = new TaskCompletionSource<StringTable>();
        handle.Completed += completedHandle => tcs.SetResult(completedHandle.Result);
        return tcs.Task;
    }
}
