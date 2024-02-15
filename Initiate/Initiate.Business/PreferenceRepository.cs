using AutoMapper;
using Initiate.DataAccess;
using Initiate.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace Initiate.Business
{
    /// <summary>
    /// News repository to connect with db for news
    /// </summary>
    public class PreferenceRepository : IPreferenceRepository
    {
        private readonly ApplicationDbContext m_db;
        private readonly IMapper m_mapper;

        public PreferenceRepository(ApplicationDbContext db, IMapper mapper)
        {
            m_db = db;
            m_mapper = mapper;
        }

        public Task<bool> CreatePreference(PreferenceDTO newsDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePreference(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PreferenceDTO> Getpreference(string? email)
        {

            var preference = await m_db.Users.Include(x => x.Preference).FirstOrDefaultAsync(x => x.Email == email);

            var preferenceDTO = m_mapper.Map<Preference, PreferenceDTO>(preference?.Preference);
            return preferenceDTO;
        }

        public async Task<PreferenceDTO> GetPreference(string email)
        {
            var user = await m_db.Users.Include(x => x.Preference).FirstOrDefaultAsync(x => x.Email == email);
            var preference = user?.Preference;

            if (preference == null)
                return null;

            var preferenceDTO = new PreferenceDTO()
            {
                Country = preference?.Country??string.Empty,
                IsSetPreference = preference?.IsSetPreference??false,
                Language = preference?.Language??"English",
                NewsGenerationTime = preference.NewsGenerationTime??"",
                Province = preference?.Province??"",
                Email = user.Email,
            };

            return preferenceDTO;
        }

        public async Task<bool> UpdatePreference(PreferenceDTO preferenceDTO)
        {
            try
            {
                var user = await m_db.Users.Include(x => x.Preference).FirstOrDefaultAsync(x => x.UserName == preferenceDTO.Email);

                if (user?.Preference == null)
                    return false;

                user.Preference.IsSetPreference = true;
                user.Preference.Country = preferenceDTO.Country;
                user.Preference.Language = preferenceDTO.Language;
                user.Preference.NewsGenerationTime = preferenceDTO.NewsGenerationTime;
                user.Preference.Province = preferenceDTO.Province;

                await m_db.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {

                return false;
            }

            
        }
    }
}
