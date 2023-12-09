namespace SV20T1080012.Web.Models
{
    public class PersonDAL
    {
        public List<Person> List()
        {
            List<Person> list = new List<Person>(); // tên biến sd camelCale vd : firstName

            list.Add(new Person()
            {
                PersonId = 1, 
                Name = "Nguyen Van Hung",
                LivePlace = "8/22/HamNghi",
                Email = "hun@gmail.com"
            });
            list.Add(new Person()
            {
                PersonId = 2,
                Name = "Nguyen Thi Hue",
                LivePlace = "8/12/HamNghi",
                Email = "hue@gmail.com"
            });
            list.Add(new Person()
            {
                PersonId = 3,
                Name = "Nguyen Van Hoa",
                LivePlace = "11/HamNghi",
                Email = "hoa@gmail.com"
            });

            return list;
        }
    }
}
