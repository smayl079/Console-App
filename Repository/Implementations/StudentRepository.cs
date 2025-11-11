using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Repository.Implementations;

public class StudentRepository : IStudentRepository
{
    private static readonly List<Student> _students = new List<Student>();
    private static int _nextId = 1;

    public Student CreateStudent(Student student)
    {
        student.Id = _nextId++;
        _students.Add(student);
        // Add student to group's students collection
        if (student.Group != null && !student.Group.Students.Contains(student))
        {
            student.Group.Students.Add(student);
        }
        return student;
    }

    public Student UpdateStudent(Student student)
    {
        var existingStudent = _students.FirstOrDefault(s => s.Id == student.Id);
        if (existingStudent == null)
            throw new KeyNotFoundException($"Student with ID {student.Id} not found.");

        // Remove from old group if group changed
        if (existingStudent.Group != null && existingStudent.Group.Id != student.Group.Id)
        {
            existingStudent.Group.Students.Remove(existingStudent);
        }

        existingStudent.Name = student.Name;
        existingStudent.Surname = student.Surname;
        existingStudent.Age = student.Age;
        existingStudent.Group = student.Group;

        // Add to new group if group changed
        if (student.Group != null && !student.Group.Students.Contains(existingStudent))
        {
            student.Group.Students.Add(existingStudent);
        }

        return existingStudent;
    }

    public bool DeleteStudent(int id)
    {
        var student = _students.FirstOrDefault(s => s.Id == id);
        if (student == null)
            return false;

        // Remove student from group's students collection
        if (student.Group != null)
        {
            student.Group.Students.Remove(student);
        }

        _students.Remove(student);
        return true;
    }

    public Student? GetStudentById(int id)
    {
        return _students.FirstOrDefault(s => s.Id == id);
    }

    public List<Student> GetAllStudents()
    {
        return _students.ToList();
    }

    public List<Student> GetStudentsByGroup(Group group)
    {
        return _students.Where(s => s.Group.Id == group.Id).ToList();
    }
}


