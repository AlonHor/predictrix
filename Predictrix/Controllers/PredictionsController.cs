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
    public class PredictionsController(ISender mediator, IUserService userService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreatePrediction([FromBody] CreatePredictionRequest request)
        {
            var userId = userService.GetUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var prediction = new Prediction
            {
                UserId = userId,
                AssertionId = request.AssertionId,
                Forecast = request.Forecast,
                Confidence = request.Confidence,
                Rationale = request.Rationale
            };
            
            Log.Information("Creating prediction for user {UserId}", userId);

            var result = await mediator.Send(new CreatePredictionCommand
            {
                Prediction = prediction
            });
            
            if (result.IsFail)
                return BadRequest(result.Message);
            
            return Ok();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPredictions([FromQuery] int assertionId)
        {
            var userId = userService.GetUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            Log.Information("Retrieving assertions for user {UserId}", userId);

            var result = await mediator.Send(new GetPredictionsQuery
            {
                UserId = userId,
                AssertionId = assertionId
            });
            
            if (result.IsFail)
                return BadRequest(result.Message);
            
            var predictions = result.Data;
            
            if (predictions == null)
                throw new Exception("Predictions not found");
            
            return Ok(predictions);
        }
    }
}
