using System;

namespace WpfRefactorFramework.Models
{
    public sealed class Move
    {
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? LastTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public string Section { get; set;} = string.Empty;
        public string Room { get; set; } = string.Empty;
    }
}