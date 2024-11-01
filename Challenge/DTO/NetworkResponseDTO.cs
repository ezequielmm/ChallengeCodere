using Challenge.DTOs;

namespace Challenge.DTO
{
    public class NetworkResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CountryResponseDto Country { get; set; }
    }
}
