namespace Frankstein.Common.Mvc.ExceptionHandling
{
    public class ErrorModel
    {
        public string Message { get; set; }
        public string FullMessage { get; set; }
        public string StackTrace { get; set; }
        public string Url { get; set; }
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Message, Url);
        }
    }
}