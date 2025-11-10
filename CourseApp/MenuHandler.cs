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
        var name = Helpers.ReadInput("Qrup adi");
        var teacher = ReadLettersOnly("Müəllim");
        var room = Helpers.ReadInput("Otaq");
        var created = _groupService.CreateGroup(new Group
        {
            Name = name,
            Teacher = teacher,
            Room = room.Trim(),
            CreatedAt = DateTime.UtcNow
        });
        Helpers.DisplaySuccess($"Group yaradildi. ID: {created.Id}, Tarix: {created.CreatedAt:g}");
    }

    public void UpdateGroup()
    {
        var id = Helpers.ReadIntInput("Qrup ID");
        var existing = _groupService.GetGroupById(id);
        if (existing == null)
        {
            Helpers.DisplayError("Group tapılmadı.");
            return;
        }

        var name = ReadWithDefault("Yeni qrup adı", existing.Name);
        var teacher = ReadLettersOnlyOptional("Yeni müəllim", existing.Teacher);
        var room = ReadWithDefault("Yeni otaq", existing.Room);

        if (string.IsNullOrWhiteSpace(name)) name = existing.Name;
        if (string.IsNullOrWhiteSpace(teacher)) teacher = existing.Teacher;
        if (string.IsNullOrWhiteSpace(room)) room = existing.Room;

        _groupService.UpdateGroup(new Group { Id = id, Name = name, Teacher = teacher, Room = room, CreatedAt = existing.CreatedAt });
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
        Console.WriteLine($"\nID: {group.Id} | Name: {group.Name} | Teacher: {group.Teacher} | Room: {group.Room} | Created: {group.CreatedAt:g} | Students: {group.Students.Count}");
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
        var name = ReadLettersOnly("Tələbə adı");
        var surname = ReadLettersOnly("Tələbə soyadı");
        var age = Helpers.ReadIntInput("Yaş");
        var groupId = Helpers.ReadIntInput("Qrup ID");

        var group = _groupService.GetGroupById(groupId) ?? throw new Exception("Qrup tapılmadı.");
        var created = _studentService.CreateStudent(new Student { Name = name, Surname = surname, Age = age, Group = group });
        Helpers.DisplaySuccess($"Student yaradıldı. ID: {created.Id}");
    }

    public void UpdateStudent()
    {
        var id = Helpers.ReadIntInput("Tələbə ID");
        var existing = _studentService.GetStudentById(id);
        if (existing == null)
        {
            Helpers.DisplayError("Student tapılmadı.");
            return;
        }
        if (existing.Group == null)
        {
            Helpers.DisplayError("Student üçün qrup məlumatı tapılmadı.");
            return;
        }

        var name = ReadLettersOnlyOptional("Yeni ad", existing.Name);
        var surname = ReadLettersOnlyOptional("Yeni soyad", existing.Surname);
        var age = ReadIntWithDefault("Yeni yaş", existing.Age);
        var groupId = ReadIntWithDefault("Yeni qrup ID", existing.Group.Id);

        if (string.IsNullOrWhiteSpace(name)) name = existing.Name;
        if (string.IsNullOrWhiteSpace(surname)) surname = existing.Surname;

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
            Console.WriteLine($"ID: {g.Id} | {g.Name} | {g.Teacher} | {g.Room} | Created: {g.CreatedAt:g} | Students: {g.Students.Count}");
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

    private static string ReadLettersOnly(string prompt)
    {
        while (true)
        {
            var input = (Helpers.ReadInput(prompt) ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                Helpers.DisplayError("Dəyər boş ola bilməz.");
                continue;
            }
            var allLetters = input.All(char.IsLetter);
            if (!allLetters)
            {
                Helpers.DisplayError("Yalnız hərf daxil edin (rəqəm və simvol olmaz).");
                continue;
            }
            return CapitalizeFirst(input);
        }
    }

    private static string CapitalizeFirst(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        var first = char.ToUpper(text[0]);
        var rest = text.Length > 1 ? text.Substring(1) : string.Empty;
        return first + rest;
    }

    private static string ReadWithDefault(string prompt, string current)
    {
        while (true)
        {
            var input = Helpers.ReadInput($"{prompt} ({current})");
            if (string.IsNullOrWhiteSpace(input))
                return current;

            return input.Trim();
        }
    }

    private static string ReadLettersOnlyOptional(string prompt, string current)
    {
        while (true)
        {
            var input = Helpers.ReadInput($"{prompt} ({current})") ?? string.Empty;
            input = input.Trim();
            if (string.IsNullOrWhiteSpace(input))
                return current;

            if (!input.All(char.IsLetter))
            {
                Helpers.DisplayError("Yalnız hərf daxil edin (rəqəm və simvol olmaz).");
                continue;
            }

            return CapitalizeFirst(input);
        }
    }

    private static int ReadIntWithDefault(string prompt, int current)
    {
        while (true)
        {
            var input = Helpers.ReadInput($"{prompt} ({current})");
            if (string.IsNullOrWhiteSpace(input))
                return current;

            if (int.TryParse(input.Trim(), out var value))
                return value;

            Helpers.DisplayError("Yalnız rəqəm daxil edin.");
        }
    }
}

