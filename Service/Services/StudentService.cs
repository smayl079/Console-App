using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;

namespace Service.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IGroupRepository _groupRepository;

    public StudentService(IStudentRepository studentRepository, IGroupRepository groupRepository)
    {
        _studentRepository = studentRepository;
        _groupRepository = groupRepository;
    }

    public Student CreateStudent(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.Name))
            throw new ArgumentException("Student name cannot be empty.", nameof(student));

        if (string.IsNullOrWhiteSpace(student.Surname))
            throw new ArgumentException("Student surname cannot be empty.", nameof(student));

        if (student.Age <= 0)
            throw new ArgumentException("Student age must be greater than zero.", nameof(student));

        if (student.Group == null)
            throw new ArgumentException("Student must have a group.", nameof(student));

        var group = _groupRepository.GetGroupById(student.Group.Id);
        if (group == null)
            throw new KeyNotFoundException($"Group with ID {student.Group.Id} not found.");

        student.Group = group;
        return _studentRepository.CreateStudent(student);
    }

    public Student UpdateStudent(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.Name))
            throw new ArgumentException("Student name cannot be empty.", nameof(student));

        if (string.IsNullOrWhiteSpace(student.Surname))
            throw new ArgumentException("Student surname cannot be empty.", nameof(student));

        if (student.Age <= 0)
            throw new ArgumentException("Student age must be greater than zero.", nameof(student));

        if (student.Group == null)
            throw new ArgumentException("Student must have a group.", nameof(student));

        var existingStudent = _studentRepository.GetStudentById(student.Id);
        if (existingStudent == null)
            throw new KeyNotFoundException($"Student with ID {student.Id} not found.");

        var group = _groupRepository.GetGroupById(student.Group.Id);
        if (group == null)
            throw new KeyNotFoundException($"Group with ID {student.Group.Id} not found.");

        student.Group = group;
        return _studentRepository.UpdateStudent(student);
    }

    public bool DeleteStudent(int id)
    {
        var student = _studentRepository.GetStudentById(id);
        if (student == null)
            return false;

        return _studentRepository.DeleteStudent(id);
    }

    public Student? GetStudentById(int id)
    {
        return _studentRepository.GetStudentById(id);
    }

    public List<Student> GetAllStudents()
    {
        return _studentRepository.GetAllStudents();
    }

    public List<Student> GetStudentsByGroup(Group group)
    {
        if (group == null)
            throw new ArgumentNullException(nameof(group));

        var existingGroup = _groupRepository.GetGroupById(group.Id);
        if (existingGroup == null)
            throw new KeyNotFoundException($"Group with ID {group.Id} not found.");

        return _studentRepository.GetStudentsByGroup(existingGroup);
    }
}
