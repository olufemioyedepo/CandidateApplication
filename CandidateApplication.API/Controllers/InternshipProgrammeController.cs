using CandidateApplication.API.Models;
using CandidateApplication.Data.DTOs;
using CandidateApplication.Service.Interfaces;
using CandidateApplication.Service.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.ComponentModel;
using Container = Microsoft.Azure.Cosmos.Container;

namespace CandidateApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternshipProgrammeController : ControllerBase
    {
        private readonly string CosmosDBAccountUri = "https://localhost:8081/";
        private readonly string CosmosDBAccountPrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private readonly string CosmosDbName = "InternshipApplicationDb";
        private readonly string CosmosDbContainerName = "InternshipApplicationData";
        
        private readonly IInternshipProgammeInterface _progammeRepository;
        public InternshipProgrammeController(IInternshipProgammeInterface internshipProgammeRepository)
        {
            _progammeRepository = internshipProgammeRepository;
        }


        [HttpPost]
        public async Task<IActionResult> Post(InternshipProgrammeSetup internshipProgrammeSetup)
        {
            var response = await _progammeRepository.CreateInternshipProgramme(internshipProgrammeSetup);
            return StatusCode(response.Code, response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _progammeRepository.GetInternshipProgrammes();
            return StatusCode(response.Code, response);
        }

        [HttpPut]
        public async Task<IActionResult> Put(InternshipProgrammeSetupQuestionForEdit internshipProgrammeSetupQuestionForEdit)
        {
            var response = await _progammeRepository.UpdateApplicationQuestions(internshipProgrammeSetupQuestionForEdit);
            return StatusCode(response.Code, response);
        }

        [HttpGet]
        [Route("question-types")]
        public IActionResult GetQuestionTypes()
        {
            var response = _progammeRepository.GetQuestionTypes();
            return StatusCode(response.Code, response);
        }
        
        [HttpGet]
        [Route("programme-detail")]
        public async Task<IActionResult> GetInternshipProgrammeDetails(string programmeId)
        {
            var response = await _progammeRepository.GetInternshipProgrammeDetails(programmeId);
            return StatusCode(response.Code, response);
        }

        [HttpPost]
        [Route("submit-applicant-data")]
        public async Task<IActionResult> SubmitApplicationData(ApplicantFormData applicantFormData)
        {
            var response = await _progammeRepository.SubmitApplicationData(applicantFormData);
            return StatusCode(response.Code, response);
        }
    }
}
