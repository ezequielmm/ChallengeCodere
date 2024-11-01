using System.Collections.Generic;
using Challenge.DTO;

namespace Challenge.DTOs
{
    public class ShowResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public List<string> Genres { get; set; }
        public NetworkResponseDto Network { get; set; }
        public RatingResponseDto Rating { get; set; }
        public ExternalsResponseDto Externals { get; set; }
    }

}
