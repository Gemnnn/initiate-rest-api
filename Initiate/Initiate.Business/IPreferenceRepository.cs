using Initiate.Model;

namespace Initiate.Business
{
    public interface IPreferenceRepository
    {
        public Task<bool> CreatePreference(PreferenceDTO preferecneDTO);
        public Task<bool> UpdatePreference(PreferenceDTO preferenceDTO);
        public Task<bool> DeletePreference(int id);
        public Task<PreferenceDTO> GetPreference(string email);
    }
}
