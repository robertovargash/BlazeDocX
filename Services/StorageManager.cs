using BlazeDocX.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazeDocX.Services
{
    public class StorageManager
    {
        private const string ACCOUNT_LOCAL_STORAGE = "accounting";
        private const string CV_LOCAL_STORAGE = "cv";

        //public Accounting accounting { get; set; } = new Accounting();
        protected readonly ILocalStorageService _localStorage;

        public StorageManager(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SaveAccountingData(Accounting accounting)
        {
            //await localStorage.RemoveItemAsync(ACCOUNT_LOCAL_STORAGE);
            await _localStorage.SetItemAsync(ACCOUNT_LOCAL_STORAGE, accounting);
        }

        public async Task SaveCVData(CV cv)
        {
            //await localStorage.RemoveItemAsync(ACCOUNT_LOCAL_STORAGE);
            await _localStorage.SetItemAsync(CV_LOCAL_STORAGE, cv);
        }

        public async Task RemoveAccountingData(Accounting accounting)
        {
            await _localStorage.RemoveItemAsync(ACCOUNT_LOCAL_STORAGE);
            accounting = new Accounting();
        }

        public async Task RemoveCVData(CV cv)
        {
            await _localStorage.RemoveItemAsync(CV_LOCAL_STORAGE);
            cv = new CV();
        }
    }
}
