using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateApplication.Data.DTOs
{
    public class InternshipProgrammeSetupQuestion
    {
        [Required]
        public string QuestionType { get; set; }
        [Required]
        public string QuestionText { get; set; }
        public List<string> Choices { get; set; }
        public bool EnableOtherOption { get; set; }
        public int? MaxChoicesAllowed { get; set; }
    }
}
