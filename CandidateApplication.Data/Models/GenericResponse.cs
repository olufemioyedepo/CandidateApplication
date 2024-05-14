using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateApplication.Data.Models
{
    public class GenericResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
        public int Code { get; set; }
        public bool Success { get; set; }
    }
}
