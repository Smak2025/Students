using System;
using System.Collections.Generic;
using System.Text;

namespace Students
{
    public class Student
    {
        private static long maxId = 0L;
        public long Id
        {
            get => field;
            set
            {
                field = value;
                maxId = Math.Max(maxId, value);
            }
        }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public DateTime Birthday { get; set; } = DateTime.Today.AddYears(-18);
        public string Level {  get; set; } = "Бакалавр";
        public string Speciality { get; set; } = "Математика и компьютерные науки";

        public Student() { }
        public override string ToString()
        {
            return $"{Id}: {LastName} {FirstName}";
        }

        public void GenerateId()
        {
            Id = maxId + 1;
        }
    }
}
