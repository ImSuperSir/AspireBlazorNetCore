using System;

namespace BlazorAspire.Model.Models;

public class LoginResponseModel
{
    public string Token { get; set; }

    public long TokenExpired { get; set; }
}
