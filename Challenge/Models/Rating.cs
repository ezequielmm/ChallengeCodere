﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Challenge.Models;

namespace Challenge.Models
{
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("Show")]
        public int Id { get; set; }

        [JsonPropertyName("average")]
        public double? Average { get; set; }

        public Show Show { get; set; }
    }
}
