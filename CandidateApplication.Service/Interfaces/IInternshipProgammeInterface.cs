﻿using CandidateApplication.Data.DTOs;
using CandidateApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateApplication.Service.Interfaces
{
    public interface IInternshipProgammeInterface
    {
        Task<GenericResponse<InternshipProgrammeSetup>> CreateInternshipProgramme(InternshipProgrammeSetup internshipProgrammeSetup);
        Task<GenericResponse<List<InternshipProgrammeSetup>>> GetInternshipProgrammes();
        Task<GenericResponse<InternshipProgrammeSetup>> UpdateApplicationQuestions(InternshipProgrammeSetupQuestionForEdit internshipProgrammeSetupQuestionForEdit);
        Task<GenericResponse<InternshipProgrammeSetup>> GetInternshipProgrammeDetails(string id);
        GenericResponse<List<string>> GetQuestionTypes();
        Task<GenericResponse<ApplicantFormData>> SubmitApplicationData(ApplicantFormData applicantFormDataQuestion);

    }
}
