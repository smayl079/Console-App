using CourseApp;
using Domain.Interfaces;
using Repository.Implementations;
using Service.Helpers;
using Service.Implementations;
using System.Linq;

IGroupRepository groupRepository = new GroupRepository();
IStudentRepository studentRepository = new StudentRepository();
IGroupService groupService = new GroupService(groupRepository, studentRepository);
IStudentService studentService = new StudentService(studentRepository, groupRepository);

Helpers.DisplayInfo("Welcome to Course Management Console Application!");

var exitRequested = false;
while (!exitRequested)
{
    var handler = new MenuHandler(groupService, studentService);
    handler.PrintMenu();
    var choice = Helpers.ReadInput("Enter your choice (0 - Exit)");
    
    if (choice == null) continue; // ESC pressed - show menu again

    try
    {
        switch (choice)
        {
            case "0":
                exitRequested = true;
                Helpers.DisplayMessage("Program is stopping. Thank you!", ConsoleColor.Yellow);
                break;

            case "1":
                handler.CreateGroup();
                break;
            case "2":
                handler.UpdateGroup();
                break;
            case "3":
                handler.DeleteGroup();
                break;
            case "4":
                handler.GetGroupById();
                break;
            case "5":
                handler.GetGroupsByTeacher();
                break;
            case "6":
                handler.GetGroupsByRoom();
                break;
            case "7":
                handler.GetAllGroups();
                break;

            case "8":
                handler.CreateStudent();
                break;
            case "9":
                handler.UpdateStudent();
                break;
            case "10":
                handler.GetStudentById();
                break;
            case "11":
                handler.DeleteStudent();
                break;
            case "12":
                handler.GetStudentsByAge();
                break;
            case "13":
                handler.GetStudentsByGroupId();
                break;
            case "14":
                handler.SearchGroupsByName();
                break;
            case "15":
                handler.SearchStudents();
                break;
            case "16":
                handler.GetAllGroupsWithStudents();
                break;

            default:
                Helpers.DisplayError("Invalid choice. Please use the menu.");
                break;
        }
    }
    catch (Exception ex)
    {
        Helpers.DisplayError(ex.Message);
    }

    if (!exitRequested)
    {
        Helpers.ReadInput("\nPress Enter to continue", allowEscape: false);
        Console.Clear();
    }
}
