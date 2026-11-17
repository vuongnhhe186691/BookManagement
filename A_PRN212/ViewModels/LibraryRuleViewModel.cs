using System.Collections.ObjectModel;
using System.Windows.Input;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Infrastructure.DependencyInjection;
using A_PRN212.MVVM;

namespace A_PRN212.ViewModels
{
    public class LibraryRuleViewModel : ViewModelBase
    {
        private readonly ILibraryRuleService _ruleService;
        private readonly IDialogService _dialogService;

        private LibraryRule? _selectedRule;
        private string _ruleIdText = string.Empty;
        private string _ruleText = string.Empty;

        public ObservableCollection<LibraryRule> Rules { get; } = new();

        public LibraryRule? SelectedRule
        {
            get => _selectedRule;
            set
            {
                if (SetProperty(ref _selectedRule, value) && value != null)
                {
                    RuleIdText = value.RuleId.ToString();
                    RuleText = value.Rule ?? string.Empty;
                }
            }
        }

        public string RuleIdText { get => _ruleIdText; set => SetProperty(ref _ruleIdText, value); }
        public string RuleText { get => _ruleText; set => SetProperty(ref _ruleText, value); }

        public ICommand LoadRulesCommand { get; }

        public LibraryRuleViewModel()
            : this(ServiceProviderFactory.Instance.LibraryRuleService, WpfDialogService.Instance)
        {
        }

        public LibraryRuleViewModel(ILibraryRuleService ruleService, IDialogService dialogService)
        {
            _ruleService = ruleService;
            _dialogService = dialogService;

            LoadRulesCommand = new RelayCommand(LoadRules);
            LoadRules();
        }

        public void LoadRules()
        {
            try
            {
                Rules.Clear();
                foreach (var rule in _ruleService.GetAllRules())
                {
                    Rules.Add(rule);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error loading library rules: {ex.Message}");
            }
        }
    }
}
