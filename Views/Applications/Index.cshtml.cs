using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using TransportRequestSystem.Models;

namespace TransportRequestSystem.Pages.Applications
{
    public class IndexModel : PageModel
    {
        // Получение отображаемого имени статуса
        public static string GetDisplayName(ApplicationStatus status)
        {
            var member = typeof(ApplicationStatus).GetMember(status.ToString())[0];
            var displayAttr = member.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                                    .FirstOrDefault() as System.ComponentModel.DataAnnotations.DisplayAttribute;
            return displayAttr?.Name ?? status.ToString();
        }

        // Получение CSS класса для бейджа статуса
        public static string GetStatusBadgeClass(ApplicationStatus status) => status switch
        {
            ApplicationStatus.Approved or ApplicationStatus.Completed => "bg-success",
            ApplicationStatus.InProgress => "bg-primary",
            ApplicationStatus.RejectedByDirector or ApplicationStatus.RejectedByDispatcher => "bg-danger",
            ApplicationStatus.AssignedToVehicle => "bg-info",
            ApplicationStatus.CreatedOrModified => "bg-secondary",
            ApplicationStatus.NotCompleted => "bg-warning",
            ApplicationStatus.Deleted => "bg-dark",
            _ => "bg-secondary"
        };
    }
}