using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bascula.Model
{
    class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;

        public bool Enabled { get; set; }

        public string Name { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Rfc { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string Sucursal { get; set; } = string.Empty;

        public int IdSucursal { get; set; }

        public string Color { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;

    }
}
