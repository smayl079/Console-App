using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Repository.Implementations;

public class GroupRepository : IGroupRepository
{
    private static readonly List<Group> _groups = new List<Group>();
    private static int _nextId = 1;

    public Group CreateGroup(Group group)
    {
        group.Id = _nextId++;
        _groups.Add(group);
        return group;
    }

    public Group UpdateGroup(Group group)
    {
        var existingGroup = _groups.FirstOrDefault(g => g.Id == group.Id);
        if (existingGroup == null)
            throw new KeyNotFoundException($"Group with ID {group.Id} not found.");

        existingGroup.Name = group.Name;
        existingGroup.Teacher = group.Teacher;
        existingGroup.Room = group.Room;
        return existingGroup;
    }

    public bool DeleteGroup(int id)
    {
        var group = _groups.FirstOrDefault(g => g.Id == id);
        if (group == null)
            return false;

        _groups.Remove(group);
        return true;
    }

    public Group? GetGroupById(int id)
    {
        return _groups.FirstOrDefault(g => g.Id == id);
    }

    public List<Group> GetAllGroupsByTeacher(string teacher)
    {
        return _groups.Where(g => g.Teacher.Equals(teacher, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<Group> GetAllGroupsByRoom(string room)
    {
        return _groups.Where(g => g.Room.Equals(room, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<Group> GetAllGroups()
    {
        return _groups.ToList();
    }

    public List<Group> SearchGroupsByName(string name)
    {
        return _groups.Where(g => g.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}


