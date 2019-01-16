using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcMovie.Models
{
    public class Movie
    {
        public int Id { get; set; } // Id is required as the primary key for the database id and ID/Id are interchangeable

        [StringLength(60,MinimumLength =3)] // maxium of 60 characters, minimum of 3. however white space is allowed
        [Required] // this attribute forces something to be entered
        public string Title { get; set; }

        [Display(Name ="Release Date")] // changes the display name of the field
        [DataType(DataType.Date)] // datatype.date specifies that the time is not required for this field, only the date is displayed
        public DateTime ReleaseDate { get; set; }

        [Range(1,100)] // forces the range between 1 and 100 inclusive
        [DataType(DataType.Currency)] // assigning a data type forces the input to be required
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z""'/s-]*$")] // limits the character input. must only use letters (First letter uppercase, white space, numbers and special characters are not allowed)
        [Required]
        [StringLength(30)] // max of 30 characters, no limit to minimum
        public string Genre { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$"),StringLength(5),Required] // example showing all rules applied on a single line
        public string Rating { get; set; }
    }
}