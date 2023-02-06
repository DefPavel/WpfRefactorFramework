using System;

namespace WpfRefactorFramework.Models
{
    public sealed class Person
    {
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public byte[] Avatar { get; set; }
        public string StatusPerson { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string StatusPropusk { get; set; } = string.Empty;
        public int Id { get; set; }

        public int IdPropusk { get; set; }
    }
}
