using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kevsoft.PDFtk;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace WebApplicationFillForm.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PDFtk _pdFtk;

        public IndexModel(PDFtk pdFtk, ILogger<IndexModel> logger)
        {
            _pdFtk = pdFtk;
            _logger = logger;
        }

        public string[] Genders { get; } =
        {
            "Man",
            "Woman"
        };

        private string[] Countries { get; } =
        {
            "Austria",
            "Belgium",
            "Britain",
            "Bulgaria",
            "Croatia",
            "Cyprus",
            "Czech-Republic",
            "Denmark",
            "Estonia",
            "Finland",
            "France",
            "Germany",
            "Greece",
            "Hungary",
            "Ireland",
            "Italy",
            "Latvia",
            "Lithuania",
            "Luxembourg",
            "Malta",
            "Netherlands",
            "Poland",
            "Portugal",
            "Romania",
            "Slovakia",
            "Slovenia",
            "Spain",
            "Sweden",
        };

        public IEnumerable<SelectListItem> CountriesSelectListItems => Countries.Select(x => new SelectListItem(x, x));


        private string[] FavouriteColours { get; } =
        {
            "Black",
            "Blue",
            "Brown",
            "Green",
            "Grey",
            "Orange",
            "Red",
            "Violet",
            "White",
            "Yellow",
        };

        public IEnumerable<SelectListItem> FavouriteColoursSelectListItems =>
            FavouriteColours.Select(x => new SelectListItem(x, x));

        [BindProperty] public string GivenName { get; set; } = string.Empty;

        [BindProperty] public string FamilyName { get; set; } = string.Empty;

        [BindProperty] public string HouseNumber { get; set; } = string.Empty;

        [BindProperty] public string Address1 { get; set; } = string.Empty;

        [BindProperty] public string Address2 { get; set; } = string.Empty;

        [BindProperty] public string Postcode { get; set; } = string.Empty;

        [BindProperty] public string City { get; set; } = string.Empty;

        [BindProperty] public string Country { get; set; } = string.Empty;

        [BindProperty] public string Gender { get; set; } = string.Empty;

        [BindProperty] public int Height { get; set; } = 0;

        [BindProperty] public bool HasDrivingLicense { get; set; } = false;
        [BindProperty] public bool Deutsch { get; set; } = false;
        [BindProperty] public bool English { get; set; } = false;
        [BindProperty] public bool Français { get; set; } = false;
        [BindProperty] public bool Esperanto { get; set; } = false;
        [BindProperty] public bool Latin { get; set; } = false;

        public string FavouriteColour { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            var pdfFile = await System.IO.File.ReadAllBytesAsync("Form.pdf");
            var readOnlyDictionary = new Dictionary<string, string>()
            {
                ["Given Name Text Box"] = GivenName,
                ["Family Name Text Box"] = FamilyName,
                ["House nr Text Box"] = HouseNumber,
                ["Address 1 Text Box"] = Address1,
                ["Address 2 Text Box"] = Address2,
                ["Postcode Text Box"] = Postcode,
                ["City Text Box"] = City,
                ["Country Combo Box"] = Country,
                ["Gender List Box"] = Gender,
                ["Height Formatted Field"] = Height.ToString(),
                ["Driving License Check Box"] = HasDrivingLicense ? "Yes" : "Off",
                ["Language 1 Check Box"] = Deutsch ? "Yes" : "Off",
                ["Language 2 Check Box"] = English ? "Yes" : "Off",
                ["Language 3 Check Box"] = Français ? "Yes" : "Off",
                ["Language 4 Check Box"] = Esperanto ? "Yes" : "Off",
                ["Language 5 Check Box"] = Latin ? "Yes" : "Off",
                ["Favourite Colour List Box"] = FavouriteColour,
            };
            var fillFormAsync = await _pdFtk.FillFormAsync(pdfFile, readOnlyDictionary, false, false);

            if (!fillFormAsync.Success)
                throw new Exception($"Oops: {fillFormAsync.StandardError}");

            return File(fillFormAsync.Result, "application/pdf", $"{Guid.NewGuid().ToString()}.pdf");
        }
    }
}