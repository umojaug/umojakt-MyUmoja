﻿using GrapesTl.Models.Audit;

namespace GrapesTl.Controllers.Audit;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AuditResidualRiskController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;



    [Authorize]
    [HttpGet("List")]
    public async Task<IActionResult> List()
    {
        try
        {
            var data = await _unitOfWork.SP_Call.List<ResidualRisk>("AuditResidualRiskGetAll");

            return Ok(data);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
           "Error retrieve list of data." + e.Message);
        }
    }

    [HttpGet("Select")]
    public async Task<IActionResult> Select()
    {
        try
        {
            var data = await _unitOfWork.SP_Call.List<ResidualRisk>("AuditResidualRiskGetAll");
            return Ok(data.Select(a => new { listId = a.ResidualRiskId, listName = a.ResidualRiskName }));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
           "Error retrieve list of data." + e.Message);
        }
    }

    [Authorize]
    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(string id)
    {
        try
        {
            var parameter = new DynamicParameters();
            parameter.Add("@ResidualRiskID", id);

            var data = await _unitOfWork.SP_Call.OneRecord<ResidualRisk>("AuditResidualRiskGetByID", parameter);

            if (data == null)
                return NotFound(SD.Message_NotFound);

            return Ok(data);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
           "Error retrieve details data." + e.Message);
        }
    }

    [Authorize]
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] ResidualRisk model)
    {
        if (!ModelState.IsValid)
            return BadRequest(SD.Message_Model_Error);

        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var parameter = new DynamicParameters();
            parameter.Add("@ResidualRiskName", model.ResidualRiskName);
            parameter.Add("@ResidualRiskValue", model.ResidualRiskValue);



            parameter.Add("@Message", "", dbType: DbType.String, direction: ParameterDirection.Output);
            await _unitOfWork.SP_Call.Execute("AuditResidualRiskCreate", parameter);

            var message = parameter.Get<string>("Message");

            if (message == "Already exists")
                return BadRequest(message);

            return Created("", SD.Message_Save);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
           "Error saving data." + e.Message);
        }
    }
    [Authorize]
    [HttpPost("Update")]
    public async Task<IActionResult> Update([FromBody] ResidualRisk model)
    {
        if (!ModelState.IsValid)
            return BadRequest(SD.Message_Model_Error);

        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var parameter = new DynamicParameters();
            parameter.Add("@ResidualRiskID", model.ResidualRiskId);
            parameter.Add("@ResidualRiskName", model.ResidualRiskName);
            parameter.Add("@ResidualRiskValue", model.ResidualRiskValue);


            parameter.Add("@Message", "", dbType: DbType.String, direction: ParameterDirection.Output);
            await _unitOfWork.SP_Call.Execute("AuditResidualRiskUpdate", parameter);
            var message = parameter.Get<string>("Message");

            if (message == "Not found")
                return NotFound(message);

            if (message == "Already exists")
                return BadRequest(message);

            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
           "Error updating data." + e.Message);
        }
    }

    [Authorize]
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var parameter = new DynamicParameters();
            parameter.Add("@ResidualRiskID", id);


            parameter.Add("@Message", "", dbType: DbType.String, direction: ParameterDirection.Output);
            await _unitOfWork.SP_Call.Execute("AuditResidualRiskDelete", parameter);

            var message = parameter.Get<string>("Message");

            if (message == "Not found")
                return NotFound(message);

            if (message == "Cannot delete")
                return BadRequest(message);

            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
             "Error deleting data." + e.Message);
        }
    }

}
