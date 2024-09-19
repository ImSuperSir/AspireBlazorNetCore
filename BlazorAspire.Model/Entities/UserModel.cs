using System;
using System.Collections;
using System.Collections.Generic;

namespace BlazorAspire.Model.Entities;

public class UserModel
{
    public UserModel()
    {
        UserRoles = new List<UserRoleModel>();
    }
    public int Id { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public virtual ICollection<UserRoleModel> UserRoles { get; set; }

}
