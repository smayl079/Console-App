using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Service.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IStudentRepository _studentRepository;

    public GroupService(IGroupRepository groupRepository, IStudentRepository studentRepository)
    {
        _groupRepository = groupRepository;
        _studentRepository = studentRepository;
    }

    public Group CreateGroup(Group group)
    {
        if (string.IsNullOrWhiteSpace(group.Name))
            throw new ArgumentException("Group name cannot be empty.", nameof(group));

        if (string.IsNullOrWhiteSpace(group.Teacher))
            throw new ArgumentException("Teacher name cannot be empty.", nameof(group));

        if (string.IsNullOrWhiteSpace(group.Room))
            throw new ArgumentException("Room cannot be empty.", nameof(group));

        // Check if a group with the same name already exists
        var existingGroups = _groupRepository.GetAllGroups();
        if (existingGroups.Any(g => g.Name.Equals(group.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A group with the name '{group.Name}' already exists.");

        return _groupRepository.CreateGroup(group);
    }

    public Group UpdateGroup(Group group)
    {
        if (string.IsNullOrWhiteSpace(group.Name))
            throw new ArgumentException("Group name cannot be empty.", nameof(group));

        if (string.IsNullOrWhiteSpace(group.Teacher))
            throw new ArgumentException("Teacher name cannot be empty.", nameof(group));

        if (string.IsNullOrWhiteSpace(group.Room))
            throw new ArgumentException("Room cannot be empty.", nameof(group));

        var existingGroup = _groupRepository.GetGroupById(group.Id);
        if (existingGroup == null)
            throw new KeyNotFoundException($"Group with ID {group.Id} not found.");

        // Check if another group (with different ID) has the same name
        var existingGroups = _groupRepository.GetAllGroups();
        if (existingGroups.Any(g => g.Id != group.Id && g.Name.Equals(group.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A group with the name '{group.Name}' already exists.");

        return _groupRepository.UpdateGroup(group);
    }

    public bool DeleteGroup(int id)
    {
        var group = _groupRepository.GetGroupById(id);
        if (group == null)
            return false;

        return _groupRepository.DeleteGroup(id);
    }

    public Group? GetGroupById(int id)
    {
        var group = _groupRepository.GetGroupById(id);
        if (group != null)
        {
            // Load students for this group
            var students = _studentRepository.GetStudentsByGroup(group);
            group.Students.Clear();
            foreach (var student in students)
            {
                group.Students.Add(student);
            }
        }
        return group;
    }

    public List<Group> GetAllGroupsByTeacher(string teacher)
    {
        if (string.IsNullOrWhiteSpace(teacher))
            throw new ArgumentException("Teacher name cannot be empty.", nameof(teacher));

        var groups = _groupRepository.GetAllGroupsByTeacher(teacher);
        LoadStudentsForGroups(groups);
        return groups;
    }

    public List<Group> GetAllGroupsByRoom(string room)
    {
        if (string.IsNullOrWhiteSpace(room))
            throw new ArgumentException("Room cannot be empty.", nameof(room));

        var groups = _groupRepository.GetAllGroupsByRoom(room);
        LoadStudentsForGroups(groups);
        return groups;
    }

    public List<Group> GetAllGroups()
    {
        var groups = _groupRepository.GetAllGroups();
        LoadStudentsForGroups(groups);
        return groups;
    }

    public List<Group> SearchGroupsByName(string name)
    {
        List<Group> groups;
        if (string.IsNullOrWhiteSpace(name))
            groups = _groupRepository.GetAllGroups();
        else
            groups = _groupRepository.SearchGroupsByName(name);
        
        LoadStudentsForGroups(groups);
        return groups;
    }

    private void LoadStudentsForGroups(List<Group> groups)
    {
        var allStudents = _studentRepository.GetAllStudents();
        foreach (var group in groups)
        {
            group.Students.Clear();
            var groupStudents = allStudents.Where(s => s.Group.Id == group.Id).ToList();
            foreach (var student in groupStudents)
            {
                group.Students.Add(student);
            }
        }
    }
}
