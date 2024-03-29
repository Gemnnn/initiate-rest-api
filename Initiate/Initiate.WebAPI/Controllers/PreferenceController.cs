﻿using Initiate.Business;
using Initiate.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Initiate.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreferenceController : Controller
    {
        private IPreferenceRepository m_preferenceRepository;
        private INewsService m_newsService;
        ILogger<PreferenceController> m_logger;

        public PreferenceController(IPreferenceRepository preferenceRepository, INewsService newsService,  ILogger<PreferenceController> logger)
        {
            m_preferenceRepository = preferenceRepository;
            m_logger = logger;
            m_newsService = newsService;
        }

        [HttpPut()]
        public async Task<IActionResult> UpdatePreference([FromBody] PreferenceDTO preferenceDTO)
        {
            string Method = nameof(UpdatePreference);
            bool result = false;

            try
            {
                m_logger.LogInformation("Entering UpdatePreference {Language}, {Province}, {Country}", preferenceDTO.Language, preferenceDTO.Province, preferenceDTO.Country);
                result = await m_preferenceRepository.UpdatePreference(preferenceDTO);

                if (result)
                {
                    m_newsService.UpdateTimer(preferenceDTO.Email,preferenceDTO.NewsGenerationTime);
                }
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "Error updating preference");
                Ok(new ResponseBase("Preference update failed.", result));
            }

            m_logger.LogInformation($"Exit {Method}");

            return Ok(new ResponseBase(result ? "Preference updated." : "Preference update failed.", result));
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<PreferenceDTO>> GetPreference(string email)
        {
            string Method = nameof(UpdatePreference);
            PreferenceDTO result = new PreferenceDTO();

            try
            {
                m_logger.LogInformation($"Entering UpdatePreference {email}");
                result = await m_preferenceRepository.GetPreference(email);
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "Error updating preference");
                BadRequest(new ResponseBase("Preference update failed.", false));
            }

            m_logger.LogInformation($"Exit {Method}");

            return Ok(result);
        }
    }
}
