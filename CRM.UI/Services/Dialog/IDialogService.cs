using CRM.UI.ViewModels.Base;

namespace CRM.UI.Services.Dialog
{
    public interface IDialogService
    {
        Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation");
        Task ShowAlertAsync(string message, string title = "Alert");
        Task ShowErrorAsync(string message, string title = "Error");
        Task<TResult> ShowDialogAsync<TViewModel, TResult>(object parameter = null)
            where TViewModel : DialogViewModelBase<TResult>;
        Task<string> ShowInputAsync(string message, string title = "Input", string defaultValue = "");
        Task<string> ShowFileOpenDialogAsync(string filter = "All files (*.*)|*.*");
        Task<string> ShowFileSaveDialogAsync(string filter = "All files (*.*)|*.*", string defaultFileName = "");
    }
}
