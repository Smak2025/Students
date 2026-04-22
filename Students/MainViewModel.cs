using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Students
{
    public class MainViewModel : INotifyPropertyChanged
    {
        ObservableCollection<Student> students = new ObservableCollection<Student>();
        //[
        //    new Student(){Id = 1, LastName="Иванов", FirstName="Иван", Group="05-401"},
        //    new Student(){Id = 2, LastName="Петров", FirstName="Петр", Group="05-401"},
        //    new Student(){Id = 3, LastName="Морозова", FirstName="Ксения", Group="05-401"},
        //    new Student(){Id = 4, LastName="Полякова", FirstName="Ирина", Group="05-402"},
        //    new Student(){Id = 5, LastName="Хабибуллин", FirstName="Руслан", Group="05-402"},
        //];

        public ObservableCollection<Student> Students => students;

        private RelayCommand addButtonPress;
        private RelayCommand expelButtonPress;
        private RelayCommand saveButtonPress;

        private bool changed;

        public event PropertyChangedEventHandler? PropertyChanged;

        public RelayCommand AddButtonPress => addButtonPress;
        public RelayCommand ExpelButtonPress => expelButtonPress;
        public RelayCommand SaveButtonPress => saveButtonPress;


        private Student? selectedStudent;
        public Student? SelectedStudent
        {
            get => selectedStudent;
            set
            {
                selectedStudent = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            addButtonPress = new RelayCommand(AddNewStudent);
            expelButtonPress = new RelayCommand(ExpelStudent, (p) => { return SelectedStudent != null; });
            saveButtonPress = new RelayCommand(async (obj) => { await Save(obj); });
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
            stud.GenerateId();
            students.Add(stud);
            SelectedStudent = stud;
            changed = true;
        }

        private async void ExpelStudent(object? param)
        {
            if (SelectedStudent != null)
            {
                await DbHelper.DeleteStudentInfoAsync(SelectedStudent);
                students.Remove(SelectedStudent);
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
    }
}
