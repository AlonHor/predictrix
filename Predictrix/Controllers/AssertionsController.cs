using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Predictrix.Application.Commands;
using Predictrix.Application.Queries;
using Predictrix.Domain.Models;
using Predictrix.Domain.Requests;
using Predictrix.Services;
using Serilog;

namespace Predictrix.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AssertionsController(ISender mediator, IUserService userService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAssertion([FromBody] CreateAssertionRequest request)
        {
            var userId = userService.GetUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var assertion = new Assertion
            {
                UserId = userId,
                ChatId = request.ChatId,
                Text = request.Text,
                CastingForecastDeadline = request.CastingForecastDeadline,
                ValidationDate = request.ValidationDate
            };
            
            Log.Information("Creating assertion for user {UserId}", userId);

            var result = await mediator.Send(new CreateAssertionCommand
            {
                Assertion = assertion
            });
            
            if (result.IsFail)
                return BadRequest(result.Message);
            
            return Ok();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAssertions([FromQuery] int chatId)
        {
            var userId = userService.GetUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            Log.Information("Retrieving assertions for user {UserId}", userId);

            var result = await mediator.Send(new GetAssertionsQuery
            {
                UserId = userId,
                ChatId = chatId
            });
            
            if (result.IsFail)
                return BadRequest(result.Message);
            
            var assertions = result.Data;
            
            if (assertions == null)
                throw new Exception("Assertions not found");
            
            return Ok(assertions);
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAssertion(int id)
        {
            var userId = userService.GetUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            Log.Information("Retrieving assertion {Id} for user {UserId}", id, userId);

            var result = await mediator.Send(new GetAssertionQuery
            {
                UserId = userId,
                AssertionId = id
            });
            
            if (result.IsFail)
                return BadRequest(result.Message);
            
            var assertion = result.Data;
            
            if (assertion == null)
                throw new Exception("Assertion not found");
            
            return Ok(assertion);
        }
        
        [HttpPost("{id:int}/vote")]
        public async Task<IActionResult> VoteOnAssertion(int id, [FromBody] VoteOnAssertionRequest request)
        {
            var userId = userService.GetUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            Log.Information("Voting on assertion {Id} for user {UserId} with answer {Answer}", id, userId, request.Answer);
        
            var result = await mediator.Send(new VoteOnAssertionCommand
            {
                AssertionId = id,
                UserId = userId,
                Answer = request.Answer
            });
            
            if (result.IsFail)
                return BadRequest(result.Message);
            
            return Ok();
        }
    }
}
