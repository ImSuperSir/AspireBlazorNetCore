using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorAspire.Model.Models;

public class LoginModel
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }


}
