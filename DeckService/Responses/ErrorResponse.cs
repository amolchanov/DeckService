using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeckService.Responses
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public string ErrorCode { get; set; }
    }
}
