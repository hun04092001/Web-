namespace SV20T1080012.Web.Models
{
    public class Student
    {
        public string  StudentId { get; set; }
        public string StudentName { get; set;}

    }
    public class StudentDAL
    {
        public List<Student> List()
        {
            List<Student> students = new List<Student>();
            students.Add(new Student()
            {
                StudentId = "20T1080012",
                StudentName = " Nguyễn Văn Hùng"
            });
            students.Add(new Student()
            {
                StudentId = "20T1080013",
                StudentName = " Nguyễn Văn Hào"
            });
          
            return students;
        }
    }
}
