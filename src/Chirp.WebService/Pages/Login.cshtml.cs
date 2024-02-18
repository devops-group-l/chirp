using Chirp.Core.Dto;
using Chirp.Core.Extensions;
using Chirp.Core.Repositories;
using Chirp.WebService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class LoginModel: PageModel
{
    public FooterPartialModel FooterPartialModel { get; set; }
    
    public LoginModel()
    {
      
    }

    public IActionResult OnGet()
    {
        int pageNumber = 1;
        int amountOfPages = 1;

        FooterPartialModel = FooterPartialModel.BuildFooterPartialModel(pageNumber, amountOfPages);
        
        return Page();
    }
}