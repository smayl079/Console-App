using Domain.Entities;
using Domain.Interfaces;
using Service.Helpers;
using System.Linq;

namespace CourseApp;

public class MenuHandler
{
    private readonly IGroupService _groupService;
    private readonly IStudentService _studentService;

    public MenuHandler(IGroupService groupService, IStudentService studentService)
    {
        _groupService = groupService;
        _studentService = studentService;
    }

    public void PrintMenu()
    {
        Console.Clear();
        Helpers.DisplayMessage("=== Menyu ===", ConsoleColor.Yellow);
        Console.WriteLine("1 - Create Group");
        Console.WriteLine("2 - Update Group");
        Console.WriteLine("3 - Delete Group");
        Console.WriteLine("4 - Get Group by Id");
        Console.WriteLine("5 - Get All Groups by Teacher");
        Console.WriteLine("6 - Get All Groups by Room");
        Console.WriteLine("7 - Get All Groups");
        Console.WriteLine("8 - Create Student");
        Console.WriteLine("9 - Update Student");
        Console.WriteLine("10 - Get Student by Id");
        Console.WriteLine("11 - Delete Student");
        Console.WriteLine("12 - Get Students by Age");
        Console.WriteLine("13 - Get All Students by Group Id");
        Console.WriteLine("14 - Search Groups by Name");
        Console.WriteLine("15 - Search Students by Name or Surname");
        Console.WriteLine();
    }

    public void CreateGroup()
    {
        var name = Helpers.ReadInput("Qrup adı");
        var teacher = Helpers.ReadInput("Müəllim");
        var room = Helpers.ReadInput("Otaq");
        var created = _groupService.CreateGroup(new Group { Name = name, Teacher = teacher, Room = room });
        Helpers.DisplaySuccess($"Group yaradıldı. ID: {created.Id}");
    }

    public void UpdateGroup()
    {
        var id = Helpers.ReadIntInput("Qrup ID");
        var name = Helpers.ReadInput("Yeni qrup adı");
        var teacher = Helpers.ReadInput("Yeni müəllim");
        var room = Helpers.ReadInput("Yeni otaq");
        _groupService.UpdateGroup(new Group { Id = id, Name = name, Teacher = teacher, Room = room });
        Helpers.DisplaySuccess("Group yeniləndi.");
    }

    public void DeleteGroup()
    {
        var id = Helpers.ReadIntInput("Qrup ID");
        var ok = _groupService.DeleteGroup(id);
        if (ok) Helpers.DisplaySuccess("Group silindi.");
        else Helpers.DisplayError("Group tapılmadı.");
    }

    public void GetGroupById()
    {
        var id = Helpers.ReadIntInput("Qrup ID");
        var group = _groupService.GetGroupById(id);
        if (group == null)
        {
            Helpers.DisplayError("Group tapılmadı.");
            return;
        }
        Console.WriteLine($"\nID: {group.Id} | Name: {group.Name} | Teacher: {group.Teacher} | Room: {group.Room} | Students: {group.Students.Count}");
    }

    public void GetGroupsByTeacher()
    {
        var teacher = Helpers.ReadInput("Müəllim adı");
        var groups = _groupService.GetAllGroupsByTeacher(teacher);
        PrintGroups(groups);
    }

    public void GetGroupsByRoom()
    {
        var room = Helpers.ReadInput("Otaq");
        var groups = _groupService.GetAllGroupsByRoom(room);
        PrintGroups(groups);
    }

    public void GetAllGroups()
    {
        var groups = _groupService.GetAllGroups();
        PrintGroups(groups);
    }

    public void CreateStudent()
    {
        var name = Helpers.ReadInput("Tələbə adı");
        var surname = Helpers.ReadInput("Tələbə soyadı");
        var age = Helpers.ReadIntInput("Yaş");
        var groupId = Helpers.ReadIntInput("Qrup ID");

        var group = _groupService.GetGroupById(groupId) ?? throw new Exception("Qrup tapılmadı.");
        var created = _studentService.CreateStudent(new Student { Name = name, Surname = surname, Age = age, Group = group });
        Helpers.DisplaySuccess($"Student yaradıldı. ID: {created.Id}");
    }

    public void UpdateStudent()
    {
        var id = Helpers.ReadIntInput("Tələbə ID");
        var name = Helpers.ReadInput("Yeni ad");
        var surname = Helpers.ReadInput("Yeni soyad");
        var age = Helpers.ReadIntInput("Yeni yaş");
        var groupId = Helpers.ReadIntInput("Yeni qrup ID");

        var group = _groupService.GetGroupById(groupId) ?? throw new Exception("Qrup tapılmadı.");
        _studentService.UpdateStudent(new Student { Id = id, Name = name, Surname = surname, Age = age, Group = group });
        Helpers.DisplaySuccess("Student yeniləndi.");
    }

    public void GetStudentById()
    {
        var id = Helpers.ReadIntInput("Tələbə ID");
        var s = _studentService.GetStudentById(id);
        if (s == null) { Helpers.DisplayError("Student tapılmadı."); return; }
        Console.WriteLine($"\nID: {s.Id} | {s.Name} {s.Surname} | Age: {s.Age} | Group: {s.Group.Name} ({s.Group.Id})");
    }

    public void DeleteStudent()
    {
        var id = Helpers.ReadIntInput("Tələbə ID");
        var ok = _studentService.DeleteStudent(id);
        if (ok) Helpers.DisplaySuccess("Student silindi.");
        else Helpers.DisplayError("Student tapılmadı.");
    }

    public void GetStudentsByAge()
    {
        var age = Helpers.ReadIntInput("Yaş");
        var students = _studentService.GetAllStudents().Where(s => s.Age == age).ToList();
        PrintStudents(students);
    }

    public void GetStudentsByGroupId()
    {
        var groupId = Helpers.ReadIntInput("Qrup ID");
        var group = _groupService.GetGroupById(groupId);
        if (group == null) { Helpers.DisplayError("Qrup tapılmadı."); return; }
        var students = _studentService.GetStudentsByGroup(group);
        PrintStudents(students);
    }

    public void SearchGroupsByName()
    {
        var name = Helpers.ReadInput("Axtarış (qrup adı)");
        var groups = _groupService.SearchGroupsByName(name);
        PrintGroups(groups);
    }

    public void SearchStudents()
    {
        var query = Helpers.ReadInput("Axtarış (ad və ya soyad)");
        var students = _studentService.GetAllStudents()
            .Where(s => s.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        s.Surname.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
        PrintStudents(students);
    }

    private static void PrintGroups(IEnumerable<Group> groups)
    {
        Console.WriteLine("\n=== Qruplar ===");
        foreach (var g in groups)
        {
            Console.WriteLine($"ID: {g.Id} | {g.Name} | {g.Teacher} | {g.Room} | Students: {g.Students.Count}");
        }
    }

    private static void PrintStudents(IEnumerable<Student> students)
    {
        Console.WriteLine("\n=== Tələbələr ===");
        foreach (var s in students)
        {
            Console.WriteLine($"ID: {s.Id} | {s.Name} {s.Surname} | Age: {s.Age} | Group: {s.Group.Name} ({s.Group.Id})");
        }
    }
}

