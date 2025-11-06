using CommunityToolkit.Mvvm.ComponentModel;
using CRM.Application.Dtos.Project;
using CRM.UI.ViewModels.Base;

namespace CRM.UI.ViewModels.Admin.ProjectManagement
{
    public partial class ProjectItemViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isSelected;
        [ObservableProperty]
        private ProjectDto _project;

        public ProjectItemViewModel(ProjectDto project, int index)
        {
            Project = project;
            Index = index;
        }

        public int Index { get; }

        public int Id => Project.ProjectId;
        public string Code => Project.ProjectCode;
        public string Name => Project.ProjectName;
        public string Address => Project.ProjectAddress;
        public DateTime? StartDate => Project.ProjectStartDate?.ToDateTime(TimeOnly.MinValue);
        public DateTime? EndDate => Project.ProjectEndDate?.ToDateTime(TimeOnly.MinValue);
        public string Description => Project.ProjectDescription;
        public string Status => Project.ProjectStatus;
        public int? DaysRemaining => Project.DaysRemaining;
    }
}
