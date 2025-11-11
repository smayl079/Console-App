using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces;

public interface IStudentService
{
    Student CreateStudent(Student student);
    Student UpdateStudent(Student student);
    bool DeleteStudent(int id);
    Student? GetStudentById(int id);
    List<Student> GetAllStudents();
    List<Student> GetStudentsByGroup(Group group);
}

