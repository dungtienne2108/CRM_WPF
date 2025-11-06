using CRM.UI.ViewModels.LeadManagement;
using System.Windows;

namespace CRM.UI.Views.LeadManagement
{
    /// <summary>
    /// Interaction logic for ConvertDialog.xaml
    /// </summary>
    public partial class ConvertDialog : Window
    {
        private ConvertStageViewModel _viewModel;

        public ConvertDialog()
        {
            InitializeComponent();
        }

        public ConvertDialog(Window owner) : this()
        {
            Owner = owner;
        }

        public async Task InitializeFor(ConvertStageViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            if (_viewModel != null)
            {
                _viewModel.DialogCancelled -= OnDialogCancelled;
            }

            _viewModel = viewModel;
            await _viewModel.LoadDataAsync();
            DataContext = _viewModel;

            _viewModel.DialogCancelled += OnDialogCancelled;
        }

        public static async Task<bool> ShowConvertDialog(Window owner, ConvertStageViewModel viewModel)
        {
            var dialog = new ConvertDialog(owner);
            await dialog.InitializeFor(viewModel);
            var res = dialog.ShowDialog();

            return res.HasValue && res.Value;
        }

        private void OnDialogCancelled()
        {
            DialogResult = false;
            Close();
        }
    }
}
