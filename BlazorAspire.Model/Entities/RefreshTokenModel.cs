using System;

namespace BlazorAspire.Model.Entities;

public class RefreshTokenModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string RefreshToken { get; set; }

    public virtual UserModel User { get; set; }   //Navigation property

}
