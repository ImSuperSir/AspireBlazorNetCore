using System;

namespace BlazorAspire.Model;

public class BaseResponseModel
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public object Data { get; set; }
}
