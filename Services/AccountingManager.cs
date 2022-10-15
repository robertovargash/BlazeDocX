using BlazeDocX.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazeDocX.Services
{
    public class AccountingManager
    {
        private const string ACCOUNT_LOCAL_STORAGE = "accounting";

        //public Accounting accounting { get; set; } = new Accounting();
        protected readonly ILocalStorageService _localStorage;

        public AccountingManager(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SaveData(Accounting accounting)
        {
            //await localStorage.RemoveItemAsync(ACCOUNT_LOCAL_STORAGE);
            await _localStorage.SetItemAsync(ACCOUNT_LOCAL_STORAGE, accounting);
        }

        public async Task RemoveData(Accounting accounting)
        {
            await _localStorage.RemoveItemAsync(ACCOUNT_LOCAL_STORAGE);
            accounting = new Accounting();
        }
    }
}
