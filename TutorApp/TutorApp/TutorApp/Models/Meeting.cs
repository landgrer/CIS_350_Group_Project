namespace TutorApp.Models
{
    public class Meeting
    {
        public string TutorProfileID { get; set; } = string.Empty;
        public string StudentProfileID { get; set; } = string.Empty;
        public string ID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Stars { get; set; } = -1;
    }
}
