using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;

namespace Service.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;

    public GroupService(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public Group CreateGroup(Group group)
    {
        if (string.IsNullOrWhiteSpace(group.Name))
            throw new ArgumentException("Group name cannot be empty.", nameof(group));

        if (string.IsNullOrWhiteSpace(group.Teacher))
            throw new ArgumentException("Teacher name cannot be empty.", nameof(group));

        if (string.IsNullOrWhiteSpace(group.Room))
            throw new ArgumentException("Room cannot be empty.", nameof(group));

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
        return _groupRepository.GetGroupById(id);
    }

    public List<Group> GetAllGroupsByTeacher(string teacher)
    {
        if (string.IsNullOrWhiteSpace(teacher))
            throw new ArgumentException("Teacher name cannot be empty.", nameof(teacher));

        return _groupRepository.GetAllGroupsByTeacher(teacher);
    }

    public List<Group> GetAllGroupsByRoom(string room)
    {
        if (string.IsNullOrWhiteSpace(room))
            throw new ArgumentException("Room cannot be empty.", nameof(room));

        return _groupRepository.GetAllGroupsByRoom(room);
    }

    public List<Group> GetAllGroups()
    {
        return _groupRepository.GetAllGroups();
    }

    public List<Group> SearchGroupsByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return _groupRepository.GetAllGroups();

        return _groupRepository.SearchGroupsByName(name);
    }
}
