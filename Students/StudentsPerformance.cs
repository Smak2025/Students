using System;
using System.Collections.Generic;
using System.Text;

namespace Students
{
    public class StudentsPerformance
    {
        public long PerformanceId { get; set; }
        public long StudentId { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public int Points { get; set; }

        public string StudentName => $"{LastName} {FirstName}";
        public string PointsColor => Points switch
        {
            >= 86 => "Green",
            >= 71 => "Yellow",
            >= 56 => "Orange",
            _ => "Pink"
        };
    }
}
