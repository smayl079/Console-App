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
        Helpers.PlayMenuSound();
        Helpers.DisplayMessage("=== Menu ===", ConsoleColor.Yellow);
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
        Console.WriteLine("16 - Get All Groups with Students");
        Console.WriteLine();
    }

    public void CreateGroup()
    {
        var name = Helpers.ReadInput("Group name");
        if (name == null) return; 
        
        var teacher = ReadLettersOnly("Teacher");
        if (teacher == null) return; 
        
        var room = Helpers.ReadInput("Room");
        if (room == null) return; 
        
        var created = _groupService.CreateGroup(new Group
        {
            Name = name,
            Teacher = teacher,
            Room = room.Trim(),
            CreatedAt = DateTime.UtcNow
        });
        Helpers.DisplaySuccess($"Group created. ID: {created.Id}, Date: {created.CreatedAt:g}");
    }

    public void UpdateGroup()
    {
        var id = Helpers.ReadIntInput("Group ID");
        if (id == null) return; 
        
        var existing = _groupService.GetGroupById(id.Value);
        if (existing == null)
        {
            Helpers.DisplayError("Group not found.");
            return;
        }

        var name = ReadWithDefault("New group name", existing.Name);
        if (name == null) return; 
        
        var teacher = ReadLettersOnlyOptional("New teacher", existing.Teacher);
        if (teacher == null) return; 
        
        var room = ReadWithDefault("New room", existing.Room);
        if (room == null) return; 

        if (string.IsNullOrWhiteSpace(name)) name = existing.Name;
        if (string.IsNullOrWhiteSpace(teacher)) teacher = existing.Teacher;
        if (string.IsNullOrWhiteSpace(room)) room = existing.Room;

        _groupService.UpdateGroup(new Group { Id = id.Value, Name = name, Teacher = teacher, Room = room, CreatedAt = existing.CreatedAt });
        Helpers.DisplaySuccess("Group updated.");
    }

    public void DeleteGroup()
    {
        var id = Helpers.ReadIntInput("Group ID");
        if (id == null) return; 
        
        var ok = _groupService.DeleteGroup(id.Value);
        if (ok) Helpers.DisplaySuccess("Group deleted.");
        else Helpers.DisplayError("Group not found.");
    }

    public void GetGroupById()
    {
        var id = Helpers.ReadIntInput("Group ID");
        if (id == null) return; 
        
        var group = _groupService.GetGroupById(id.Value);
        if (group == null)
        {
            Helpers.DisplayError("Group not found.");
            return;
        }
        Console.WriteLine($"\nID: {group.Id} | Name: {group.Name} | Teacher: {group.Teacher} | Room: {group.Room} | Created: {group.CreatedAt:g} | Students: {group.Students.Count}");
    }

    public void GetGroupsByTeacher()
    {
        var teacher = Helpers.ReadInput("Teacher name");
        if (teacher == null) return;
        
        var groups = _groupService.GetAllGroupsByTeacher(teacher);
        PrintGroups(groups);
    }

    public void GetGroupsByRoom()
    {
        var room = Helpers.ReadInput("Room");
        if (room == null) return; 
        
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
        var name = ReadLettersOnly("Student name");
        if (name == null) return;
        
        var surname = ReadLettersOnly("Student surname");
        if (surname == null) return;
        
        var age = Helpers.ReadIntInput("Age");
        if (age == null) return; 
        
        var groupId = Helpers.ReadIntInput("Group ID");
        if (groupId == null) return; 

        var group = _groupService.GetGroupById(groupId.Value) ?? throw new Exception("Group not found.");
        var created = _studentService.CreateStudent(new Student { Name = name, Surname = surname, Age = age.Value, Group = group });
        Helpers.DisplaySuccess($"Student created. ID: {created.Id}");
    }

    public void UpdateStudent()
    {
        var id = Helpers.ReadIntInput("Student ID");
        if (id == null) return; 
        
        var existing = _studentService.GetStudentById(id.Value);
        if (existing == null)
        {
            Helpers.DisplayError("Student not found.");
            return;
        }
        if (existing.Group == null)
        {
            Helpers.DisplayError("Group information not found for student.");
            return;
        }

        var name = ReadLettersOnlyOptional("New name", existing.Name);
        if (name == null) return; 
        
        var surname = ReadLettersOnlyOptional("New surname", existing.Surname);
        if (surname == null) return; 
        
        var age = ReadIntWithDefault("New age", existing.Age);
        if (age == null) return; 
        
        var groupId = ReadIntWithDefault("New group ID", existing.Group.Id);
        if (groupId == null) return; 
        if (string.IsNullOrWhiteSpace(name)) name = existing.Name;
        if (string.IsNullOrWhiteSpace(surname)) surname = existing.Surname;

        var group = _groupService.GetGroupById(groupId.Value) ?? throw new Exception("Group not found.");
        _studentService.UpdateStudent(new Student { Id = id.Value, Name = name, Surname = surname, Age = age.Value, Group = group });
        Helpers.DisplaySuccess("Student updated.");
    }

    public void GetStudentById()
    {
        var id = Helpers.ReadIntInput("Student ID");
        if (id == null) return; 
        
        var s = _studentService.GetStudentById(id.Value);
        if (s == null) { Helpers.DisplayError("Student not found."); return; }
        Console.WriteLine($"\nID: {s.Id} | {s.Name} {s.Surname} | Age: {s.Age} | Group: {s.Group.Name} ({s.Group.Id})");
    }

    public void DeleteStudent()
    {
        var id = Helpers.ReadIntInput("Student ID");
        if (id == null) return; 
        
        var ok = _studentService.DeleteStudent(id.Value);
        if (ok) Helpers.DisplaySuccess("Student deleted.");
        else Helpers.DisplayError("Student not found.");
    }

    public void GetStudentsByAge()
    {
        var age = Helpers.ReadIntInput("Age");
        if (age == null) return; 
        
        var students = _studentService.GetAllStudents().Where(s => s.Age == age.Value).ToList();
        PrintStudents(students);
    }

    public void GetStudentsByGroupId()
    {
        var groupId = Helpers.ReadIntInput("Group ID");
        if (groupId == null) return; 
        
        var group = _groupService.GetGroupById(groupId.Value);
        if (group == null) { Helpers.DisplayError("Group not found."); return; }
        var students = _studentService.GetStudentsByGroup(group);
        PrintStudents(students);
    }

    public void SearchGroupsByName()
    {
        var name = Helpers.ReadInput("Search (group name)");
        if (name == null) return; 
        
        var groups = _groupService.SearchGroupsByName(name);
        PrintGroups(groups);
    }

    public void SearchStudents()
    {
        var query = Helpers.ReadInput("Search (name or surname)");
        if (query == null) return; 
        
        var students = _studentService.GetAllStudents()
            .Where(s => s.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        s.Surname.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
        PrintStudents(students);
    }

    public void GetAllGroupsWithStudents()
    {
        var groups = _groupService.GetAllGroups();
        PrintGroupsWithStudents(groups);
    }

    private static void PrintGroups(IEnumerable<Group> groups)
    {
        Console.WriteLine("\n=== Groups ===");
        foreach (var g in groups)
        {
            Console.WriteLine($"ID: {g.Id} | {g.Name} | {g.Teacher} | {g.Room} | Created: {g.CreatedAt:g} | Students: {g.Students.Count}");
        }
    }

    private static void PrintStudents(IEnumerable<Student> students)
    {
        Console.WriteLine("\n=== Students ===");
        foreach (var s in students)
        {
            Console.WriteLine($"ID: {s.Id} | {s.Name} {s.Surname} | Age: {s.Age} | Group: {s.Group.Name} ({s.Group.Id})");
        }
    }

    private static void PrintGroupsWithStudents(IEnumerable<Group> groups)
    {
        Console.WriteLine("\n=== Groups with Students ===");
        var groupsList = groups.ToList();
        if (groupsList.Count == 0)
        {
            Console.WriteLine("No groups found.");
            return;
        }

        foreach (var group in groupsList)
        {
            Console.WriteLine($"\nGroup: {group.Name} (ID: {group.Id})");
            Console.WriteLine($"Teacher: {group.Teacher} | Room: {group.Room} | Created: {group.CreatedAt:g}");
            
            if (group.Students.Count == 0)
            {
                Console.WriteLine("  No students in this group.");
            }
            else
            {
                Console.WriteLine($"  Students ({group.Students.Count}):");
                foreach (var student in group.Students)
                {
                    Console.WriteLine($"    - {student.Name} {student.Surname} (ID: {student.Id}, Age: {student.Age})");
                }
            }
        }
    }

    private static string? ReadLettersOnly(string prompt)
    {
        while (true)
        {
            var input = Helpers.ReadInput(prompt);
            if (input == null) return null; 
            
            input = input.Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                Helpers.DisplayError("Value cannot be empty.");
                continue;
            }
            if (input.Length < 3)
            {
                Helpers.DisplayError("Name must be at least 3 characters long.");
                continue;
            }
            var allLetters = input.All(char.IsLetter);
            if (!allLetters)
            {
                Helpers.DisplayError("Please enter only letters (no numbers or symbols).");
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

    private static string? ReadWithDefault(string prompt, string current)
    {
        while (true)
        {
            var input = Helpers.ReadInput($"{prompt} ({current})");
            if (input == null) return null; 
            
            if (string.IsNullOrWhiteSpace(input))
                return current;

            return input.Trim();
        }
    }

    private static string? ReadLettersOnlyOptional(string prompt, string current)
    {
        while (true)
        {
            var input = Helpers.ReadInput($"{prompt} ({current})");
            if (input == null) return null; 
            
            input = input.Trim();
            if (string.IsNullOrWhiteSpace(input))
                return current;

            if (input.Length < 3)
            {
                Helpers.DisplayError("Name must be at least 3 characters long.");
                continue;
            }

            if (!input.All(char.IsLetter))
            {
                Helpers.DisplayError("Please enter only letters (no numbers or symbols).");
                continue;
            }

            return CapitalizeFirst(input);
        }
    }

    private static int? ReadIntWithDefault(string prompt, int current)
    {
        while (true)
        {
            var input = Helpers.ReadInput($"{prompt} ({current})");
            if (input == null) return null; 
            if (string.IsNullOrWhiteSpace(input))
                return current;

            if (int.TryParse(input.Trim(), out var value))
                return value;

            Helpers.DisplayError("Please enter only numbers.");
        }
    }
}

