using CRM.Application.Dtos.Lead;
using CRM.UI.ViewModels.LeadManagement;
using System.Windows;

namespace CRM.UI.Views.LeadManagement
{
    /// <summary>
    /// Interaction logic for AddLeadDialog.xaml
    /// </summary>
    public partial class AddLeadDialog : Window
    {
        private AddLeadViewModel _viewModel;
        public LeadDto Result { get; private set; }

        public AddLeadDialog()
        {
            InitializeComponent();

            //Loaded += (s, e) => NameTextBox.Focus();

        }

        public AddLeadDialog(Window owner) : this()
        {
            Owner = owner;
        }

        public void InitializeFor(AddLeadViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            if (_viewModel != null)
            {
                _viewModel.LeadCreated -= OnLeadCreated;
                _viewModel.DialogCancelled -= OnDialogCancelled;
            }

            _viewModel = viewModel;
            DataContext = _viewModel;
            _viewModel.LeadCreated += OnLeadCreated;
            _viewModel.DialogCancelled += OnDialogCancelled;
        }

        public AddLeadDialog(Window owner, LeadDto leadToEdit) : this(owner)
        {
            Title = "Chỉnh sửa khách hàng tiềm năng";
            _viewModel.LoadLead(leadToEdit);
        }

        private void OnLeadCreated(LeadDto newLead)
        {
            Result = newLead;
            DialogResult = true;
            Close();
        }

        private void OnDialogCancelled()
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _viewModel.LeadCreated -= OnLeadCreated;
            _viewModel.DialogCancelled -= OnDialogCancelled;

            base.OnClosed(e);
        }

        public static LeadDto? ShowAddDialog(Window owner, AddLeadViewModel viewModel)
        {
            var dialog = new AddLeadDialog(owner);
            dialog.InitializeFor(viewModel);

            var result = dialog.ShowDialog();

            return result == true ? dialog.Result : null;
        }

        public static LeadDto? ShowEditDialog(Window owner, LeadDto leadToEdit)
        {
            var dialog = new AddLeadDialog(owner, leadToEdit);
            var result = dialog.ShowDialog();

            return result == true ? dialog.Result : null;
        }
    }
}
