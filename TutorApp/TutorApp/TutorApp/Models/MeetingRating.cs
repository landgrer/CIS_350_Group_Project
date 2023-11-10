namespace TutorApp.Models
{
    public class MeetingRating
    {
        public string TutorProfileID { get; set; } = string.Empty;
        public string StudentProfileID { get; set; } = string.Empty;
        public string ID { get; set; } = string.Empty;
        public int Stars { get; set; } = 0;
        public string Comment { get; set; } = string.Empty;
        public string TutorName { get; set; } = string.Empty;
    }
}
