using CourseApp;
using Domain.Interfaces;
using Repository.Repositories;
using Service.Helpers;
using Service.Services;
using System.Linq;

IGroupRepository groupRepository = new GroupRepository();
IStudentRepository studentRepository = new StudentRepository();
IGroupService groupService = new GroupService(groupRepository);
IStudentService studentService = new StudentService(studentRepository, groupRepository);

Helpers.DisplayInfo("Course Management Console Application-a xoş gəldiniz!");

var exitRequested = false;
while (!exitRequested)
{
    var handler = new MenuHandler(groupService, studentService);
    handler.PrintMenu();
    var choice = Helpers.ReadInput("Seçiminizi daxil edin (0 - Çıxış)");

    try
    {
        switch (choice)
        {
            case "0":
                exitRequested = true;
                Helpers.DisplayMessage("Proqram dayandırılır. Sağ olun!", ConsoleColor.Yellow);
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

            default:
                Helpers.DisplayError("Yanlış seçim. Menyudan istifadə edin.");
                break;
        }
    }
    catch (Exception ex)
    {
        Helpers.DisplayError(ex.Message);
    }

    if (!exitRequested)
    {
        Helpers.ReadInput("\nDavam etmək üçün Enter basın");
        Console.Clear();
    }
}
