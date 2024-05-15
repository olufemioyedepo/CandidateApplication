using CandidateApplication.Data.DTOs;
using CandidateApplication.Data.Models;
using CandidateApplication.Service.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Container = Microsoft.Azure.Cosmos.Container;
using PartitionKey = Microsoft.Azure.Cosmos.PartitionKey;

namespace CandidateApplication.Service.Repository
{
    public class InternshipProgammeRepository : IInternshipProgammeInterface
    {
        private readonly string _cosmosDbAccountUri =string.Empty;
        private readonly string _cosmosDBAccountPrimaryKey = string.Empty;
        private readonly string _cosmosDbName = string.Empty;
        private readonly string _cosmosDbContainerName = string.Empty;

        private Microsoft.Azure.Cosmos.Container containerClient;
        private readonly IConfiguration _configuration;
        private readonly string _internshipProgrammeSetupPartitionKey;
        private readonly string _applicationDataPartitionKey;

       
        public InternshipProgammeRepository(IConfiguration configuration)
        {
            _configuration = configuration;

            _cosmosDbAccountUri = _configuration.GetSection("CosmosDb").GetSection("Account").Value;
            _cosmosDBAccountPrimaryKey = _configuration.GetSection("CosmosDb").GetSection("Key").Value;
            _cosmosDbName = _configuration.GetSection("CosmosDb").GetSection("DatabaseName").Value;
            _cosmosDbContainerName = _configuration.GetSection("CosmosDb").GetSection("ContainerName").Value;
            _internshipProgrammeSetupPartitionKey = _configuration.GetSection("CosmosDb").GetSection("InternshipProgrammeSetupPartitionKey").Value;
            _applicationDataPartitionKey = _configuration.GetSection("CosmosDb").GetSection("ApplicationPartitionKey").Value;

            CosmosClient cosmosDbClient = new CosmosClient(_cosmosDbAccountUri, _cosmosDBAccountPrimaryKey, new CosmosClientOptions { LimitToEndpoint = true, ConnectionMode = ConnectionMode.Gateway });
            containerClient = cosmosDbClient.GetContainer(_cosmosDbName, _cosmosDbContainerName);
        }

        public async Task<GenericResponse<InternshipProgrammeSetup>> CreateInternshipProgramme(InternshipProgrammeSetup internshipProgrammeSetup)
        {
            try
            {
                var validQuestionTypes = GetQuestionTypes()?.Data;
                internshipProgrammeSetup.Id = Guid.NewGuid().ToString();
                internshipProgrammeSetup.DataCategory = "InternshipProgramme";

                foreach (var question in internshipProgrammeSetup.Questions)
                {
                    question.QuestionId = Guid.NewGuid().ToString();
                    // validate if question type is valid
                    if (validQuestionTypes?.Contains(question.QuestionType) == false)
                    {
                        return new GenericResponse<InternshipProgrammeSetup>() { Code = 400, Message = $"{question.QuestionType} is an invalid question type!" };
                    }

                    switch (question.QuestionType)
                    {
                        case "MultipleChoice":
                            //choices cannot be null/empty
                            if (question.Choices == null || question.Choices?.Count == 0)
                            {
                                return new GenericResponse<InternshipProgrammeSetup>() { Code = 400, Message = $"There must be at least one choice for {question.QuestionType.ToLower()} question: {question.QuestionText}" };
                            }
                        break;

                        case "Dropdown":
                            //choices cannot be null/empty
                            if (question.Choices == null || question.Choices?.Count == 0)
                            {
                                return new GenericResponse<InternshipProgrammeSetup>() { Code = 400, Message = $"There must be at least one choice for {question.QuestionType.ToLower()} question: {question.QuestionText}" };
                            }
                            break;

                        default:
                            break;
                    }
                }
                
                
                var response = await containerClient.CreateItemAsync(internshipProgrammeSetup, new PartitionKey(internshipProgrammeSetup.DataCategory));
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return new GenericResponse<InternshipProgrammeSetup>() { Code = 201, Success = true, Data = internshipProgrammeSetup };
                }

                return new GenericResponse<InternshipProgrammeSetup>() { Code = 400, Message = "Operation failed, an error must have occured" };
            }
            catch (Exception ex)
            {
                // implement error logging mechanism
                return new GenericResponse<InternshipProgrammeSetup>() { Code=500, Message = $"Server error: {ex.Message}" };
            }
        }

        public async Task<GenericResponse<List<InternshipProgrammeSetup>>> GetInternshipProgrammes()
        {
            try
            {
                var sqlQuery = "SELECT * FROM c";
                QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
                FeedIterator<InternshipProgrammeSetup> queryResultSetIterator = containerClient.GetItemQueryIterator<InternshipProgrammeSetup>(queryDefinition);
                List<InternshipProgrammeSetup> programmeSetups = [];
                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<InternshipProgrammeSetup> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (InternshipProgrammeSetup employee in currentResultSet)
                    {
                        programmeSetups.Add(employee);
                    }
                }

                return new GenericResponse<List<InternshipProgrammeSetup>>() { Code=200, Data = programmeSetups, Success = true};
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<InternshipProgrammeSetup>>() { Code = 500, Message = $"Server error: {ex.Message}" };

            }
        }

        public async Task<GenericResponse<InternshipProgrammeSetup>> UpdateApplicationQuestions(InternshipProgrammeSetupQuestionForEdit internshipProgrammeSetupQuestionForEdit)
        {
            try
            {
                ItemResponse<InternshipProgrammeSetup> res = await containerClient.ReadItemAsync<InternshipProgrammeSetup>(internshipProgrammeSetupQuestionForEdit.Id, new PartitionKey(_internshipProgrammeSetupPartitionKey));

                var existingItem = res.Resource;
                existingItem.Questions = internshipProgrammeSetupQuestionForEdit.Questions;
                var updateResponse = await containerClient.ReplaceItemAsync(existingItem, internshipProgrammeSetupQuestionForEdit.Id, new PartitionKey(_internshipProgrammeSetupPartitionKey));
                
                if (updateResponse.StatusCode == HttpStatusCode.OK)
                {
                    return new GenericResponse<InternshipProgrammeSetup>() { Code = 200, Message = "Questions updated successfully!", Success = true, Data = updateResponse.Resource };
                }

                return new GenericResponse<InternshipProgrammeSetup>() { Code = 400, Message = "Could not update questions, an error must have occured" };
            }
            
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound) {
                return new GenericResponse<InternshipProgrammeSetup>() { Code = 404, Message = "Invalid application program Id!" };
            }

            catch (Exception ex)
            {
                return new GenericResponse<InternshipProgrammeSetup>() { Code = 500, Message = $"Server error: {ex.Message}" };
            }
        }

        public GenericResponse<List<string>> GetQuestionTypes()
        {
            try
            {
                var dropdownTypes = _configuration.GetSection("ValidQuestionTypes")
                                   .GetChildren()
                                   .Select(x => x.Value)
                                   .ToList();

                return new GenericResponse<List<string>>() { 
                    Code = 200, 
                    Data = dropdownTypes, 
                    Message =  dropdownTypes?.Count == 0 ? "No question type found" : $"{dropdownTypes.Count} question types found!",
                    Success = true
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GenericResponse<InternshipProgrammeSetup>> GetInternshipProgrammeDetails(string id)
        {
            try
            {
                ItemResponse<InternshipProgrammeSetup> response = await containerClient.ReadItemAsync<InternshipProgrammeSetup>(id, new PartitionKey(_internshipProgrammeSetupPartitionKey));
                var existingItem = response.Resource;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new GenericResponse<InternshipProgrammeSetup>() { Code = 200, Message = "Application detail retrieved successfully!", Success = true, Data = response.Resource };
                }

                return new GenericResponse<InternshipProgrammeSetup>() { Code = 400, Message = "Could not retrieve application details, an error must have occured" };
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new GenericResponse<InternshipProgrammeSetup>() { Code = 404, Message = "Invalid application program Id!" };
            }

            catch (Exception ex)
            {
                return new GenericResponse<InternshipProgrammeSetup>() { Code = 500, Message = $"Server error: {ex.Message}" };
            }
        }

        public async Task<GenericResponse<ApplicantFormData>> SubmitApplicationData(ApplicantFormData applicantFormDataQuestion)
        {
            try
            {
                ItemResponse<InternshipProgrammeSetup> applicationProgrammeCheckResponse = await containerClient.ReadItemAsync<InternshipProgrammeSetup>(applicantFormDataQuestion.ApplicationProgrammeId, new PartitionKey(_internshipProgrammeSetupPartitionKey));
                var internshipProgramme = applicationProgrammeCheckResponse.Resource;

                // validate that questions submitted from frontend are valid
                var allowedQuestionIds = internshipProgramme.Questions.Select(s=>s.QuestionId).ToList();

                foreach (var submittedQuestion in applicantFormDataQuestion.AnswersToQuestions)
                {
                    if (!allowedQuestionIds.Contains(submittedQuestion.QuestionId))
                    {
                        return new GenericResponse<ApplicantFormData>() { Code = 400, Message = $"Question ID: {submittedQuestion.QuestionId} does not exist under this question!" };
                    }

                }

                applicantFormDataQuestion.Id = Guid.NewGuid().ToString();
                applicantFormDataQuestion.DataCategory = _applicationDataPartitionKey;
                var response = await containerClient.CreateItemAsync(applicantFormDataQuestion, new PartitionKey(applicantFormDataQuestion.DataCategory));

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return new GenericResponse<ApplicantFormData>() { Code = 201, Success = true, Data = applicantFormDataQuestion };
                }

                return new GenericResponse<ApplicantFormData>() { Code = 400, Message = "Could not submit application data, an error must have occured!" };

            }
            catch (Exception ex)
            {
                return new GenericResponse<ApplicantFormData>() { Code = 500, Message = $"Server error: {ex.Message}" };
            }
        }
    }
}
