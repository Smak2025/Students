using System;
using System.Collections.Generic;
using System.Text;

namespace Students
{
    public class Performance
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public int Points { get; set; }
        public Performance() { }
        public Performance(
            long studentId,
            string subject,
            int points)
        {
            StudentId = studentId;
            Subject = subject;
            Points = points;
        }
    }
}
