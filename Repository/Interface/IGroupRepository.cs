using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces;

public interface IGroupRepository
{
    Group CreateGroup(Group group);
    Group UpdateGroup(Group group);
    bool DeleteGroup(int id);
    Group? GetGroupById(int id);
    List<Group> GetAllGroupsByTeacher(string teacher);
    List<Group> GetAllGroupsByRoom(string room);
    List<Group> GetAllGroups();
    List<Group> SearchGroupsByName(string name);
}

