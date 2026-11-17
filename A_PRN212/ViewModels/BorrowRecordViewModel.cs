using System.Collections.ObjectModel;
using System.Windows.Input;
using A_PRN212.Application.Common;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Infrastructure.DependencyInjection;
using A_PRN212.MVVM;

namespace A_PRN212.ViewModels
{
    public class BorrowRecordViewModel : ViewModelBase
    {
        private readonly IBorrowService _borrowService;
        private readonly IDialogService _dialogService;
        private readonly int? _userId;

        private BorrowRecord? _selectedRecord;
        private string _latestBorrowDateText = string.Empty;
        private string _latestDueDateText = string.Empty;
        private string _latestStatusText = string.Empty;

        public ObservableCollection<BorrowRecord> BorrowRecords { get; } = new();

        public BorrowRecord? SelectedRecord
        {
            get => _selectedRecord;
            set => SetProperty(ref _selectedRecord, value);
        }

        public string LatestBorrowDateText { get => _latestBorrowDateText; set => SetProperty(ref _latestBorrowDateText, value); }
        public string LatestDueDateText { get => _latestDueDateText; set => SetProperty(ref _latestDueDateText, value); }
        public string LatestStatusText { get => _latestStatusText; set => SetProperty(ref _latestStatusText, value); }

        public ICommand MarkReturnedCommand { get; }
        public ICommand PayFineCommand { get; }
        public ICommand RefreshCommand { get; }

        public BorrowRecordViewModel(int? userId = null)
            : this(userId, ServiceProviderFactory.Instance.BorrowService, WpfDialogService.Instance)
        {
        }

        public BorrowRecordViewModel(int? userId, IBorrowService borrowService, IDialogService dialogService)
        {
            _userId = userId;
            _borrowService = borrowService;
            _dialogService = dialogService;

            MarkReturnedCommand = new RelayCommand(ExecuteMarkReturned);
            PayFineCommand = new RelayCommand(ExecutePayFine);
            RefreshCommand = new RelayCommand(LoadRecords);
        }

        public void LoadRecords()
        {
            try
            {
                var records = _userId.HasValue
                    ? _borrowService.GetBorrowRecordsByUser(_userId.Value)
                    : _borrowService.GetAllBorrowRecords();

                BorrowRecords.Clear();
                foreach (var record in records)
                {
                    BorrowRecords.Add(record);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error: {ex.Message}");
            }
        }

        public void LoadLatestForUser(int userId)
        {
            try
            {
                var record = _borrowService.GetLatestBorrowRecordByUser(userId);
                if (record != null)
                {
                    LatestBorrowDateText = record.BorrowDate.ToString("yyyy-MM-dd");
                    LatestDueDateText = record.DueDate.ToString("yyyy-MM-dd");
                    LatestStatusText = record.Status;
                }
                else
                {
                    _dialogService.ShowError("No borrow records found.");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error: {ex.Message}");
            }
        }

        public void SetDirectPreview(DateTime borrowDate, DateTime dueDate, string status)
        {
            LatestBorrowDateText = borrowDate.ToString("yyyy-MM-dd");
            LatestDueDateText = dueDate.ToString("yyyy-MM-dd");
            LatestStatusText = status;
        }

        private void ExecuteMarkReturned()
        {
            try
            {
                if (SelectedRecord == null)
                {
                    _dialogService.ShowError("Please select a record to update.");
                    return;
                }

                var result = _borrowService.ReturnBook(SelectedRecord.BorrowId);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowError(result.Message);
                    return;
                }

                _dialogService.ShowInfo(result.Message, "Success");
                LoadRecords();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error: {ex.Message}");
            }
        }

        private void ExecutePayFine()
        {
            try
            {
                if (SelectedRecord == null)
                {
                    _dialogService.ShowError("Please select a record to make the payment.");
                    return;
                }

                var result = _borrowService.PayFine(SelectedRecord.BorrowId);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowError(result.Message);
                    return;
                }

                _dialogService.ShowInfo(result.Message, "Success");
                LoadRecords();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error: {ex.Message}");
            }
        }
    }
}
