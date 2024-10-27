namespace Investment.Web.Models;

public class JsonResultMessage
{
    public string Title { get; set; }
    public string Message { get; set; }
}

public class JsonResultError
{
    public string Title { get; set; }
    public string Message { get; set; }
}

public class JsonDataResult
{
    public JsonResultMessage Message { get; set; }
    public JsonResultError Error { get; set; }
    public bool IsSuccess { get; set; }
    public bool IsError { get; set; }
    public string Data { get; set; }
    public object DataObject { get; set; }
    
    public JsonDataResult()
    {
        Message = new JsonResultMessage();
        Error = new JsonResultError();
        IsSuccess = false;
        IsError = false;
        Data = string.Empty;
    }
}

