using System;
using System.Text.Json.Serialization;

namespace NoraCollection.Shared.Dtos.ResponseDtos;

public class ResponseDto<T>
{
    public T Data { get; set; } = default!;
    public bool IsSuccessful { get; set; }
    public List<string> Errors { get; set; }= [];

    [JsonIgnore]
    public int StatusCode { get; set; }
    public static ResponseDto<T> Success(T data, int statusCode)
    {
        return new ResponseDto<T>
        {
            Data=data,
            StatusCode=statusCode,
            IsSuccessful=true
        };
    }
    public static ResponseDto<T> Success(int statusCode)
    {
        return new ResponseDto<T>
        {
          IsSuccessful=true,
          StatusCode=statusCode  
        };
    }
    public static ResponseDto<T> Fail(string error, int statusCode)
    {
        return new ResponseDto<T>
        {
          Errors=[error],
          StatusCode=statusCode  
        };
    }
    public static ResponseDto<T> Fail(List<string> errors, int statusCode)
    {
        return new ResponseDto<T>
        {
          Errors=errors,
          StatusCode=statusCode  
        };
    }
 
}
