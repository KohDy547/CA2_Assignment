using System;
using System.Net;

namespace CA2_Assignment.Models
{
    public class Response
    {
        public HttpStatusCode HttpStatus { get; set; }

        public string Message { get; set; }
        public object Payload { get; set; }
        public Exception ExceptionPayload { get; set; } = null;

        public bool HasError => ExceptionPayload != null;
    }
}
