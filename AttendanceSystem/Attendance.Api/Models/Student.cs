namespace Attendance.Api.Models
{
    public class Student
    {
        public int userId { get; set; } // foreign key to User user.userId
        public int classNum { get; set; } // foreign key to Class class.classNumber

        //navigation properties to User and Class
        public User? User { get; set; }
        public Class? Class { get; set; }
    }
}
//we can join tables by mapping students or users to classes
//the primary key would be userId and classNum together, since a student can only be in one class but a class
// the controller can use the userId to find the classNum and then find the class information
//the services can use the userId to find the classNum and then find the class information