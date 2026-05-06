using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace Students
{
    public class MainViewModel : INotifyPropertyChanged
    {
        ObservableCollection<Student> _students = new ObservableCollection<Student>();
        public ObservableCollection<Student> Students => _students;

        ObservableCollection<StudentsPerformance> _performances = new ObservableCollection<StudentsPerformance>();
        public ObservableCollection<StudentsPerformance> Performances => _performances;

        private RelayCommand _addButtonPress;
        private RelayCommand _expelButtonPress;
        private RelayCommand _saveButtonPress;

        private RelayCommand? _addPerformancePress;
        private RelayCommand? _deletePerformancePress;
        private RelayCommand? _savePerforamcePress;

        private bool changed;

        public event PropertyChangedEventHandler? PropertyChanged;

        public RelayCommand AddButtonPress => _addButtonPress;
        public RelayCommand ExpelButtonPress => _expelButtonPress;
        public RelayCommand SaveButtonPress => _saveButtonPress;

        public RelayCommand AddPerformancePress => _addPerformancePress ??=
            new RelayCommand(AddNewPerformance, _ => SelectedStudent != null);
        public RelayCommand DeletePerformancePress => _deletePerformancePress ??=
            new RelayCommand(_ => DeleteSelectedPerformanceAsync(), _ => SelectedPerformance != null);
        public RelayCommand SavePerformancePress => _savePerforamcePress ??=
            new RelayCommand(async _ => await SavePerformanceAsync());

        private Student? _selectedStudent;
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
                LoadPerformancesForSelectedStudentAsync();
            }
        }

        private string _newSubject = string.Empty;
        public string NewSubject
        {
            get => _newSubject;
            set { _newSubject = value; OnPropertyChanged(); }
        }

        private int _newPoints;
        public int NewPoints
        {
            get => _newPoints;
            set { _newPoints = value; OnPropertyChanged(); }
        }

        private StudentsPerformance? _selectedPerformance;
        public StudentsPerformance? SelectedPerformance
        {
            get => _selectedPerformance;
            set
            {
                _selectedPerformance = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public MainViewModel()
        {
            _addButtonPress = new RelayCommand(AddNewStudent);
            _expelButtonPress = new RelayCommand(ExpelStudent, (p) => { return SelectedStudent != null; });
            _saveButtonPress = new RelayCommand(async (obj) => { await Save(obj); });
        }

        private async void LoadPerformancesForSelectedStudentAsync()
        {
            Performances.Clear();
            if (SelectedStudent?.Id > 0)
            {
                var perfList = await DbHelper.LoadPerformanceForStudentAsync(SelectedStudent.Id);
                foreach (var perf in perfList)
                {
                    Performances.Add(perf);
                }
            }
        }

        public async Task LoadData()
        {
            var stds = await DbHelper.LoadStudentsAsync();
            Students.Clear();
            foreach (var student in stds)
            {
                Students.Add(student);
            }
        }

        private void AddNewStudent(object? param)
        {
            var stud = new Student();
            _students.Add(stud);
            SelectedStudent = stud;
            changed = true;
        }

        private async void ExpelStudent(object? param)
        {
            if (SelectedStudent != null)
            {
                await DbHelper.DeleteStudentInfoAsync(SelectedStudent);
                _students.Remove(SelectedStudent);
                changed = true;
            }
        }

        private async Task Save(object? param)
        {
            if (SelectedStudent != null)
                await DbHelper.SaveStudentAsync(SelectedStudent);
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SaveOnExit()
        {
            if (changed)
            {
                if (MessageBox.Show("Сохранить изменения в файле?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Save(null);
                }
            }
        }

        private void AddNewPerformance(object? param)
        {
            if (SelectedStudent == null 
                || string.IsNullOrWhiteSpace(NewSubject))
                return;

            // Добавляем в коллекцию для немедленного отображения
            var dto = new StudentsPerformance
            {
                StudentId = SelectedStudent.Id,
                LastName = SelectedStudent.LastName,
                FirstName = SelectedStudent.FirstName,
                Subject = NewSubject,
                Points = NewPoints
            };
            Performances.Add(dto);

            // Сбрасываем поля ввода
            NewSubject = string.Empty;
            NewPoints = 0;
        }

        private async Task SavePerformanceAsync()
        {
            if (SelectedStudent == null) return;

            foreach (var p in Performances)
            {
                if (p.PerformanceId == 0 
                        && !string.IsNullOrEmpty(p.Subject)
                    )
                {
                    var perf = new Performance(
                        p.StudentId, p.Subject, p.Points
                    );
                    await DbHelper.SavePerformanceAsync(perf);
                }
            }
        }

        private async void DeleteSelectedPerformanceAsync()
        {
            if (SelectedPerformance?.PerformanceId > 0)
            {
                await DbHelper.DeletePerformanceAsync(
                    SelectedPerformance.PerformanceId);
                Performances.Remove(SelectedPerformance);
            }
        }
    }
}
